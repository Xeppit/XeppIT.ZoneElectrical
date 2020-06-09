using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace XeppIT.ZoneElectrical.Identity
{
    public class ApplicationRole
    {
		public ApplicationRole()
		{
			Id = ObjectId.GenerateNewId().ToString();
		}

		public ApplicationRole(string roleName) : this()
		{

			Name = roleName;
		}

		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		public string Name { get; set; }

		public string NormalizedName { get; set; }

		public override string ToString() => Name;
	}
}
