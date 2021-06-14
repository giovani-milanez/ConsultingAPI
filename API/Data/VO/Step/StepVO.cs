using API.Hypermedia;
using API.Hypermedia.Abstract;
using System.Collections.Generic;
using System.Text.Json;

namespace API.Data.VO
{
    public class StepVO : ISupportsHyperMedia
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string DisplayName { get; set; }
        public JsonDocument CreateSchema { get; set; }
        public JsonDocument SubmitSchema { get; set; }
        public bool AllowFileUpload { get; set; }
        public string TargetUser { get; set; }
        public List<HyperMediaLink> Links { get; set; } = new List<HyperMediaLink>();
    }
}
