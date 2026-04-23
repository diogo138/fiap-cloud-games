using FIAP.FCG.Infrastructure.Dados.Entidades;

namespace FIAP.FCG.Application.Promocoes.Dtos
{
    public class PromocaoNovoJogoDto
    {
        public int JogoId { get; set; }

        public decimal Valor { get; set; }

        public int? PromocaoId { get; set; }

        public decimal? PercDesconto { get; set; }

        public DateTime DataInicio { get; set; }

    }
}
