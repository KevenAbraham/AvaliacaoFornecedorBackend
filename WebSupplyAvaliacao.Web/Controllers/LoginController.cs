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
        try
        {
            var userIdentity = User.Identity?.Name;
            if (userIdentity == null)
            {
                _logger.LogWarning("User.Identity.Name is null.");
                return RedirectToAction("Error", "Home");
            }

            var user = _context.Usuario.FirstOrDefault(x => x.Email == userIdentity);
            if (user == null)
            {
                _logger.LogWarning("User not found in database for email: {Email}", userIdentity);
                return RedirectToAction("Error", "Home");
            }

            _auditoriaService.RegistrarAuditoria(user.ID, user.ID, Models.Enum.AcaoEnum.Login);
            _logger.LogInformation("Login audit registered for user ID: {UserID}", user.ID);

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the login.");
            return RedirectToAction("Error", "Home");
        }
    }

    public IActionResult Sair()
    {
        try
        {
            var userIdentity = User.Identity?.Name;
            if (userIdentity == null)
            {
                _logger.LogWarning("User.Identity.Name is null.");
                return RedirectToAction("Error", "Home");
            }

            var user = _context.Usuario.FirstOrDefault(x => x.Email == userIdentity);
            if (user == null)
            {
                _logger.LogWarning("User not found in database for email: {Email}", userIdentity);
                return RedirectToAction("Error", "Home");
            }

            // Registrar auditoria de logout
            _auditoriaService.RegistrarAuditoria(user.ID, user.ID, Models.Enum.AcaoEnum.Logout);
            _logger.LogInformation("Logout audit registered for user ID: {UserID}", user.ID);

            // Logout do sistema local
            HttpContext.SignOutAsync().Wait();

            // Redirect para logout do Azure AD
            return Redirect("https://login.microsoftonline.com/f42acaec-b2b8-4cf1-aa36-d6ffdd442618/oauth2/logout?post_logout_redirect_uri=https://localhost:7298");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the logout.");
            return RedirectToAction("Error", "Home");
        }
    }
}


