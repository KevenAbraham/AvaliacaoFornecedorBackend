using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
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

    public IActionResult Criar()
    {
        ViewBag.Especializacao = _context.Especializacao.ToList();
        return View();
    }

    [HttpPost]
    public IActionResult Criar(Fornecedor fornecedor, int[] especializacoesSelecionadas)
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
            return RedirectToAction("Documento", "Fornecedor", new { fornecedorId = fornecedor.ID });
        }

        if (fornecedor.UF == null)
        {
            TempData["ErrorMessage"] = "Selecione o estado.";
            return RedirectToAction("Criar");
        }

        if (especializacoesSelecionadas != null)
        {
            TempData["ErrorMessage"] = "Caracterização do Fornecedor não pode ser nula.";
        }

        ViewBag.Especializacao = _context.Especializacao.ToList();
        return View(fornecedor);
    }

    public IActionResult Documento(int fornecedorId)
    {
        ViewBag.FornecedorId = fornecedorId;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Documento(int fornecedorId, List<IFormFile> upload)
    {
        if (upload != null && upload.Count > 0)
        {
            foreach (var file in upload)
            {
                if (file.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);

                        var documento = new Documento
                        {
                            FornecedorID = fornecedorId,
                            NomeDocumento = file.FileName,
                            Conteudo = stream.ToArray()
                        };

                        _context.Documento.Add(documento);
                    }
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Conclusao", "Fornecedor");
        }
        else
        {
            // Se não houver arquivo enviado, retorne para a mesma view com uma mensagem de erro
            TempData["ErrorMessage"] = "Nenhum arquivo foi enviado.";
            ViewBag.FornecedorId = fornecedorId;
            return View();
        }
    }

    public IActionResult Conclusao()
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
