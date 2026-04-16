using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace singleProject.ViewModels
{
    public class EditProductViewModel
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อสินค้า")]
        [Display(Name = "ชื่อสินค้า")]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [Display(Name = "รายละเอียดสินค้า")]
        [StringLength(2000)]
        public string? ProductDescription { get; set; }

        [Required(ErrorMessage = "กรุณากรอกราคา")]
        [Display(Name = "ราคา (บาท)")]
        [Range(0.01, 999999.99, ErrorMessage = "ราคาต้องมากกว่า 0")]
        public decimal Price { get; set; }

        [Display(Name = "ภาพหน้าปก (อัพโหลดใหม่เพื่อเปลี่ยน)")]
        public IFormFile? CoverImage { get; set; }

        public string? ExistingCoverPicture { get; set; }

        [Display(Name = "หมวดหมู่")]
        public int CategoryId { get; set; }

        [Display(Name = "สถานะ")]
        public int Status { get; set; } = 1;
    }
}
