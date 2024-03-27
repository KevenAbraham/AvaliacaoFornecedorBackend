using Microsoft.AspNetCore.Mvc;
using WebSupplyAvaliacao.Dados.Context;
using WebSupplyAvaliacao.Dominio.Entidade;

namespace WebSupplyAvaliacao.Web.Controllers;

public class UsuarioController : Controller
{
    private readonly AppDbContext _context;

    public UsuarioController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpPost]
    public IActionResult Cadastrar()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Cadastrar(Usuario usuario)
    {
        if (ModelState.IsValid)
        {
            _context.Usuario.Add(usuario);
            _context.SaveChanges();
            return RedirectToAction("Confirmacao", "Usuario");
        }
        return View(usuario);
    }

    [HttpGet]
    public IActionResult Confirmacao(Usuario usuario)
    {
        return View();
    }

    [HttpGet]
    public IActionResult Listar()
    {
        var usuarios = _context.Usuario.ToList();
        return View(usuarios);
    }

    [HttpGet]
    public IActionResult Editar(int id)
    {
        var usuario = _context.Usuario.SingleOrDefault(user => user.ID == id);

        return View(usuario);
    }

    [HttpPost]
    public IActionResult Editar(int id, [Bind("ID", "Nome", "Email", "Status")] Usuario usuario)
    {
        _context.Usuario.Update(usuario);
        _context.SaveChanges();

        return View(usuario);

    }
}
