namespace ForumNG.Models.ForumModel
{
    public class ForumPost
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
