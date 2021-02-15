﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Newtonsoft.Json.Linq;
using System.Text;
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
        public WebHooksTFSDevOps(ILogger<WebHooksTFSDevOps> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;
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
        public IActionResult Post([FromBody] JObject payload)
        {
            string tags = "";
            //string authHeader = "";
            string pat = "";


            PayloadViewModel vm = this.BuildPayloadViewModel(payload);
            vm.pat = (!string.IsNullOrEmpty(pat)) ? pat : _appSettings.Value.AzureDevOpsToken;

            if (vm.eventType != "workitem.created")
            {
                return new OkResult();
            }

            if (vm.id == -1)
            {
                return new StatusCodeResult(500);
            }

            JsonPatchDocument patchDocument = new JsonPatchDocument
                {
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Test,
                        Path = "/rev",
                        Value = (vm.rev + 1).ToString()
                    }
                };

            if (string.IsNullOrEmpty(vm.assignedTo))
            {
                patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/System.AssignedTo",
                        Value = vm.createdBy
                    }
                );
            }

            if (!string.IsNullOrEmpty(tags))
            {
                patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/System.Tags",
                        Value = tags
                    }
                );
            }

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.IterationPath",
                    Value = vm.teamProject
                }
            );

            //var result = _workItemRepo.UpdateWorkItem(patchDocument, vm);

            return new OkResult();

        }

        private PayloadViewModel BuildPayloadViewModel(JObject body)
        {
            PayloadViewModel vm = new PayloadViewModel();

            vm.id = body["resource"]["id"] == null ? -1 : Convert.ToInt32(body["resource"]["id"].ToString());
            vm.eventType = body["eventType"] == null ? null : body["eventType"].ToString();
            vm.rev = body["resource"]["rev"] == null ? -1 : Convert.ToInt32(body["resource"]["rev"].ToString());
            vm.url = body["resource"]["url"] == null ? null : body["resource"]["url"].ToString();
            //vm.organization = org;
            vm.teamProject = body["resource"]["fields"]["System.AreaPath"] == null ? null : body["resource"]["fields"]["System.AreaPath"].ToString();
            vm.createdBy = body["resource"]["fields"]["System.CreatedBy"]["displayName"] == null ? null : body["resource"]["fields"]["System.CreatedBy"]["displayName"].ToString();
            vm.assignedTo = body["resource"]["fields"]["System.AssignedTo"] == null ? null : body["resource"]["fields"]["System.Assigned"].ToString();

            return vm;
        }        
    }
}