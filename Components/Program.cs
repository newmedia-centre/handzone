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
            IProgram program = null;
            io.GetData(0, ref program);
            if (program == null || program.Code == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Program code is null");
                return;
            }
            
            // convert the program
            GrasshopperProgramIn grasshopperProgramIn = new GrasshopperProgramIn()
            {
                Program = string.Join("\n", program.Code[0][0])
            };
            
            // send the program
            try
            {
                State.RobotConnection.SendProgram(grasshopperProgramIn);
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
        public override Guid ComponentGuid => new Guid("45b8d103-8a21-47b0-94da-0e4f1b1e1038");
    }
}