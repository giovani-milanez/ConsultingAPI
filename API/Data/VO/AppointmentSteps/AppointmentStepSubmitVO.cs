using System;
using System.Text.Json;

namespace API.Data.VO
{
    public class AppointmentStepSubmitVO
    {
        public long AppointmentId { get; set; }
        public long StepId { get; set; }
        public JsonDocument SubmitData { get; set; }       
    }
}
