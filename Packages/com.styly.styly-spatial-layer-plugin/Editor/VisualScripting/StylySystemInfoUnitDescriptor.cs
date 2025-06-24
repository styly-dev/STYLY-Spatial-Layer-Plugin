using Unity.VisualScripting;

namespace Styly.SpatialLayer.Plugin.VisualScripting
{

    
    [Descriptor(typeof(GetSystemInfoUnit))]
    public class GetSystemInfoUnitDescriptor : UnitDescriptor<GetSystemInfoUnit>
    {
        public GetSystemInfoUnitDescriptor(GetSystemInfoUnit unit) : base(unit) {}
    
        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);
            if (port == unit.key)
            {
                description.summary = "The identifier for the specific system information to retrieve.";
            }else if (port == unit.value)
            {
                description.summary = "The system information associated with the specified key.";
            }
        }
    }
}

