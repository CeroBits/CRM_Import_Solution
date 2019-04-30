using System.Management.Automation;

namespace CRM_Import_Solution
{
        [Cmdlet(VerbsCommon.Get, "CRM_Import_Solution")]
        public class CRM_Import_Solution : Cmdlet
        {
            [Parameter]
            public string ManagedSolutionLocation { get; set; }
            [Parameter]
            public string CrmUrl { get; set; }
            [Parameter]
            public string Organization { get; set; }


            protected override void ProcessRecord()
            {
                XrmSDK.Main(ManagedSolutionLocation, CrmUrl, Organization);
            }

        
    }
}
