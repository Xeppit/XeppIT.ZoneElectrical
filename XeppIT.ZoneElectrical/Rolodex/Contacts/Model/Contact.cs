﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace XeppIT.ZoneElectrical.Rolodex.Contacts.Model
{
    public class Contact
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Company { get; set; }
        public override string ToString()
        {
            return $"{FirstName} {LastName} ({Email})";
        }
    }
}
