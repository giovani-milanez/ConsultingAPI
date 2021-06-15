using API.Data.Converter.Contract;
using API.Data.VO;
using Database.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace API.Data.Converter.Implementations
{
    public class AppointmentStepConverter : IParser<AppointmentStep, AppointmentStepVO>
    {
        private FileConverter FileConverter { get; set; }

        public AppointmentStepConverter(FileConverter fileConverter)
        {
            FileConverter = fileConverter;
        }

        public AppointmentStepVO Parse(AppointmentStep origin)
        {
            if (origin == null) return null;
            return new AppointmentStepVO
            {
                //Id = origin.Id,
                //Step = StepConverter.Parse(origin.Step),
                StepId = origin.StepId,
                SubmitData = JsonDocument.Parse(origin.SubmitData),
                IsCompleted = origin.IsCompleted,
                DateCompleted = origin.DateCompleted,
                Files = origin.AppointmentStepFiles == null ? new List<FileDetailVO>() : origin.AppointmentStepFiles.Select(x => FileConverter.Parse(x.File)).ToList()
            };
        }

        public List<AppointmentStepVO> Parse(List<AppointmentStep> origin)
        {
            if (origin == null) return null;
            return origin.Select(item => Parse(item)).ToList();
        }
    }
}
