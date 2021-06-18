using API.Data.Converter.Contract;
using API.Data.VO;
using Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Data.Converter.Implementations
{
    public class AppointmentShortConverter : IParser<Appointment, AppointmentShortVO>
    {
        private readonly ServiceShortConverter ServiceConverter;
        private readonly UserConverter UserConverter;
        public AppointmentShortConverter(FileConverter fileConverter)
        {
            ServiceConverter = new ServiceShortConverter();
            UserConverter = new UserConverter(fileConverter);
        }
       

        public AppointmentShortVO Parse(Appointment origin)
        {
            if (origin == null) return null;
            return new AppointmentShortVO
            {
                Id = origin.Id,
                Service = ServiceConverter.Parse(origin.Service),
                Client = UserConverter.Parse(origin.Client),
                StartDate = origin.StartDate,
                EndDate = origin.EndDate               
            };
        }

        public List<AppointmentShortVO> Parse(List<Appointment> origin)
        {
            if (origin == null) return null;
            return origin.Select(item => Parse(item)).ToList();
        }
    }
}
