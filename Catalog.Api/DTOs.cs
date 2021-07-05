using System;
using System.ComponentModel.DataAnnotations;

namespace Catalog.Api.DTOs
{
    public record ItemDTO (Guid Id, string Name, string Description, decimal Price, DateTimeOffset CreatedDate);

    //No required for price since it is a value type can never be null
    public record CreateItemDTO([Required]string Name, string Description, [Range(1,1000)]decimal Price);

    public record UpdateItemDTO([Required]string Name, string Description, [Range(1,1000)]decimal Price);
}