using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebSupplyAvaliacao.Dominio.Entidade;

namespace WebSupplyAvaliacao.Dados.Context
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Documento>()
            .HasOne(d => d.Fornecedor)
            .WithMany(f => f.Documentos)
            .HasForeignKey(d => d.FornecedorID);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Fornecedor> Fornecedor { get; set; }
        public DbSet<Especializacao> Especializacao { get; set; }
        public DbSet<Documento> Documento { get; set; }
        public DbSet<Avaliar> Avaliar { get; set; }
        public DbSet<ServicoAvaliado> ServicoAvaliado { get; set; }
        public DbSet<Auditoria> Auditoria { get; set; }
        public DbSet<Acao> Acao { get; set; }

    }
}
