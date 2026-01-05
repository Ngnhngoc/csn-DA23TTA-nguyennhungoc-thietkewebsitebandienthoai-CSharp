using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webbandt.Models
{
    [Table("Sanphamtronggiohangs")]
    public class Sanphamtronggiohang
    {
        [Key]
        public int ProductId { get; set; }
        public string? TenSanPham { get; set; }
        public string? HinhAnh { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public string KichThuoc { get; set; } = "";
        public decimal ThanhTien => DonGia * SoLuong;
    }
}
