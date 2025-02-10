using DishesAPI.Entities;
using DishesWebApi.DTOs;

namespace DishesWebApi.Mappings
{
    public static class GeneralMapping
    {
        public static DishDto ToDishDto(this Dish dish)
        {
            return new DishDto
            {
                Id = dish.Id,
                Name = dish.Name,
            };
        }

        public static IngredientDto ToIngredientDto(this Ingredient ingredient)
        {
            return new IngredientDto
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                DishId = ingredient.Dishes.First().Id,
            };
        }
    }
}
