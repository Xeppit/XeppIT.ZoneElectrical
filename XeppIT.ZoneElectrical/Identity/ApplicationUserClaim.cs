﻿using System.Security.Claims;

namespace XeppIT.ZoneElectrical.Identity
{
    /// <summary>
	/// A claim that a user possesses.
	/// </summary>
	public class ApplicationUserClaim
	{
		public ApplicationUserClaim()
		{
		}

		public ApplicationUserClaim(Claim claim)
		{
			Type = claim.Type;
			Value = claim.Value;
		}

		/// <summary>
		/// Claim type
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// Claim value
		/// </summary>
		public string Value { get; set; }

		public Claim ToSecurityClaim()
		{
			return new Claim(Type, Value);
		}
	}
}