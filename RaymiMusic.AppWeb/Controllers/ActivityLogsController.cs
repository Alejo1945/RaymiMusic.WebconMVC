using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaymiMusic.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace RaymiMusic.AppWeb.Controllers
{
    public class ActivityLogsController : Controller
    {
        private readonly AppDbContext _ctx;

        public ActivityLogsController(AppDbContext ctx) => _ctx = ctx;

        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var logs = await _ctx.ActivityLogs.ToListAsync();
            return View(logs);
        }
    }
}
