using System;
using Handzone.Core;
using Grasshopper.Kernel;
using Schema.Socket.Grasshopper;

namespace Handzone.Components
{
    public class SimulateComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public SimulateComponent()
            : base("Simulate on Robot", "Simulate",
                "Runs a robots program on the robot",
                "HANDZONe", "Robot")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager input)
        {
            input.AddBooleanParameter("Run", "R", "Whether the simulation should play or stop", GH_ParamAccess.item);
            input.AddNumberParameter("Speed", "S", "The speed at which the simulation should run", GH_ParamAccess.item);
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
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="io">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess io)
        {
            if (!State.RobotConnection.IsConnected)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Not connected to robot");
                return;
            }

            // get the program
            bool run = false;
            double speed = 0.0;
            io.GetData(0, ref run);
            io.GetData(1, ref speed);
            
            // convert the program
            GrasshopperSimulateIn grasshopperSimulateIn = new GrasshopperSimulateIn()
            {
                Run = run,
                Speed = speed
            };
            
            // send the program
            try
            {
                State.RobotConnection.SendSimulate(grasshopperSimulateIn);
                io.SetData(0, State.RobotConnection.Status);
                io.SetData(1, State.RobotConnection.Info.Name);
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
            }
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
        public override Guid ComponentGuid => new Guid("c4a1d6d1-a33e-4ee7-a865-3b3b2520a7ad");
    }
}