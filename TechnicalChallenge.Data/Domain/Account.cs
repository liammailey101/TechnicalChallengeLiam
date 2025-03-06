using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechnicalChallenge.Data.Domain
{
    public class Account : BaseEntity
    {
        public int CustomerId { get; set; }

        [Column(TypeName = "decimal(19,4)")]
        public decimal Balance { get; set; }

        public int AccountTypeId { get; set; }

        public Guid AccountId { get; set; }

        public string AccountNumber { get; set; }

        public Customer Customer { get; set; }

        public AccountType AccountType { get; set; }
    }
}
