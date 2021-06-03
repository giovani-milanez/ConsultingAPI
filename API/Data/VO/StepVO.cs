using API.Hypermedia;
using API.Hypermedia.Abstract;
using System.Collections.Generic;

namespace API.Data.VO
{
    public class StepVO : ISupportsHyperMedia
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string DisplayName { get; set; }
        public string CreateSchema { get; set; }
        public string SubmitSchema { get; set; }
        public List<HyperMediaLink> Links { get; set; } = new List<HyperMediaLink>();
    }
}
