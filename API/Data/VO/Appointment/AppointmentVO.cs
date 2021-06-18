using API.Data.VO.Rating;
using API.Hypermedia;
using API.Hypermedia.Abstract;
using System;
using System.Collections.Generic;

namespace API.Data.VO
{
    public class AppointmentVO : ISupportsHyperMedia
    {
        public long Id { get; set; }
        public virtual ServiceVO Service { get; set; }
        public virtual UserShortVO Client { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCompleted { get; set; }
        public virtual ICollection<AppointmentStepVO> Steps { get; set; }
        public virtual RatingShortVO Rating { get; set; }

        public List<HyperMediaLink> Links { get; set; } = new List<HyperMediaLink>();
    }
}
