using Postgrest.Models;
using Postgrest.Attributes;

namespace VolunteerTracker.Models
{
    [Table("user_preferences")]
    public class UserPreferences : BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; }
        
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;
        
        [Column("display_name")]
        public string DisplayName { get; set; } = string.Empty;
        
        [Column("theme")]
        public string Theme { get; set; } = "Light";
        
        [Column("email_notifications")]
        public bool EmailNotifications { get; set; } = true;
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
} 