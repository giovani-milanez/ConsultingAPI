﻿using API.Hypermedia;
using API.Hypermedia.Abstract;
using System.Collections.Generic;

namespace API.Data.VO
{
    public class ServiceVO
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsGlobal { get; set; }
        public bool IsDeleted { get; set; }

        public virtual UserShortVO User { get; set; }
        public virtual ICollection<ServicesStepVO> Steps { get; set; }
    }
}
