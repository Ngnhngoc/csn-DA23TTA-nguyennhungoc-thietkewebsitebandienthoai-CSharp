using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webbandt.Models
{
    [Table("HangSanXuats")] // Tên bảng trong Database
    public class HangSanXuat
    {
        [Key]
        public int MaHang { get; set; }

        [Required]
        [StringLength(100)]
        public string? TenHang { get; set; } // Ví dụ: Apple, Samsung

        public string? Logo { get; set; } // Link ảnh logo (nếu có)

        // Liên kết với bảng Product (Một hãng có nhiều sản phẩm)
        public virtual ICollection<Product>? Products { get; set; }
    }
}