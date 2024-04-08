using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSupplyAvaliacao.Dados.Context;
using WebSupplyAvaliacao.Dominio.Entidade;
using WebSupplyAvaliacao.Web.Models;

namespace WebSupplyAvaliacao.Web.Controllers;

public class UsuarioController : Controller
{
    private readonly AppDbContext _context;
    private readonly Validacao _validacao;

    public UsuarioController(AppDbContext context, Validacao validacao)
    {
        _context = context;
        _validacao = validacao;
    }

    [Authorize]
    [ServiceFilter(typeof(Validacao))]
    public IActionResult Cadastrar()
    {
        //var user = _validacao.Validator(User.Identity.Name);

        //if(user != null)
        //{
        //    return View();
        //}
        return View();
    }

    [HttpPost]
    [Authorize]
    [ServiceFilter(typeof(Validacao))]
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

    [Authorize]
    [ServiceFilter(typeof(Validacao))]
    public IActionResult Confirmacao(Usuario usuario)
    {
        return View();
    }

    [Authorize]
    [ServiceFilter(typeof(Validacao))]
    public IActionResult Listar()
    {
        var usuarios = _context.Usuario.OrderByDescending(x => x.ID).ToList();
        return View(usuarios);
    }

    [Authorize]
    [ServiceFilter(typeof(Validacao))]
    public IActionResult Editar(int id)
    {
        var usuario = _context.Usuario.SingleOrDefault(user => user.ID == id);

        return View(usuario);
    }

    [HttpPost]
    [Authorize]
    [ServiceFilter(typeof(Validacao))]
    public IActionResult Editar(int id, Usuario usuario)
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
        else if(ModelState.IsValid)
        {
            _context.Entry(checkUser).State = EntityState.Detached;

            _context.Usuario.Update(usuario);
            _context.SaveChanges();
            TempData["InfoMessage"] = "Usuário alterado com sucesso.";
            return RedirectToAction("Editar");
        }
        return View(usuario);
    }

    public IActionResult Negado()
    {
        return View();
    }
}
