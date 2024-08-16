using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Schema.Socket.Index;

namespace Handzone.Params
{
    public sealed class SessionType: GH_Goo<JoinSessionOut>
    {
        public SessionType()
        {
            Value = null;
        }
        
        public SessionType(JoinSessionOut session)
        {
            Value = session;
        }
        
        public SessionType(SessionType source)
        {
            Value = source.Value;
        }
        
        public override IGH_Goo Duplicate()
        {
            return new SessionType(this);
        }

        public override string ToString()
        {
            return Value.Robot.Name;
        }

        public override bool IsValid => Value != null;
        public override string TypeName => "Robot Session";
        public override string TypeDescription => "Contains the information for connecting to an active robot session";
    }

    public class SessionParameter : GH_Param<SessionType>
    {
        public SessionParameter() : base(new GH_InstanceDescription("Robot Session","Session","Contains the information for connecting to an active robot session","HANDZONe", "Connection"))
        {}

        public override Guid ComponentGuid => new Guid("d01f10a7-317e-4ce2-8abd-f0241908eaa9");
    }
}