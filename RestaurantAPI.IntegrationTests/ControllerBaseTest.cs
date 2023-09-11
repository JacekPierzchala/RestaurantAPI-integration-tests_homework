using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestaurantAPI.Entities;
using System.Linq;
using System.Net.Http;
using Xunit;

namespace RestaurantAPI.IntegrationTests
{
    public class ControllerBaseTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        protected HttpClient _client;
        protected WebApplicationFactory<Startup> _factory;
        public ControllerBaseTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory
             .WithWebHostBuilder(builder =>
             {
                 builder.ConfigureServices(services =>
                 {
                     var dbContextOptions = services
                         .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));

                     services.Remove(dbContextOptions);

                     services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();

                     services.AddMvc(option => option.Filters.Add(new FakeUserFilter()));


                     services
                      .AddDbContextFactory<RestaurantDbContext>(options => options.UseInMemoryDatabase("RestaurantDb"),ServiceLifetime.Transient);

                 });
             });

            _client = _factory.CreateClient();
        }

        protected void SeedEntity<T>(T Entity)
            where T:class
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<RestaurantDbContext>();

            _dbContext.Set<T>().Add(Entity);
            _dbContext.SaveChanges();
        }


    }
}
