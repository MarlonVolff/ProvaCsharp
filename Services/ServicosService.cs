// ServicoService.cs
using avaliacao.data;
using servico.models;
using System.Threading.Tasks;

namespace servicoss.Services
{
    public class ServicoService
    {
        private readonly AvaliacaoDbContext _dbContext;
        public ServicoService(AvaliacaoDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Servicos> GetServicoByIdAsync(int id)
        {
            return await _dbContext.Servicos.FindAsync(id);
        }

         // Gravar novo serviço
        public async Task AddServicoAsync(Servicos servico)
        {
            _dbContext.Servicos.Add(servico);
            await _dbContext.SaveChangesAsync();
        }
    }
}
