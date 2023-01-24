using LiteraturePlatformWebApi.Data;
using LiteraturePlatformWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace LiteraturePlatformWebApi.Controllers
{
    public class PlatformController
    {
        private LiteraturePlatformContext _context;
        public PlatformController(LiteraturePlatformContext context)
        {
            _context = context;
            InitDb.Initialize(_context);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Composition>>> GetAllPhone()
        {
            return await _context.Composition.Include(e=>e.User)
                .Include(e => e.Text)
                .Include(e => e.Comments)
                .ToListAsync();
        }

    }
}
