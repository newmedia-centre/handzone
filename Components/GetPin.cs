using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Grasshopper.Kernel;
using Handzone.Core;

namespace Handzone.Components
{
    public class GetPinComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public GetPinComponent()
            : base("Get HANDZONe PIN", "Get PIN",
                "Get a PIN code for the HANDZONe authentication process.",
                "HANDZONe", "Connection")
        {
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
            output.AddTextParameter("PIN", "P", "The PIN code to enter on the HANDZONe website", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="io">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess io)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // encode the data as JSON
                    string jsonData = JsonSerializer.Serialize(new
                    {
                        signature = State.NewSignature()
                    });
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    
                    // make the POST request and block until the result is returned
                    HttpResponseMessage response = client.PostAsync("https://handzone.tudelft.nl/api/auth/pin", content).Result;

                    // get the pin from the response
                    response.EnsureSuccessStatusCode();
                    string pin = response.Content.ReadAsStringAsync().Result;
                    
                    // set the output
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Enter the PIN after logging in on https://handzone.tudelft.nl");
                    io.SetData(0, pin);
                }
            }
            catch (HttpRequestException e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Request error: {e.Message}");
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Unexpected error: {e.Message}");
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
        public override Guid ComponentGuid => new Guid("e278e181-d5dd-434c-8d88-6dc85b2952c8");
    }
}