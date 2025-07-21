using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaymiMusic.Api.Data;

namespace RaymiMusic.AppWeb.Controllers
{
    public class DashAdminController : Controller
    {
        private readonly AppDbContext _ctx;

        public DashAdminController(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index()
        {

            ViewBag.TotalUsuarios = await _ctx.Usuarios.CountAsync();
            ViewBag.TotalArtistas = await _ctx.Usuarios.CountAsync(u => u.Rol == "artista");
            ViewBag.TotalAlbumes = await _ctx.Albumes.CountAsync();
            ViewBag.TotalCanciones = await _ctx.Canciones.CountAsync();
            ViewBag.TotalGeneros = await _ctx.Generos.CountAsync();
            ViewBag.TotalPlanes = await _ctx.Planes.CountAsync();

           
            return View();
        }
    }

}
