using System.ComponentModel.DataAnnotations;

namespace WebSupplyAvaliacao.Dominio.Entidade;

public class Especializacao
{
    [Key]
    public int ID { get; set; }

    public string Tipo { get; set; }

    public ICollection<Fornecedor>? Fornecedores { get; set; }
}
