using System;
using Grasshopper.Kernel;
using Handzone.Core;

namespace Handzone.Components
{
    public class ServerConnectComponent : GH_Component
    {
        private string _pin;
        private string _status = "Not Connected";
        private ComponentButton _button;
        
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ServerConnectComponent()
            : base("Server Connection", "Server",
                "Manages the connection to the HANDZONe Server",
                "HANDZONe", "Connection")
        {
            State.GlobalConnection.OnStatus += message =>
            {
                _status = message;
                Console.WriteLine(message);
                
                ExpireSolution(true);
                Rhino.RhinoApp.InvokeOnUiThread((Action) delegate { OnDisplayExpired(true); });
            };

            State.GlobalConnection.OnError += message =>
            {
                _status = message;
                Console.WriteLine(message);
                
                ExpireSolution(true);
                
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, message);
                Rhino.RhinoApp.InvokeOnUiThread((Action) delegate { OnDisplayExpired(true); });
            };

            State.GlobalConnection.OnConnectionChange += connected =>
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
            input.AddTextParameter("PIN", "P", "The PIN code to entered on the HANDZONe website", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager output)
        {
            output.AddTextParameter("Status", "S", "The status of the connection", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="io">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess io)
        {
            io.GetData(0, ref _pin);
            io.SetData(0, _status);
        }
        
        public override void CreateAttributes()
        {
            _button = new ComponentButton(this, "Connect", Connect);
            m_attributes = _button;
        }

        void Connect()
        {
            if (_pin != null)
            {
                State.GlobalConnection.TryConnectToGlobalServer(_pin);
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"PIN input not connected");
            }
            
        }

        void Disconnect()
        {
            State.GlobalConnection.Disconnect();
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.Util.GetIcon("iconServer");

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("e4e38df9-4fbf-4f53-82b5-fcfee7cc0852");
    }
}