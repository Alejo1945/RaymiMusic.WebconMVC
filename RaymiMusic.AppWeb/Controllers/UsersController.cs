using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RaymiMusic.Api.Data;
using RaymiMusic.AppWeb.Models;
using RaymiMusic.Modelos;
using System.Numerics;

namespace RaymiMusic.AppWeb.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext _ctx;
        private readonly ILogger<UsersController> _logger;
        public UsersController(AppDbContext ctx) => _ctx = ctx;

        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Statistics()
        {
            var usuariosPorPlan = await _ctx.Usuarios
                .GroupBy(u => u.PlanSuscripcion.Nombre)
                .Select(g => new { Plan = g.Key, Count = g.Count() })
                .ToListAsync();

            return View(usuariosPorPlan);
        }

        // GET: /Users
        public async Task<IActionResult> Index()
        {
            var usuarios = await _ctx.Usuarios
                .Include(u => u.PlanSuscripcion)
                .AsNoTracking()
                .ToListAsync();
            return View(usuarios);
        }

        // GET: /Users/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var user = await _ctx.Usuarios
                .Include(u => u.PlanSuscripcion)
                .Include(u => u.Perfil)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();
            return View(user);
        }

        // GET: /Users/Create
        public IActionResult Create()
        {
            ViewData["Planes"] = new SelectList(_ctx.Planes, "Id", "Nombre");
            return View();
        }

        // POST: /Users/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Correo,HashContrasena,Rol,PlanSuscripcionId")] Usuario user)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Planes"] = new SelectList(_ctx.Planes, "Id", "Nombre", user.PlanSuscripcionId);
                return View(user);
            }

            user.Id = Guid.NewGuid();
            _ctx.Usuarios.Add(user);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // GET: /Users/Edit/{id}
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var usuario = await _ctx.Usuarios
                .Include(u => u.PlanSuscripcion)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null) return NotFound();

            // Mapeo a ViewModel
            var viewModel = new UsuarioEditViewModel
            {
                Id = usuario.Id,
                Correo = usuario.Correo,
                PlanSuscripcionId = usuario.PlanSuscripcionId
            };

            // Cargar planes para el dropdown
            ViewBag.Planes = new SelectList(await _ctx.Planes.ToListAsync(), "Id", "Nombre");

            return View(viewModel); // ← Envía el ViewModel, no la entidad
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UsuarioEditViewModel model)
        {
            if (id != model.Id) return BadRequest();

            // Validación optimizada en una sola consulta
            var planValido = await _ctx.Planes
                .AnyAsync(p => p.Id == model.PlanSuscripcionId);

            if (!planValido)
            {
                ModelState.AddModelError("PlanSuscripcionId", "El plan seleccionado no es válido");
            }

            if (!ModelState.IsValid)
            {
                var planes = await _ctx.Planes.AsNoTracking().ToListAsync();
                ViewBag.Planes = new SelectList(planes, "Id", "Nombre", model.PlanSuscripcionId);
                return View(model);
            }

            var usuario = await _ctx.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            // Actualización selectiva
            usuario.Correo = model.Correo;
            usuario.PlanSuscripcionId = model.PlanSuscripcionId;

            try
            {
                await _ctx.SaveChangesAsync();
                TempData["Success"] = "Usuario actualizado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario");
                TempData["Error"] = "Error al guardar los cambios";
                return RedirectToAction(nameof(Edit), new { id });
            }
        }

        // GET: /Users/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _ctx.Usuarios
                .Include(u => u.PlanSuscripcion)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST: /Users/Delete/{id}
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var user = await _ctx.Usuarios.FindAsync(id);
            if (user != null)
            {
                _ctx.Usuarios.Remove(user);
                await _ctx.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
