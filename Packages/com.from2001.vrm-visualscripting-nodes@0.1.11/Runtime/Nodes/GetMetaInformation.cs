using UnityEngine;
using Unity.VisualScripting;
using UniVRM10;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VrmVisualScriptingNodes
{
    [UnitTitle("Get Meta Information of VRM")]
    [UnitShortTitle("VRM Meta info")]
    [UnitCategory("VRM")]
    [UnitSubtitle("Get Meta Information of VRM")]
    public class GetMetaInformation : Unit
    {
        [DoNotSerialize]
        public ControlInput inputTrigger;

        [DoNotSerialize]
        public ControlOutput outputTrigger;

        [DoNotSerialize]
        public ValueInput vrmGameObject;

        [DoNotSerialize]
        public ValueOutput m_thumbnail;
        [DoNotSerialize]
        public ValueOutput m_name;
        [DoNotSerialize]
        public ValueOutput m_version;
        [DoNotSerialize]
        public ValueOutput m_copyright;
        [DoNotSerialize]
        public ValueOutput m_authors;
        [DoNotSerialize]
        public ValueOutput m_references;
        [DoNotSerialize]
        public ValueOutput m_contact;
        [DoNotSerialize]
        public ValueOutput m_thirdPartyLicenses;
        [DoNotSerialize]
        public ValueOutput m_OtherLicenseUrl;
        [DoNotSerialize]
        public ValueOutput m_AvatarPermission;
        [DoNotSerialize]
        public ValueOutput m_ViolentUssage;
        [DoNotSerialize]
        public ValueOutput m_SexualUssage;
        [DoNotSerialize]
        public ValueOutput m_CommercialUssage;
        [DoNotSerialize]
        public ValueOutput m_PoliticalOrReligiousUsage;
        [DoNotSerialize]
        public ValueOutput m_AntisocialOrHateUsage;
        [DoNotSerialize]
        public ValueOutput m_CreditNotation;
        [DoNotSerialize]
        public ValueOutput m_Redistribution;
        [DoNotSerialize]
        public ValueOutput m_Modification;

        private class MetaInfo
        {
            public Texture2D m_thumbnail;
            public string m_name;
            public string m_version;
            public string m_copyright;
            public string[] m_authors;
            public string[] m_references;
            public string m_contact;
            public string m_thirdPartyLicenses;
            public string m_OtherLicenseUrl;
            public int m_AvatarPermission;
            public bool m_ViolentUssage;
            public bool m_SexualUssage;
            public int m_CommercialUssage;
            public bool m_PoliticalOrReligiousUsage;
            public bool m_AntisocialOrHateUsage;
            public int m_CreditNotation;
            public bool m_Redistribution;
            public int m_Modification;
        }
        private MetaInfo resultMetaInfo = new MetaInfo();


        [DoNotSerialize]
        public ValueOutput result_thumbnail;
        [DoNotSerialize]
        public ValueOutput result_name;
        [DoNotSerialize]
        public ValueOutput result_version;
        [DoNotSerialize]
        public ValueOutput result_copyright;
        [DoNotSerialize]
        public ValueOutput result_authors;
        [DoNotSerialize]
        public ValueOutput result_references;
        [DoNotSerialize]
        public ValueOutput result_contact;
        [DoNotSerialize]
        public ValueOutput result_thirdPartyLicenses;
        [DoNotSerialize]
        public ValueOutput result_OtherLicenseUrl;
        [DoNotSerialize]
        public ValueOutput result_AvatarPermission;
        [DoNotSerialize]
        public ValueOutput result_ViolentUssage;
        [DoNotSerialize]
        public ValueOutput result_SexualUssage;
        [DoNotSerialize]
        public ValueOutput result_CommercialUssage;
        [DoNotSerialize]
        public ValueOutput result_PoliticalOrReligiousUsage;
        [DoNotSerialize]
        public ValueOutput result_AntisocialOrHateUsage;
        [DoNotSerialize]
        public ValueOutput result_CreditNotation;
        [DoNotSerialize]
        public ValueOutput result_Redistribution;
        [DoNotSerialize]
        public ValueOutput result_Modification;


        protected override void Definition()
        {
            inputTrigger = ControlInput("inputTrigger", Enter);
            outputTrigger = ControlOutput("outputTrigger");
            vrmGameObject = ValueInput<GameObject>("VRM GameObject");
            
            result_thumbnail = ValueOutput<Texture2D>("Thumbnail", (flow) => resultMetaInfo.m_thumbnail );
            result_name = ValueOutput<string>("Name", (flow) => resultMetaInfo.m_name );
            result_version = ValueOutput<string>("Version", (flow) => resultMetaInfo.m_version );
            result_copyright = ValueOutput<string>("Copyright", (flow) => resultMetaInfo.m_copyright);
            result_authors = ValueOutput<string[]>("Authors", (flow) => resultMetaInfo.m_authors );
            result_references = ValueOutput<string[]>("References", (flow) => resultMetaInfo.m_references );
            result_contact = ValueOutput<string>("Contact", (flow) => resultMetaInfo.m_contact );
            result_thirdPartyLicenses = ValueOutput<string>("ThirdPartyLicenses", (flow) => resultMetaInfo.m_thirdPartyLicenses );
            result_OtherLicenseUrl = ValueOutput<string>("OtherLicenseUrl", (flow) => resultMetaInfo.m_OtherLicenseUrl );
            result_AvatarPermission = ValueOutput<int>("AvatarPermission", (flow) => resultMetaInfo.m_AvatarPermission );
            result_ViolentUssage = ValueOutput<bool>("ViolentUssage", (flow) => resultMetaInfo.m_ViolentUssage );
            result_SexualUssage = ValueOutput<bool>("SexualUssage", (flow) => resultMetaInfo.m_SexualUssage );
            result_CommercialUssage = ValueOutput<int>("CommercialUssage", (flow) => resultMetaInfo.m_CommercialUssage );
            result_PoliticalOrReligiousUsage = ValueOutput<bool>("PoliticalOrReligiousUsage", (flow) => resultMetaInfo.m_PoliticalOrReligiousUsage );
            result_AntisocialOrHateUsage = ValueOutput<bool>("AntisocialOrHateUsage", (flow) => resultMetaInfo.m_AntisocialOrHateUsage );
            result_CreditNotation = ValueOutput<int>("CreditNotation", (flow) => resultMetaInfo.m_CreditNotation );
            result_Redistribution = ValueOutput<bool>("Redistribution", (flow) => resultMetaInfo.m_Redistribution );
            result_Modification = ValueOutput<int>("Modification", (flow) => resultMetaInfo.m_Modification );
        }

        private ControlOutput Enter(Flow flow)
        {
            Vrm10Instance vrmInstance  = flow.GetValue<GameObject>(vrmGameObject).GetComponent<Vrm10Instance>();
            var meta = vrmInstance.Vrm.Meta;
            UniGLTF.Extensions.VRMC_vrm.CreditNotationType creditn = meta.CreditNotation;

            resultMetaInfo.m_thumbnail = meta.Thumbnail;
            resultMetaInfo.m_name = meta.Name;
            resultMetaInfo.m_version = meta.Version;
            resultMetaInfo.m_copyright = meta.CopyrightInformation;
            resultMetaInfo.m_authors = meta.Authors.ToArray();
            resultMetaInfo.m_references = meta.References.ToArray();
            resultMetaInfo.m_contact = meta.ContactInformation;
            resultMetaInfo.m_thirdPartyLicenses = meta.ThirdPartyLicenses;
            resultMetaInfo.m_OtherLicenseUrl = meta.OtherLicenseUrl;
            resultMetaInfo.m_AvatarPermission = (int)meta.AvatarPermission;
            resultMetaInfo.m_ViolentUssage = meta.ViolentUsage;
            resultMetaInfo.m_SexualUssage = meta.SexualUsage;
            resultMetaInfo.m_CommercialUssage = (int)meta.CommercialUsage;
            resultMetaInfo.m_PoliticalOrReligiousUsage = meta.PoliticalOrReligiousUsage;
            resultMetaInfo.m_AntisocialOrHateUsage = meta.AntisocialOrHateUsage;
            resultMetaInfo.m_CreditNotation = (int)meta.CreditNotation;
            resultMetaInfo.m_Redistribution = meta.Redistribution;
            resultMetaInfo.m_Modification = (int)meta.Modification;

            return outputTrigger;
        }
    }

}

