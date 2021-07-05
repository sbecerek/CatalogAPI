using Catalog.Api.DTOs;
using Catalog.Api.Entities;

namespace Catalog.Api
{
    public static class Extensions{
        public static ItemDTO AsDto(this Item item)
        {
            return new ItemDTO(item.Id,item.Name,item.Description,item.Price,item.CreatedDate);
        }
    }
}