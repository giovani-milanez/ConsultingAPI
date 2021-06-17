using API.Data.Converter.Contract;
using API.Data.VO.Rating;
using Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Data.Converter.Implementations
{
    public class RatingConverter : IParser<Rating, RatingVO>, IParser<RatingCreateVO, Rating>, IParser<RatingEditVO, Rating>
    {
        public RatingVO Parse(Rating origin)
        {
            if (origin == null) return null;
            return new RatingVO
            {
                Id = origin.Id,
                AppointmentId = origin.AppointmentId,
                Stars = origin.Stars,
                Comment = origin.Comment,
                CreatedAt = origin.CreatedAt
            };
        }

        public Rating Parse(RatingCreateVO origin)
        {
            if (origin == null) return null;
            return new Rating
            {
                AppointmentId = origin.AppointmentId,
                Stars = origin.Stars,
                Comment = origin.Comment, 
                CreatedAt = DateTime.UtcNow
            };
        }

        public Rating Parse(RatingEditVO origin)
        {
            if (origin == null) return null;
            return new Rating
            {
                Id = origin.Id,
                Stars = origin.Stars,
                Comment = origin.Comment,
                CreatedAt = DateTime.UtcNow
            };
        }

        public List<RatingVO> Parse(List<Rating> origin)
        {
            if (origin == null) return null;
            return origin.Select(item => Parse(item)).ToList();
        }

        public List<Rating> Parse(List<RatingCreateVO> origin)
        {
            if (origin == null) return null;
            return origin.Select(item => Parse(item)).ToList();
        }

        public List<Rating> Parse(List<RatingEditVO> origin)
        {
            if (origin == null) return null;
            return origin.Select(item => Parse(item)).ToList();
        }
    }
}
