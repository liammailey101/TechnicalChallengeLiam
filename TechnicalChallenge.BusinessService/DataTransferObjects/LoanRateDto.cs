namespace TechnicalChallenge.BusinessService.DataTransferObjects
{
    /// <summary>
    /// Data transfer object for loan rate information.
    /// </summary>
    public class LoanRateDto
    {
        public byte RatingFrom { get; set; }
        public byte RatingTo { get; set; }
        public byte Duratiion { get; set; }
        public byte Rate { get; set; }
    }
}
