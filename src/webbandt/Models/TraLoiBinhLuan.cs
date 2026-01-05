using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webbandt.Models
{
    public class TraLoiBinhLuan
    {
        [Key]
        public int Id { get; set; }

        public int DanhGiaId { get; set; }

        [ForeignKey("DanhGiaId")]
      
        public virtual DanhGia? DanhGia { get; set; }

        public string? UserName { get; set; }
        public string? NoiDung { get; set; }
        public string? QuanTriVien { get; set; } // Thêm dấu ? cho dòng này luôn để tránh lỗi vàng tương tự
        public DateTime NgayTraLoi { get; set; } = DateTime.Now;
    }
}