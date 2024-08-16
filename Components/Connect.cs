using System;
using System.Threading;
using Grasshopper.Kernel;
using GrasshopperAsyncComponent;
using Handzone.Core;


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
            output.AddTextParameter("Robot", "R", "The connected robot", GH_ParamAccess.item);
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
        private string _pin;
        private int _seconds;
        private const int MaxSeconds = 60 * 15;

        public ConnectWorker() : base(null) { }

        public override void DoWork(Action<string, double> status, Action done)
        {
            // Checking for cancellation
            if (CancellationToken.IsCancellationRequested) { return; }
            
            // connect to server
            State.ServerConnection.Connect(_pin);
            Console.WriteLine("Connecting to Server...");

            // wait for connection to establish
            while (!State.ServerConnection.IsConnected && !State.ServerConnection.IsErrored && _seconds < MaxSeconds)
            {
                status(Id, (double)_seconds / MaxSeconds);
                
                // sleep for 1 second
                Thread.Sleep(1000);
                _seconds++;
                
                // Checking for cancellation
                if (CancellationToken.IsCancellationRequested) { return; }
            }
            
            // set status to 1
            status(Id, 1);

            // connect to the robot
            var session = State.ServerConnection.GetActiveSession();
            State.NewRobotConnection(session);
            Console.WriteLine("Connected to Robot...");
            
            // wait for connection to establish
            while (!State.RobotConnection.IsConnected && !State.RobotConnection.IsErrored)
            {
                Thread.Sleep(1000);
                
                // Checking for cancellation
                if (CancellationToken.IsCancellationRequested) { return; }
            }

            done();
        }

        public override WorkerInstance Duplicate() => new ConnectWorker();

        public override void GetData(IGH_DataAccess io, GH_ComponentParamServer paramServer)
        {
            if (CancellationToken.IsCancellationRequested) return;

            io.GetData(0, ref _pin);
        }

        public override void SetData(IGH_DataAccess io)
        {
            if (CancellationToken.IsCancellationRequested) return;
            io.SetData(0, State.ServerConnection.Status);
            io.SetData(1, State.RobotConnection.Info.Name);
        }
    }
}