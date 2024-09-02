using System;
using System.Collections.Generic;
using System.Linq;
using Handzone.Core;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using Schema.Socket.Grasshopper;

namespace Handzone.Components
{
    public class MeshesComponent : GH_Component
    {
        private Mesh _mesh1;
        private Mesh _mesh2;
        private Mesh _mesh3;
        private Mesh _mesh4;
        private Mesh _mesh5;
        private ComponentButton _button;
        
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public MeshesComponent()
            : base("Send Mesh Data", "Meshes",
                "Sends meshes to the robot session",
                "HANDZONe", "Robot")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager input)
        {
            input.AddMeshParameter("Mesh 1", "1", "Program to upload", GH_ParamAccess.item);
            input.AddMeshParameter("Mesh 2", "2", "Program to upload", GH_ParamAccess.item);
            input.AddMeshParameter("Mesh 3", "3", "Program to upload", GH_ParamAccess.item);
            input.AddMeshParameter("Mesh 4", "4", "Program to upload", GH_ParamAccess.item);
            input.AddMeshParameter("Mesh 5", "5", "Program to upload", GH_ParamAccess.item);
            input[0].Optional = true;
            input[1].Optional = true;
            input[2].Optional = true;
            input[3].Optional = true;
            input[4].Optional = true;
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
            // prepare mesh 1
            io.GetData(0, ref _mesh1);
            if (_mesh1 != null)
            {
                CheckMesh(_mesh1, "Mesh 1");
            }
            
            // prepare mesh 2
            io.GetData(1, ref _mesh2);
            if (_mesh2 != null) {
                CheckMesh(_mesh2, "Mesh 2");
            }
            
            // prepare mesh 3
            io.GetData(2, ref _mesh3);
            if (_mesh3 != null) {
                CheckMesh(_mesh3, "Mesh 3");
            }
            
            // prepare mesh 4
            io.GetData(3, ref _mesh4);
            if (_mesh4 != null) {
                CheckMesh(_mesh4, "Mesh 4");
            }
            
            // prepare mesh 5
            io.GetData(4, ref _mesh5);
            if (_mesh5 != null) {
                CheckMesh(_mesh5, "Mesh 5");
            }
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
            
            // prepare the command
            GrasshopperMeshesIn grasshopperMeshesIn = new GrasshopperMeshesIn()
            {
                Mesh1 = _mesh1 == null ? null : ConvertMesh(_mesh1, "Mesh 1"),
                Mesh2 = _mesh2 == null ? null : ConvertMesh(_mesh2, "Mesh 2"),
                Mesh3 = _mesh3 == null ? null : ConvertMesh(_mesh3, "Mesh 3"),
                Mesh4 = _mesh4 == null ? null : ConvertMesh(_mesh4, "Mesh 4"),
                Mesh5 = _mesh5 == null ? null : ConvertMesh(_mesh5, "Mesh 5")
            };
            
            // send the program
            try
            {
                await State.SessionConnection.Meshes(grasshopperMeshesIn);
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                RhinoApp.InvokeOnUiThread((Action) delegate { OnDisplayExpired(true); });
            }
        }

        bool CheckMesh(Mesh mesh, string name)
        {
            // ensure vertex count limit
            if (mesh.Vertices.Count > 10000)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"{name} exceeds the maximum amount of 10.000 vertices");
                return false;
            }

            // convert to triangles
            if (!mesh.Faces.ConvertQuadsToTriangles())
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Failed to triangulate {name}");
                return false;
            }

            return true;
        }
        
        MeshData ConvertMesh(Mesh reference, string name)
        {
            Mesh mesh = reference.DuplicateMesh();
            // ensure vertex count limit
            if (mesh.Vertices.Count > 10000)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"{name} exceeds the maximum amount of 10.000 vertices");
                return null;
            }

            // scale to meters unit
            var scale = RhinoMath.UnitScale(RhinoDoc.ActiveDoc.ModelUnitSystem, UnitSystem.Meters);
            Console.WriteLine(scale);

            // convert to triangles
            if (!mesh.Faces.ConvertQuadsToTriangles())
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Failed to triangulate {name}");
                return null;
            }

            return new MeshData()
            {
                Vertices = mesh.Vertices.Select(v => new Vector3D() { X = v.X * scale, Y = v.Y * scale, Z = v.Z * scale }).ToList(),
                Tris = mesh.Faces.Select(f => new List<double>() { f.A, f.B, f.C }).ToList()
            };
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.Util.GetIcon("iconMesh");

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("bf6db9cf-3e45-417e-bec4-1ca9aa2a5dff");
    }
}