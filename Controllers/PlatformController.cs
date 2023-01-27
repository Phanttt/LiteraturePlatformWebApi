using LiteraturePlatformWebApi.Data;
using LiteraturePlatformWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

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
        [HttpGet("{id}")]
        public async Task<ActionResult<Composition>> GetComposition(int id)
        {
            return await _context.Composition.Where(e=>e.CompositionId == id)
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
        public async Task<IResult> PostComposition(Composition composition, string text)
        {
            Text Text = new Text()
            {
                Content = text
            };
            _context.Add(Text);
            //юзера надо в контроллере добавить
            composition.Text = Text;

            return Results.Ok();
        }


    }
}
