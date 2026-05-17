CREATE DATABASE SHOPGUITARSINHVIEN
USE SHOPGUITARSINHVIEN

--Tài khoản người dùng
CREATE TABLE USERS (
	user_id INT IDENTITY(1,1) PRIMARY KEY,
	user_name NVARCHAR(50) NOT NULL,
	user_password NVARCHAR(MAX) NOT NULL,
	user_email NVARCHAR(255) UNIQUE NULL,
	user_phone NVARCHAR(10) UNIQUE NULL,
	last_login DATETIME NULL,
	user_status NVARCHAR(50) NOT NULL,
	created_at DATETIME DEFAULT GETDATE(),
	user_role NVARCHAR(30) NOT NULL DEFAULT N'Khách hàng',
	--chỉ có email hoặc phone null
	CHECK (user_email is not null or user_phone is not null)
)

CREATE UNIQUE INDEX UX_USERS_Email 
    ON USERS(user_email) 
    WHERE user_email IS NOT NULL

CREATE UNIQUE INDEX UX_USERS_Phone 
    ON USERS(user_phone) 
    WHERE user_phone IS NOT NULL

--Nhân viên
CREATE TABLE NHANVIEN (
	MaNV NVARCHAR(20) PRIMARY KEY,
	user_id INT NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
	HoNV NVARCHAR(10) NOT NULL,
	TenNV NVARCHAR(30) NOT NULL,
	AnhNV NVARCHAR(MAX) NOT NULL,
	EmailNV NVARCHAR(MAX) NOT NULL,
	SDTNV NVARCHAR(10) NOT NULL,
	DiaChiNV NVARCHAR(MAX) NOT NULL,
	GioiTinhNV BIT NOT NULL,
	NgaySinhNV DATE NOT NULL,
	TrangThaiLamViec NVARCHAR(20) NOT NULL DEFAULT N'Đang làm',
	NgayVaoLam DATE NOT NULL
)

--Khách hàng
CREATE TABLE KHACHHANG (
	MaKH NVARCHAR(20) PRIMARY KEY,
	user_id INT NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
	HoKH NVARCHAR(10) NOT NULL,
	TenKH NVARCHAR(30) NOT NULL,
	AnhKH NVARCHAR(MAX) NULL,
	DiaChiKH NVARCHAR(MAX) NULL,
	GioiTinhKH BIT NULL,
	NgaySinhKH DATE NULL,
	LoaiKH NVARCHAR(30) NOT NULL DEFAULT N'Thường'
)

CREATE TABLE DANHMUCSANPHAM (
	MaLoaiSP NVARCHAR(20) PRIMARY KEY,
	TenLoaiSP NVARCHAR(30) NOT NULL
)

CREATE TABLE SANPHAM (
	MaSP NVARCHAR(20) PRIMARY KEY,
	TenSP NVARCHAR(40) NOT NULL,
	AnhSP NVARCHAR(MAX) NOT NULL,
	MoTaSP NVARCHAR(MAX) NULL,
	GiaNhap DECIMAL(18,0) NOT NULL,
	GiaBan DECIMAL(18,0) NOT NULL,
	TrangThaiSP BIT NOT NULL, --1: đang bán, 0: ngừng bán
	ThuongHieu NVARCHAR(30) NULL,
	NgayThemSP DATETIME DEFAULT GETDATE(),
	MaLoaiSP NVARCHAR(20) NULL REFERENCES DANHMUCSANPHAM(MaLoaiSP),
	SoLuongTon INT NOT NULL DEFAULT 0
)

-- Thêm dữ liệu vào USERS
INSERT INTO USERS (user_name, user_password, user_email, user_phone, user_status, user_role) VALUES
(N'admin01',      N'$2a$11$tprCEO.shXE5GhO/Gw6QXeDwuK/oMQMitlBJSftZ5JJvBtnLszZii', N'admin@shop.com',     '0901111111', N'Đang hoạt động', N'Admin'),
(N'quanly01',     N'$2a$11$tprCEO.shXE5GhO/Gw6QXeDwuK/oMQMitlBJSftZ5JJvBtnLszZii', N'quanly@shop.com',    '0902222222', N'Đang hoạt động', N'Quản lý'),
(N'quanlykho01',  N'$2a$11$tprCEO.shXE5GhO/Gw6QXeDwuK/oMQMitlBJSftZ5JJvBtnLszZii', N'quanlykho@shop.com', '0903333333', N'Đang hoạt động', N'Quản lý kho'),
(N'nvkho01',      N'$2a$11$tprCEO.shXE5GhO/Gw6QXeDwuK/oMQMitlBJSftZ5JJvBtnLszZii', N'nvkho@shop.com',     '0904444444', N'Đang hoạt động', N'Nhân viên kho'),
(N'nvbanhang01',  N'$2a$11$tprCEO.shXE5GhO/Gw6QXeDwuK/oMQMitlBJSftZ5JJvBtnLszZii', N'nvbh@shop.com',      '0905555555', N'Đang hoạt động', N'Nhân viên bán hàng'),
(N'khachhang01',  N'$2a$11$tprCEO.shXE5GhO/Gw6QXeDwuK/oMQMitlBJSftZ5JJvBtnLszZii', N'kh01@gmail.com',     '0906666666', N'Đang hoạt động', N'Khách hàng')
GO

