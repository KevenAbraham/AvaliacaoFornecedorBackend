using System.ComponentModel.DataAnnotations;

namespace WebSupplyAvaliacao.Dominio.Entidade;

public class ServicoAvaliado
{
    [Key]
    public int ID { get; set; }

    public string Descricao { get; set; }
}