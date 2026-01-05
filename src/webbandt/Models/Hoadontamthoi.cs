using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace webbandt.Models
{
    [Table("Hoadontamthois")]
    public class Hoadontamthoi

    {
        
        [Key]
        public string HoTen { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;

        // --- CÁI DÒNG QUAN TRỌNG ĐANG THIẾU ---
        public string GhiChu { get; set; } = string.Empty;

        // --- SỬA LẠI DANH SÁCH GIỎ HÀNG CHO CHUẨN ---
        public List<Sanphamtronggiohang> GioHang { get; set; } = new List<Sanphamtronggiohang>();

        // --- SỬA TỔNG TIỀN THÀNH BIẾN (Viết Hoa chữ T) ---
        public decimal TongTien { get; set; }
    }
}