namespace Medinova.DTOs.DoctorDtos
{
    public class DailyScheduleDto
    {
        public string Time { get; set; }     
        public bool IsFull { get; set; }   
        public string PatientName { get; set; } 
        public int AppointmentId { get; set; }
    }
}
