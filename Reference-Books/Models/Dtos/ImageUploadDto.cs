using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Reference_Books.Models.Dtos
{
    public class ImageUploadDto
    {
        [Required]
        public IFormFile Image { get; set; }

        //[Required]
        [FromRoute]
        public int Id { get; set; }
    }
}
