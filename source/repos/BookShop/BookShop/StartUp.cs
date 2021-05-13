namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            //int input = int.Parse(Console.ReadLine());

            int result = RemoveBooks(db);

            Console.WriteLine(result);
        }
        
        //Problem 02
        private static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            List<string> bookTitles = context.Books.AsEnumerable()
                .Where(b => b.AgeRestriction.ToString().ToLower() == command.ToLower())
                .Select(b => b.Title)
                .OrderBy(bt => bt)
                .ToList();

            return String.Join(Environment.NewLine, bookTitles);
        }

        //Problem 03
        public static string GetGoldenBooks(BookShopContext context)
        {
            List<string> bookTitles = context.Books
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToList();

            return String.Join(Environment.NewLine, bookTitles);
        }

        //Problem 04
        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .AsEnumerable()
                .Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })                
                .ToList();

            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title} - ${b.Price}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 05
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            List<string> booksNotReleasedIn = context.Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToList();

            return String.Join(Environment.NewLine, booksNotReleasedIn);
        }

        //Problem 06
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower())
                .ToArray();

            List<string> bookTitles = new List<string>();

            foreach (var category in categories)
            {
                List<string> currentCategoryBookTitles = context.Books
                    .Where(b => b.BookCategories.Any(bc => bc.Category.Name.ToLower() == category))
                    .Select(b => b.Title)
                    .ToList();

                bookTitles.AddRange(currentCategoryBookTitles);
            }

            bookTitles = bookTitles.OrderBy(bt => bt).ToList();

            return String.Join(Environment.NewLine, bookTitles);
        }

        //Problem 07
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(b => b.ReleaseDate < DateTime.Parse(date))
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price
                })
                .ToList();

            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title} - {b.EditionType} - ${b.Price}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 08
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var authors = context.Authors
                .Where(a => a.FirstName.ToLower().EndsWith(input.ToLowerInvariant()))
                .OrderBy(a => a)
                .Select(a => new
                {
                    FullName = a.FirstName + " " + a.LastName
                })
                .ToList();

            foreach (var a in authors)
            {
                sb.AppendLine($"{a.FullName}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 09
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            List<string> bookTitlesContainingString = context.Books
                .AsEnumerable()
                .Where(b => b.Title.Contains(input, StringComparison.InvariantCultureIgnoreCase))
                .Select(b => b.Title)
                .OrderBy(bt => bt)
                .ToList();

            return String.Join(Environment.NewLine, bookTitlesContainingString);
        }

        //Problem 10
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    b.Title,
                    AuthorFullName = b.Author.FirstName + " " + b.Author.LastName
                })
                .ToList();

            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title} ({b.AuthorFullName})");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 11
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var bookCount = context.Books
                .Where(b => b.Title.Length > lengthCheck)
                .Count();

            return bookCount;
        }

        //Problem 12
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var authorCopies = context.Authors
                .Select(a => new
                {
                    FullName = a.FirstName + " " + a.LastName,
                    BookCopies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.BookCopies)
                .ToList();

            foreach (var a in authorCopies)
            {
                sb.AppendLine($"{a.FullName} - {a.BookCopies}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 13
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categoryProfits = context.Categories
                .Select(c => new
                {
                    c.Name,
                    TotalProfit = c.CategoryBooks.Select(cb => new
                    {
                        BookProfit = cb.Book.Copies * cb.Book.Price
                    })
                    .Sum(cb => cb.BookProfit)
                })
                .OrderByDescending(c => c.TotalProfit)
                .ThenBy(c => c.Name)
                .ToList();

            foreach (var c in categoryProfits)
            {
                sb.AppendLine($"{c.Name} ${c.TotalProfit:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 14
        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categoriesWithMostResentBooks = context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    MostRecentBooks = c.CategoryBooks
                    .OrderByDescending(cb => cb.Book.ReleaseDate)
                    .Take(3)
                    .Select(cb => new
                    {
                        BookTitle = cb.Book.Title,
                        ReleaseYear = cb.Book.ReleaseDate.Value.Year
                    })
                    .ToList()
                })
                .OrderBy(c => c.CategoryName)
                .ToList();

            foreach (var c in categoriesWithMostResentBooks)
            {
                sb.AppendLine($"--{c.CategoryName}");

                foreach (var b in c.MostRecentBooks)
                {
                    sb.AppendLine($"{b.BookTitle} ({b.ReleaseYear})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 15
        public static void IncreasePrices(BookShopContext context)
        {
            var booksToUpdate = context.Books
                .Where(b => b.ReleaseDate.Value.Year < 2010);

            foreach (var book in booksToUpdate)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        //Problem 16
        public static int RemoveBooks(BookShopContext context)
        {
            int removedBooksCount = 0;

            var removedBooks = context.Books
                .Where(b => b.Copies < 4200)
                .ToList();

            foreach (var b in removedBooks)
            {
                context.Books.Remove(b);

                removedBooksCount++;
            }

            return removedBooksCount;
        }
    }
}
