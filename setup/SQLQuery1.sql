CREATE TABLE DonHang (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100),
    SoDienThoai NVARCHAR(20),
    Email NVARCHAR(100),
    DiaChi NVARCHAR(255),
    GhiChu NVARCHAR(MAX),
    TongTien DECIMAL(18, 0),
    NgayDat DATETIME DEFAULT GETDATE()
);

CREATE TABLE ChiTietDonHang (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DonHangId INT,
    TenSanPham NVARCHAR(200),
    KichThuoc NVARCHAR(50),
    SoLuong INT,
    DonGia DECIMAL(18, 0),
    FOREIGN KEY (DonHangId) REFERENCES DonHang(Id)
);