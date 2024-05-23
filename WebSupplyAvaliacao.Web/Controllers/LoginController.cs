using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using WebSupplyAvaliacao.Dados.Context;
using WebSupplyAvaliacao.Web.Models;

namespace WebSupplyAvaliacao.Web.Controllers;

public class LoginController : Controller
{
    private readonly AppDbContext _context;
    private readonly AuditoriaService _auditoriaService;
    private readonly ILogger<LoginController> _logger;

    public LoginController(AppDbContext context, AuditoriaService auditoriaService, ILogger<LoginController> logger)
    {
        _context = context;
        _auditoriaService = auditoriaService;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Sair()
    {
        return Redirect("https://login.microsoftonline.com/f42acaec-b2b8-4cf1-aa36-d6ffdd442618/oauth2/logout?post_logout_redirect_uri=https://localhost:7298");
    }
}


