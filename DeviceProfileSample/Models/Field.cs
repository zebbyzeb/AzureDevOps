using Newtonsoft.Json;

namespace DeviceProfileSample.Models
{
    public class Field
    {
        [JsonProperty(PropertyName = "System.AreaPath")]
        public string AreaPath { get; set; }

        [JsonProperty(PropertyName = "System.IterationPath")]
        public string IterationPath { get; set; }

        [JsonProperty(PropertyName = "System.WorkItemType")]
        public string WorkItemType { get; set; }

        [JsonProperty(PropertyName = "System.State")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "System.Title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "System.AssignedTo")]
        public UserEntity AssignedTo { get; set; }

        [JsonProperty(PropertyName = "Microsoft.VSTS.Common.Severity")]
        public string Priority { get; set; }
    }
}