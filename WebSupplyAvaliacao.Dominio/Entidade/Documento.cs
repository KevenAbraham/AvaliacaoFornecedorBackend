using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebSupplyAvaliacao.Dominio.Entidade;

public class Documento
{
    [Key]
    public int ID { get; set; }

    [Required]
    public string NomeDocumento { get; set; }

    [Required]
    public byte[] Conteudo { get; set; }

    [ForeignKey("Fornecedor")]
    public int FornecedorID { get; set; }

    public virtual Fornecedor Fornecedor { get; set; }

    public DateTime Data { get; set; } = DateTime.Now;
}
