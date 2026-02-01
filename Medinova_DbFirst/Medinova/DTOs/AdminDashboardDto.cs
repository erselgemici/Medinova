using System.Collections.Generic;

namespace Medinova.DTOs
{
    public class AdminDashboardDto
    {
        public int TotalDoctors { get; set; }
        public int TotalDepartments { get; set; }

        public int TotalActiveAppointments { get; set; } 
        public int TotalAppointments { get; set; }      

        public int TotalRegisteredPatients { get; set; }

        public List<string> DoctorNames { get; set; }
        public List<int> AppointmentCounts { get; set; }
        public List<string> DepartmentNames { get; set; }
        public List<int> DepartmentPercentages { get; set; }
        public List<string> Last6Months { get; set; }
        public List<int> MonthlyAppointmentCounts { get; set; }
        public List<Medinova.Models.Appointment> LastAppointments { get; set; }
    }
}
