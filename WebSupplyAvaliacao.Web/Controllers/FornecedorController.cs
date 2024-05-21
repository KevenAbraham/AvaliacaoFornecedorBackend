using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;
using WebSupplyAvaliacao.Dados.Context;
using WebSupplyAvaliacao.Dominio.Entidade;
using WebSupplyAvaliacao.Web.Models;
using WebSupplyAvaliacao.Web.Models.Enum;

namespace WebSupplyAvaliacao.Web.Controllers;

[Authorize]
[ServiceFilter(typeof(Validacao))]
public class FornecedorController : Controller
{
    private readonly AppDbContext _context;
    private readonly AuditoriaService _auditoriaService;

    public FornecedorController(AppDbContext context, AuditoriaService auditoriaService)
    {
        _context = context;
        _auditoriaService = auditoriaService;
    }

    public bool IsValidCNPJ(string cnpj)
    {
        // Remove todos os caracteres não numéricos do CNPJ.
        cnpj = Regex.Replace(cnpj, "[^0-9]", "");

        // Verifica se o CNPJ possui 14 dígitos.
        if (cnpj.Length != 14)
        {
            return false;
        }

        // Define os multiplicadores para os cálculos dos dígitos verificadores.
        int[] multiplicadores1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicadores2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        string tempCnpj = cnpj.Substring(0, 12);
        int soma = 0;

        // Calcula o primeiro dígito verificador.
        for (int i = 0; i < 12; i++)
        {
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicadores1[i];
        }

        int resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;
        string digito = resto.ToString();
        tempCnpj += digito;

        // Calcula o segundo dígito verificador.
        soma = 0;
        for (int i = 0; i < 13; i++)
        {
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicadores2[i];
        }

        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        digito += resto.ToString();

        // Verifica se os dígitos calculados correspondem aos dígitos informados no CNPJ.
        return cnpj.EndsWith(digito);
    }

    public IActionResult Cadastrar()
    {
        // Recupera as especializações disponíveis e passa para a view.
        ViewBag.Especializacao = _context.Especializacao.ToList();
        return View();
    }

    [HttpPost]
    public IActionResult Cadastrar(Fornecedor fornecedor, int[] especializacoesSelecionadas)
    {
        var userIdentity = User.Identity?.Name;

        var userID = _context.Usuario.FirstOrDefault(x => x.Email == userIdentity)?.ID;

        fornecedor.UsuarioId = (int)userID;

        var cnpjForn = _context.Fornecedor.Any(x => x.CNPJ == fornecedor.CNPJ);

        if (cnpjForn)
        {
            TempData["CNPJMensagem"] = "Este CNPJ já está cadastrado";
            return RedirectToAction("Cadastrar");
        }

        if (!IsValidCNPJ(fornecedor.CNPJ))
        {
            TempData["CNPJMensagem"] = "CNPJ inválido.";
            return RedirectToAction("Cadastrar");
        }

        if (string.IsNullOrEmpty(fornecedor.Complemento))
        {
            fornecedor.Complemento = null;
        }

        if (ModelState.IsValid)
        {
            _context.Fornecedor.Add(fornecedor);

            fornecedor.Especializacoes = new List<Especializacao>();

            _context.SaveChanges();
            
            if (especializacoesSelecionadas == null || especializacoesSelecionadas.Length == 0)
            {
                TempData["ErrorMessage"] = "Caracterização do Fornecedor não pode ser nula.";
                return RedirectToAction("Cadastrar");
            }

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

            //AcaoEnum idAcao = AcaoEnum.CadastrarFornecedor;

            //_auditoriaService.RegistrarAuditoria(fornecedor.ID, (int)userID, idAcao);

            return RedirectToAction("Documento", "Fornecedor", new { fornecedorId = fornecedor.ID });
        }

        if (fornecedor.UF == null)
        {
            TempData["ErrorMessage"] = "Selecione o estado.";
            return RedirectToAction("Cadastrar");
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
        var userIdentity = User.Identity?.Name;
        var userID = _context.Usuario.FirstOrDefault(x => x.Email == userIdentity)?.ID;

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

                    //AcaoEnum idAcao = AcaoEnum.CadastrarDocumentoFornecedor;

                    //_auditoriaService.RegistrarAuditoria(fornecedorId, (int)userID, idAcao);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Conclusao", "Fornecedor");
        }
        else
        {
            ViewBag.FornecedorId = fornecedorId;
            return View();
        }
    }

