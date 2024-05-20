using System.ComponentModel.DataAnnotations;

namespace WebSupplyAvaliacao.Dominio.Entidade;

public class Acao
{
    [Key]
    public int Id { get; set; }

    public string Descricao { get; set; }
}
