namespace BlogApi.DTOs
{
    public class PostCreateUpdateDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
