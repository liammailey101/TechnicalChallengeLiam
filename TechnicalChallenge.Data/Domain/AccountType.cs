using System.ComponentModel.DataAnnotations;

namespace TechnicalChallenge.Data.Domain
{
    public class AccountType : BaseEntity
    {
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}
