namespace TechnicalChallenge.Data
{
    /// <summary>
    /// Provides utility methods for seeding the database with demo data.
    /// </summary>
    public static class DataUtility
    {
        /// <summary>
        /// Seeds the database with demo data.
        /// </summary>
        /// <param name="context">The database context to seed.</param>
        /// <example>
        /// var dbContext = serviceProvider.GetRequiredService<TechnicalChallengeDbContext>();
        /// DataUtility.SeedDatabase(dbContext);
        /// </example>
        public static void SeedDatabase(TechnicalChallengeDbContext context)
        {
            SeedCustomers(context);
            SeedAccountTypes(context);
            SeedAccounts(context);
            SeedLoanRates(context);
        }

        /// <summary>
        /// Seeds the database with customer data.
        /// </summary>
        /// <param name="context">The database context to seed.</param>
        private static void SeedCustomers(TechnicalChallengeDbContext context)
        {
            if (!context.Customers.Any())
            {
                context.Customers.AddRange(
                    new Data.Domain.Customer
                    {
                        Id = 1,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "Admin",
                        FirstName = "Bob",
                        LastName = "Smith",
                        CreditScore = 15,
                        CustomerNumber = Guid.NewGuid()
                    },
                    new Data.Domain.Customer
                    {
                        Id = 2,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "Admin",
                        FirstName = "Jim",
                        LastName = "Jones",
                        CreditScore = 45,
                        CustomerNumber = Guid.NewGuid()
                    },
                    new Data.Domain.Customer
                    {
                        Id = 3,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "Admin",
                        FirstName = "Anne",
                        LastName = "Murphy",
                        CreditScore = 80,
                        CustomerNumber = Guid.NewGuid()
                    }
                );
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Seeds the database with account type data.
        /// </summary>
        /// <param name="context">The database context to seed.</param>
        private static void SeedAccountTypes(TechnicalChallengeDbContext context)
        {
            if (!context.AccountTypes.Any())
            {
                context.AccountTypes.AddRange(
                    new Data.Domain.AccountType
                    {
                        Id = 1,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "Admin",
                        Name = "Current"
                    },
                    new Data.Domain.AccountType
                    {
                        Id = 2,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "Admin",
                        Name = "Savings"
                    },
                    new Data.Domain.AccountType
                    {
                        Id = 3,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "Admin",
                        Name = "Loan"
                    }
                );
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Seeds the database with account data.
        /// </summary>
        /// <param name="context">The database context to seed.</param>
        private static void SeedAccounts(TechnicalChallengeDbContext context)
        {
            if (!context.Accounts.Any())
            {
                context.Accounts.AddRange(
                    new Data.Domain.Account
                    {
                        Id = 1,
                        CustomerId = 1,
                        AccountTypeId = 1,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "Admin",
                        ModifiedDate = DateTime.Now,
                        ModifiedBy = "Admin",
                        AccountId = Guid.NewGuid(),
                        Balance = 564034.04m,
                        AccountNumber = "80786774"
                    },
                    new Data.Domain.Account
                    {
                        Id = 2,
                        CustomerId = 1,
                        AccountTypeId = 2,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "Admin",
                        ModifiedDate = DateTime.Now,
                        ModifiedBy = "Admin",
                        AccountId = Guid.NewGuid(),
                        Balance = 23045.55m,
                        AccountNumber = "32454687"
                    },
                    new Data.Domain.Account
                    {
                        Id = 3,
                        CustomerId = 2,
                        AccountTypeId = 1,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "Admin",
                        ModifiedDate = DateTime.Now,
                        ModifiedBy = "Admin",
                        AccountId = Guid.NewGuid(),
                        Balance = 8006.52m,
                        AccountNumber = "80453366"
                    },
                    new Data.Domain.Account
                    {
                        Id = 4,
                        CustomerId = 2,
                        AccountTypeId = 2,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "Admin",
                        ModifiedDate = DateTime.Now,
                        ModifiedBy = "Admin",
                        AccountId = Guid.NewGuid(),
                        Balance = 809223.25m,
                        AccountNumber = "22554678"
                    },
                    new Data.Domain.Account
                    {
                        Id = 5,
                        CustomerId = 3,
                        AccountTypeId = 1,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "Admin",
                        ModifiedDate = DateTime.Now,
                        ModifiedBy = "Admin",
                        AccountId = Guid.NewGuid(),
                        Balance = 1234887.33m,
                        AccountNumber = "90045663"
                    },
                    new Data.Domain.Account
                    {
                        Id = 6,
                        CustomerId = 3,
                        AccountTypeId = 2,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "Admin",
                        ModifiedDate = DateTime.Now,
                        ModifiedBy = "Admin",
                        AccountId = Guid.NewGuid(),
                        Balance = 44211.18m,
                        AccountNumber = "45456787"
                    }
                );
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Seeds the database with loan rate data.
        /// </summary>
        /// <param name="context">The database context to seed.</param>
        private static void SeedLoanRates(TechnicalChallengeDbContext context)
        {
            if (!context.LoanRates.Any())
            {
                context.LoanRates.AddRange(
                    new Data.Domain.LoanRate
                    {
                        Id = 1,
                        RatingFrom = 20,
                        RatingTo = 50,
                        Duration = 1,
                        Rate = 20
                    },
                   new Data.Domain.LoanRate
                   {
                       Id = 2,
                       RatingFrom = 20,
                       RatingTo = 50,
                       Duration = 3,
                       Rate = 15
                   },
                   new Data.Domain.LoanRate
                   {
                       Id = 3,
                       RatingFrom = 20,
                       RatingTo = 50,
                       Duration = 5,
                       Rate = 10
                   },
                   new Data.Domain.LoanRate
                   {
                       Id = 4,
                       RatingFrom = 50,
                       RatingTo = 101,
                       Duration = 1,
                       Rate = 12
                   },
                   new Data.Domain.LoanRate
                   {
                       Id = 5,
                       RatingFrom = 50,
                       RatingTo = 101,
                       Duration = 3,
                       Rate = 8
                   },
                   new Data.Domain.LoanRate
                   {
                       Id = 6,
                       RatingFrom = 50,
                       RatingTo = 101,
                       Duration = 5,
                       Rate = 5
                   }
                );
                context.SaveChanges();
            }
        }
    }
}