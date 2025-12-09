/*
 * Copyright 2024 NewMedia Centre - Delft University of Technology
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using Handzone.Core;
using Grasshopper.Kernel;
using Schema.Socket.Grasshopper;
using Robots;
using Robots.Grasshopper;

namespace Handzone.Components
{
    public class ProgramComponent : GH_Component
    {
        private IProgram _program;
        private ComponentButton _button;

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ProgramComponent()
            : base("Send Program", "Program",
                "Sends a robots program to the robot",
                "HANDZONe", "Robot")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager input)
        {
            input.AddParameter(new ProgramParameter(), "Program", "P", "Program to upload", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager output)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="io">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess io)
        {
            // get the program
            io.GetData(0, ref _program);

            _button.Label = "Upload";
            _button.Action = Upload;

            Rhino.RhinoApp.InvokeOnUiThread((Action)delegate { OnDisplayExpired(true); });
        }

        public override void CreateAttributes()
        {
            _button = new ComponentButton(this, "Upload", Upload);
            m_attributes = _button;
        }

        async void Upload()
        {
            if (!State.SessionConnection.Connected)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Not connected to robot");
                return;
            }

            if (_program == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Program is null");
                return;
            }

            // Serialize the program to JSON
            string programJson;
            try
            {
                // Use a robust serializer that skips troublesome runtime/computed properties (e.g., Robots.RobotUR.BasePlane)
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.None,
                    // Ignore specific known-problematic properties and keep going on errors
                    ContractResolver = new IgnoreRobotRuntimePropertiesContractResolver(),
                    Error = (sender, args) => { args.ErrorContext.Handled = true; }
                };

                programJson = JsonConvert.SerializeObject(_program, settings);
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Failed to serialize program: {e.Message}");
                return;
            }

            // Compress JSON (gzip + base64) for bandwidth efficiency
            string CompressToBase64(string input)
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(mso, CompressionLevel.Optimal, leaveOpen: true))
                    {
                        gs.Write(bytes, 0, bytes.Length);
                    }
                    return Convert.ToBase64String(mso.ToArray());
                }
            }

            // Best-effort extraction of compact joint-space trajectory from program (to avoid heavy deserialization in Unity)
            // - joints: double[waypoint][6]
            // - times: optional double[waypoint-1] delta seconds, or dt: fixed timestep
            // If extraction fails, fall back to sending only the compressed program string.
            System.Collections.Generic.List<System.Collections.Generic.List<double>> joints = null;
            System.Collections.Generic.List<double> times = null;
            double? dt = null;
            string units = "rad";

            try
            {
                // Try to extract via reflection to avoid hard dependency on Robots internals
                // Common patterns attempted:
                // 1) _program.Targets: IEnumerable of items with property 'Joints' or 'Q' (double[])
                // 2) _program.Simulation.Steps: items with 'Joints' (double[]), optional 'Dt' or 'Duration'
                // 3) _program.Instructions: sequence of MoveJ/MoveL etc. with embedded 'Joints'

                (joints, times, dt) = JointExtractor.TryExtract(_program);
            }
            catch
            {
                // Ignore extraction errors, we'll still send full program
            }

            var revision = DateTime.UtcNow.ToString("o");

            // If we successfully extracted joints, reduce payload: skip sending the heavy program JSON.
            // Also lightly quantize to shrink JSON size.
            if (joints != null && joints.Count > 0)
            {
                Quantize(joints, 1e-5);
                if (times != null && times.Count > 0) Quantize(times, 1e-5);
                programJson = string.Empty; // only send what Unity needs
            }

            // build the payload with compression and reload flags
            GrasshopperProgramIn grasshopperProgramIn = new GrasshopperProgramIn()
            {
                Program = string.IsNullOrEmpty(programJson) ? "" : CompressToBase64(programJson),
                Compressed = !string.IsNullOrEmpty(programJson),
                Reload = true,
                Revision = revision,
                Joints = joints,
                Times = times,
                Dt = dt,
                Units = units
            };

            // send the program
            try
            {
                await State.SessionConnection.Program(grasshopperProgramIn);
                Start();
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                Rhino.RhinoApp.InvokeOnUiThread((Action)delegate { OnDisplayExpired(true); });
            }
        }

        // Quantize values to reduce JSON size without affecting motion noticeably
        private static void Quantize(System.Collections.Generic.List<System.Collections.Generic.List<double>> data, double step)
        {
            if (data == null) return;
            for (int i = 0; i < data.Count; i++)
            {
                var row = data[i]; if (row == null) continue;
                for (int j = 0; j < row.Count; j++)
                {
                    row[j] = Math.Round(row[j] / step) * step;
                }
            }
        }

        private static void Quantize(System.Collections.Generic.List<double> data, double step)
        {
            if (data == null) return;
            for (int i = 0; i < data.Count; i++)
            {
                data[i] = Math.Round(data[i] / step) * step;
            }
        }


        async void Start()
        {
            if (!State.SessionConnection.Connected)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Not connected to robot");
                return;
            }

            try
            {
                var grasshopperRunIn = new GrasshopperRunIn()
                {
                    Run = true
                };

                await State.SessionConnection.Run(grasshopperRunIn);

                _button.Label = "Pause";
                _button.Action = Pause;

                Rhino.RhinoApp.InvokeOnUiThread((Action)delegate { OnDisplayExpired(true); });
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                Rhino.RhinoApp.InvokeOnUiThread((Action)delegate { OnDisplayExpired(true); });
            }
        }

        async void Pause()
        {
            if (!State.SessionConnection.Connected)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Not connected to robot");
                return;
            }

            try
            {
                var grasshopperRunIn = new GrasshopperRunIn()
                {
                    Run = false
                };

                await State.SessionConnection.Run(grasshopperRunIn);

                _button.Label = "Start";
                _button.Action = Start;

                Rhino.RhinoApp.InvokeOnUiThread((Action)delegate { OnDisplayExpired(true); });
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                Rhino.RhinoApp.InvokeOnUiThread((Action)delegate { OnDisplayExpired(true); });
            }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.Util.GetIcon("iconSimulation");

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("45b8d103-8a21-47b0-94da-0e4f1b1e1038");
    }

    /// <summary>
    /// Custom contract resolver to ignore runtime/computed properties from the Robots library
    /// that can throw during JSON serialization (e.g., BasePlane on RobotUR).
    /// </summary>
    class IgnoreRobotRuntimePropertiesContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            // Skip properties known to cause getter-side effects or require Rhino context
            if (prop != null && prop.Readable && prop.PropertyName == "BasePlane")
            {
                var declaring = member.DeclaringType?.FullName ?? string.Empty;
                if (declaring == "Robots.RobotUR" || declaring == "Robots.Robot")
                {
                    prop.Ignored = true;
                }
            }

            return prop;
        }
    }

    internal static class JointExtractor
    {
        // Try to extract a compact joint trajectory from an arbitrary Robots IProgram instance using reflection.
        // Returns (joints, times, dt). Any may be null if not found.
        public static (System.Collections.Generic.List<System.Collections.Generic.List<double>> joints, System.Collections.Generic.List<double> times, double? dt) TryExtract(object program)
        {
            if (program == null) return (null, null, null);

            // Prefer explicit Robots.Program structure: Program.Targets -> SystemTarget.ProgramTargets[0].Kinematics.Joints
            var jx = TryExtractFromProgramTargetsStructured(program, out var timesX);
            if (jx != null && jx.Count > 0)
                return (jx, timesX, null);

            // 0) Try Simulation method invocation first (most complete path sampling)
            var j0 = TryExtractViaSimulate(program, out var times0, out var dt0);
            if (j0 != null && j0.Count > 0)
                return (j0, times0, dt0);

            // 1) Direct Targets collection
            var j1 = TryExtractFromTargets(program, out var times1, out var dt1);
            if (j1 != null && j1.Count > 0)
                return (j1, times1, dt1);

            // 2) Simulation -> Steps/Frames
            var j2 = TryExtractFromSimulation(program, out var times2, out var dt2);
            if (j2 != null && j2.Count > 0)
                return (j2, times2, dt2);

            // 3) Instructions sequence (movej/movel)
            var j3 = TryExtractFromInstructions(program, out var times3, out var dt3);
            if (j3 != null && j3.Count > 0)
                return (j3, times3, dt3);

            return (null, null, null);
        }

        // Structured extraction based on visose/Robots:
        // Program.Targets: List<SystemTarget>
        //   SystemTarget.ProgramTargets: List<ProgramTarget>
        //     ProgramTarget.Kinematics.Joints: double[]
        //   SystemTarget.DeltaTime (seconds), SystemTarget.TotalTime (seconds)
        private static System.Collections.Generic.List<System.Collections.Generic.List<double>> TryExtractFromProgramTargetsStructured(object program, out System.Collections.Generic.List<double> times)
        {
            times = null;
            try
            {
                var targetsProp = GetProperty(program.GetType(), new[] { "Targets" });
                if (targetsProp == null) return null;
                var targetsObj = targetsProp.GetValue(program);
                if (targetsObj is not System.Collections.IEnumerable targetsEnum) return null;

                var jointsList = new System.Collections.Generic.List<System.Collections.Generic.List<double>>();
                var totalTimes = new System.Collections.Generic.List<double>();
                var deltaTimes = new System.Collections.Generic.List<double>();

                foreach (var st in targetsEnum)
                {
                    if (st == null) continue;
                    // Extract group 0 joints
                    var programTargetsProp = GetProperty(st.GetType(), new[] { "ProgramTargets" });
                    var programTargetsObj = programTargetsProp?.GetValue(st) as System.Collections.IEnumerable;
                    if (programTargetsObj == null) continue;

                    object firstPt = null;
                    foreach (var pt in programTargetsObj) { firstPt = pt; break; }
                    if (firstPt == null) continue;

                    // pt.Kinematics.Joints
                    var kinProp = GetProperty(firstPt.GetType(), new[] { "Kinematics" });
                    var kinObj = kinProp?.GetValue(firstPt);
                    var jArr = TryGetDoubleArray(kinObj, new[] { "Joints" });
                    if (jArr != null && jArr.Length > 0)
                        jointsList.Add(new System.Collections.Generic.List<double>(jArr));

                    // Collect timing
                    var tt = TryGetDouble(st, new[] { "TotalTime", "totalTime" });
                    if (tt.HasValue) totalTimes.Add(tt.Value);
                    var dt = TryGetDouble(st, new[] { "DeltaTime", "deltaTime", "Duration", "Dt", "dT" });
                    if (dt.HasValue) deltaTimes.Add(dt.Value);
                }

                if (jointsList.Count == 0) return null;

                // Prefer per-segment delta times. If we have one per segment, use them.
                if (deltaTimes.Count >= jointsList.Count - 1)
                {
                    // Align to segments: use next target's delta time
                    times = new System.Collections.Generic.List<double>(jointsList.Count - 1);
                    for (int i = 0; i < jointsList.Count - 1; i++)
                    {
                        var val = i + 1 < deltaTimes.Count ? deltaTimes[i + 1] : deltaTimes[i];
                        if (val <= 0 && totalTimes.Count == jointsList.Count)
                            val = totalTimes[i + 1] - totalTimes[i];
                        if (val <= 0) val = 0.02; // fallback
                        times.Add(val);
                    }
                }
                else if (totalTimes.Count == jointsList.Count)
                {
                    times = new System.Collections.Generic.List<double>(jointsList.Count - 1);
                    for (int i = 0; i < jointsList.Count - 1; i++)
                    {
                        var val = totalTimes[i + 1] - totalTimes[i];
                        if (val <= 0) val = 0.02;
                        times.Add(val);
                    }
                }

                return jointsList;
            }
            catch { }
            return null;
        }

        private static System.Collections.Generic.List<System.Collections.Generic.List<double>> TryExtractViaSimulate(object program, out System.Collections.Generic.List<double> times, out double? dt)
        {
            times = null; dt = null;
            try
            {
                var t = program.GetType();
                // Look for methods that sound like simulation runners
                var m = t.GetMethod("Simulate", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase)
                        ?? t.GetMethod("GetSimulation", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
                if (m == null) return null;

                object simOut = null;
                var pars = m.GetParameters();
                if (pars.Length == 0)
                {
                    simOut = m.Invoke(program, null);
                }
                else if (pars.Length == 1)
                {
                    // Often a dt parameter
                    var p0 = pars[0].ParameterType;
                    if (p0 == typeof(double) || p0 == typeof(float))
                    {
                        simOut = m.Invoke(program, new object[] { 0.02 });
                        dt = 0.02;
                    }
                }

                if (simOut == null) return null;

                // Some methods return an enumerable directly, others return a wrapper that has Frames/Steps
                if (simOut is System.Collections.IEnumerable)
                {
                    return CollectJointsFromEnumerable(simOut, out times, out var _);
                }
                else
                {
                    // Try common container properties
                    var framesProp = GetProperty(simOut.GetType(), new[] { "Frames", "Steps", "States", "Samples", "Kinematics", "Outputs", "Points" });
                    if (framesProp != null)
                    {
                        var coll = framesProp.GetValue(simOut);
                        return CollectJointsFromEnumerable(coll, out times, out var _);
                    }
                }
            }
            catch { }
            return null;
        }

        private static System.Collections.Generic.List<System.Collections.Generic.List<double>> TryExtractFromTargets(object program, out System.Collections.Generic.List<double> times, out double? dt)
        {
            times = null; dt = null;
            var targetsProp = GetProperty(program.GetType(), new[] { "Targets", "ProgramTargets", "Waypoints" });
            if (targetsProp == null) return null;
            var coll = targetsProp.GetValue(program);
            return CollectJointsFromEnumerable(coll, out times, out dt);
        }

        private static System.Collections.Generic.List<System.Collections.Generic.List<double>> TryExtractFromSimulation(object program, out System.Collections.Generic.List<double> times, out double? dt)
        {
            times = null; dt = null;
            var simProp = GetProperty(program.GetType(), new[] { "Simulation", "Sim", "Result", "Results", "SimulationResults" });
            if (simProp == null) return null;
            var sim = simProp.GetValue(program);
            if (sim == null) return null;

            // Prefer a dt on simulation
            var dtProp = GetProperty(sim.GetType(), new[] { "Dt", "dT", "DeltaTime", "Timestep" });
            if (dtProp != null)
            {
                var dtObj = dtProp.GetValue(sim);
                if (dtObj is double d) dt = d;
                else if (dtObj is float f) dt = f;
            }

            // Steps/Frames/States
            var stepsProp = GetProperty(sim.GetType(), new[] { "Steps", "Frames", "States", "Samples", "Kinematics", "Outputs", "Points" });
            if (stepsProp == null) return null;
            var coll = stepsProp.GetValue(sim);
            return CollectJointsFromEnumerable(coll, out times, out var _);
        }

        private static System.Collections.Generic.List<System.Collections.Generic.List<double>> TryExtractFromInstructions(object program, out System.Collections.Generic.List<double> times, out double? dt)
        {
            times = null; dt = null;
            var instrProp = GetProperty(program.GetType(), new[] { "Instructions", "Motions", "Moves" });
            if (instrProp == null) return null;
            var coll = instrProp.GetValue(program);
            return CollectJointsFromEnumerable(coll, out times, out dt);
        }

        private static System.Collections.Generic.List<System.Collections.Generic.List<double>> CollectJointsFromEnumerable(object enumerableObj, out System.Collections.Generic.List<double> times, out double? dt)
        {
            times = null; dt = null;
            if (enumerableObj == null) return null;

            var result = new System.Collections.Generic.List<System.Collections.Generic.List<double>>();
            var e = enumerableObj as System.Collections.IEnumerable;
            if (e == null) return null;

            double? prevTime = null;
            var timesList = new System.Collections.Generic.List<double>();

            foreach (var item in e)
            {
                if (item == null) continue;

                // Try to access array of doubles on 'Joints' or 'Q' or 'Angles'
                var v = TryGetDoubleArray(item, new[] { "Joints", "Q", "Angles" });
                if (v == null)
                {
                    // Some structures nest a 'Target' object containing joints
                    var targetProp = GetProperty(item.GetType(), new[] { "Target", "Pose", "State" });
                    if (targetProp != null)
                    {
                        var target = targetProp.GetValue(item);
                        v = TryGetDoubleArray(target, new[] { "Joints", "Q", "Angles" });
                    }
                }

                if (v != null && v.Length > 0)
                {
                    result.Add(new System.Collections.Generic.List<double>(v));

                    // extract timing if present per item
                    var tAbs = TryGetDouble(item, new[] { "Time", "T", "Timestamp" });
                    var dur = TryGetDouble(item, new[] { "Duration", "Dt", "dT" });

                    if (tAbs.HasValue)
                    {
                        if (prevTime.HasValue)
                        {
                            var delta = tAbs.Value - prevTime.Value;
                            if (delta > 0) timesList.Add(delta);
                        }
                        prevTime = tAbs.Value;
                    }
                    else if (dur.HasValue)
                    {
                        timesList.Add(dur.Value);
                    }
                }
            }

            if (result.Count == 0) return null;

            // If we collected consistent times (one less than joints), return them
            if (timesList.Count == result.Count - 1)
                times = timesList;
            else if (timesList.Count > 0)
                dt = Average(timesList);

            return result;
        }

        private static PropertyInfo GetProperty(Type type, string[] names)
        {
            foreach (var n in names)
            {
                var p = type.GetProperty(n, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
                if (p != null) return p;
            }
            return null;
        }

        private static double[] TryGetDoubleArray(object obj, string[] propNames)
        {
            if (obj == null) return null;
            foreach (var name in propNames)
            {
                var p = GetProperty(obj.GetType(), new[] { name });
                if (p == null) continue;
                var val = p.GetValue(obj);
                // Direct numeric arrays
                if (val is double[] arr) return arr;
                if (val is float[] farr)
                {
                    var res = new double[farr.Length];
                    for (int i = 0; i < farr.Length; i++) res[i] = farr[i];
                    return res;
                }
                if (val is System.Collections.Generic.IEnumerable<double> en)
                    return new System.Collections.Generic.List<double>(en).ToArray();
                if (val is System.Collections.IEnumerable e)
                {
                    var list = new System.Collections.Generic.List<double>();
                    foreach (var x in e)
                    {
                        if (x is IConvertible)
                        {
                            try { list.Add(Convert.ToDouble(x)); } catch { }
                        }
                    }
                    if (list.Count > 0) return list.ToArray();
                }

                // Nested containers like Joints.Values, Joints.Array, Joints.Radians, etc.
                if (val != null && !(val is string))
                {
                    var nested = TryGetDoubleArray(val, new[] { "Values", "Array", "Rad", "Radians", "Deg", "Degrees" });
                    if (nested != null) return nested;
                }
            }
            return null;
        }

        private static double? TryGetDouble(object obj, string[] propNames)
        {
            if (obj == null) return null;
            foreach (var name in propNames)
            {
                var p = GetProperty(obj.GetType(), new[] { name });
                if (p == null) continue;
                var val = p.GetValue(obj);
                if (val is double d) return d;
                if (val is float f) return f;
                if (val is IConvertible)
                {
                    try { return Convert.ToDouble(val); } catch { }
                }
            }
            return null;
        }

        private static double Average(System.Collections.Generic.List<double> vals)
        {
            if (vals == null || vals.Count == 0) return 0;
            double sum = 0; foreach (var v in vals) sum += v; return sum / vals.Count;
        }
    }
}