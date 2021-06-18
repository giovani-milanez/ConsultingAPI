using System;

namespace API.Data.VO
{
    public class AppointmentShortVO
    {
        public long Id { get; set; }
        public virtual ServiceShortVO Service { get; set; }
        public virtual UserShortVO Client { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
