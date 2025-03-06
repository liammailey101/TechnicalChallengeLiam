using System.ComponentModel.DataAnnotations;

namespace TechnicalChallenge.Data.Domain
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [MaxLength(50)]
        public string CreatedBy { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? ModifiedBy { get; set; }
    }
}
