namespace SimpleDotNetForum.Models
{
    public class Reply
    {
        public int PostId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
