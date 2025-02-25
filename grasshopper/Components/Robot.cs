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
using Grasshopper.Kernel;
using Handzone.Core;

namespace Handzone.Components
{
    public class RobotConnectComponent : GH_Component
    {
        private string _status = "Not Connected";
        private string _name;
        private ComponentButton _button;

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public RobotConnectComponent()
            : base("Robot Connection", "Robot",
                "Manages the connection to the robot through the HANDZONe Server",
                "HANDZONe", "Connection")
        {
            State.SessionConnection.OnStatus += message =>
            {
                _status = message;
                Console.WriteLine(message);

                ExpireSolution(true);
                Rhino.RhinoApp.InvokeOnUiThread((Action)delegate { OnDisplayExpired(true); });
            };

            State.SessionConnection.OnError += message =>
            {
                _status = message;
                Console.WriteLine(message);

                ExpireSolution(true);

                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, message);
                Rhino.RhinoApp.InvokeOnUiThread((Action)delegate { OnDisplayExpired(true); });
            };

            State.SessionConnection.OnConnectionChange += connected =>
            {
                if (_button == null) return;

                if (connected)
                {
                    _button.Label = "Disconnect";
                    _button.Action = Disconnect;

                    Rhino.RhinoApp.InvokeOnUiThread((Action)delegate { OnDisplayExpired(true); });
                }
                else
                {
                    _button.Label = "Connect";
                    _button.Action = Connect;

                    Rhino.RhinoApp.InvokeOnUiThread((Action)delegate { OnDisplayExpired(true); });
                }
            };
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager input)
        {
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager output)
        {
            output.AddTextParameter("Status", "S", "The status of the connection", GH_ParamAccess.item);
            output.AddTextParameter("Name", "N", "The name of the connected robot", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="io">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess io)
        {
            io.SetData(0, _status);
            io.SetData(1, _name);
        }

        public override void CreateAttributes()
        {
            _button = new ComponentButton(this, "Connect", Connect);
            m_attributes = _button;
        }

        async void Connect()
        {
            if (!State.GlobalConnection.Connected)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Not connected to HANDZONe Server");
                Rhino.RhinoApp.InvokeOnUiThread((Action)delegate { OnDisplayExpired(true); });

                return;
            }

            try
            {
                // get the current session
                var session = await State.GlobalConnection.GetNamespace();

                // update the outputs
                _name = session.Robot.Name;
                ExpireSolution(true);
                Rhino.RhinoApp.InvokeOnUiThread((Action)delegate { OnDisplayExpired(true); });

                // connect to the robot session
                State.SessionConnection.TryConnectToSession(session);
            }
            catch (Exception e)
            {
                _status = "Failed to connect to robot.";

                ExpireSolution(true);
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                Rhino.RhinoApp.InvokeOnUiThread((Action)delegate { OnDisplayExpired(true); });
            }
        }

        void Disconnect()
        {
            State.SessionConnection.Disconnect();
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.Util.GetIcon("iconRobot");

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("0aec24b4-0348-466c-8e60-c3738438ed2b");
    }
}