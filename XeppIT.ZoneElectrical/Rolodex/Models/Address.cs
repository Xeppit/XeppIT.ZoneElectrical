using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace XeppIT.ZoneElectrical.Rolodex.Models
{
    public class Address
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public override string ToString()
        {
            return $"{Name}, {Street}, {Town}, {Postcode}";
        }
    }
}
