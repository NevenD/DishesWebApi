namespace DishesWebApi.DTOs
{
    public record DishDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
    }
}
