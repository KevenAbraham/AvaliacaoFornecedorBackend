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

namespace WebSupplyAvaliacao.Web.Controllers;

[Authorize]
[ServiceFilter(typeof(Validacao))]
// Controlador responsável por lidar com as ações relacionadas aos fornecedores do sistema.
// Requer autorização para acessar as ações, garantindo que apenas usuários autenticados possam utilizá-las.
public class FornecedorController : Controller
{
    private readonly AppDbContext _context;

    public FornecedorController(AppDbContext context)
    {
        _context = context;
    }

    // Método para calcular se o CNPJ é válido.
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

    // Ação para exibir a página de cadastro de fornecedor.
    public IActionResult Cadastrar()
    {
        // Recupera as especializações disponíveis e passa para a view.
        ViewBag.Especializacao = _context.Especializacao.ToList();
        return View();
    }

    // Ação para cadastrar um novo fornecedor.
    [HttpPost]
    public IActionResult Cadastrar(Fornecedor fornecedor, int[] especializacoesSelecionadas)
    {
        // Obtém o email do usuário autenticado.
        var userIdentity = User.Identity?.Name;

        // Obtém o ID do usuário autenticado com base no email.
        var userID = _context.Usuario.FirstOrDefault(x => x.Email == userIdentity)?.ID;

        // Define o ID do usuário para o fornecedor.
        fornecedor.UsuarioId = (int)userID;

        // Verifica se o CNPJ do fornecedor já está cadastrado.
        var cnpjForn = _context.Fornecedor.Any(x => x.CNPJ == fornecedor.CNPJ);

        //Se já está cadastrado, retorna uma mensagem de erro.
        if (cnpjForn)
        {
            TempData["CNPJMensagem"] = "Este CNPJ já está cadastrado";
            return RedirectToAction("Cadastrar");
        }

        // Verifica se o CNPJ fornecido é válido.
        if (!IsValidCNPJ(fornecedor.CNPJ))
        {
            TempData["CNPJMensagem"] = "CNPJ inválido.";
            return RedirectToAction("Cadastrar");
        }

        // Se o complemento do endereço do fornecedor estiver vazio, define como null.
        if (string.IsNullOrEmpty(fornecedor.Complemento))
        {
            fornecedor.Complemento = null;
        }

        // Verifica se o modelo do fornecedor é válido.
        if (ModelState.IsValid)
        {
            // Adiciona o fornecedor ao contexto do banco de dados.
            _context.Fornecedor.Add(fornecedor);

            // Inicializa a lista de especializações do fornecedor.
            fornecedor.Especializacoes = new List<Especializacao>();

            // Salva as mudanças no banco de dados.
            _context.SaveChanges();

            // Verifica se foram selecionadas especializações para o fornecedor.
            if (especializacoesSelecionadas == null || especializacoesSelecionadas.Length == 0)
            {
                TempData["ErrorMessage"] = "Caracterização do Fornecedor não pode ser nula.";
                return RedirectToAction("Cadastrar");
            }

            // Se houver especializações selecionadas, as adiciona ao fornecedor.
            if (especializacoesSelecionadas != null)
            {
                var especializacoes = _context.Especializacao.Where(e => especializacoesSelecionadas.Contains(e.ID)).ToList();

                foreach (var especializacao in especializacoes)
                {
                    fornecedor.Especializacoes.Add(especializacao);
                }
            }

            // Atualiza o fornecedor com as especializações selecionadas.
            _context.Fornecedor.Update(fornecedor);

            // Salva as mudanças no banco de dados.
            _context.SaveChanges();

            // Redireciona para a ação de envio de documentos, passando o ID do fornecedor.
            return RedirectToAction("Documento", "Fornecedor", new { fornecedorId = fornecedor.ID });
        }

        // Se o modelo não for válido, verifica se o estado (UF) foi selecionado.
        if (fornecedor.UF == null)
        {
            TempData["ErrorMessage"] = "Selecione o estado.";
            return RedirectToAction("Cadastrar");
        }

        // Se o modelo não for válido e o estado foi selecionado, recarrega a view de cadastro com os dados fornecidos.
        ViewBag.Especializacao = _context.Especializacao.ToList();
        return View(fornecedor);
    }

