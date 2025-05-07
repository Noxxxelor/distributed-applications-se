using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities
{
    public class Application
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; } // Навигационное свойство

        public int InternshipId { get; set; }
        public Internship Internship { get; set; } // Навигационное свойство

        public DateTime ApplicationDate { get; set; } // Дата подачи заявки
        public ApplicationStatus Status { get; set; } // Статус заявки
    }

    public enum ApplicationStatus
    {
        Pending,    // Ожидает рассмотрения
        Accepted,   // Принята
        Rejected    // Отклонена
    }
}