    public IActionResult DocumentoEdicao(int fornecedorId)
    {
        ViewBag.FornecedorId = fornecedorId;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DocumentoEdicao(int fornecedorId, List<IFormFile> upload)
    {
        var idForn = _context.Fornecedor
            .Include(f => f.Especializacoes)
            .Include(f => f.Documentos)
            .FirstOrDefault(f => f.ID == fornecedorId);

        var userIdentity = User.Identity?.Name;
        var userID = _context.Usuario.FirstOrDefault(x => x.Email == userIdentity)?.ID;

        if (upload != null && upload.Count > 0)
        {
            foreach (var file in upload)
            {
                if (file.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);

                        var docExiste = await _context.Documento.FirstOrDefaultAsync(d => d.FornecedorID == fornecedorId && d.NomeDocumento == file.FileName);

                        if (docExiste != null)
                        {
                            TempData["DocError"] = "Esse documento já foi cadastrado.";
                            ViewBag.EspecializacoesSelecionadas = idForn.Especializacoes.Select(e => e.ID).ToList();
                            ViewBag.Especializacoes = _context.Especializacao.ToList();
                            return View("Editar", idForn);
                        }

                        var documento = new Documento
                        {
                            FornecedorID = fornecedorId,
                            NomeDocumento = file.FileName,
                            Conteudo = stream.ToArray()
                        };

                        _context.Documento.Add(documento);
                    }

                    //AcaoEnum idAcao = AcaoEnum.CadastrarDocumentoFornecedor;

                    //_auditoriaService.RegistrarAuditoria(fornecedorId, (int)userID, idAcao);
                }
            }
            await _context.SaveChangesAsync();

            ViewBag.FornecedorId = fornecedorId;

            return RedirectToAction("DocumentoEdicao", "Fornecedor", new { fornecedorId = fornecedorId });
        }

