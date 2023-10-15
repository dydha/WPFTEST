namespace WPFTEST.DTOs
{
    public class VariantDTO
    {
        public int Id { get; set; } 
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? EanNumber { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is VariantDTO other)
            {
                return Id == other.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
