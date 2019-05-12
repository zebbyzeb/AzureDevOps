using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceProfileSample.Models
{
    public class UserEntity
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string uniqueName { get; set; }
        public string url { get; set; }
        public string imageUrl { get; set; }
    }
}
