using WebSupplyAvaliacao.Dominio.Entidade;

namespace WebSupplyAvaliacao.Web.ViewModel;

public class MediaAvaliacaoViewModel
{
    public List<Fornecedor> Fornecedores { get; set; }
    public double MediaAvaliacao { get; set; }
    public DateTime? DataUltimaAval { get; set; }
}
