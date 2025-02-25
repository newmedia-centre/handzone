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

            if (_program == null || _program.Code == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Program code is null");
                return;
            }

            // convert the program
            GrasshopperProgramIn grasshopperProgramIn = new GrasshopperProgramIn()
            {
                Program = string.Join("\n", _program.Code[0][0])
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
}