using AutoMapper;
using TechnicalChallenge.BusinessService.DataTransferObjects;

namespace TechnicalChallenge.BusinessService
{
    /// <summary>
    /// AutoMapper profile for mapping between DTOs and domain models.
    /// </summary>
    public class BusinessServiceMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessServiceMappingProfile"/> class.
        /// Configures the mappings between DTOs and domain models.
        /// </summary>
        public BusinessServiceMappingProfile()
        {
            CreateMap<CustomerDto, Data.Domain.Customer>().ReverseMap();
            CreateMap<AccountDto, Data.Domain.Account>().ReverseMap();
            CreateMap<AccountTypeDto, Data.Domain.AccountType>().ReverseMap();
            CreateMap<LoanRateDto, Data.Domain.LoanRate>().ReverseMap();
        }
    }
}