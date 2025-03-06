namespace TechnicalChallenge.BusinessService.DataTransferObjects
{
    /// <summary>
    /// Data transfer object for customer information.
    /// </summary>
    public class CustomerDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public byte CreditScore { get; set; }
        public Guid CustomerNumber { get; set; }
        public ICollection<AccountDto> Accounts { get; set; } = [];
    }
}
