using System.ComponentModel.DataAnnotations;

namespace GetMeThere.API.Models
{
    public class Roles
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }

    }
}
