using API.Data.Converter.Contract;
using API.Data.VO;
using Database.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace API.Data.Converter.Implementations
{
    public class StepConverter : IParser<StepVO, Step>, IParser<Step, StepVO>, IParser<StepCreateVO, Step>, IParser<StepEditVO, Step>
    {
        public Step Parse(StepVO origin)
        {
            if (origin == null) return null;
            return new Step
            {
                Id = origin.Id,
                Type = origin.Type,
                DisplayName = origin.DisplayName,
                CreateSchema = JsonSerializer.Serialize(origin.CreateSchema),
                SubmitSchema = JsonSerializer.Serialize(origin.SubmitSchema)
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
                CreateSchema = JsonDocument.Parse(origin.CreateSchema),
                SubmitSchema = JsonDocument.Parse(origin.SubmitSchema)
            };
        }

        public Step Parse(StepCreateVO origin)
        {
            if (origin == null) return null;
            return new Step
            {
                Type = origin.Type,
                DisplayName = origin.DisplayName,
                CreateSchema = JsonSerializer.Serialize(origin.CreateSchema),
                SubmitSchema = JsonSerializer.Serialize(origin.SubmitSchema)
            };
        }

        public Step Parse(StepEditVO origin)
        {
            if (origin == null) return null;
            return new Step
            {
                Id = origin.Id,
                Type = origin.Type,
                DisplayName = origin.DisplayName,
                CreateSchema = JsonSerializer.Serialize(origin.CreateSchema),
                SubmitSchema = JsonSerializer.Serialize(origin.SubmitSchema)
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

        public List<Step> Parse(List<StepCreateVO> origin)
        {
            if (origin == null) return null;
            return origin.Select(item => Parse(item)).ToList();
        }

        public List<Step> Parse(List<StepEditVO> origin)
        {
            if (origin == null) return null;
            return origin.Select(item => Parse(item)).ToList();
        }
    }
}
