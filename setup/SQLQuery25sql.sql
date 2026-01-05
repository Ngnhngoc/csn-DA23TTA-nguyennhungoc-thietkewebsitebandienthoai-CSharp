SELECT 
    d.Id AS MaDonHang,
    d.HoTen,
    d.NgayDat,
    d.TongTien,
    c.TenSanPham,
    c.SoLuong,
    c.DonGia,
    c.KichThuoc
FROM DonHang d
JOIN ChiTietDonHang c ON d.Id = c.DonHangId