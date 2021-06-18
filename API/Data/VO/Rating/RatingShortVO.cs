using System;

namespace API.Data.VO.Rating
{
    public class RatingShortVO
    {
        public long Id { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
