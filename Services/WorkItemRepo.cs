<<<<<<< HEAD:Services/WorkItemRepo.cs
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using WebHooksDevOps.Models;
using WorkItem = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem;

namespace WebHooksDevOps.Services
{
    public class WorkItemRepo: IWorkItemRepo
    {               
        static WorkItemTrackingHttpClient WitClient;
        private readonly IOptions<AppSettings> _appSettings;      

        public WorkItemRepo(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        private void ConnectToTFS()
        {
            try
            {
                // Uri baseUri = new Uri("");
                string baseUri = _appSettings.Value.AzureDevOpsOrgUrl;
                string pat = _appSettings.Value.AzureDevOpsToken;
                //string projectName = "";               
                VssConnection connection = new VssConnection(new Uri(baseUri), new VssBasicCredential(string.Empty, pat));
                WitClient = connection.GetClient<WorkItemTrackingHttpClient>();               
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public  async Task<WorkItem> CreateWorkItem(JsonPatchDocument patchDocument, CreateWorkItemModel vm)
        {
            try
            {
                ConnectToTFS();
                string projectName = vm.Resource.Revision.Fields.SystemTeamProject.ToString();
                return await WitClient.CreateWorkItemAsync(patchDocument, projectName, "Epic");
            }
            catch (Exception ex)
            {
                throw;
            }

        }

      
        public async Task<WorkItem> UpdateWorkItem(JsonPatchDocument patchDocument, UpdateWorkItemModel vm)
        {
            try
            {
                ConnectToTFS();
                string projectName = vm.Resource.Revision.Fields.SystemTeamProject.ToString();
                return await WitClient.UpdateWorkItemAsync(patchDocument, (int)vm.Resource.WorkItemId);
            }
            catch (Exception ex)
            {
                throw;
            }

        }


    }


    public interface IWorkItemRepo
    {
        Task<WorkItem> CreateWorkItem(JsonPatchDocument patchDocument, CreateWorkItemModel vm);
        Task<WorkItem> UpdateWorkItem(JsonPatchDocument patchDocument, UpdateWorkItemModel vm);
    }

}
