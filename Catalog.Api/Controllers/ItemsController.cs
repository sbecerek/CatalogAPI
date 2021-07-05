using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Api.DTOs;
using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {

        //we would rather introduce an Interface not concrete class - let's do that
        private readonly IItemsRepository repository;

        private readonly ILogger<ItemsController> logger;
        public ItemsController(IItemsRepository _repository,ILogger<ItemsController> logger)
        {
            repository = _repository;
            this.logger = logger;
        }


        [HttpGet]
        public async Task<IEnumerable<ItemDTO>> GetItemsAsync(string nameToMatch = null)
        {
            var items = (await repository.GetItemsAsync())
                        .Select(item => item.AsDto());
            if(!string.IsNullOrWhiteSpace(nameToMatch))
            {
                items = items.Where(item => item.Name.Contains(nameToMatch,StringComparison.OrdinalIgnoreCase));
            }
            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {items.Count()}");
            return items;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDTO>> GetItemAsync(Guid id)
        {
            var item = await repository.GetItemAsync(id);
            if(item is null)
                return NotFound();
            return item.AsDto();
        }

        //it is convention to Create and return created Item
        [HttpPost]
        public async Task<ActionResult<ItemDTO>> CreateItemAsync(CreateItemDTO itemDTO)
        {
            //if client doesn't specify name of the itemDTO then we get an item with name null hence we need validation data annotations

            Item item = new(){
                Id = Guid.NewGuid(),
                Name = itemDTO.Name,
                Description = itemDTO.Description,
                Price = itemDTO.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
            await repository.CreateItemAsync(item);

            //ARGS: first where to find the item second is anonymous object with ID only saying specify it to he method third is the object to return
            //.Net 3.0 at runtime removes Async Suffix from the names of methods - However we can remove such behavior
            return CreatedAtAction(nameof(GetItemAsync),new {id = item.Id},item.AsDto());
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItemAsync(Guid id,UpdateItemDTO itemDTO)
        {
            var existingItem = await repository.GetItemAsync(id);

            if(existingItem is null)
                return NotFound();
            
            existingItem.Name = itemDTO.Name;
            existingItem.Price = itemDTO.Price;

            await repository.UpdateItemAsync(existingItem);

            //It is convention to report NoContent
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItemAsync(Guid id)
        {
            var existingItem = repository.GetItemAsync(id);

            if(existingItem is null)
                return NotFound();

            await repository.DeleteItemAsync(id);

            return NoContent();
        }
        
    }
}