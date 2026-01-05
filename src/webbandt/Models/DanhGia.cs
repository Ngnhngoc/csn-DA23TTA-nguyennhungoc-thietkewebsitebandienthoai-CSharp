using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webbandt.Models
{
    [Table("DanhGias")]
    public class DanhGia
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string? UserName { get; set; }

        public int SoSao { get; set; }


        public string? NoiDung { get; set; }

        
        public DateTime NgayDanhGia { get; set; }

        public int LuotThich { get; set; } = 0;


        // --- LOGIC TRẢ LỜI BÌNH LUẬN (REPLY) ---

        // ParentId phải là nullable (int?) vì bình luận gốc không có ParentId
        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        // Parent phải là nullable (DanhGia?)
        public virtual DanhGia? Parent { get; set; }

        // Replies được khởi tạo để tránh lỗi CS9036
        public virtual ICollection<DanhGia> Replies { get; set; } = new List<DanhGia>();
    }
}