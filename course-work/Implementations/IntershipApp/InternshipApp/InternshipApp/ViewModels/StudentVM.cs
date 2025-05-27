using System;
using System.ComponentModel.DataAnnotations;

namespace InternshipApp.ViewModels
{
    public class StudentVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Полето 'Име и фамилия' е задължително.")]
        [StringLength(100, ErrorMessage = "Максималната дължина е 100 символа.")]
        [Display(Name = "Име и фамилия")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Полето 'Имейл' е задължително.")]
        [EmailAddress(ErrorMessage = "Невалиден формат на имейл.")]
        [StringLength(100, ErrorMessage = "Максималната дължина е 100 символа.")]
        [Display(Name = "Имейл")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Полето 'Университет' е задължително.")]
        [StringLength(100, ErrorMessage = "Максималната дължина е 100 символа.")]
        [Display(Name = "Университет")]
        public string University { get; set; }

        [Required(ErrorMessage = "Полето 'Степен' е задължително.")]
        [StringLength(50, ErrorMessage = "Максималната дължина е 50 символа.")]
        [Display(Name = "Степен")]
        public string Degree { get; set; }

        [Required(ErrorMessage = "Полето 'Дата на раждане' е задължително.")]
        [DataType(DataType.Date)]
        [Display(Name = "Дата на раждане")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Трябва да се посочи дали студентът е активен.")]
        [Display(Name = "Активен")]
        public bool IsActive { get; set; }
    }
}
