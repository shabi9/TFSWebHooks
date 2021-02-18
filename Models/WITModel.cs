using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace WebHooksDevOps.Models
{

    public class BaseWorkItemModel
    {
        [JsonPropertyName("subscriptionId")]
        public Guid SubscriptionId { get; set; }

        [JsonPropertyName("notificationId")]
        public long NotificationId { get; set; }

        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("eventType")]
        public string EventType { get; set; }

        [JsonPropertyName("publisherId")]
        public string PublisherId { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; }

        [JsonPropertyName("detailedMessage")]
        public Message DetailedMessage { get; set; }

       

        [JsonPropertyName("resourceVersion")]
        public string ResourceVersion { get; set; }

        [JsonPropertyName("resourceContainers")]
        public ResourceContainers ResourceContainers { get; set; }

        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; }

    }
    public partial class CreateWorkItemModel:BaseWorkItemModel
    {
        [JsonPropertyName("resource")]
        public CreateResource Resource { get; set; }

    }

    public partial class UpdateWorkItemModel : BaseWorkItemModel
    {
        [JsonPropertyName("resource")]
        public UpdateResource Resource { get; set; }

    }

    public partial class Message
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("html")]
        public string Html { get; set; }

        [JsonPropertyName("markdown")]
        public string Markdown { get; set; }
    }

    public class BaseResource
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("workItemId")]
        public long WorkItemId { get; set; }

        [JsonPropertyName("rev")]
        public long Rev { get; set; }

        [JsonPropertyName("revisedBy")]
        public object RevisedBy { get; set; }

        [JsonPropertyName("revisedDate")]
        public DateTime RevisedDate { get; set; }

       

        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        [JsonPropertyName("url")]
        public Uri Url { get; set; }

        [JsonPropertyName("revision")]
        public Revision Revision { get; set; }

    }

    public partial class CreateResource: BaseResource
    {
        [JsonPropertyName("fields")]
        public Dictionary<string, object> Fields { get; set; }

    }

    public partial class UpdateResource : BaseResource
    {
        [JsonPropertyName("fields")]
        public Dictionary<string, OldNewValuePair> Fields { get; set; }

    }


    //public partial class ResourceFields
    //{
    //    [JsonPropertyName("System.Rev")]
    //    public OldNewValuePair SystemRev { get; set; }

    //    [JsonPropertyName("System.AuthorizedDate")]
    //    public OldNewValuePair SystemAuthorizedDate { get; set; }

    //    [JsonPropertyName("System.RevisedDate")]
    //    public OldNewValuePair SystemRevisedDate { get; set; }

    //    [JsonPropertyName("System.State")]
    //    public OldNewValuePair SystemState { get; set; }

    //    [JsonPropertyName("System.Reason")]
    //    public OldNewValuePair SystemReason { get; set; }

    //    [JsonPropertyName("System.AssignedTo")]
    //    public SystemAssignedTo SystemAssignedTo { get; set; }

    //    [JsonPropertyName("System.ChangedDate")]
    //    public OldNewValuePair SystemChangedDate { get; set; }

    //    [JsonPropertyName("System.Watermark")]
    //    public OldNewValuePair SystemWatermark { get; set; }

    //    [JsonPropertyName("Microsoft.VSTS.Common.Severity")]
    //    public OldNewValuePair MicrosoftVstsCommonSeverity { get; set; }
    //}

    public partial class OldNewValuePair
    {
        [JsonPropertyName("oldValue")]
        public JsonElement OldValue { get; set; }

        [JsonPropertyName("newValue")]
        public JsonElement NewValue { get; set; }
    }

    public partial class SystemAssignedTo
    {
        [JsonPropertyName("newValue")]
        public string NewValue { get; set; }
    }

    public partial class Links
    {
        [JsonPropertyName("self")]
        public Parent Self { get; set; }

        [JsonPropertyName("parent")]
        public Parent Parent { get; set; }

        [JsonPropertyName("workItemUpdates")]
        public Parent WorkItemUpdates { get; set; }
    }

    public partial class Parent
    {
        [JsonPropertyName("href")]
        public Uri Href { get; set; }
    }

    public partial class Revision
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("rev")]
        public long Rev { get; set; }

        [JsonPropertyName("fields")]
        public RevisionFields Fields { get; set; }

        [JsonPropertyName("url")]
        public Uri Url { get; set; }
    }

    public partial class RevisionFields
    {
        [JsonPropertyName("System.AreaPath")]
        public string SystemAreaPath { get; set; }

        [JsonPropertyName("System.TeamProject")]
        public string SystemTeamProject { get; set; }

        [JsonPropertyName("System.IterationPath")]
        public string SystemIterationPath { get; set; }

        [JsonPropertyName("System.WorkItemType")]
        public string SystemWorkItemType { get; set; }

        [JsonPropertyName("System.State")]
        public string SystemState { get; set; }

        [JsonPropertyName("System.Reason")]
        public string SystemReason { get; set; }

        [JsonPropertyName("System.CreatedDate")]
        public DateTime SystemCreatedDate { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [JsonPropertyName("System.CreatedBy")]
        public JsonElement SystemCreatedBy { get; set; }

        [JsonPropertyName("System.ChangedDate")]
        public DateTime SystemChangedDate { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [JsonPropertyName("System.ChangedBy")]
        public JsonElement SystemChangedBy { get; set; }

        [JsonPropertyName("System.Title")]
        public string SystemTitle { get; set; }

        [JsonPropertyName("Microsoft.VSTS.Common.Severity")]
        public string MicrosoftVstsCommonSeverity { get; set; }

        [JsonPropertyName("WEF_EB329F44FE5F4A94ACB1DA153FDF38BA_Kanban.Column")]
        public string WefEb329F44Fe5F4A94Acb1Da153Fdf38BaKanbanColumn { get; set; }
    }

    public partial class ResourceContainers
    {
        [JsonPropertyName("collection")]
        public Account Collection { get; set; }

        [JsonPropertyName("account")]
        public Account Account { get; set; }

        [JsonPropertyName("project")]
        public Account Project { get; set; }
    }

    public partial class Account
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
    }

    //public partial class Welcome
    //{
    //    public static Welcome FromJson(string json) => JsonConvert.DeserializeObject<Welcome>(json, QuickType.Converter.Settings);
    //}

    //public static class Serialize
    //{
    //    public static string ToJson(this Welcome self) => JsonConvert.SerializeObject(self, QuickType.Converter.Settings);
    //}

    //internal static class Converter
    //{
    //    public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    //    {
    //        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
    //        DateParseHandling = DateParseHandling.None,
    //        Converters =
    //        {
    //            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
    //        },
    //    };
    //}
}
