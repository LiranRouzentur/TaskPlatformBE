using System.ComponentModel.DataAnnotations;

namespace TaskPlatformBE.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
} 