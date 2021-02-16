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

namespace WebHooksDevOps.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebHooksTFSDevOps : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WebHooksTFSDevOps> _logger;
        IOptions<AppSettings> _appSettings;
        //IWorkItemRepo _workItemRepo;
        public WebHooksTFSDevOps(ILogger<WebHooksTFSDevOps> logger, IOptions<AppSettings> appSettings/*, IWorkItemRepo WorkItemRepo*/)
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
        //[Route("workitem/new")]
        public IActionResult Post([FromBody] WorkItemModel vm)
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
            OldNewValuePair assignedTo = vm.Resource.Fields["System.AssignedTo"];
            //string teamProject = vm.Resource.Revision.Fields.SystemTeamProject.ToString();


            JsonPatchDocument patchDocument = new JsonPatchDocument
                {
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/System.Title",
                        Value = "this was the test"
                    }
                };
            
            if (string.IsNullOrEmpty(assignedTo.NewValue))
            {
                patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/System.AssignedTo",
                        Value = "Shahbaz Khan"
                    }
                );
            }

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.IterationPath",
                    Value = "ADUP Request Portal\\Sprint 1"
                }
            );
           
              
            WorkItemRepo.CreateWorkItem(patchDocument, vm);

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
