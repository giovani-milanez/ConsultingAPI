﻿using API.Data.Converter.Contract;
using API.Data.VO;
using Database.Model;
using System.Collections.Generic;
using System.Linq;

namespace API.Data.Converter.Implementations
{
    public class StepConverter : IParser<StepVO, Step>, IParser<Step, StepVO>
    {
        public Step Parse(StepVO origin)
        {
            if (origin == null) return null;
            return new Step
            {
                Id = origin.Id,
                Type = origin.Type,
                DisplayName = origin.DisplayName,
                CreateSchema = origin.CreateSchema,
                SubmitSchema = origin.SubmitSchema
            };
        }

        public StepVO Parse(Step origin)
        {
            if (origin == null) return null;
            return new StepVO
            {
                Id = origin.Id,
                Type = origin.Type,
                DisplayName = origin.DisplayName,
                CreateSchema = origin.CreateSchema,
                SubmitSchema = origin.SubmitSchema
            };
        }

        public List<StepVO> Parse(List<Step> origin)
        {
            if (origin == null) return null;
            return origin.Select(item => Parse(item)).ToList();
        }


        public List<Step> Parse(List<StepVO> origin)
        {
            if (origin == null) return null;
            return origin.Select(item => Parse(item)).ToList();
        }
    }
}