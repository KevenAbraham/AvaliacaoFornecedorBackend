using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebSupplyAvaliacao.Dados.Context;
using WebSupplyAvaliacao.Dominio.Entidade;

namespace WebSupplyAvaliacao.Web.Models;

public class Validacao : IAuthorizationFilter
{
    private readonly AppDbContext _context;

    public Validacao(AppDbContext context)
    {
        _context = context;
    }

    //public bool Validator(string email)
    //{
    //    var consulta = _context.Usuario.Any(x => x.Email == email && x.Status == true);

    //    if (consulta)
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var email = context.HttpContext.User.Identity.Name;

        var status = _context.Usuario.Any(x => x.Email == email && x.Status == true);

        if(!status)
        {
            context.Result = new RedirectToActionResult("Negado", "Usuario", null);
        }
    }
}
