using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    [HttpGet]
    public IActionResult Cadastrar()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Cadastrar(Usuario usuario)
    {
        if (ModelState.IsValid)
        {
            var emailExistente = _context.Usuario.Any(x => x.Email == usuario.Email);

            if (emailExistente)
            {
                TempData["ErrorMessage"] = "Este email já está cadastrado.";
                return RedirectToAction("Cadastrar");
            }

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
        var emailExiste = _context.Usuario.FirstOrDefault(x => x.Email == usuario.Email && x.ID != id);

        var checkUser = _context.Usuario.SingleOrDefault(x => x.ID == id);

        if (usuario.Nome == null || usuario.Email == null)
        {
            TempData["ErrorMessage"] = "Os campos não podem ser nulos.";
            return RedirectToAction("Editar");
        }

        if (usuario.Nome == checkUser.Nome && usuario.Email == checkUser.Email && usuario.Status == checkUser.Status)
        {
            TempData["AlertMessage"] = "Essas informações já estão cadastradas.";
            return RedirectToAction("Editar");
        }

        if (emailExiste != null)
        {
            TempData["ErrorMessage"] = "Este email já está cadastrado.";
            return RedirectToAction("Editar");
        } 
        else
        {
            _context.Entry(checkUser).State = EntityState.Detached;

            _context.Usuario.Update(usuario);
            _context.SaveChanges();
            TempData["InfoMessage"] = "Usuário alterado com sucesso.";
            return RedirectToAction("Editar");
        }
    }
}
