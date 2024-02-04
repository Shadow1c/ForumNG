namespace ForumNG.Models.DTO
{
	public class ForumTitle
	{
		public int Id { get; set; }
		public string? Title { get; set; }
		public string? Description { get; set; }
		public virtual ICollection<ForumPost>? ForumPosts { get; set; }

		public ForumPost LastPost
		{
			get
			{
				ForumPost lastPost = null;

				if (ForumPosts != null)
				{
					lastPost = ForumPosts.OrderByDescending(p => p.CreatedDate).FirstOrDefault();
				}

				if (lastPost == null)
				{
					lastPost = new ForumPost();
				}

				return lastPost;
			}
		}
	}
}