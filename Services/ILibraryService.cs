using lab05.Models;

namespace lab05.Services
{

    public interface ILibraryService
    {
        public List<Books> Books { get; set; }

        public List<User> Users { get; set; }

        public Dictionary<int, List<Books>> BorrowedBooks { get; set; }

        private void ReadBooks() { }

        private void ReadUsers() { }

        void AddBook(string title, string author, string isbn);
        bool EditBook(int id, string? title, string? author, string? isbn);
        bool DeleteBook(int id);
        List<(Books Book, int Count)> GetAvailableBooks();

        void AddUser(string name, string email);
        bool EditUser(int id, string? name, string? email);
        bool DeleteUser(int id);

        string BorrowBook(int bookId, int userId);
        string ReturnBook(int userId, int bookIndex);

        List<Books> GetUserBorrowedBooks(int userId);
        Dictionary<User, List<Books>> GetAllBorrowedBooks();
    }
}
