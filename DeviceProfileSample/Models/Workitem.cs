using System.Collections.Generic;

namespace DeviceProfileSample.Models
{
    public class WorkItem
    {
        public int id { get; set; }
        public int rev { get; set; }
        public Field fields { get; set; }
        public List<Relation> relations { get; set; }
    }
}