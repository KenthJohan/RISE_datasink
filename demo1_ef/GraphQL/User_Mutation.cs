using System;
using System.Threading.Tasks;
using System.Linq;


using Microsoft.EntityFrameworkCore;

using Serilog;

using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Types;

namespace Demo
{


	[ExtendObjectType("Mutation")]
	public class User_Mutation
	{
		private readonly ILogger log = Log.ForContext<User_Mutation>();

		public IQueryable<User> user_register([Service] Demo_Context context, string email, string password)
		{
			if (string.IsNullOrEmpty(email))
			{
				throw new QueryException(
					ErrorBuilder.New()
						.SetMessage("The email cannot be empty.")
						.SetCode("EMAIL_EMPTY")
						.Build());
			}
			if (string.IsNullOrEmpty(password))
			{
				throw new QueryException(
					ErrorBuilder.New()
						.SetMessage("The password cannot be empty.")
						.SetCode("PASSWORD_EMPTY")
						.Build());
			}

			User user = new User{email = email};
			user.guid = Guid.NewGuid();
			user.pwhash = PBKDF2.genhash(password, 16, 20, 100000);

			log.Information("Adding {@User}", user);
			context.users.Add(user);
			try
			{
				context.SaveChanges();
			}
			catch
			{
				return null;
			}


			context.books.Add(new Book{author_id = user.id, name = "Hello1"});
			context.books.Add(new Book{author_id = user.id, name = "Hello2"});
			context.books.Add(new Book{author_id = user.id, name = "Hello3"});
			try
			{
				context.SaveChanges();
			}
			catch
			{
				return null;
			}
			
			return context.users.Where(u => u.id == user.id);
		}

		public IQueryable<User> user_login([Service] Demo_Context context, string email, string password)
		{
			if (string.IsNullOrEmpty(email))
			{
				throw new QueryException(
					ErrorBuilder.New()
						.SetMessage("The email cannot be empty.")
						.SetCode("EMAIL_EMPTY")
						.Build());
			}
			if (string.IsNullOrEmpty(password))
			{
				throw new QueryException(
					ErrorBuilder.New()
						.SetMessage("The password cannot be empty.")
						.SetCode("PASSWORD_EMPTY")
						.Build());
			}
			User user = context.users.FirstOrDefault(u => u.email == email);
			bool success = PBKDF2.verify(user.pwhash, password, 16, 20, 100000);
			if (success)
			{
				log.Information("The {@User} logged in: {success}", user, success);
				return context.users.Where(u => u.id == user.id);
			}

			return null;
		}


	}
}