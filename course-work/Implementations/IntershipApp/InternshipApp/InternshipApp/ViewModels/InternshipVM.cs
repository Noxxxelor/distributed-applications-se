using System;
using System.ComponentModel.DataAnnotations;

namespace InternshipApp.ViewModels
{
    public class InternshipVM
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата начала")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата окончания")]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(0, 1000000)]
        [Display(Name = "Зарплата")]
        public decimal Salary { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Локация")]
        public string Location { get; set; }

        
        public int CompanyId { get; set; }
    }
}
