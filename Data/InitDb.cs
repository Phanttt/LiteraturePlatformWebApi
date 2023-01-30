
using LiteraturePlatformWebApi.Models;

namespace LiteraturePlatformWebApi.Data
{
    public class InitDb
    {
        public static void Initialize(LiteraturePlatformContext context)
        {
            if (!context.Composition.Any())
            {
                User user = new User()
                {
                    Login="User",
                    Email="qwere@gmail.com",
                    Password="12121"
                };
                context.Add(user);
                Genre genre1 = new Genre()
                {
                    Name = "Detective"
                };
                Genre genre2 = new Genre()
                {
                    Name = "Science fiction"
                };
                Genre genre3 = new Genre()
                {
                    Name = "Horror"
                };
                Genre genre4 = new Genre()
                {
                    Name = "Thriller"
                };
                context.AddRange(genre1, genre2, genre3, genre4);

                Text text = new Text()
                {
                    Content = "nbonjoijjioiqpjjoijiojiojonjoijjioiqpjjoijiojiojonjoijjioiqpjjoijiojiojiojiojijiji"
                };
                context.Add(text);

                byte[] imageData = null;
                string curdir = Directory.GetCurrentDirectory();
                Stream stream = new FileStream(curdir+"\\wwwroot\\forest.jpg", FileMode.Open);
                // считываем переданный файл в массив байтов
                using (var binaryReader = new BinaryReader(stream))
                {
                    imageData = binaryReader.ReadBytes((int)stream.Length);
                }
                // установка массива байтов
                

                Composition composition = new Composition()
                {
                    Title = "Сomposition1 ",
                    Description = "Very cool book",
                    Date = DateTime.Now,
                    Genre = genre1,
                    Rating = 4.3,
                    Text = text,
                    User = user,
                    Comments = new List<Comment>(),
                    Image = imageData
            };
                Comment c1 = new Comment()
                {
                    Text = "I consumed this book almost in one go – I found it very well written, easy to read and understand.\r\nThe book contains a nice mix of examples from real life as well as Ilse’s own lovely honesty.\r\nThis book will accompany me throughout my life.\r\nI look forward to reading it again – more slowly and spending more time on the details.",
                    User = user
                };
                Comment c2 = new Comment()
                {
                    Text = "I’m french. I read your book”‘highly sensitives”??? or “hypersensibles ” in french. I was very happy to read it. It made mee feel better.",
                    User = user
                };
                context.AddRange(c1,c2);

                composition.Comments.Add(c1);
                composition.Comments.Add(c2);

                context.Add(composition);

                context.SaveChanges();
            }
        }
    }
}
