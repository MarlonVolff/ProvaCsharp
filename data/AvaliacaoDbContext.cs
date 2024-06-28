using Microsoft.EntityFrameworkCore;
using servico.models;
using contrato.models;


namespace avaliacao.data
{
   public class AvaliacaoDbContext : DbContext
    {
        public AvaliacaoDbContext(DbContextOptions<AvaliacaoDbContext> options) : base(options) {}

        public DbSet<Servicos> Servicos { get; set; }
        public DbSet<Contratos> Contratos { get; set; }
    }
}