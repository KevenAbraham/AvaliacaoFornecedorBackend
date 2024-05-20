using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebSupplyAvaliacao.Dados.Context;
using WebSupplyAvaliacao.Web.Models.Enum;

namespace WebSupplyAvaliacao.Web.Models;

public class Validacao : IAuthorizationFilter
{
    private readonly AppDbContext _context;
    private readonly AuditoriaService _auditoriaService;

    public Validacao(AppDbContext context, AuditoriaService auditoriaService)
    {
        _context = context;
        _auditoriaService = auditoriaService;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var email = context.HttpContext.User.Identity.Name;
        var status = _context.Usuario.Any(x => x.Email == email && x.Status == true);

        if (!status)
        {
            context.Result = new RedirectToActionResult("Negado", "Usuario", null);
        }
    }
}
