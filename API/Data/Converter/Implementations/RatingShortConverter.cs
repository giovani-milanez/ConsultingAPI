using API.Data.Converter.Contract;
using API.Data.VO.Rating;
using Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Data.Converter.Implementations
{
    public class RatingShortConverter : IParser<Rating, RatingShortVO>
    {
        public RatingShortVO Parse(Rating origin)
        {
            if (origin == null) return null;
            return new RatingShortVO
            {
                Id = origin.Id,
                Stars = origin.Stars,
                Comment = origin.Comment,
                CreatedAt = origin.CreatedAt
            };
        }

        public List<RatingShortVO> Parse(List<Rating> origin)
        {
            if (origin == null) return null;
            return origin.Select(item => Parse(item)).ToList();
        }

    }
}
