using System;

namespace Catalog.Api.Entities
{
    //How about Record types?
    //pretty much classes but better:
    //* To use for immutable objects
    //* with-expression support
    //* Value based equality support 
    public class Item
    {
        //init allows only value initialization no change
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public DateTimeOffset CreatedDate {get; set;}

        public string Description {get;set;}
    }
}