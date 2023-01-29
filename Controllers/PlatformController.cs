using LiteraturePlatformWebApi.Data;
using LiteraturePlatformWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LiteraturePlatformWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlatformController
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
        public async Task<IResult> AddComment(Comment text)
        {            
            var comp = await _context.Composition.FirstOrDefaultAsync(e=>e.CompositionId == text.TextId);
            _context.Add(text);
            comp.Text = text;

            await _context.SaveChangesAsync();

            return Results.Ok();
        }
    }
}
