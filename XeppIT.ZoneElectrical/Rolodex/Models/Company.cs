using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace XeppIT.ZoneElectrical.Rolodex.Models
{
    public class Company
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Contact> Employees { get; } = new List<Contact>();

        public void AddEmployee(Contact contact)
        {
            Employees.Add(contact);
        }

        public void RemoveEmployee(Contact contact)
        {
            Employees.Remove(contact);
        }
    }
}
