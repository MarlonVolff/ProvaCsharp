using avaliacao.data;
using contrato.models;
using Microsoft.EntityFrameworkCore;
using System;
// using System.Collections.Generic;
// using System.Linq;
using System.Threading.Tasks;

namespace Contrato.Services
{
    public class ContratoService
    {
        private readonly AvaliacaoDbContext _dbContext;
        public ContratoService(AvaliacaoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Gravar novo contrato
        public async Task AddContratoAsync(Contratos contrato)
        {
            _dbContext.Contratos.Add(contrato);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<List<servico>> GetServicosByClienteIdAsync(int clienteId)
        {
            var contratos = await _dbContext.Contratos
                .Where(c => c.ClienteId == clienteId)
                .Include(c => c.Servico)
                .ToListAsync();

            return contratos.Select(c => c.Servico).ToList();
        }

    }
}

// ERROS NO SERVICOS