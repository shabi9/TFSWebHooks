using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using WebHooksDevOps.ViewModels;
using WorkItem = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem;

namespace WebHooksDevOps
{
    public class WorkItemRepo/* : IWorkItemRepo*/
    {
        private IOptions<AppSettings> _appSettings;       
        static WorkItemTrackingHttpClient WitClient;

        public WorkItemRepo(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        public static void connectToTFS(WorkItemModel vm)
        {
            try
            {

                // Uri baseUri = new Uri("");
                string baseUri = "https://dev.azure.com/shabi9/";
                string pat = "jcs7a2xjaytizhlijntwfiwm6sgtp5uto4ppmw7bz67tcnm4cpna";
                //string projectName = "";               

                VssConnection connection = new VssConnection(new Uri(baseUri), new VssBasicCredential(string.Empty, pat));
                WitClient = connection.GetClient<WorkItemTrackingHttpClient>();
               // CreateWorkItem(patchDocument, projectName, "Epic");
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static WorkItem CreateWorkItem(JsonPatchDocument patchDocument, WorkItemModel vm)
        {
            try
            {
                connectToTFS(vm);
                string projectName = vm.Resource.Revision.Fields.SystemTeamProject.ToString();
                return WitClient.CreateWorkItemAsync(patchDocument, projectName, "Epic").Result;
            }
            catch (Exception ex)
            {
                throw;
            }

        }


    }

    //public interface IWorkItemRepo
    //{
    //    WorkItem connectToTFS(JsonPatchDocument patchDocument, WorkItemModel vm);
    //}

}
