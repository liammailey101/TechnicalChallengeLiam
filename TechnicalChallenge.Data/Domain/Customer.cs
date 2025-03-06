using System.ComponentModel.DataAnnotations;

namespace TechnicalChallenge.Data.Domain
{
    public class Customer : BaseEntity
    {
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Range(1, 100)]
        public byte CreditScore { get; set; }

        public Guid CustomerNumber { get; set; }

        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
