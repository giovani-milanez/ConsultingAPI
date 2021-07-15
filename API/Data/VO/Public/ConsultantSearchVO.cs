using API.Hypermedia;
using API.Hypermedia.Abstract;
using System;
using System.Collections.Generic;

namespace API.Data.VO.Public
{
    public class ConsultantSearchVO : ISupportsHyperMedia
    {
        public List<HyperMediaLink> Links { get; set; } = new List<HyperMediaLink>();
    }
}
