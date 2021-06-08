﻿using API.Hypermedia;
using API.Hypermedia.Abstract;
using System.Collections.Generic;
using System.Text.Json;

namespace API.Data.VO
{
    public class StepCreateVO
    {
        public string Type { get; set; }
        public string DisplayName { get; set; }
        public JsonDocument CreateSchema { get; set; }
        public JsonDocument SubmitSchema { get; set; }
    }
}
