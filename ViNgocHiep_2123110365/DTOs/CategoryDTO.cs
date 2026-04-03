namespace ViNgocHiep_2123110365.DTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Image { get; set; }
        public string? Description { get; set; }
    }
}
