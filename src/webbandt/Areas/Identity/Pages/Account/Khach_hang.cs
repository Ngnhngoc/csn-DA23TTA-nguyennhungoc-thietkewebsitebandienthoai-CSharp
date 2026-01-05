namespace webbandt.Models
{
    public class Khach_hang
    {
        public int Id { get; set; }

        // Thêm = string.Empty; vào cuối để hết lỗi
        public string ho_ten { get; set; } = string.Empty;
        public string so_dien_thoai { get; set; } = string.Empty;
        public string dia_chi { get; set; } = string.Empty;
        public bool trang_thai { get; set; } = false;
    }
}