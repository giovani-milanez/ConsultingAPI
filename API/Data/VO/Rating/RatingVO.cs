﻿using System;

namespace API.Data.VO.Rating
{
    public class RatingVO
    {
        public long Id { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual AppointmentShortVO Appointment { get; set; }
    }
}
