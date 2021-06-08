using API.Data.Converter.Contract;
using API.Data.VO;
using Database.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace API.Data.Converter.Implementations
{
    public class ServicesStepConverter : IParser<ServicesStepCreateVO, ServicesStep>, IParser<ServicesStep, ServicesStepVO>
    {
        private readonly StepConverter StepConverter;

        public ServicesStepConverter()
        {
            StepConverter = new StepConverter();
        }

        public ServicesStep Parse(ServicesStepCreateVO origin)
        {
            if (origin == null) return null;
            return new ServicesStep
            {
                StepId = origin.StepId,
                //ServiceId = origin.ServiceId,
                Service = null,
                Order = origin.Order,
                Title = origin.Title,
                CreateData = JsonSerializer.Serialize(origin.CreateData)
            };
        }

        public ServicesStepVO Parse(ServicesStep origin)
        {
            if (origin == null) return null;
            return new ServicesStepVO
            {
                Id = origin.Id,
                Step = StepConverter.Parse(origin.Step),
                //ServiceId = origin.ServiceId.Value,
                Order = origin.Order,
                CreateData = JsonDocument.Parse(origin.CreateData),
                Title = origin.Title
            };
        }

        public List<ServicesStepVO> Parse(List<ServicesStep> origin)
        {
            if (origin == null) return null;
            return origin.Select(item => Parse(item)).ToList();
        }


        public List<ServicesStep> Parse(List<ServicesStepCreateVO> origin)
        {
            if (origin == null) return null;
            return origin.Select(item => Parse(item)).ToList();
        }
    }
}
