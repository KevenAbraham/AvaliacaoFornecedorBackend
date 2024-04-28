using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebSupplyAvaliacao.Dados.Context;
using WebSupplyAvaliacao.Dominio.Entidade;
using WebSupplyAvaliacao.Web.Models;
using WebSupplyAvaliacao.Web.ViewModel;

namespace WebSupplyAvaliacao.Web.Controllers;

[Authorize]
[ServiceFilter(typeof(Validacao))]
public class AvaliarController : Controller
{
    private readonly AppDbContext _context;

    public AvaliarController(AppDbContext context)
    {
        _context = context;
    }

    //Avaliar o Fornecedor
    public IActionResult ListaAvaliar()
    {
        var forn = _context.Fornecedor.OrderByDescending(x => x.ID).ToList();
        return View(forn);
    }

    public IActionResult AvaliarFornecedor(int fornecedorId)
    {
        var servicoAvaliado = _context.ServicoAvaliado.ToList();

        var userIdentity = User.Identity?.Name;
        var userID = _context.Usuario.FirstOrDefault(x => x.Email == userIdentity)?.ID;

        var nomeFornecedor = _context.Fornecedor.Where(x => x.ID == fornecedorId).Select(x => x.NomeFantasia).FirstOrDefault();
        var nomeContato = _context.Fornecedor.Where(x => x.ID == fornecedorId).Select(x => x.NomeContato).FirstOrDefault();
        var emailFornecedor = _context.Fornecedor.Where(x => x.ID == fornecedorId).Select(x => x.Email).FirstOrDefault();

        ViewBag.IdFornecedor = fornecedorId;
        ViewBag.NomeUsuário = userID;
        ViewBag.NomeFornecedor = nomeFornecedor;
        ViewBag.ContatoFornecedor = nomeContato;
        ViewBag.EmailFornecedor = emailFornecedor;
        return View(servicoAvaliado);
    }

    [HttpPost]
    public IActionResult AvaliarFornecedor(int fornecedorId, int servicoAvaliadoId, Avaliar avaliacao)
    {
        if (ModelState.IsValid)
        {
            var userIdentity = User.Identity?.Name;
            var userID = _context.Usuario.FirstOrDefault(x => x.Email == userIdentity)?.ID;

            // Preencha as informações da avaliação
            avaliacao.UsuarioId = userID;
            avaliacao.FornecedorId = fornecedorId;
            avaliacao.ServicoAvaliadoId = servicoAvaliadoId;

            // Salve a avaliação no banco de dados
            _context.Avaliar.Add(avaliacao);
            _context.SaveChanges();

            // Redirecione para alguma página após salvar a avaliação
            return RedirectToAction("ConclusaoAvaliacao", "Avaliar");
        }

        return View();
    }

    public IActionResult ConclusaoAvaliacao()
    {
        return View();
    }







    //Histórico de Avaliações
    public IActionResult ListaAvaliados()
    {
        return View();
    }

    public IActionResult HistoricoAvaliacao()
    {
        return View();
    }

    public IActionResult VisualizarAvaliacao()
    {
        return View();
    }
}
