using System.Linq;

using Serilog;

using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;


namespace Demo
{


	[ExtendObjectType("Book")]
	public class Book_Resolver
	{
		public IQueryable<User> GetAuthor(Book book, [Service] Demo_Context context)
		{
			return context.users.Where(u => u.id == book.author_id);
		}
	}


	[ExtendObjectType("Query")]
	public class Book_Query
	{
		private readonly ILogger log = Log.ForContext<User_Mutation>();
		public IQueryable<Book> books([Service] Demo_Context context) 
		{
			return context.books;
		}

		public IQueryable<Book> book_by_id([Service] Demo_Context context, [ID(nameof(Book))]int id) 
		{
			return context.books.Where(b => b.id == id);
		}


	}
}
