using System.ComponentModel.DataAnnotations;

namespace TechnicalChallenge.Data.Domain
{
    public class LoanRate : BaseEntity
    {
        public byte RatingFrom { get; set; }
        public byte RatingTo { get; set; }
        public byte Duration { get; set; }
        public byte Rate { get; set; }
    }
}
