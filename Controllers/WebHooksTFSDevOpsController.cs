﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using WebHooksDevOps.Models;
using JsonPatchDocument = Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchDocument;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Text.Json;
using WebHooksDevOps.Services;

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
        IWorkItemRepo _workItemRepo;
        public WebHooksTFSDevOpsController(ILogger<WebHooksTFSDevOpsController> logger, IOptions<AppSettings> appSettings, IWorkItemRepo workItemRepo)
        {
            _logger = logger;
            _appSettings = appSettings;
            _workItemRepo = workItemRepo;
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
            if (vm.EventType != "workitem.updated")
            {
                return new OkResult();
            }

            if (vm.Resource.WorkItemId == -1)
            {
                return new StatusCodeResult(500);
            }

            //JsonElement o = JsonSerializer.Deserialize<JsonElement>();
            //if (o.TryGetProperty("System.AssignedTo", out var Value))
            //{
            //}

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
            
            if (vm.Resource.Fields.ContainsKey("System.AssignedTo"))
            {
                var assignedToElement = vm.Resource.Fields["System.AssignedTo"].NewValue;
                var assignedTo = assignedToElement.GetString();
                patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Replace,
                    Path = "/fields/System.AssignedTo",
                    Value = "Shahbaz Khan"
                }
                );
            }

            patchDocument.Add(
            new JsonPatchOperation()
            {
                Operation = Operation.Replace,
                Path = "/fields/System.IterationPath",
                Value = "ADUP Request Portal\\Sprint 3"
            }
        );


            await _workItemRepo.UpdateWorkItem(patchDocument, vm);

            return new OkResult();

        }
        [HttpPost]
        [Route("workitem/new")]
        public async Task<IActionResult> Post([FromBody] CreateWorkItemModel vm)
        {
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


            await _workItemRepo.CreateWorkItem(patchDocument, vm);

            return new OkResult();

        }
      
    }
}
