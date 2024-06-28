// using Microsoft.EntityFrameworkCore;
namespace servico.models
{
    public class Servicos
    {
        public int id {get ; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public bool Status { get; set; }
    }
}