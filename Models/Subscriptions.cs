using System.ComponentModel.DataAnnotations;

namespace JobIntelPro_API.Models
{
    public class Subscriptions
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Verified { get; set; }
    }
}
