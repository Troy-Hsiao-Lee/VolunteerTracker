using System.ComponentModel.DataAnnotations;
using Postgrest.Models;
using Postgrest.Attributes;

namespace VolunteerTracker.Models
{
    [Table("volunteer_service_entries")]
    public class VolunteerServiceEntry : BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; }
        
        [Column("user_id")]
        [Required]
        public string UserId { get; set; } = string.Empty; // FK to Supabase Auth user
        
        [Column("date_of_service")]
        [Required]
        public DateTime DateOfService { get; set; }
        
        [Column("service_type")]
        [Required]
        [StringLength(100)]
        public string ServiceType { get; set; } = string.Empty;
        
        [Column("description")]
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Column("hours")]
        [Required]
        [Range(0.1, 24.0)]
        public double Hours { get; set; }
        
        [Column("supervisor_name")]
        [Required]
        [StringLength(100)]
        public string SupervisorName { get; set; } = string.Empty;
        
        [Column("supervisor_signature_image_url")]
        [Required]
        public string SupervisorSignatureImageUrl { get; set; } = string.Empty; // Image stored in Supabase Storage
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
} 