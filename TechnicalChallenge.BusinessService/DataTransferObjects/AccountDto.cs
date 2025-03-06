namespace TechnicalChallenge.BusinessService.DataTransferObjects
{
    /// <summary>
    /// Data transfer object for account information.
    /// </summary>
    public class AccountDto
    {
        public int CustomerId { get; set; }
        public decimal Balance { get; set; }
        public int AccountTypeId { get; set; }
        public Guid AccountId { get; set; }
        public required String AccountNumber { get; set; }
        public required AccountTypeDto AccountType { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
