using System.ComponentModel.DataAnnotations;

namespace InternshipApp.ViewModels
{
    public class StudentVM
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string University { get; set; }

        [Required]
        public string Degree { get; set; }
    }
}
