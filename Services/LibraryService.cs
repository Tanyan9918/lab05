using lab05.Models;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;
using System.Net;

namespace lab05.Services
{
    public class LibraryService : ILibraryService
    {
        public List<Books> Books { get; set; } = new();
        public List<User> Users { get; set; } = new();

        public Dictionary<int, List<Books>> BorrowedBooks { get; set; } = new();

        public LibraryService()
        {
            ReadBooks();
            ReadUsers();
        }

        private void ReadBooks()
        {
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Books.csv");

                if (!File.Exists(path)) return;

                using var parser = new TextFieldParser(path);
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true; // <-- handles commas inside quotes

                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields(); // returns string[]
                    if (fields.Length >= 4)
                    {
                        Books.Add(new Books
                        {
                            Id = int.Parse(fields[0].Trim()),
                            Title = fields[1].Trim(),
                            Author = fields[2].Trim(),
                            ISBN = fields[3].Trim()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading books: " + ex.Message);
            }
        }

        private void ReadUsers()
        {
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Users.csv");

                if (!File.Exists(path)) return;

                foreach (var line in File.ReadLines(path))
                {
                    var fields = line.Split(',');

                    if (fields.Length >= 3)
                    {
                        Users.Add(new User
                        {
                            Id = int.Parse(fields[0].Trim()),
                            Name = fields[1].Trim(),
                            Email = fields[2].Trim()
                        });
                    }
                }
            }
            catch { }
        }


        public void AddBook(string title, string author, string isbn)
        {
            int id = Books.Any() ? Books.Max(b => b.Id) + 1 : 1;

            Books.Add(new Models.Books
            {
                Id = id,
                Title = title,
                Author = author,
                ISBN = isbn
            });
        }

        public bool EditBook(int id, string? title, string? author, string? isbn)
        {
            var book = Books.FirstOrDefault(b => b.Id == id);
            if (book == null) return false;

            if (!string.IsNullOrWhiteSpace(title)) book.Title = title;
            if (!string.IsNullOrWhiteSpace(author)) book.Author = author;
            if (!string.IsNullOrWhiteSpace(isbn)) book.ISBN = isbn;

            return true;
        }

        public bool DeleteBook(int id)
        {
            var book = Books.FirstOrDefault(b => b.Id == id);
            if (book == null) return false;

            Books.Remove(book);
            return true;
        }

        public void AddUser(string name, string email)
        {
            int id = Users.Any() ? Users.Max(u => u.Id) + 1 : 1;

            Users.Add(new User
            {
                Id = id,
                Name = name,
                Email = email
            });
        }

        public bool EditUser(int id, string? name, string? email)
        {
            var user = Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return false;

            if (!string.IsNullOrWhiteSpace(name)) user.Name = name;
            if (!string.IsNullOrWhiteSpace(email)) user.Email = email;

            return true;
        }

        public bool DeleteUser(int id)
        {
            var user = Users.FirstOrDefault(u =>u.Id == id);
            if (user == null) return false;

            Users.Remove(user);
            BorrowedBooks.Remove(id);

            return true;

        }
        
        public string BorrowBook(int bookId, int userId)
        {
            var book = Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null)
                return "Book not found or unavailable";

            var user = Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return "User not found.";

            if (!BorrowedBooks.ContainsKey(userId))
                BorrowedBooks[userId] = new List<Books>();

            BorrowedBooks[userId].Add(book);
            Books.Remove(book);

            return "Book borrowed successfully.";
        }

        public string ReturnBook(int userId, int bookId)
        {
            if (!BorrowedBooks.ContainsKey(userId) || BorrowedBooks[userId].Count == 0)
                return "No borrowed books found for this user.";

            var book = BorrowedBooks[userId].FirstOrDefault(b => b.Id == bookId);
            if (book == null)
                return "Invalid selection.";

            BorrowedBooks[userId].Remove(book);
            Books.Add(book);

            return "Book returned successfully.";
        }

        public List<(Books Book, int Count)> GetAvailableBooks()
        {
            return Books
                .GroupBy(b => b.Id)
                .Select(g => (g.First(), g.Count()))
                .ToList();
        }

        public List<Books> GetUserBorrowedBooks(int userId)
        {
            if (!BorrowedBooks.ContainsKey(userId))
                return new List<Books>();

            return BorrowedBooks[userId];
        }

        public Dictionary<User, List<Books>> GetAllBorrowedBooks()
        {
            var result = new Dictionary<User, List<Books>>();

            foreach (var entry in BorrowedBooks)
            {
                var user = Users.FirstOrDefault(u => u.Id == entry.Key);
                if (user != null)
                {
                    result[user] = entry.Value;
                }
            }

            return result;
        }
    }
}