    /// <summary>
    /// Exibe a página para upload de documentos relacionados a um fornecedor específico.
    /// </summary>
    /// <param name="fornecedorId">O ID do fornecedor para o qual os documentos serão enviados.</param>
    /// <returns>Uma IActionResult representando a exibição da página de upload de documentos.</returns>
    public IActionResult Documento(int fornecedorId)
    {
        // Define o ID do fornecedor na ViewBag para ser acessado na view.
        ViewBag.FornecedorId = fornecedorId;

        // Retorna a view responsável pelo upload de documentos.
        return View();
    }

    /// <summary>
    /// Método utilizado para receber e armazenar documentos enviados por um fornecedor.
    /// </summary>
    /// <param name="fornecedorId">O ID do fornecedor para o qual os documentos serão armazenados.</param>
    /// <param name="upload">Uma lista de arquivos (documentos) enviados pelo fornecedor.</param>
    /// <returns>Uma IActionResult representando o redirecionamento para outra ação.</returns>
    [HttpPost]
    public async Task<IActionResult> Documento(int fornecedorId, List<IFormFile> upload)
    {
        // Verifica se foram enviados arquivos (documentos) pelo fornecedor.
        if (upload != null && upload.Count > 0)
        {
            foreach (var file in upload)
            {
                // Verifica se o arquivo tem conteúdo.
                if (file.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        // Copia o conteúdo do arquivo para um MemoryStream.
                        await file.CopyToAsync(stream);

                        // Cria um novo objeto Documento para armazenar os detalhes do arquivo.
                        var documento = new Documento
                        {
                            FornecedorID = fornecedorId,
                            NomeDocumento = file.FileName,
                            Conteudo = stream.ToArray()
                        };

                        // Adiciona o documento ao contexto do banco de dados.
                        _context.Documento.Add(documento);
                    }
                }
            }

            // Salva as mudanças no banco de dados.
            await _context.SaveChangesAsync();

            // Redireciona para a página de conclusão do processo de envio de documentos.
            return RedirectToAction("Conclusao", "Fornecedor");
        }
        else
        {
            // Se nenhum documento foi enviado, retorna para a página de upload de documentos.
            ViewBag.FornecedorId = fornecedorId;
            return View();
        }
    }

    /// <summary>
    /// Exibe a página para edição e adição de documentos relacionados a um fornecedor específico.
    /// </summary>
    /// <param name="fornecedorId">O ID do fornecedor para o qual os documentos serão editados ou adicionados.</param>
    /// <returns>Uma IActionResult representando a exibição da página de edição e adição de documentos.</returns>
    public IActionResult DocumentoEdicao(int fornecedorId)
    {
        // Define o ID do fornecedor na ViewBag para ser acessado na view.
        ViewBag.FornecedorId = fornecedorId;

        // Retorna a view responsável pela edição e adição de documentos.
        return View();
    }

    /// <summary>
    /// Método utilizado para editar e adicionar documentos relacionados a um fornecedor específico.
    /// </summary>
    /// <param name="fornecedorId">O ID do fornecedor para o qual os documentos serão editados ou adicionados.</param>
    /// <param name="upload">Uma lista de arquivos (documentos) enviados pelo fornecedor para edição ou adição.</param>
    /// <returns>Uma IActionResult representando o redirecionamento para outra ação.</returns>
    [HttpPost]
    public async Task<IActionResult> DocumentoEdicao(int fornecedorId, List<IFormFile> upload)
    {
        // Obtém o fornecedor com suas especializações e documentos relacionados.
        var idForn = _context.Fornecedor
            .Include(f => f.Especializacoes)
            .Include(f => f.Documentos)
            .FirstOrDefault(f => f.ID == fornecedorId);

        // Verifica se foram enviados arquivos (documentos) pelo fornecedor para edição ou adição.
        if (upload != null && upload.Count > 0)
        {
            foreach (var file in upload)
            {
                // Verifica se o arquivo tem conteúdo.
                if (file.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);

                        // Verifica se o documento já existe para evitar duplicatas.
                        var docExiste = await _context.Documento.FirstOrDefaultAsync(d => d.FornecedorID == fornecedorId && d.NomeDocumento == file.FileName);

                        // Se o documento já existir, exibe uma mensagem de erro e retorna para a página de edição.
                        if (docExiste != null)
                        {
                            TempData["DocError"] = "Esse documento já foi cadastrado.";
                            ViewBag.EspecializacoesSelecionadas = idForn.Especializacoes.Select(e => e.ID).ToList();
                            ViewBag.Especializacoes = _context.Especializacao.ToList();
                            return View("Editar", idForn);
                        }

                        // Cria um novo objeto Documento para armazenar os detalhes do arquivo e o adiciona ao contexto do banco de dados.
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
            // Salva as mudanças no banco de dados
            await _context.SaveChangesAsync();

            ViewBag.FornecedorId = fornecedorId;

            // Redireciona para a página de edição de documentos do fornecedor.
            return RedirectToAction("DocumentoEdicao", "Fornecedor", new { fornecedorId = fornecedorId });
        }

        // Se nenhum documento foi enviado, retorna para a página de edição sem fazer alterações.
        return View();
    }


    //3° passo do Cadastro - Tela de conclusão de cadastro do fornecedor.
    public IActionResult Conclusao()
    {
        return View();
    }

    //Tela para listar todos os fornecedores e ordená-los por ordem decrescente (o último adicionado fica como primeiro na lista).
    public IActionResult Listar()
    {
        var forn = _context.Fornecedor.OrderByDescending(x => x.ID).ToList();
        return View(forn);
    }

    /// <summary>
    /// Exibe a página para edição dos dados de um fornecedor, incluindo a visualização e seleção de documentos e especializações.
    /// </summary>
    /// <param name="id">O ID do fornecedor a ser editado.</param>
    /// <returns>Uma IActionResult representando a exibição da página de edição de fornecedor.</returns>
    public IActionResult Editar(int id)
    {
        // Obtém o fornecedor e seus documentos relacionados com base no ID fornecido.
        var forn = _context.Fornecedor.SingleOrDefault(forn => forn.ID == id);
        var fornDoc = _context.Fornecedor.Include(f => f.Documentos).FirstOrDefault(f => f.ID == id);

        // Obtém o fornecedor e suas especializações relacionadas com base no ID fornecido.
        var fornEspec = _context.Fornecedor.Include(f => f.Especializacoes).SingleOrDefault(forn => forn.ID == id);

        // Verifica se o fornecedor possui documentos relacionados.
        if (fornDoc == null)
        {
            // Se não houver documentos, exibe uma mensagem de aviso e redireciona para a página de edição.
            TempData["DocumentoVazio"] = "Não há documentos inseridos";
            return RedirectToAction("Editar");
        }

        // Recupera as especializações do fornecedor
        var especializacoesSelecionadas = fornEspec.Especializacoes.Select(e => e.ID).ToList();

        // Passa as especializações para a view
        ViewBag.EspecializacoesSelecionadas = especializacoesSelecionadas;
        ViewBag.Especializacoes = _context.Especializacao.ToList();

        // Retorna a view de edição do fornecedor.
        return View(forn);
    }

    /// <summary>
    /// Processa a submissão do formulário de edição de dados de um fornecedor.
    /// </summary>
    /// <param name="id">O ID do fornecedor a ser editado.</param>
    /// <param name="fornecedor">O objeto Fornecedor contendo os novos dados do fornecedor.</param>
    /// <param name="especializacoesSelecionadas">Um array de inteiros representando as especializações selecionadas para o fornecedor.</param>
    /// <returns>Uma IActionResult representando o redirecionamento para a página de edição de fornecedor ou a página de detalhes do fornecedor.</returns>
    [HttpPost]
    public IActionResult Editar(int id, Fornecedor fornecedor, int[] especializacoesSelecionadas)
    {
        // Obtém o nome de usuário atualmente autenticado.
        var userIdentity = User.Identity?.Name;

        // Obtém o ID do usuário com base no email do usuário autenticado.
        var userID = _context.Usuario.FirstOrDefault(x => x.Email == userIdentity)?.ID;

        // Define o ID do usuário no objeto fornecedor.
        fornecedor.UsuarioId = (int)userID;

        // Verifica se já existe um fornecedor com o CNPJ fornecido, excluindo o fornecedor atual.
        var cnpjExistente = _context.Fornecedor.FirstOrDefault(x => x.CNPJ == fornecedor.CNPJ && x.ID != id);

        // Obtém os dados do fornecedor a ser editado.
        var idForn = _context.Fornecedor
            .Include(f => f.Especializacoes)
            .Include(f => f.Documentos)
            .FirstOrDefault(f => f.ID == id);

        //Mensagem de erro caso o CNPJ já esteja inserido no banco de dados.
        if (cnpjExistente != null)
        {
            TempData["CNPJMensagem"] = "Este CNPJ já está cadastrado";
            return RedirectToAction("Editar", idForn);
        }

        //Validando caso o CNPJ inserido não seja válido.
        if (!IsValidCNPJ(fornecedor.CNPJ))
        {
            TempData["CNPJMensagem"] = "CNPJ inválido.";
            return RedirectToAction("Editar", idForn);
        }

        //Lógica para algum campo nulo.
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

        //Validnando se o ModelState é válido
        if (ModelState.IsValid)
        {
            // Atualiza os dados do fornecedor com os novos dados fornecidos.
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

            // Remove todas as especializações associadas ao fornecedor.
            idForn.Especializacoes.Clear();

            // Adiciona as novas especializações selecionadas ao fornecedor.
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

                // Se nenhuma especialização for selecionada, exibe uma mensagem de erro.
                if (especializacoes.IsNullOrEmpty())
                {
                    TempData["EspecEmpty"] = "A especialização não pode ser nula.";
                    ViewBag.EspecializacoesSelecionadas = idForn.Especializacoes.Select(e => e.ID).ToList();
                    ViewBag.Especializacoes = _context.Especializacao.ToList();
                    return View("Editar", idForn);
                }
            }

            // Salva as alterações no banco de dados.
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Dados alterados com sucesso";
            return RedirectToAction("Editar", new { id });
        }

        // Se houver erros de validação, recarregue a página com os dados fornecidos
        ViewBag.EspecializacoesSelecionadas = idForn.Especializacoes.Select(e => e.ID).ToList();
        ViewBag.Especializacoes = _context.Especializacao.ToList();
        return View(idForn);
    }


    /// <summary>
    /// Processa a exclusão de um documento associado a um fornecedor.
    /// </summary>
    /// <param name="id">O ID do documento a ser excluído.</param>
    /// <returns>Uma IActionResult representando o redirecionamento para a página de edição de fornecedor.</returns>
    [HttpPost]
    public IActionResult ExcluirDocumento(int id)
    {
        // Obtém o documento com base no ID fornecido.
        var documento = _context.Documento.FirstOrDefault(d => d.ID == id);

        // Verifica se o documento existe.
        if (documento != null)
        {
            // Remove o documento do contexto.
            _context.Documento.Remove(documento);
            // Salva as alterações no banco de dados.
            _context.SaveChanges();
        }

        // Redireciona para a página de edição de fornecedor.
        return RedirectToAction("Editar", "Fornecedor");
    }

    /// <summary>
    /// Retorna o conteúdo de um documento para visualização ou download.
    /// </summary>
    /// <param name="documentoId">O ID do documento a ser visualizado.</param>
    /// <returns>Um FileResult representando o conteúdo do documento.</returns>
    public IActionResult VisualizarDocumento(int documentoId)
    {
        // Obtém o documento com base no ID fornecido.
        var doc = _context.Documento.FirstOrDefault(x => x.ID == documentoId);

        // Retorna o conteúdo do documento como um arquivo para download ou visualização.
        // O tipo MIME "application/octet-stream" é usado para indicar que o conteúdo é um fluxo de bytes arbitrários.
        return File(doc.Conteudo, "application/octet-stream", doc.NomeDocumento);
    }
}
