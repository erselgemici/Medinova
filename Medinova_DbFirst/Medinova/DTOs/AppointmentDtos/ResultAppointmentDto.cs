using System;

namespace Medinova.DTOs.AppointmentDtos
{
    public class ResultAppointmentDto
    {
        public int AppointmentID { get; set; }
        public string PatientName { get; set; }    
        public string PatientPhone { get; set; }    
        public string DoctorName { get; set; }     
        public string DepartmentName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentTime { get; set; }
        public bool IsActive { get; set; }
    }
}
