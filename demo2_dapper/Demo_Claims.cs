using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;


namespace Arena
{
	public static class Demo_Claims
	{

		public static AuthenticationProperties authentication_properties = new AuthenticationProperties
		{
			ExpiresUtc = DateTime.UtcNow.AddMonths(3),
			AllowRefresh = true
		};

		public static ClaimsPrincipal impersonate(ClaimsPrincipal current, int id)
		{
			if (current.Identities.Any(c => c.AuthenticationType == "FalseIdentity"))
			{
				return current;
			}
			var claims = new List<Claim>
			{
				new Claim("id", id.ToString()),
			};
			List<ClaimsIdentity> identities = new List<ClaimsIdentity>
			{
				new ClaimsIdentity(claims, "FalseIdentity"),
				new ClaimsIdentity(current.Identity)
			};
			ClaimsPrincipal principal = new ClaimsPrincipal(identities);
			return principal;
		}

		public static ClaimsPrincipal impersonate_revert(ClaimsPrincipal current, string keep = "Cookies")
		{
			ClaimsIdentity identities = current.Identities.FirstOrDefault(c => c.AuthenticationType == keep);
			ClaimsPrincipal principal = new ClaimsPrincipal(identities);
			return principal;
		}


		public static int? get_int(IEnumerable<ClaimsIdentity> identities, string authentication, string claimtype)
		{
			int value = 0;
			Claim a = identities.FirstOrDefault(c => c.AuthenticationType == authentication).Claims.FirstOrDefault(c => c.Type == claimtype);
			if (a == null)
			{
				return null;
			}
			if (Int32.TryParse(a.Value, out value) == false)
			{
				return null;
			};
			return value;
		}

		public static int? get_int(IEnumerable<Claim> claims, string claimtype)
		{
			int value = 0;
			Claim a = claims.FirstOrDefault(c => c.Type == claimtype);
			if (a == null)
			{
				return null;
			}
			if (Int32.TryParse(a.Value, out value) == false)
			{
				return null;
			};
			return value;
		}


		public static string get_string(IEnumerable<ClaimsIdentity> identities, string authentication, string claimtype)
		{
			return identities.FirstOrDefault(c => c.AuthenticationType == authentication).Claims.FirstOrDefault(c => c.Type == claimtype).Value;
		}

		public static string get_string(IEnumerable<Claim> claims, string claimtype)
		{
			return claims.FirstOrDefault(c => c.Type == claimtype).Value;
		}





	};

}