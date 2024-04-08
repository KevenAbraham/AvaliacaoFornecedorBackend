using Microsoft.AspNetCore.Mvc;
using WebSupplyAvaliacao.Dados.Context;

namespace WebSupplyAvaliacao.Web.Controllers
{
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

        public IActionResult Analisar()
        {
            return View();
        }
    }
}
