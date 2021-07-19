using System;
using System.Collections.Generic;

namespace API.Data.VO.User
{
    public class ConsultantVO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public float RateMeanStars { get; set; }
        public long RateCount { get; set; }
        public string ProfilePicUrl { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Gender { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<ServiceShortVO> Services { get; set; }
    }
}
