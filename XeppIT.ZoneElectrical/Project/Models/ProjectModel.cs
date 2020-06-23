using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace XeppIT.ZoneElectrical.Project.Models
{
    public class ProjectModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int JobNo { get; set; }
        public ProjectAddress ProjectAddress { get; set; } = new ProjectAddress();
        public string WorkDescription { get; set; }
        public string Company { get; set; }
        public ProjectContact Client { get; set; } = new ProjectContact();
        public string Status { get; set; }
        public override string ToString()
        {
            return $"{JobNo} {ProjectAddress.Name} {WorkDescription}";
        }
    }
}
