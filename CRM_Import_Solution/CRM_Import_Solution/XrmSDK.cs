using System;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System.ServiceModel;
using System.IO;


namespace CRM_Import_Solution
{
    public class XrmSDK
    {
        private static CrmServiceClient _client;
        public static void Main(String managedSolutionLocation, String crmUrl, String organization)
        {
            Guid _importJobId = Guid.NewGuid();
            try
            {
                using (_client = new CrmServiceClient("Url=" + crmUrl + organization + "; authtype=AD; RequireNewInstance=True;"))
                {
                    try
                    {
                        Console.WriteLine("----------------------------------------------------------------------");
                        if (_client.IsReady == false)
                        {
                            Console.WriteLine("No se pudo establecer la conexion verifique la fuente");
                            return;
                        }
                        byte[] fileBytes = File.ReadAllBytes(managedSolutionLocation);
                        ImportSolutionRequest impSolReq = new ImportSolutionRequest()
                        {
                            CustomizationFile = fileBytes,
                            ImportJobId = _importJobId,
                            OverwriteUnmanagedCustomizations = true,
                            SkipProductUpdateDependencies = true,
                            PublishWorkflows = true,
                        };
                        ImportSolutionResponse impSolResp = new ImportSolutionResponse();
                        _client.Execute(impSolReq);
                        // On success escribe el customization file .xml
                        CreateFile(impSolReq.ImportJobId, managedSolutionLocation, ".xml");
                    }
                    catch (FaultException<OrganizationServiceFault> ex)
                    {
                        string _voidMessageDumpError = ex.Message;
                        //No hago nada con el error ya que el metodo create file lo busca en tabla ImportJob y escribe si hay error en el customization file 
                        CreateFile(_importJobId, managedSolutionLocation, ".xml");
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                string message = ex.Message;
                throw;
            }
        }
        private static void CreateFile(Guid importJobIDGuid, String pathFile, String format)
        {
            RetrieveFormattedImportJobResultsRequest formatedImportJobRequest = new RetrieveFormattedImportJobResultsRequest
            {
                ImportJobId = importJobIDGuid
            };

            RetrieveFormattedImportJobResultsResponse formatedImportJobResponse = (RetrieveFormattedImportJobResultsResponse)_client.Execute(formatedImportJobRequest);
            int i = 0;
            string path = pathFile.Substring(0, pathFile.Length - 4) + format;
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.Write(formatedImportJobResponse.FormattedResults);
                }
            }
            else
            {
                while (File.Exists(path))
                {
                    i++;
                    String fileName = pathFile.Substring(0, pathFile.Length - 4) + "(" + i.ToString() + ")";
                    path = fileName + format;
                }
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.Write(formatedImportJobResponse.FormattedResults);
                }
            }
        }
    }
}
