

using Ecommerce2.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce2.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;
        public HomeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Genre>> Genres()
        {
            return await _db.Genres.ToListAsync();
        }
        public async Task<IEnumerable<Book>> GetBooks(string sTerm="", int GenreId = 0)
        {
            sTerm = sTerm.ToLower();
            IEnumerable<Book> books = await (from book in _db.Books
                         join genre in _db.Genres
                         on book.GenreId equals genre.Id
                         where string.IsNullOrWhiteSpace(sTerm) || book.BookName.ToLower().StartsWith(sTerm)

                         select new Book
                         {
                             Id = book.Id,
                             ImageUrl = book.ImageUrl,
                             AuthorName = book.AuthorName,
                             BookName = book.BookName,
                             GenreId = book.GenreId,
                             Price = book.Price,
                             GenreName = book.GenreName,
                         }
                         ).ToListAsync();
            if(GenreId > 0)
            {
                books = books.Where(a =>a.GenreId == GenreId).ToList();
            }

            return books;
        }
    }
}
