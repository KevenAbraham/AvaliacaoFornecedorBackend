namespace WebSupplyAvaliacao.Web.Models;

public class AuditoriaService
{
    //private readonly AppDbContext _context;

    //public AuditoriaService(AppDbContext context)
    //{
    //    _context = context;
    //}

    //private readonly Dictionary<AcaoEnum, int> _acaoIdMap = new Dictionary<AcaoEnum, int>
    //{
    //    { AcaoEnum.Login, 1 },
    //    { AcaoEnum.CadastrarFornecedor, 2 },
    //    { AcaoEnum.CadastrarDocumentoFornecedor, 3 },
    //    { AcaoEnum.RemoverDocumentoFornecedor, 4 },
    //    { AcaoEnum.AlterarFornecedor, 5 },
    //    { AcaoEnum.AvaliarFornecedor, 6 },
    //    { AcaoEnum.CriarUsuario, 7 },
    //    { AcaoEnum.AlterarUsuario, 8 },
    //    { AcaoEnum.Logout, 9 }
    //};

    //public void RegistrarAuditoria(int chave, int usuarioId, AcaoEnum acao)
    //{
    //    // Verificar se o enum está mapeado para um ID na tabela Acao
    //    if (!_acaoIdMap.ContainsKey(acao))
    //    {
    //        throw new Exception("O valor do enum AcaoEnum não está mapeado para um ID válido na tabela Acao.");
    //    }

    //    // Pegar o ID correspondente na tabela Acao
    //    int acaoId = _acaoIdMap[acao];

    //    var auditoria = new Auditoria
    //    {
    //        Chave = chave,
    //        UsuarioId = usuarioId,
    //        AcaoID = acaoId
    //    };

    //    _context.Auditoria.Add(auditoria);
    //    _context.SaveChanges();
    //}
}

