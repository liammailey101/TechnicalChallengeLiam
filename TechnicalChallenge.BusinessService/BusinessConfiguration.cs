using Microsoft.Extensions.DependencyInjection;
using TechnicalChallenge.Data;

namespace TechnicalChallenge.BusinessService
{
    /// <summary>
    /// Provides extension methods for configuring business services.
    /// </summary>
    public static class BusinessConfiguration
    {
        /// <summary>
        /// Adds business services and configurations to the specified IServiceCollection.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <example>
        /// var builder = WebApplication.CreateBuilder(args);
        /// builder.Services.AddBusinessConfiguration();
        /// var app = builder.Build();
        /// </example>
        public static void AddBusinessConfiguration(this IServiceCollection services)
        {
            // Add AutoMapper with the BusinessServiceMappingProfile
            services.AddAutoMapper(typeof(BusinessServiceMappingProfile));

            // Add data configuration so that the business services can access the data layer
            services.AddDataConfiguration();

            // Register services
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<ILoanService, LoanService>();
        }

        /// <summary>
        /// Sets up demo data using the specified IServiceProvider.
        /// </summary>
        /// <param name="serviceProvider">The IServiceProvider to use for setting up demo data.</param>
        /// <example>
        /// using (var scope = app.Services.CreateScope())
        /// {
        ///     BusinessConfiguration.SetUpDemoData(scope.ServiceProvider);
        /// }
        /// </example>
        public static void SetUpDemoData(IServiceProvider serviceProvider)
        {
            DataConfiguration.SetUpDemoData(serviceProvider);
        }
    }
}