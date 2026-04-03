namespace ViNgocHiep_2123110365.DTOs
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int BookId { get; set; }

        public UserDTO? User { get; set; }
    }
}
