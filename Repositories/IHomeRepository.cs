using Ecommerce2.Models;

namespace Ecommerce2.Repositories
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Book>> GetBooks(string sTerm = "", int GenreId = 0);
        Task<IEnumerable<Genre>> Genres();
    }
}