        return View();
    }


    public IActionResult Conclusao()
    {
        return View();
    }

    public IActionResult Listar()
    {
        var forn = _context.Fornecedor.OrderByDescending(x => x.ID).ToList();
        return View(forn);
    }

    public IActionResult Editar(int id)
    {
        var forn = _context.Fornecedor.SingleOrDefault(forn => forn.ID == id);
        var fornDoc = _context.Fornecedor.Include(f => f.Documentos).FirstOrDefault(f => f.ID == id);

        var fornEspec = _context.Fornecedor.Include(f => f.Especializacoes).SingleOrDefault(forn => forn.ID == id);

        if (fornDoc == null)
        {
            TempData["DocumentoVazio"] = "Não há documentos inseridos";
            return RedirectToAction("Editar");
        }

        var especializacoesSelecionadas = fornEspec.Especializacoes.Select(e => e.ID).ToList();

        ViewBag.EspecializacoesSelecionadas = especializacoesSelecionadas;
        ViewBag.Especializacoes = _context.Especializacao.ToList();

        return View(forn);
    }

    [HttpPost]
    public IActionResult Editar(int id, Fornecedor fornecedor, int[] especializacoesSelecionadas)
    {
        var userIdentity = User.Identity?.Name;

        var userID = _context.Usuario.FirstOrDefault(x => x.Email == userIdentity)?.ID;

        fornecedor.UsuarioId = (int)userID;

        var cnpjExistente = _context.Fornecedor.FirstOrDefault(x => x.CNPJ == fornecedor.CNPJ && x.ID != id);

        var idForn = _context.Fornecedor
            .Include(f => f.Especializacoes)
            .Include(f => f.Documentos)
            .FirstOrDefault(f => f.ID == id);

        if (cnpjExistente != null)
        {
            TempData["CNPJMensagem"] = "Este CNPJ já está cadastrado";
            return RedirectToAction("Editar", idForn);
        }

        if (!IsValidCNPJ(fornecedor.CNPJ))
        {
            TempData["CNPJMensagem"] = "CNPJ inválido.";
            return RedirectToAction("Editar", idForn);
        }

        if (fornecedor.NomeFantasia == null ||
                fornecedor.NomeContato == null ||
                fornecedor.Email == null ||
                fornecedor.CNPJ == null ||
                fornecedor.Telefone == null ||
                fornecedor.Endereco == null ||
                fornecedor.Cidade == null ||
                fornecedor.CEP == null ||
                fornecedor.Bairro == null ||
                fornecedor.Numero == null ||
                fornecedor.UF == null)
        {
            TempData["NullMessage"] = "Os campos não podem estar nulos.";
            return RedirectToAction("Editar", idForn);
        }

        if (ModelState.IsValid)
        {
            idForn.UsuarioId = fornecedor.UsuarioId;
            idForn.NomeFantasia = fornecedor.NomeFantasia;
            idForn.NomeContato = fornecedor.NomeContato;
            idForn.Email = fornecedor.Email;
            idForn.CNPJ = fornecedor.CNPJ;
            idForn.Telefone = fornecedor.Telefone;
            idForn.Status = fornecedor.Status;
            idForn.CEP = fornecedor.CEP;
            idForn.Endereco = fornecedor.Endereco;
            idForn.Complemento = fornecedor.Complemento;
            idForn.Cidade = fornecedor.Cidade;
            idForn.Bairro = fornecedor.Bairro;
            idForn.Numero = fornecedor.Numero;
            idForn.UF = fornecedor.UF;

            idForn.Especializacoes.Clear();

            if (especializacoesSelecionadas != null)
            {
                var especializacoes = _context.Especializacao.Where(e => especializacoesSelecionadas.Contains(e.ID)).ToList();

                foreach (var especializacao in especializacoes)
                {
                    if (!idForn.Especializacoes.Any(e => e.ID == especializacao.ID))
                    {
                        idForn.Especializacoes.Add(especializacao);
                    }
                }

                if (especializacoes.IsNullOrEmpty())
                {
                    TempData["EspecEmpty"] = "A especialização não pode ser nula.";
                    ViewBag.EspecializacoesSelecionadas = idForn.Especializacoes.Select(e => e.ID).ToList();
                    ViewBag.Especializacoes = _context.Especializacao.ToList();
                    return View("Editar", idForn);
                }
            }

            //AcaoEnum idAcao = AcaoEnum.AlterarFornecedor;
            //_auditoriaService.RegistrarAuditoria(fornecedor.ID, (int)userID, idAcao);

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Dados alterados com sucesso";
            return RedirectToAction("Editar", new { id });
        }

        ViewBag.EspecializacoesSelecionadas = idForn.Especializacoes.Select(e => e.ID).ToList();
        ViewBag.Especializacoes = _context.Especializacao.ToList();
        return View(idForn);
    }


    [HttpPost]
    public IActionResult ExcluirDocumento(int id)
    {
        var userIdentity = User.Identity?.Name;
        var userID = _context.Usuario.FirstOrDefault(x => x.Email == userIdentity)?.ID;

        var documento = _context.Documento.FirstOrDefault(d => d.ID == id);

        if (documento != null)
        {
            _context.Documento.Remove(documento);
            _context.SaveChanges();
        }

        //AcaoEnum idAcao = AcaoEnum.RemoverDocumentoFornecedor;
        //_auditoriaService.RegistrarAuditoria(id, (int)userID, idAcao);

        return RedirectToAction("Editar", "Fornecedor");
    }

    public IActionResult VisualizarDocumento(int documentoId)
    {
        var doc = _context.Documento.FirstOrDefault(x => x.ID == documentoId);

        return File(doc.Conteudo, "application/octet-stream", doc.NomeDocumento);
    }
}
