using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InternshipApp.ViewModels
{
    public class RegisterVM : IValidatableObject
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтвердите пароль")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Роль")]
        public string Role { get; set; } = "Student";

        // Студент
        [Display(Name = "ФИО студента")]
        public string? StudentFullName { get; set; }

        [Display(Name = "Университет")]
        public string? University { get; set; }

        [Display(Name = "Степень")]
        public string? Degree { get; set; }

        // Компания
        [Display(Name = "Название компании")]
        public string? CompanyName { get; set; }

        [Display(Name = "Адрес компании")]
        public string? CompanyAddress { get; set; }

        [Display(Name = "Телефон компании")]
        public string? CompanyPhone { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Role == "Student")
            {
                if (string.IsNullOrWhiteSpace(StudentFullName))
                    yield return new ValidationResult("ФИО студента обязательно.", new[] { nameof(StudentFullName) });

                if (string.IsNullOrWhiteSpace(University))
                    yield return new ValidationResult("Университет обязателен.", new[] { nameof(University) });

                if (string.IsNullOrWhiteSpace(Degree))
                    yield return new ValidationResult("Степень обязательна.", new[] { nameof(Degree) });
            }
            else if (Role == "Company")
            {
                if (string.IsNullOrWhiteSpace(CompanyName))
                    yield return new ValidationResult("Название компании обязательно.", new[] { nameof(CompanyName) });

                if (string.IsNullOrWhiteSpace(CompanyAddress))
                    yield return new ValidationResult("Адрес компании обязателен.", new[] { nameof(CompanyAddress) });

                if (string.IsNullOrWhiteSpace(CompanyPhone))
                    yield return new ValidationResult("Телефон компании обязателен.", new[] { nameof(CompanyPhone) });
            }
        }
    }
}
