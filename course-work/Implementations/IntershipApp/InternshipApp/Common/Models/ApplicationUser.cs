using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models
{
    public class ApplicationUser : IdentityUser
    {
        [NotMapped]
        public string Role { get; set; }  
    }
}
