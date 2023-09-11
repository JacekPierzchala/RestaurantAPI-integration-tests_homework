using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestaurantAPI.Entities;
using RestaurantAPI.IntegrationTests.Helpers;
using RestaurantAPI.Models;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantAPI.IntegrationTests
{
    public class DishControllerTests : ControllerBaseTest
    {
        public DishControllerTests(WebApplicationFactory<Startup> factory)
            :base(factory) { }

        [Fact]
        public async Task CreateDish_WithValidModel_ReturnsCreatedStatus()
        {
            // arrange
            var restaurantId = 1;
            var model = new CreateDishDto()
            {
                 Description= "Test",
                 Name= "Test",
                 Price=9.9m,
                 RestaurantId= restaurantId
            };

            var httpContent = model.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync($"/api/restaurant/{restaurantId}/dish", httpContent);

            // arrange 
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateDish_WithInvalidModel_ReturnsBadRequest()
        {
            // arrange
            var restaurantId = 1;

            var model = new CreateDishDto()
            {
                Description = "Test",
                Price = 9.9m,
                RestaurantId = restaurantId
            };

            var httpContent = model.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync($"/api/restaurant/{restaurantId}/dish", httpContent);

            // arrange
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Delete_AllDishesForRestaurant_ReturnsNoContent()
        {
            // arrange

            var restaurant = new Restaurant()
            {
                CreatedById = 1,
                Name = "Test"
            };
            SeedEntity(restaurant);

            var dish = new Dish()
            {
                Description = "Test",
                Price = 9.9m,
                RestaurantId = restaurant.Id
            };

            SeedEntity(dish);

            // act
            var response = await _client.DeleteAsync($"/api/restaurant/{restaurant.Id}/dish");

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_AllDishesForNonExistingRestaurant_ReturnsNotFound()
        {
            // arrange
            var restaurantId = 10;
     
            // act
            var response = await _client.DeleteAsync($"/api/restaurant/{restaurantId}/dish");

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetAllDishes_ForRestaurant_ReturnsOkResult()
        {
            // arrange
            var restaurantId = 1;

            // act
            var response = await _client.GetAsync($"/api/restaurant/{restaurantId}/dish");

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAllDishes_ForNonExistingRestaurant_ReturnsNotFoundResult()
        {
            // arrange
            var restaurantId = 100;

            // act
            var response = await _client.GetAsync($"/api/restaurant/{restaurantId}/dish");
            
            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetDish_ForGivenRestaurant_ReturnsOkResult()
        {
            // arrange

            var restaurant = new Restaurant()
            {
                CreatedById = 1,
                Name = "Test"
            };
            SeedEntity(restaurant);

            var dish = new Dish()
            {
                Description = "Test",
                Price = 9.9m,
                RestaurantId = restaurant.Id
            };

            SeedEntity(dish);

            // act
            var response = await _client.GetAsync($"/api/restaurant/{restaurant.Id}/dish/{dish.Id}");
                       
            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Theory]
        [InlineData(1, 1000)]
        [InlineData(100, 2)]
        public async Task GetDish_ForGivenRestaurant_ReturnsNotFoundResult(int restaurantId, int dishId)
        {
            // act
            var response = await _client.GetAsync($"/api/restaurant/{restaurantId}/dish/{dishId}");

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }


    }
}
