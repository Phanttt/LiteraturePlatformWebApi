using LiteraturePlatformWebApi.Data;
using LiteraturePlatformWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LiteraturePlatformWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlatformController : Controller
    {
        private LiteraturePlatformContext _context;
        public PlatformController(LiteraturePlatformContext context)
        {
            _context = context;
            InitDb.Initialize(_context);
        }
        [HttpGet(Name = "GetAllCompositions")]
        public async Task<ActionResult<IEnumerable<Composition>>> GetAllCompositions()
        {
            return await _context.Composition
                .Include(e => e.User)
                .Include(e => e.Text)
                .Include(e => e.Comments)
                .Include(e => e.Genre)
                .ToListAsync();
        }
        [HttpGet]
        [Route("GetComposition/{id}")]
        public async Task<ActionResult<Composition>> GetComposition(int id)
        {
            return await _context.Composition.Where(e=>e.CompositionId == id)
                .Include(e => e.Genre)
                .Include(e => e.User)
                .Include(e => e.Text)
                .Include(e => e.Comments)
                .ThenInclude(e=>e.User)
                .FirstOrDefaultAsync();
        }

        [HttpGet]
        [Route("GetTop1Raiting")]
        public async Task<ActionResult<Composition>> GetTop1Raiting()
        {
            var maxr = _context.Composition.Max(e => e.Rating);
            return await _context.Composition
                .Where(e => e.Rating == maxr)
                .Include(e => e.User)
                .Include(e => e.Text)
                .Include(e => e.Comments)
                .Include(e => e.Genre)
                .FirstAsync();
        }
        [HttpGet]
        [Route("GetTop2Latest")]
        public async Task<IEnumerable<Composition>> GetTop2Latest()
        {
            return await _context.Composition
                .OrderByDescending(e=>e.Date)
                .Take(2)
                .Include(e => e.User)
                .Include(e => e.Text)
                .Include(e => e.Comments)
                .Include(e => e.Genre)         
                .ToListAsync();
        }

        [HttpGet]
        [Route("GetGenres")]
        public async Task<IEnumerable<Genre>> GetGenres()
        {
            return await _context.Genres.ToListAsync();
        }

        [HttpPost]
        [Route("CreateComposition")]
        public async Task<IResult> CreateComposition(SendModel model)
        {

            Composition composition = new Composition()
            {
                Title = model.title,
                Description = model.descr
            };
            int userId = model.userId;

            Text Text = new Text()
            {
                Content = model.text
            };
            _context.Add(Text);

            User user = await _context.Users.FirstOrDefaultAsync(e => e.UserId == userId);
            if (user == null)
            {
                return Results.NotFound();
            }

            composition.Image = model.imageData;
            composition.Text = Text;
            composition.User = user;
            composition.Date = DateTime.Now;
            composition.Genre = await _context.Genres.FirstOrDefaultAsync(x => x.GenreId == model.genreId);

            _context.Add(composition);
            await _context.SaveChangesAsync();
            return Results.Ok();
        }

        [HttpPost]
        [Route("AddComment")]
        public async Task<IResult> AddComment(Comment comment)
        {          
            var comp = await _context.Composition.FirstOrDefaultAsync(e => e.CompositionId == comment.CommentId);
            comment.CommentId = 0;
            User user = await _context.Users.FirstOrDefaultAsync(e => e.UserId == comment.UserId);

            if (comp.Comments == null)
            {
                comp.Comments = new List<Comment>();
            }

            comment.User = user;
            _context.Add(comment);

            comp.Comments.Add(comment);

            await _context.SaveChangesAsync();

            return Results.Ok();
        }

        [HttpGet]
        [Route("GetTop50Rating")]
        public async Task<ActionResult<IEnumerable<Composition>>> GetTop50Rating()
        {
            return await _context.Composition
                .OrderByDescending(x => x.Rating)
                .Include(e => e.User)
                .Include(e => e.Text)
                .Include(e => e.Comments)
                .Include(e => e.Genre)
                .ToListAsync();
        }

        [HttpGet]
        [Route("GetTop50Comments")]
        public async Task<ActionResult<IEnumerable<Composition>>> GetTop50Comments()
        {
            return await _context.Composition
                .OrderByDescending(x => x.Comments.Count())
                .Include(e => e.User)
                .Include(e => e.Text)
                .Include(e => e.Comments)
                .Include(e => e.Genre)
                .ToListAsync();
        }

        [HttpGet]
        [Route("SearchByAutor/{text}")]
        public async Task<ActionResult<IEnumerable<Composition>>> SearchByAutor(string text)
        {
            return await _context.Composition               
                .Include(e => e.User)
                .Include(e => e.Text)
                .Include(e => e.Comments)
                .Include(e => e.Genre)
                .Where(e => e.User.Login == text)
                .ToListAsync();
        }

        [HttpGet]
        [Route("SearchByTitle/{text}")]
        public async Task<ActionResult<IEnumerable<Composition>>> SearchByTitle(string text)
        {
            return await _context.Composition
                .Where(e => e.Title == text)
                .Include(e => e.User)
                .Include(e => e.Text)
                .Include(e => e.Comments)
                .Include(e => e.Genre)
                .ToListAsync();
        }

        [HttpGet]
        [Route("FindByGenre/{id}")]
        public async Task<ActionResult<IEnumerable<Composition>>> FindByGenre(int id)
        {
            return await _context.Composition
                .Where(x => x.GenreId == id)
                .Include(e => e.User)
                .Include(e => e.Text)
                .Include(e => e.Comments)
                .Include(e => e.Genre)
                .ToListAsync();
        }
        [HttpGet]
        [Route("Rate/{composId}/{userId}/{rate}")]
        public async Task<ActionResult> Rate(int composId, int userId, int rate)
        {
            Rating exist = await _context.Rating.FirstOrDefaultAsync(e => e.UserId == userId && e.CompositionId == composId);
            if (exist == null)
            {
                Rating rating = new Rating()
                {
                    CompositionId = composId,
                    UserId = userId,
                    Rate = rate
                };
                await _context.Rating.AddAsync(rating); 
            }
            else
            {
                exist.Rate = rate;
            }

            await _context.SaveChangesAsync();

            double sumRate = _context.Rating.Where(e => e.CompositionId == composId).Average(e => e.Rate);
            Composition composition = await _context.Composition.FirstOrDefaultAsync(e => e.CompositionId == composId);
            composition.Rating = sumRate;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Route("CurrRate/{userId}/{composId}")]
        public async Task<ActionResult<int>> CurrRate(int userId, int composId)
        {
            Rating rating = await _context.Rating.Where(e => e.UserId == userId && e.CompositionId == composId).FirstOrDefaultAsync();
            return rating.Rate;
        }

        [HttpGet]
        [Route("AccountData/{id}")]
        public async Task<ActionResult<User>> AccountData(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(e => e.UserId == id);
        }

        [HttpPost]
        [Route("ChangeData")]
        public async Task<ActionResult<string>> ChangeData(User user)
        {
            User a = await _context.Users.Where(e => e.Email == user.Email).FirstOrDefaultAsync();
            if (a != null)
            {
                return BadRequest("User with this email already exist");
            }

            User newUser = await _context.Users.Where(e => e.UserId == user.UserId).FirstOrDefaultAsync();
            newUser.Email = user.Email;
            newUser.Login = user.Login;
            newUser.Password = user.Password;


            await _context.SaveChangesAsync();
            return Ok("Data was successfully changed");
        }

        //[HttpGet]
        //[Route("DeleteUser/{id}")]
        //public async Task<ActionResult<string>> DeleteUser(int id)
        //{
        //    User user = await _context.Users.FirstOrDefaultAsync(e => e.UserId == id);
        //    _context.Users.Remove(user);
        //    await _context.SaveChangesAsync();
        //    return Ok("Data was successfully deleted");
        //}
    }
}
