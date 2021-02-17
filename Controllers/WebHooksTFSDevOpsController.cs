using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using WebHooksDevOps.ViewModels;
using JsonPatchDocument = Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchDocument;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace WebHooksDevOps.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebHooksTFSDevOpsController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WebHooksTFSDevOpsController> _logger;
        IOptions<AppSettings> _appSettings;
        //IWorkItemRepo _workItemRepo;
        public WebHooksTFSDevOpsController(ILogger<WebHooksTFSDevOpsController> logger, IOptions<AppSettings> appSettings/*, IWorkItemRepo WorkItemRepo*/)
        {
            _logger = logger;
            _appSettings = appSettings;
            //_workItemRepo = WorkItemRepo;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        [Route("workitem/update")]
        public async Task<IActionResult> Post([FromBody] UpdateWorkItemModel vm)
        {
            string tags = "";

            if (vm.EventType != "workitem.updated")
            {
                return new OkResult();
            }

            if (vm.Resource.WorkItemId == -1)
            {
                return new StatusCodeResult(500);
            }
            var assignedToElement = vm.Resource.Fields["System.AssignedTo"].NewValue;
            var assignedTo = assignedToElement.GetString();
            string teamProject = vm.Resource.Revision.Fields.SystemTeamProject.ToString();
            string iterationPath = vm.Resource.Revision.Fields.SystemIterationPath.ToString();

            JsonPatchDocument patchDocument = new JsonPatchDocument
                {
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Replace,
                        Path = "/fields/System.Title",
                        Value = "this was the updated by listener"
                    }
                };

            if (string.IsNullOrEmpty(assignedTo))
            {
                patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Replace,
                        Path = "/fields/System.AssignedTo",
                        Value = assignedTo
                    }
                );
            }

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Replace,
                    Path = "/fields/System.IterationPath",
                    Value = iterationPath
                }
            );


            await WorkItemRepo.UpdateWorkItem(patchDocument, vm);

            return new OkResult();

        }
        [HttpPost]
        [Route("workitem/new")]
        public async Task<IActionResult> Post([FromBody] CreateWorkItemModel vm)
        {
            string tags = "";

            if (vm.EventType != "workitem.created")
            {
                return new OkResult();
            }

            if (vm.Resource.WorkItemId == -1)
            {
                return new StatusCodeResult(500);
            }
            string assignedTo = (string)vm.Resource.Fields["System.AssignedTo"].ToString();
            string teamProject = vm.Resource.Revision.Fields.SystemTeamProject.ToString();
            string iterationPath = vm.Resource.Revision.Fields.SystemIterationPath.ToString();

            JsonPatchDocument patchDocument = new JsonPatchDocument
                {
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/System.Title",
                        Value = "this was the test for the new Epic"
                    }
                };

            if (string.IsNullOrEmpty(assignedTo))
            {
                patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/System.AssignedTo",
                        Value = assignedTo
                    }
                );
            }

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.IterationPath",
                    Value = iterationPath
                }
            );


            await WorkItemRepo.CreateWorkItem(patchDocument, vm);

            return new OkResult();

        }
        //private PayloadViewModel BuildPayloadViewModel(JObject body)
        //{
        //    PayloadViewModel vm = new PayloadViewModel();

        //    vm.id = body["resource"]["id"] == null ? -1 : Convert.ToInt32(body["resource"]["id"].ToString());
        //    vm.eventType = body["eventType"] == null ? null : body["eventType"].ToString();
        //    vm.rev = body["resource"]["rev"] == null ? -1 : Convert.ToInt32(body["resource"]["rev"].ToString());
        //    vm.url = body["resource"]["url"] == null ? null : body["resource"]["url"].ToString();
        //    //vm.organization = org;
        //    vm.teamProject = body["resource"]["fields"]["System.AreaPath"] == null ? null : body["resource"]["fields"]["System.AreaPath"].ToString();
        //    vm.createdBy = body["resource"]["fields"]["System.CreatedBy"]["displayName"] == null ? null : body["resource"]["fields"]["System.CreatedBy"]["displayName"].ToString();
        //    vm.assignedTo = body["resource"]["fields"]["System.AssignedTo"] == null ? null : body["resource"]["fields"]["System.Assigned"].ToString();

        //    return vm;
        //}        
    }
}
