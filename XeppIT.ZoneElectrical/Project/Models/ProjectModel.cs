using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using XeppIT.ZoneElectrical.Rolodex.Addresses.Model;
using XeppIT.ZoneElectrical.Rolodex.Contacts.Model;

namespace XeppIT.ZoneElectrical.Project.Models
{
    public class ProjectModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int JobNo { get; set; }
        public Address ProjectAddress { get; set; } = new Address();
        public string WorkDescription { get; set; }
        public string Company { get; set; }
        public Contact Client { get; set; } = new Contact();
        public string Status { get; set; }
        public override string ToString()
        {
            return $"{JobNo} {ProjectAddress.Name} {WorkDescription}";
        }
    }
}
