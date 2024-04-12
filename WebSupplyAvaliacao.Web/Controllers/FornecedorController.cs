using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebSupplyAvaliacao.Dados.Context;
using WebSupplyAvaliacao.Dominio.Entidade;
using WebSupplyAvaliacao.Web.Models;

namespace WebSupplyAvaliacao.Web.Controllers;

[Authorize]
[ServiceFilter(typeof(Validacao))]
public class FornecedorController : Controller
{
    private readonly AppDbContext _context;

    public FornecedorController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Cadastrar1()
    {
        ViewBag.Especializacao = _context.Especializacao.ToList();
        return View();
    }

    [HttpPost]
    public IActionResult Cadastrar1(Fornecedor fornecedor, int[] especializacoesSelecionadas)
    {
        var userIdentity = User.Identity?.Name;
        var userID = _context.Usuario.FirstOrDefault(x => x.Email == userIdentity)?.ID;
        fornecedor.UsuarioId = (int)userID;

        if (ModelState.IsValid)
        {
            _context.Fornecedor.Add(fornecedor);
            fornecedor.Especializacoes = new List<Especializacao>();
            _context.SaveChanges();

            if (especializacoesSelecionadas != null)
            {
                var especializacoes = _context.Especializacao.Where(e => especializacoesSelecionadas.Contains(e.ID)).ToList();

                foreach (var especializacao in especializacoes)
                {
                    fornecedor.Especializacoes.Add(especializacao);
                }
            }

            _context.Fornecedor.Update(fornecedor);
            _context.SaveChanges();
            return RedirectToAction("Cadastrar2", "Fornecedor");
        }

        ViewBag.Especializacao = _context.Especializacao.ToList();
        return View(fornecedor);
    }

    public IActionResult Cadastrar2()
    {
        return View();
    }

    public IActionResult Cadastrar3()
    {
        return View();
    }

    public IActionResult Analisar()
    {
        return View();
    }

    public IActionResult DetalhesFornecedor()
    {
        return View();
    }
}
