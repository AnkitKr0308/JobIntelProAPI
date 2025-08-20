using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobIntelPro_API.Models
{
    public class Jobs
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Company { get; set; }
       
        public string WorkType { get; set; } 
        public string Experience { get; set; } 
        public string Batch { get; set; }
        public string Degree { get; set; } 
        public string ApplyURL { get; set; } 
       
        public bool IsActive { get; set; } 
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}
