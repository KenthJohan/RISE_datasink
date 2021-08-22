using System.Linq;
using System;


using Serilog;

using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;


namespace Demo
{

	[ExtendObjectType("User")]
	public class User_Resolver
	{
		public string pwhash_base64(User user)
		{
			return Convert.ToBase64String(user.pwhash);
		}

		public int id_times_20(User user)
		{
			return user.id * 20;
		}

		public IQueryable<Book> GetBooks(User user, [Service] Demo_Context context)
		{
			return context.books.Where(b => b.author_id == user.id);
		}

	}


	[ExtendObjectType("Query")]
	public class User_Query
	{
		private readonly ILogger log = Log.ForContext<User_Query>();

		public IQueryable<User> users([Service] Demo_Context context)
		{
			return context.users;
		}

		public IQueryable<User> user_by_id([Service] Demo_Context context, [ID(nameof(User))]int id) 
		{
			return context.users.Where(u => u.id == id);
		}

	}
}
