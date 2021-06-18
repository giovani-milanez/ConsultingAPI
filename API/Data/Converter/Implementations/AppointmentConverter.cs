using API.Data.Converter.Contract;
using API.Data.VO;
using Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Data.Converter.Implementations
{
    public class AppointmentConverter : IParser<AppointmentCreateVO, Appointment>, IParser<Appointment, AppointmentVO>
    {
        private readonly AppointmentStepConverter AppointmentStepConverter;
        private readonly ServiceConverter ServiceConverter;
        private readonly UserConverter UserConverter;
        private readonly RatingShortConverter RatingConverter;

        public AppointmentConverter(FileConverter fileConverter)
        {
            AppointmentStepConverter = new AppointmentStepConverter(fileConverter);
            ServiceConverter = new ServiceConverter(fileConverter);
            UserConverter = new UserConverter(fileConverter);
            RatingConverter = new RatingShortConverter();
        }

        public Appointment Parse(AppointmentCreateVO origin)
        {
            if (origin == null) return null;
            return new Appointment
            {
                ServiceId = origin.ServiceId,
                ClientId = origin.ClientId,
                IsCompleted = false,
                StartDate = DateTime.UtcNow                
            };
        }

        public AppointmentVO Parse(Appointment origin)
        {
            if (origin == null) return null;
            return new AppointmentVO
            {
                Id = origin.Id,
                Service = ServiceConverter.Parse(origin.Service),
                Client = UserConverter.Parse(origin.Client),
                StartDate = origin.StartDate,
                EndDate = origin.EndDate,
                IsCompleted = origin.IsCompleted,
                Steps = AppointmentStepConverter.Parse(origin.AppointmentSteps.ToList()),
                Rating = RatingConverter.Parse(origin.Rating)
            };
        }    

        public List<Appointment> Parse(List<AppointmentCreateVO> origin)
        {
            if (origin == null) return null;
            return origin.Select(item => Parse(item)).ToList();
        }        

        public List<AppointmentVO> Parse(List<Appointment> origin)
        {
            if (origin == null) return null;
            return origin.Select(item => Parse(item)).ToList();
        }
    }
}
