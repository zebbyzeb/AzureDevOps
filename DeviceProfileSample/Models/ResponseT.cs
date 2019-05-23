using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceProfileSample.Models
{
    public class ResponseT<T>
    {
        public int count { get; set; }
        public List<T> value { get; set; }
    }
}
