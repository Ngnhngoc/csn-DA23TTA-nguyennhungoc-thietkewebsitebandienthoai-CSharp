-- 1. Tạo bảng
CREATE TABLE khach_hang (
  id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
  ho_ten NVARCHAR(255),
  trang_thai BIT
);

-- 2. Cho phép chèn ID cũ
SET IDENTITY_INSERT khach_hang ON;

-- 3. Chèn dữ liệu (đã bỏ dấu ` và thêm N trước tên)
INSERT INTO khach_hang (id, ho_ten, trang_thai) VALUES (1, N'Nguyễn Văn A', 1);
INSERT INTO khach_hang (id, ho_ten, trang_thai) VALUES (2, N'Trần Thị B', 0);

-- 4. Khóa lại
SET IDENTITY_INSERT khach_hang OFF;