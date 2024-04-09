using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebSupplyAvaliacao.Dados.Context;
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
        return View();
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
