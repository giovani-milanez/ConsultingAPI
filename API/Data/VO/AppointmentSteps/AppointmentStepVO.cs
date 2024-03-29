﻿using System;
using System.Collections.Generic;
using System.Text.Json;

namespace API.Data.VO
{
    public class AppointmentStepVO
    {
        //public long Id { get; set; }
        public long StepId { get; set; }
        public JsonDocument SubmitData { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DateCompleted { get; set; }
        public List<FileDetailVO> Files { get; set; }
    }
}