-- Thêm dữ liệu vào NHANVIEN
INSERT INTO NHANVIEN (MaNV, user_id, HoNV, TenNV, AnhNV, EmailNV, SDTNV, DiaChiNV, GioiTinhNV, NgaySinhNV, TrangThaiLamViec, NgayVaoLam) VALUES
('NV001', 1, N'Nguyễn', N'Admin',        'anh_nv001.jpg', 'admin@shop.com',     '0901111111', N'Hà Nội', 1, '1990-01-01', N'Đang làm', '2020-01-01'),
('NV002', 2, N'Trần',   N'Quản Lý',      'anh_nv002.jpg', 'quanly@shop.com',    '0902222222', N'Hà Nội', 1, '1992-05-10', N'Đang làm', '2021-03-01'),
('NV003', 3, N'Lê',     N'Quản Lý Kho',  'anh_nv003.jpg', 'quanlykho@shop.com', '0903333333', N'Hà Nội', 0, '1993-07-15', N'Đang làm', '2021-06-01'),
('NV004', 4, N'Phạm',   N'NV Kho',       'anh_nv004.jpg', 'nvkho@shop.com',     '0904444444', N'Hà Nội', 1, '1995-03-20', N'Đang làm', '2022-01-15'),
('NV005', 5, N'Hoàng',  N'NV Bán Hàng',  'anh_nv005.jpg', 'nvbh@shop.com',      '0905555555', N'Hà Nội', 0, '1997-09-25', N'Đang làm', '2022-08-01')
GO

-- Thêm dữ liệu vào KHACHHANG
INSERT INTO KHACHHANG (MaKH, user_id, HoKH, TenKH, AnhKH, DiaChiKH, GioiTinhKH, NgaySinhKH, LoaiKH) VALUES
('KH001', 6, N'Võ', N'Khách Hàng A', NULL, N'TP.HCM', 1, '2000-04-12', N'Thường')
GO

-- Thêm danh mục sản phẩm
INSERT INTO DANHMUCSANPHAM (MaLoaiSP, TenLoaiSP) VALUES
('AC', N'Guitar Acoustic'),
('CL', N'Guitar Classic'),
('EL', N'Guitar Electric'),
('PK', N'Phụ kiện')

-- Thêm sản phẩm
INSERT INTO SANPHAM (MaSP, TenSP, AnhSP, MoTaSP, GiaNhap, GiaBan, TrangThaiSP, ThuongHieu, MaLoaiSP, SoLuongTon)
VALUES
-- Guitar Acoustic
('SP001', N'Guitar Acoustic Yamaha F310', 'yamaha_f310.jpg', N'Guitar acoustic dành cho người mới học, âm thanh ấm, dễ chơi', 1500000, 2200000, 1, N'Yamaha', 'AC', 10),
('SP002', N'Guitar Acoustic Fender CD-60S', 'fender_cd60s.jpg', N'Đàn acoustic chất lượng cao, phù hợp sinh viên', 2000000, 3000000, 1, N'Fender', 'AC', 8),
('SP003', N'Guitar Acoustic Taylor GS Mini', 'taylor_gsmini.jpg', N'Đàn cỡ nhỏ tiện mang theo, âm thanh tuyệt vời', 3500000, 5000000, 1, N'Taylor', 'AC', 5),
-- Guitar Classic
('SP004', N'Guitar Classic Yamaha C40', 'yamaha_c40.jpg', N'Guitar classic phổ biến nhất cho học sinh sinh viên', 800000, 1200000, 1, N'Yamaha', 'CL', 15),
('SP005', N'Guitar Classic Admira Juanita', 'admira_juanita.jpg', N'Đàn classic Tây Ban Nha, âm thanh chuẩn', 2500000, 3800000, 1, N'Admira', 'CL', 6),
-- Guitar Electric
('SP006', N'Guitar Electric Fender Stratocaster', 'fender_strat.jpg', N'Đàn điện huyền thoại, phù hợp mọi thể loại nhạc', 5000000, 7500000, 1, N'Fender', 'EL', 4),
('SP007', N'Guitar Electric Gibson Les Paul', 'gibson_lespaul.jpg', N'Âm thanh dày, mạnh mẽ, chuẩn rock', 8000000, 12000000, 1, N'Gibson', 'EL', 3),
-- Phụ kiện
('SP008', N'Dây đàn Acoustic D''Addario EJ16', 'daddario_ej16.jpg', N'Dây đàn acoustic phosphor bronze, độ bền cao', 80000, 150000, 1, N'D''Addario', 'PK', 50),
('SP009', N'Capo guitar Kyser Quick-Change', 'kyser_capo.jpg', N'Capo kẹp nhanh, chắc chắn, không làm lạc dây', 120000, 220000, 1, N'Kyser', 'PK', 30),
('SP010', N'Bao đàn guitar acoustic loại dày', 'bao_dan_acoustic.jpg', N'Bao đàn chống sốc, chống nước, có ngăn phụ kiện', 150000, 280000, 1, NULL, 'PK', 20)
