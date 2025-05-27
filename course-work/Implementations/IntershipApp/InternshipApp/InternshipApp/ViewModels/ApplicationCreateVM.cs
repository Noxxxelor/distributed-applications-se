using System.ComponentModel.DataAnnotations;

namespace InternshipApp.ViewModels
{
    public class ApplicationCreateVM
    {
        public int InternshipId { get; set; }

        [Required]
        [Display(Name = "Мотивационное письмо")]
        public string MotivationLetter { get; set; }

        [Display(Name = "Есть портфолио")]
        public bool HasPortfolio { get; set; }
    }
}
