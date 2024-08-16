using System;
using System.Threading;
using Grasshopper.Kernel;
using GrasshopperAsyncComponent;

namespace Handzone.Components
{
    public class ConnectComponent : GH_AsyncComponent
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ConnectComponent()
            : base("Connect to Server", "Connect", "Initializes the log in process to connect to the HANDZONe Server", "HANDZONe", "Connection")
        {
            BaseWorker = new ConnectWorker();
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager input)
        {
            input.AddBooleanParameter("Run", "R", "Runs this component", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager output)
        {
            output.AddTextParameter("Status", "S", "The status of the connection", GH_ParamAccess.item);
            output.AddTextParameter("PIN", "P", "The PIN code to enter on the HANDZONe website", GH_ParamAccess.item);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => null;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("a988b35b-382b-4596-bd95-5f3050f66f0f");
        
        
        public override GH_Exposure Exposure => GH_Exposure.primary;
    }
    
    public class ConnectWorker : WorkerInstance
    {
        private bool _connected;
        private bool _run;
        private bool _running;

        public ConnectWorker() : base(null) { }

        public override void DoWork(Action<string, double> ReportProgress, Action Done)
        {
            // Checking for cancellation
            if (CancellationToken.IsCancellationRequested) { return; }
            
            // Only connect when run is true
            if (!_run && !_running) { return; }

            int i = 0;
            _running = true;

            while (!_connected)
            {
                ReportProgress(Id, i + 1);
                i++;

                if (i > 100)
                {
                    _connected = true;
                }
                
                Thread.Sleep(1000);

                // Checking for cancellation
                if (CancellationToken.IsCancellationRequested) { return; }
            }

            _running = false;

            Done();
        }

        public override WorkerInstance Duplicate() => new ConnectWorker();

        public override void GetData(IGH_DataAccess DA, GH_ComponentParamServer Params)
        {
            if (CancellationToken.IsCancellationRequested) return;

            DA.GetData(0, ref _run);
        }

        public override void SetData(IGH_DataAccess DA)
        {
            if (CancellationToken.IsCancellationRequested) return;
            DA.SetData(0, $"Hello world. Worker {Id} has status {_connected}");
        }
    }
}