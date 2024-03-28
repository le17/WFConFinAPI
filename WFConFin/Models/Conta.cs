using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFConFin.Models
{
    public class Conta
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo Descrição é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "O campo Descrição deve ter até 200 caracteres")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O campo Valor é obrigatório")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "O campo DataVencimento é obrigatório")]
        [DataType(DataType.Date)]
        public DateTime? DataVencimento { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataPagamento { get; set; }

        [Required(ErrorMessage = "O campo Situacao é obrigatório")]
        public Situacao Situacao { get; set; }

        [Required(ErrorMessage = "O campo Pessoa é obrigatório")]
        public Guid PessoaId { get; set; }

        public Conta()
        {
            Id = Guid.NewGuid();
        }

        //Relacionamento Entity Framework
        public Pessoa Pessoa { get; set; }
    }
}
