using System;
using System.Drawing;
using Grasshopper;
using Grasshopper.Kernel;

namespace Handzone
{
    public class HandzoneInfo : GH_AssemblyInfo
    {
        public override string Name => "HANDZONe";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("027257EA-EECE-452E-8F27-3CF501325217");

        //Return a string identifying you or your company.
        public override string AuthorName => "Delft University of Technology - NewMedia Centre";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "https://newmediacentre.tudelft.nl/contact/#contact_view";
    }
}