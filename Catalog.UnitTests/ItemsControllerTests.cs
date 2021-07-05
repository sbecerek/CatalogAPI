using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Api.Controllers;
using Catalog.Api.DTOs;
using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Catalog.UnitTests
{
    public class ItemsControllerTests
    {
        private readonly Mock<IItemsRepository> repositoryStub = new();

        private readonly Mock<ILogger<ItemsController>> loggerStub = new();

        private readonly Random Rand = new();


        private Item GenerateRandomItem()
        {
            return new(){
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Price = Rand.Next(1000),
                CreatedDate = DateTimeOffset.UtcNow
            };
        }


        //UnitofWork_StateUnderTest_ExpectedBheaviour
        [Fact]
        public async Task GetItemAsync_WithUnexistingItem_ReturnsNotFound()
        {
            //Arrange
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>())).ReturnsAsync((Item)null);

            var controller = new ItemsController(repositoryStub.Object,loggerStub.Object);
            //Act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            //Assert
            //Assert.IsType<NotFoundResult>(result.Result);
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetItemAsync_WithExistingItem_ReturnsexistingItem()
        {
            //Arrange
            var existingItem = GenerateRandomItem();
            repositoryStub.Setup(repo => repo.GetItemAsync(existingItem.Id)).ReturnsAsync(existingItem);

            var controller = new ItemsController(repositoryStub.Object,loggerStub.Object);
            //Act
            var result = await controller.GetItemAsync(existingItem.Id);

            //Assert
            Assert.IsType<ItemDTO>(result.Value);
            var dto = (result as ActionResult<ItemDTO>).Value;
            // Assert.Equal(existingItem.Id,dto.Id);
            // Assert.Equal(existingItem.Name,dto.Name);
            // Assert.Equal(existingItem.Price,dto.Price);
            // Assert.Equal(existingItem.CreatedDate,dto.CreatedDate);
            result.Value.Should().BeEquivalentTo(
                existingItem,
                options => options.ComparingByMembers<Item>());
            
        }

        [Fact]
        public async Task GetItemsAsync_WithExistingItems_ReturnsAllItems()
        {
            //Arrange
            var existingItems = new[]{
                GenerateRandomItem(),
                GenerateRandomItem(),
                GenerateRandomItem(),
                GenerateRandomItem()
            };
            repositoryStub.Setup(repo => repo.GetItemsAsync()).ReturnsAsync(existingItems);

            var controller = new ItemsController(repositoryStub.Object,loggerStub.Object);
            //Act
            var actualItems = await controller.GetItemsAsync();

            //Assert
            // actualItems.Should().BeEquivalentTo(
            //     existingItems,
            //     options => options.ComparingByMembers<Item>()
            // );
            actualItems.Should().BeEquivalentTo(
                existingItems
            );
            //Item is no more a record type
        }

        [Fact]
        public async Task CreateItemAsync_WithItemToCreate_ReturnsCreatedItem()
        {
            //Arrange
            var itemToCreate = new CreateItemDTO(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Rand.Next(1000));

            var controller = new ItemsController(repositoryStub.Object,loggerStub.Object);
            //Act
            var result = await controller.CreateItemAsync(itemToCreate);
            //Assert
            var createdItem  = (result.Result as CreatedAtActionResult).Value as ItemDTO;
            itemToCreate.Should().BeEquivalentTo(
                createdItem,
                options => options.ComparingByMembers<ItemDTO>().ExcludingMissingMembers()
            );

            createdItem.Id.Should().NotBeEmpty();
            createdItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
        }

        [Fact]
        public async Task UpdateItemAsync__WithExistingItem_ReturnNoContent()
        {
            //Arrange
            var existingItem = GenerateRandomItem();
            repositoryStub.Setup(repo => repo.GetItemAsync(existingItem.Id)).ReturnsAsync(existingItem);

            var itemID = existingItem.Id;
            var itemToUpdate = new UpdateItemDTO(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                existingItem.Price + 3);

            var controller = new ItemsController(repositoryStub.Object,loggerStub.Object);
            //Act

            var result = await controller.UpdateItemAsync(itemID,itemToUpdate);

            //Assert
            result.Should().BeOfType<NoContentResult>();


        }

        [Fact]

        public async Task DeleteItemAsync_WithExistingItem_ReturnNoContent()
        {
            //Arrange
            var existingItem = GenerateRandomItem();
            repositoryStub.Setup(repo => repo.GetItemAsync(existingItem.Id)).ReturnsAsync(existingItem);

            var itemID = existingItem.Id;
            
            var controller = new ItemsController(repositoryStub.Object,loggerStub.Object);
            //Act
            var result = await controller.DeleteItemAsync(itemID);

            //Assert
            result.Should().BeOfType<NoContentResult>();

        }

        //TDD example - Querying items Searching
        [Fact]
        public async Task GetItemsAsync_WithMatchingItems_ReturnsMatchingItems()
        {
            var AllItems = new[]{
                new Item(){
                    Name = "Potion"
                },
                new Item(){
                    Name = "Poison"
                },
                new Item(){
                    Name = "Health Potion"
                }
            };
    
            var nameToMatch = "Potion";

            repositoryStub.Setup(repo=>repo.GetItemsAsync()).ReturnsAsync(AllItems);

            var controller = new ItemsController(repositoryStub.Object,loggerStub.Object);

            //Act
            IEnumerable<ItemDTO> foundItems = await controller.GetItemsAsync(nameToMatch);

            //Assert
            foundItems.Should().OnlyContain(
                item => item.Name.Contains("Potion")
            );

        }



    }
}
