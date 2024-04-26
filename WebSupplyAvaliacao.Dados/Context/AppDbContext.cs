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
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Fornecedor> Fornecedor { get; set; }
        public DbSet<Especializacao> Especializacao { get; set; }
        public DbSet<Documento> Documento { get; set; }
        public DbSet<Avaliar> Avaliar { get; set; }
        public DbSet<ServicoAvaliado> ServicoAvaliado { get; set; }

    }
}
