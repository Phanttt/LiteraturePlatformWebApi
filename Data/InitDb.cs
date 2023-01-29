
using LiteraturePlatformWebApi.Models;
using System.IO;

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
                Genre genre = new Genre()
                {
                    Name = "Детектив"
                };
                context.Add(genre);

                Text text = new Text()
                {
                    Content = "nbonjoijjioipjjoijiojiojiojiojijiji"
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
                    Title = "Сomposition1",
                    Description = "Very cool book",
                    Date = DateTime.Now,
                    Genre = genre,
                    Rating = 4.3,
                    Text = text,
                    User = user,
                    Comments = new List<Comment>(),
                    //Image = imageData
            };
                Comment c1 = new Comment()
                {
                    Text = "Comm1 qwqeq e qe q eqeqe"
                };
                Comment c2 = new Comment()
                {
                    Text = "Comm2qweqw"
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
