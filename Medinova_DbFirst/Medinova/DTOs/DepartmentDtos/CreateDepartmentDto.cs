using System.ComponentModel.DataAnnotations;

namespace Medinova.DTOs.DepartmentDtos
{
    public class CreateDepartmentDto
    {
        [Required(ErrorMessage = "Bölüm adı boş geçilemez.")]
        [StringLength(50, ErrorMessage = "Bölüm adı en fazla 50 karakter olabilir.")]
        public string Name { get; set; }

    }
}
