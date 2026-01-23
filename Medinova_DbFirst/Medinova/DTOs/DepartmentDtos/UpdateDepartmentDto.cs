using System.ComponentModel.DataAnnotations;

namespace Medinova.DTOs.DepartmentDtos
{
    public class UpdateDepartmentDto
    {
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Bölüm adı boş geçilemez.")]
        public string Name { get; set; }

    }
}
