using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechnicalChallenge.Data.Respository;

namespace TechnicalChallenge.Data
{
    /// <summary>
    /// Provides extension methods for configuring data services.
    /// </summary>
    public static class DataConfiguration
    {
        /// <summary>
        /// Adds data services and configurations to the specified IServiceCollection.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <example>
        /// var builder = WebApplication.CreateBuilder(args);
        /// builder.Services.AddDataConfiguration();
        /// var app = builder.Build();
        /// </example>
        public static void AddDataConfiguration(this IServiceCollection services)
        {
            // Configure in-memory database for use in Technical Challenge
            // For production, use a real database like SQL Server
            services.AddDbContext<TechnicalChallengeDbContext>(options =>
                options.UseInMemoryDatabase("TechnicalChallengeInMemoryDb"));

            // Register UnitOfWork and GenericRepository
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        }

        /// <summary>
        /// Sets up demo data using the specified IServiceProvider.
        /// </summary>
        /// <param name="serviceProvider">The IServiceProvider to use for setting up demo data.</param>
        /// <example>
        /// var builder = WebApplication.CreateBuilder(args);
        /// var app = builder.Build();
        /// DataConfiguration.SetUpDemoData(app.Services);
        /// </example>
        public static void SetUpDemoData(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TechnicalChallengeDbContext>();
            DataUtility.SeedDatabase(dbContext);
        }
    }
}