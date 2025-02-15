using System.ComponentModel.DataAnnotations;

namespace DishesWebApi.Entities
{
    public class DishPutDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public required string Name { get; set; }
    }
}
