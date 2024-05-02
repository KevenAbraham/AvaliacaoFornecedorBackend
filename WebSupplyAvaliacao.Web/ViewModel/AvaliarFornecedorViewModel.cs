using WebSupplyAvaliacao.Dominio.Entidade;

namespace WebSupplyAvaliacao.Web.ViewModel;

public class AvaliarFornecedorViewModel
{
    public Fornecedor? Fornecedores { get; set; }
    public List<ServicoAvaliado>? ServicosAvaliados { get; set; }
}
