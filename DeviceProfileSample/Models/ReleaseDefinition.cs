﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceProfileSample.Models
{
    public class ReleaseDefinition
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public object _links { get; set; }
    }
}
