﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceProfileSample.Models
{
    public class Release
    {
        public int id { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public string createdOn { get; set; }
        public string modifiedOn { get; set; }
        public UserEntity modifiedBy { get; set; }
        public UserEntity createdBy { get; set; }
        public List<object> environments { get; set; }
        public object variables { get; set; }
        public object variableGroups { get; set; }
        public List<Artifact> artifacts { get; set; }
        public ReleaseDefinition releaseDefinition { get; set; }
        public string description { get; set; }
        public string reason { get; set; }
        public string releaseNameFormat { get; set; }
        public bool keepForever { get; set; }
        public int definitionSnapshotRevision { get; set; }
        public string logsContainerUrl { get; set; }
        public string url { get; set; }
        public object _links { get; set; }
        public object tags { get; set; }
        public object projectReference { get; set; }
        public object properties { get; set; }
    }
}
