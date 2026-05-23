CREATE DATABASE SHOPGUITARSINHVIEN
GO
USE SHOPGUITARSINHVIEN
GO

CREATE TABLE THAMSO (
MaTS VARCHAR(20) PRIMARY KEY,
TenTS NVARCHAR(100) NOT NULL,
Kieu_DVT NVARCHAR(50) NULL,
GiaTri NVARCHAR(255) NOT NULL,
TinhTrang NVARCHAR(50) NOT NULL
)
GO

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
NgayDoiMatKhauGanNhat DATETIME DEFAULT GETDATE(),
CHECK (user_email IS NOT NULL OR user_phone IS NOT NULL)
)
GO

CREATE UNIQUE INDEX UX_USERS_Email ON USERS(user_email) WHERE user_email IS NOT NULL
GO
CREATE UNIQUE INDEX UX_USERS_Phone ON USERS(user_phone) WHERE user_phone IS NOT NULL
GO

CREATE TABLE NHANVIEN (
MaNV NVARCHAR(20) PRIMARY KEY,
user_id INT NOT NULL REFERENCES USERS(user_id) ON DELETE CASCADE,
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
GO

CREATE TABLE KHACHHANG (
MaKH NVARCHAR(20) PRIMARY KEY,
user_id INT NOT NULL REFERENCES USERS(user_id) ON DELETE CASCADE,
HoKH NVARCHAR(10) NOT NULL,
TenKH NVARCHAR(30) NOT NULL,
AnhKH NVARCHAR(MAX) NULL,
DiaChiKH NVARCHAR(MAX) NULL,
EmailKH NVARCHAR(MAX) NULL,
SDTKH NVARCHAR(10) NULL,
GioiTinhKH BIT NULL,
NgaySinhKH DATE NULL,
LoaiKH NVARCHAR(30) NOT NULL DEFAULT N'Thường'
)
GO

CREATE TABLE NHACUNGCAP (
MaNCC NVARCHAR(20) PRIMARY KEY,
TenNCC NVARCHAR(100) NOT NULL,
DiaChi NVARCHAR(255) NULL,
SDT VARCHAR(15) NULL
)
GO

CREATE TABLE DANHMUCSANPHAM (
MaLoaiSP NVARCHAR(20) PRIMARY KEY,
TenLoaiSP NVARCHAR(30) NOT NULL,
MoTa NVARCHAR(MAX)
)
GO

CREATE TABLE SANPHAM (
MaSP NVARCHAR(20) PRIMARY KEY,
TenSP NVARCHAR(40) NOT NULL,
AnhSP NVARCHAR(MAX) NOT NULL,
MoTaSP NVARCHAR(MAX) NULL,
GiaNhap DECIMAL(18,0) NOT NULL,
GiaBan DECIMAL(18,0) NOT NULL,
TrangThaiSP BIT NOT NULL,
ThuongHieu NVARCHAR(30) NULL,
NgayThemSP DATETIME DEFAULT GETDATE(),
MaLoaiSP NVARCHAR(20) NULL REFERENCES DANHMUCSANPHAM(MaLoaiSP),
SoLuongSP INT NOT NULL DEFAULT 0,
CHECK (GiaBan >= GiaNhap)
)
GO

CREATE TABLE PHIEUNHAP (
SoPN NVARCHAR(20) PRIMARY KEY,
NgayNhap DATETIME NOT NULL DEFAULT GETDATE(),
NguoiGiao NVARCHAR(50) NOT NULL,
TongTienNhap DECIMAL(18, 0) NOT NULL DEFAULT 0 CHECK (TongTienNhap >= 0),
MaNCC NVARCHAR(20) REFERENCES NHACUNGCAP(MaNCC),
MaNV NVARCHAR(20) REFERENCES NHANVIEN(MaNV)
)
GO

CREATE TABLE CT_PHIEUNHAP (
SoPN NVARCHAR(20) REFERENCES PHIEUNHAP(SoPN) ON DELETE CASCADE,
MaSP NVARCHAR(20) REFERENCES SANPHAM(MaSP),
SLChungTu INT NOT NULL CHECK (SLChungTu > 0),
SLThucNhap INT NOT NULL CHECK (SLThucNhap > 0),
DonGiaNhap DECIMAL(18, 0) NOT NULL CHECK (DonGiaNhap >= 0),
ThanhTien DECIMAL(18, 0) NOT NULL,
PRIMARY KEY (SoPN, MaSP)
)
GO

CREATE TABLE BBKIEMKE (
SoBBKK NVARCHAR(20) NOT NULL,
NgayLap DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
TrangThai NVARCHAR(50) NOT NULL DEFAULT N'Chờ duyệt',
MaNV NVARCHAR(20) NOT NULL,
PRIMARY KEY (SoBBKK),
FOREIGN KEY (MaNV) REFERENCES NHANVIEN(MaNV)
)
GO

CREATE TABLE CTKIEMKE (
SoBBKK NVARCHAR(20) NOT NULL,
MaSP NVARCHAR(20) NOT NULL,
SLSoSach INT NOT NULL,
SLThucTe INT NOT NULL,
SLThua INT NOT NULL DEFAULT 0,
SLThieu INT NOT NULL DEFAULT 0,
PhamChat NVARCHAR(255),
PRIMARY KEY (SoBBKK, MaSP),
FOREIGN KEY (SoBBKK) REFERENCES BBKIEMKE(SoBBKK) ON DELETE CASCADE,
FOREIGN KEY (MaSP) REFERENCES SANPHAM(MaSP)
)
GO

CREATE TABLE BBKIEMNGHIEM (
SoBBKN NVARCHAR(20) NOT NULL,
NgayLap DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
MaNV NVARCHAR(20) NOT NULL,
PRIMARY KEY (SoBBKN),
FOREIGN KEY (MaNV) REFERENCES NHANVIEN(MaNV)
)
GO

CREATE TABLE CTKIEMNGHIEM (
SoBBKN NVARCHAR(20) NOT NULL,
MaSP NVARCHAR(20) NOT NULL,
SLChungTu INT NOT NULL,
SLDungQuyCach INT NOT NULL,
SLLoi INT NOT NULL,
GhiChu NVARCHAR(MAX),
PRIMARY KEY (SoBBKN, MaSP),
FOREIGN KEY (SoBBKN) REFERENCES BBKIEMNGHIEM(SoBBKN) ON DELETE CASCADE,
FOREIGN KEY (MaSP) REFERENCES SANPHAM(MaSP)
)
GO

CREATE TABLE GIOHANG (
MaGH NVARCHAR(20) PRIMARY KEY,
MaSP NVARCHAR(20) NOT NULL REFERENCES SANPHAM(MaSP),
SoLuong INT NOT NULL CHECK (SoLuong > 0),
NgayCapNhatGioHang DATETIME NOT NULL DEFAULT GETDATE(),
MaKH NVARCHAR(20) NULL REFERENCES KHACHHANG(MaKH)
)
GO

CREATE TABLE KHUYENMAI (
MaKM NVARCHAR(20) PRIMARY KEY,
TenKM NVARCHAR(100) NOT NULL,
MoTaKM NVARCHAR(MAX) NULL,
AnhBannerKM NVARCHAR(MAX) NULL,
ThuTuHienThi INT NULL,
MaVoucher NVARCHAR(50) UNIQUE NULL,
LoaiGiam NVARCHAR(20) NOT NULL CHECK (LoaiGiam IN (N'Phần trăm', N'Số tiền')),
GiaTriGiam DECIMAL(18,2) NOT NULL CHECK (GiaTriGiam > 0),
GiamToiDa DECIMAL(18,0) NULL,
DonToiThieu DECIMAL(18,0) NOT NULL DEFAULT 0,
SoLuotToiDa INT NULL,
SoLuotDaDung INT NOT NULL DEFAULT 0,
SoLanDungMoiKH INT NOT NULL DEFAULT 1,
NgayBatDau DATETIME NOT NULL,
NgayKetThuc DATETIME NOT NULL,
TrangThaiKM NVARCHAR(20) NOT NULL DEFAULT N'Hoạt động' CHECK (TrangThaiKM IN (N'Hoạt động', N'Hết hạn')),
NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
MaNV NVARCHAR(20) NULL REFERENCES NHANVIEN(MaNV),
CHECK (NgayKetThuc > NgayBatDau),
CHECK (GiaTriGiam <= 100 OR LoaiGiam <> N'Phần trăm')
)
GO

CREATE TABLE KHUYENMAI_SANPHAM (
MaKM NVARCHAR(20) NOT NULL REFERENCES KHUYENMAI(MaKM) ON DELETE CASCADE,
MaSP NVARCHAR(20) NOT NULL REFERENCES SANPHAM(MaSP),
PRIMARY KEY (MaKM, MaSP)
)
GO

CREATE TABLE DONHANG (
MaDH NVARCHAR(20) PRIMARY KEY,
NgayDatHang DATETIME NOT NULL DEFAULT GETDATE(),
KenhBan NVARCHAR(20) NOT NULL,
TrangThai NVARCHAR(20) NOT NULL DEFAULT N'ChoXacNhan',
PhuongThucThanhToan NVARCHAR(20) NOT NULL,
GhiChu NVARCHAR(MAX),
MaNV NVARCHAR(20) REFERENCES NHANVIEN(MaNV),
MaKH NVARCHAR(20) NULL REFERENCES KHACHHANG(MaKH),
MaKM NVARCHAR(20) NULL REFERENCES KHUYENMAI(MaKM),
SoTienGiam DECIMAL(18,0) NOT NULL DEFAULT 0 CHECK (SoTienGiam >= 0)
)
GO

CREATE TABLE CT_DONHANG (
MaDH NVARCHAR(20) REFERENCES DONHANG(MaDH) ON DELETE CASCADE,
MaSP NVARCHAR(20) REFERENCES SANPHAM(MaSP),
SoLuong INT NOT NULL CHECK (SoLuong > 0),
DonGia DECIMAL(18, 0) NOT NULL CHECK (DonGia > 0),
ThanhTien AS (SoLuong * DonGia) PERSISTED,
PRIMARY KEY (MaDH, MaSP)
)
GO

CREATE TABLE DONVIVANCHUYEN (
MaDVVC NVARCHAR(20) PRIMARY KEY,
TenDVVC NVARCHAR(100) NOT NULL,
SoDT NVARCHAR(15),
DiaChi NVARCHAR(200)
)
GO

CREATE TABLE VANCHUYEN (
MaVC NVARCHAR(20) PRIMARY KEY,
MaDH NVARCHAR(20) NOT NULL REFERENCES DONHANG(MaDH),
MaDVVC NVARCHAR(20) NOT NULL REFERENCES DONVIVANCHUYEN(MaDVVC),
LoaiPhi NVARCHAR(20) NOT NULL,
PhiVanChuyen DECIMAL(18,0) NOT NULL CHECK (PhiVanChuyen >= 0),
NgayGuiHang DATETIME,
NgayDuKien DATETIME,
NgayNhanHang DATETIME,
TrangThai NVARCHAR(50) NOT NULL DEFAULT N'Chờ lấy hàng',
GhiChu NVARCHAR(MAX)
)
GO

CREATE TABLE LICHSUVANCHUYEN (
MaLSVC INT IDENTITY(1,1) PRIMARY KEY,
MaVC NVARCHAR(20) NOT NULL REFERENCES VANCHUYEN(MaVC) ON DELETE CASCADE,
ThoiGian DATETIME NOT NULL DEFAULT GETDATE(),
ViTri NVARCHAR(200) NOT NULL,
TrangThai NVARCHAR(50) NOT NULL,
GhiChu NVARCHAR(MAX)
)
GO

CREATE TABLE HOADON (
MaHD NVARCHAR(20) PRIMARY KEY,
MaDH NVARCHAR(20) NOT NULL REFERENCES DONHANG(MaDH),
NgayLapHD DATETIME NOT NULL DEFAULT GETDATE(),
ThueVAT DECIMAL(5, 2) NOT NULL DEFAULT 10.00,
TrangThaiThanhToan NVARCHAR(50) NOT NULL,
MaNV NVARCHAR(20) REFERENCES NHANVIEN(MaNV)
)
GO

CREATE PROCEDURE SP_DoiMatKhau
@Username NVARCHAR(50),
@OldPass NVARCHAR(MAX),
@NewPass NVARCHAR(MAX)
AS
BEGIN
IF EXISTS (SELECT 1 FROM USERS WHERE user_name = @Username AND user_password = @OldPass)
BEGIN
UPDATE USERS SET user_password = @NewPass, NgayDoiMatKhauGanNhat = GETDATE()
WHERE user_name = @Username;
SELECT N'Đổi mật khẩu thành công!' AS Result;
END
ELSE SELECT N'Mật khẩu cũ không khớp!' AS Result;
END
GO

CREATE PROCEDURE LAPPHIEUNHAP
@SoPN NVARCHAR(20),
@NguoiGiao NVARCHAR(50),
@MaNCC NVARCHAR(20),
@MaNV NVARCHAR(20)
AS
BEGIN
IF NOT EXISTS (SELECT 1 FROM NHACUNGCAP WHERE MaNCC = @MaNCC)
BEGIN
SELECT N'Thất bại: Nhà cung cấp không tồn tại!' AS ThongBao;
RETURN;
END
INSERT INTO PHIEUNHAP (SoPN, NgayNhap, NguoiGiao, TongTienNhap, MaNCC, MaNV)
VALUES (@SoPN, GETDATE(), @NguoiGiao, 0, @MaNCC, @MaNV);
SELECT N'Thành công: Đã tạo phiếu nhập mới ' + @SoPN AS ThongBao;
END
GO

CREATE TRIGGER CAPNHATTONKHO
ON CT_PHIEUNHAP
AFTER INSERT
AS
BEGIN
UPDATE SANPHAM
SET SoLuongSP = SoLuongSP + x.SLNhap
FROM SANPHAM sp
JOIN (
SELECT MaSP, SUM(SLThucNhap) AS SLNhap
FROM inserted
GROUP BY MaSP
) x ON sp.MaSP = x.MaSP;
END
GO

CREATE TRIGGER CAPNHATTONGTIEN
ON CT_PHIEUNHAP
AFTER INSERT
AS
BEGIN
UPDATE p
SET TongTienNhap = TongTienNhap + x.TongTien
FROM PHIEUNHAP p
JOIN (
SELECT SoPN, SUM(SLThucNhap * DonGiaNhap) AS TongTien
FROM inserted
GROUP BY SoPN
) x ON p.SoPN = x.SoPN;
END
GO

CREATE TRIGGER TINHCHENHLECHKIEMKE
ON CTKIEMKE
INSTEAD OF INSERT
AS
BEGIN
INSERT INTO CTKIEMKE (SoBBKK, MaSP, SLSoSach, SLThucTe, SLThua, SLThieu, PhamChat)
SELECT
SoBBKK,
MaSP,
SLSoSach,
SLThucTe,
CASE WHEN SLThucTe > SLSoSach THEN SLThucTe - SLSoSach ELSE 0 END AS SLThua,
CASE WHEN SLThucTe < SLSoSach THEN SLSoSach - SLThucTe ELSE 0 END AS SLThieu,
PhamChat
FROM inserted;
END
GO

CREATE VIEW VW_BaoCaoTonKho AS
SELECT
sp.MaSP,
sp.TenSP,
ISNULL(Nhap.TongThucNhap, 0) AS TongSoLuongNhap,
ISNULL(Ban.TongBan, 0) AS TongSoLuongBan,
sp.SoLuongSP AS TonKhoThucTe,
CASE
WHEN sp.SoLuongSP = 0 THEN N'Hết hàng'
WHEN sp.SoLuongSP <= (SELECT CAST(GiaTri AS INT) FROM THAMSO WHERE MaTS = 'TONKHO' AND TinhTrang = N'Đang áp dụng')
THEN N'Cảnh báo: Dưới định mức'
ELSE N'An toàn'
END AS TrangThaiCanhBao
FROM SANPHAM sp
LEFT JOIN (
SELECT MaSP, SUM(SLThucNhap) AS TongThucNhap
FROM CT_PHIEUNHAP GROUP BY MaSP) Nhap ON sp.MaSP = Nhap.MaSP
LEFT JOIN (
SELECT MaSP, SUM(SoLuong) AS TongBan
FROM CT_DONHANG GROUP BY MaSP) Ban ON sp.MaSP = Ban.MaSP
GO

CREATE VIEW V_HOADON AS
SELECT
hd.MaHD,
hd.NgayLapHD,
hd.TrangThaiThanhToan,
hd.ThueVAT,
nv_hd.TenNV AS NVLapHoaDon,
dh.MaDH,
dh.NgayDatHang,
dh.KenhBan,
dh.PhuongThucThanhToan,
dh.TrangThai AS TrangThaiDonHang,
nv_dh.TenNV AS NVNhanDon,
kh.TenKH,
kh.SDTKH AS SoDTKhachHang,
kh.DiaChiKH,
SUM(ct.ThanhTien) AS TongTamTinh,
SUM(ct.ThanhTien) * hd.ThueVAT / 100 AS TienThue,
ISNULL(vc.PhiVanChuyen, 0) AS PhiVanChuyen,
dh.SoTienGiam,
SUM(ct.ThanhTien) + SUM(ct.ThanhTien) * hd.ThueVAT / 100 + ISNULL(vc.PhiVanChuyen, 0) - dh.SoTienGiam AS TongTien,
dv.TenDVVC,
vc.LoaiPhi,
vc.TrangThai AS TrangThaiVanChuyen,
vc.NgayGuiHang,
vc.NgayDuKien,
vc.NgayNhanHang
FROM HOADON hd
JOIN NHANVIEN nv_hd ON hd.MaNV = nv_hd.MaNV
JOIN DONHANG dh ON hd.MaDH = dh.MaDH
JOIN NHANVIEN nv_dh ON dh.MaNV = nv_dh.MaNV
LEFT JOIN KHACHHANG kh ON dh.MaKH = kh.MaKH
LEFT JOIN CT_DONHANG ct ON dh.MaDH = ct.MaDH
LEFT JOIN VANCHUYEN vc ON dh.MaDH = vc.MaDH
LEFT JOIN DONVIVANCHUYEN dv ON vc.MaDVVC = dv.MaDVVC
GROUP BY
hd.MaHD, hd.NgayLapHD, hd.TrangThaiThanhToan, hd.ThueVAT,
nv_hd.TenNV,
dh.MaDH, dh.NgayDatHang, dh.KenhBan, dh.PhuongThucThanhToan, dh.TrangThai, dh.SoTienGiam,
nv_dh.TenNV,
kh.TenKH, kh.SDTKH, kh.DiaChiKH,
vc.PhiVanChuyen, vc.LoaiPhi,
vc.TrangThai, vc.NgayGuiHang, vc.NgayDuKien, vc.NgayNhanHang,
dv.TenDVVC
GO

INSERT INTO THAMSO (MaTS, TenTS, Kieu_DVT, GiaTri, TinhTrang) VALUES
('TS_DT01', N'Trạng thái đơn hàng hợp lệ 1 (QĐ01)', N'Chuỗi', N'Đã giao hàng', N'Đang áp dụng'),
('TS_DT02', N'Trạng thái đơn hàng hợp lệ 2 (QĐ01)', N'Chuỗi', N'Hoàn thành', N'Đang áp dụng'),
('TOP_SP', N'Số lượng SP bán chạy hiển thị (QĐ02)', N'Số nguyên', N'5', N'Đang áp dụng'),
('TS_TG', N'Thời gian thống kê mặc định (QĐ03)', N'Chuỗi', N'Tháng hiện tại', N'Đang áp dụng'),
('TONKHO', N'Định mức cảnh báo tồn kho tối thiểu (QĐ03)', N'Số nguyên', N'5', N'Đang áp dụng')
GO

INSERT INTO USERS (user_name, user_password, user_email, user_phone, user_status, user_role) VALUES
(N'admin01', N'$2a$11$tprCEO.shXE5GhO/Gw6QXeDwuK/oMQMitlBJSftZ5JJvBtnLszZii', N'admin@shop.com', '0901111111', N'Đang hoạt động', N'Admin'),
(N'quanly01', N'$2a$11$tprCEO.shXE5GhO/Gw6QXeDwuK/oMQMitlBJSftZ5JJvBtnLszZii', N'quanly@shop.com', '0902222222', N'Đang hoạt động', N'Quản lý'),
(N'quanlykho01', N'$2a$11$tprCEO.shXE5GhO/Gw6QXeDwuK/oMQMitlBJSftZ5JJvBtnLszZii', N'quanlykho@shop.com', '0903333333', N'Đang hoạt động', N'Quản lý kho'),
(N'nvkho01', N'$2a$11$tprCEO.shXE5GhO/Gw6QXeDwuK/oMQMitlBJSftZ5JJvBtnLszZii', N'nvkho@shop.com', '0904444444', N'Đang hoạt động', N'Nhân viên kho'),
(N'nvbanhang01', N'$2a$11$tprCEO.shXE5GhO/Gw6QXeDwuK/oMQMitlBJSftZ5JJvBtnLszZii', N'nvbh@shop.com', '0905555555', N'Đang hoạt động', N'Nhân viên bán hàng'),
(N'khachhang01', N'$2a$11$tprCEO.shXE5GhO/Gw6QXeDwuK/oMQMitlBJSftZ5JJvBtnLszZii', N'kh01@gmail.com', '0906666666', N'Đang hoạt động', N'Khách hàng')
GO

INSERT INTO NHANVIEN (MaNV, user_id, HoNV, TenNV, AnhNV, EmailNV, SDTNV, DiaChiNV, GioiTinhNV, NgaySinhNV, TrangThaiLamViec, NgayVaoLam) VALUES
('NV001', 1, N'Nguyễn', N'Admin', 'anh_nv001.jpg', 'admin@shop.com', '0901111111', N'Hà Nội', 1, '1990-01-01', N'Đang làm', '2020-01-01'),
('NV002', 2, N'Trần', N'Quản Lý', 'anh_nv002.jpg', 'quanly@shop.com', '0902222222', N'Hà Nội', 1, '1992-05-10', N'Đang làm', '2021-03-01'),
('NV003', 3, N'Lê', N'Quản Lý Kho', 'anh_nv003.jpg', 'quanlykho@shop.com', '0903333333', N'Hà Nội', 0, '1993-07-15', N'Đang làm', '2021-06-01'),
('NV004', 4, N'Phạm', N'NV Kho', 'anh_nv004.jpg', 'nvkho@shop.com', '0904444444', N'Hà Nội', 1, '1995-03-20', N'Đang làm', '2022-01-15'),
('NV005', 5, N'Hoàng', N'NV Bán Hàng', 'anh_nv005.jpg', 'nvbh@shop.com', '0905555555', N'Hà Nội', 0, '1997-09-25', N'Đang làm', '2022-08-01')
GO

INSERT INTO KHACHHANG (MaKH, user_id, HoKH, TenKH, AnhKH, DiaChiKH, EmailKH, SDTKH, GioiTinhKH, NgaySinhKH, LoaiKH) VALUES
('KH001', 6, N'Võ', N'Khách Hàng A', NULL, N'TP.HCM', N'kh01@gmail.com', '0906666666', 1, '2000-04-12', N'Thường')
GO

INSERT INTO NHACUNGCAP (MaNCC, TenNCC, DiaChi, SDT) VALUES
('NCC001', N'Công ty TNHH Yamaha Việt Nam', N'Hà Nội', '0241111111'),
('NCC002', N'Công ty Fender Việt Nam', N'TP.HCM', '0282222222'),
('NCC003', N'Nhạc cụ Gibson Distribution', N'TP.HCM', '0283333333'),
('NCC004', N'Công ty Taylor Guitars VN', N'Đà Nẵng', '0234444444'),
('NCC005', N'Phụ kiện nhạc cụ Admira', N'Hà Nội', '0245555555')
GO

INSERT INTO DANHMUCSANPHAM (MaLoaiSP, TenLoaiSP) VALUES
('AC', N'Guitar Acoustic'),
('CL', N'Guitar Classic'),
('EL', N'Guitar Electric'),
('PK', N'Phụ kiện')
GO

INSERT INTO SANPHAM (MaSP, TenSP, AnhSP, MoTaSP, GiaNhap, GiaBan, TrangThaiSP, ThuongHieu, MaLoaiSP, SoLuongSP) VALUES
('SP001', N'Guitar Acoustic Yamaha F310', 'yamaha_f310.jpg', N'Guitar acoustic dành cho người mới học, âm thanh ấm, dễ chơi', 1500000, 2200000, 1, N'Yamaha', 'AC', 10),
('SP002', N'Guitar Acoustic Fender CD-60S', 'fender_cd60s.jpg', N'Đàn acoustic chất lượng cao, phù hợp sinh viên', 2000000, 3000000, 1, N'Fender', 'AC', 8),
('SP003', N'Guitar Acoustic Taylor GS Mini', 'taylor_gsmini.jpg', N'Đàn cỡ nhỏ tiện mang theo, âm thanh tuyệt vời', 3500000, 5000000, 1, N'Taylor', 'AC', 5),
('SP004', N'Guitar Classic Yamaha C40', 'yamaha_c40.jpg', N'Guitar classic phổ biến nhất cho học sinh sinh viên', 800000, 1200000, 1, N'Yamaha', 'CL', 15),
('SP005', N'Guitar Classic Admira Juanita', 'admira_juanita.jpg', N'Đàn classic Tây Ban Nha, âm thanh chuẩn', 2500000, 3800000, 1, N'Admira', 'CL', 6),
('SP006', N'Guitar Electric Fender Stratocaster', 'fender_strat.jpg', N'Đàn điện huyền thoại, phù hợp mọi thể loại nhạc', 5000000, 7500000, 1, N'Fender', 'EL', 4),
('SP007', N'Guitar Electric Gibson Les Paul', 'gibson_lespaul.jpg', N'Âm thanh dày, mạnh mẽ, chuẩn rock', 8000000, 12000000, 1, N'Gibson', 'EL', 3),
('SP008', N'Dây đàn Acoustic D''Addario EJ16', 'daddario_ej16.jpg', N'Dây đàn acoustic phosphor bronze, độ bền cao', 80000, 150000, 1, N'D''Addario', 'PK', 50),
('SP009', N'Capo guitar Kyser Quick-Change', 'kyser_capo.jpg', N'Capo kẹp nhanh, chắc chắn, không làm lạc dây', 120000, 220000, 1, N'Kyser', 'PK', 30),
('SP010', N'Bao đàn guitar acoustic loại dày', 'bao_dan_acoustic.jpg', N'Bao đàn chống sốc, chống nước, có ngăn phụ kiện', 150000, 280000, 1, NULL, 'PK', 20)
GO

INSERT INTO PHIEUNHAP (SoPN, NgayNhap, NguoiGiao, TongTienNhap, MaNCC, MaNV) VALUES
('PN001', '2024-01-10', N'Nguyễn Văn Giao', 0, 'NCC001', 'NV004'),
('PN002', '2024-02-15', N'Trần Thị Giao', 0, 'NCC002', 'NV004')
GO

INSERT INTO CT_PHIEUNHAP (SoPN, MaSP, SLChungTu, SLThucNhap, DonGiaNhap, ThanhTien) VALUES
('PN001', 'SP001', 10, 10, 1500000, 15000000),
('PN001', 'SP004', 15, 15, 800000, 12000000),
('PN002', 'SP006', 4, 4, 5000000, 20000000),
('PN002', 'SP008', 50, 50, 80000, 4000000)
GO

INSERT INTO DONVIVANCHUYEN (MaDVVC, TenDVVC, SoDT, DiaChi) VALUES
('DVVC001', N'Giao Hàng Nhanh', '1900636677', N'Hà Nội'),
('DVVC002', N'Giao Hàng Tiết Kiệm', '1900636678', N'TP.HCM'),
('DVVC003', N'J&T Express', '19001922', N'TP.HCM')
GO

INSERT INTO KHUYENMAI (MaKM, TenKM, MoTaKM, AnhBannerKM, ThuTuHienThi, MaVoucher, LoaiGiam, GiaTriGiam, GiamToiDa, DonToiThieu, SoLuotToiDa, SoLuotDaDung, SoLanDungMoiKH, NgayBatDau, NgayKetThuc, TrangThaiKM, NgayTao, MaNV) VALUES
('KM001', N'Giảm 10% cho đơn từ 1 triệu', N'Áp dụng cho tất cả sản phẩm', NULL, 1, N'GIAM10', N'Phần trăm', 10.00, 200000, 1000000, 100, 0, 1, '2024-01-01', '2024-12-31', N'Hoạt động', '2024-01-01', 'NV002'),
('KM002', N'Giảm 50K cho đơn từ 500K', N'Áp dụng cho phụ kiện', NULL, 2, N'PK50K', N'Số tiền', 50000.00, NULL, 500000, 50, 0, 1, '2024-02-01', '2024-06-30', N'Hết hạn', '2024-02-01', 'NV002'),
('KM003', N'Giảm 5% không giới hạn đơn', N'Dành cho khách hàng thân thiết', NULL, 3, N'LOYAL5', N'Phần trăm', 5.00, 500000, 0, NULL, 0, 3, '2024-03-01', '2024-12-31', N'Hoạt động', '2024-03-01', 'NV002')
GO

INSERT INTO KHUYENMAI_SANPHAM (MaKM, MaSP) VALUES
('KM001', 'SP001'),
('KM001', 'SP002'),
('KM001', 'SP003'),
('KM001', 'SP006'),
('KM001', 'SP007'),
('KM002', 'SP008'),
('KM002', 'SP009'),
('KM002', 'SP010'),
('KM003', 'SP001'),
('KM003', 'SP004'),
('KM003', 'SP005')
GO

INSERT INTO DONHANG (MaDH, NgayDatHang, KenhBan, TrangThai, PhuongThucThanhToan, GhiChu, MaNV, MaKH, MaKM, SoTienGiam) VALUES
('DH001', '2024-03-01', N'Online', N'HoanThanh', N'Chuyển khoản', N'Giao giờ hành chính', 'NV005', 'KH001', NULL, 0),
('DH002', '2024-03-10', N'Tại quầy', N'HoanThanh', N'Tiền mặt', NULL, 'NV005', 'KH001', NULL, 0),
('DH003', '2024-04-05', N'Online', N'ChoXacNhan', N'Chuyển khoản', N'Gọi trước khi giao', 'NV005', 'KH001', NULL, 0)
GO

INSERT INTO CT_DONHANG (MaDH, MaSP, SoLuong, DonGia) VALUES
('DH001', 'SP001', 1, 2200000),
('DH001', 'SP008', 2, 150000),
('DH002', 'SP004', 1, 1200000),
('DH002', 'SP009', 1, 220000),
('DH003', 'SP006', 1, 7500000)
GO

INSERT INTO GIOHANG (MaGH, MaSP, SoLuong, NgayCapNhatGioHang, MaKH) VALUES
('GH001', 'SP002', 1, GETDATE(), 'KH001'),
('GH002', 'SP007', 1, GETDATE(), 'KH001')
GO

INSERT INTO VANCHUYEN (MaVC, MaDH, MaDVVC, LoaiPhi, PhiVanChuyen, NgayGuiHang, NgayDuKien, NgayNhanHang, TrangThai, GhiChu) VALUES
('VC001', 'DH001', 'DVVC001', N'Nội thành', 20000, '2024-03-02', '2024-03-04', '2024-03-04', N'Đã giao', NULL),
('VC002', 'DH003', 'DVVC002', N'Ngoại thành', 35000, NULL, NULL, NULL, N'Chờ lấy hàng', NULL)
GO

INSERT INTO LICHSUVANCHUYEN (MaVC, ThoiGian, ViTri, TrangThai, GhiChu) VALUES
('VC001', '2024-03-02 08:00:00', N'Kho Hà Nội', N'Đã lấy hàng', NULL),
('VC001', '2024-03-03 14:00:00', N'Trung tâm phân loại Hà Nội', N'Đang vận chuyển', NULL),
('VC001', '2024-03-04 10:00:00', N'TP.HCM', N'Đã giao', N'Khách đã nhận hàng')
GO

INSERT INTO HOADON (MaHD, MaDH, NgayLapHD, ThueVAT, TrangThaiThanhToan, MaNV) VALUES
('HD001', 'DH001', '2024-03-04', 10.00, N'Đã thanh toán', 'NV005'),
('HD002', 'DH002', '2024-03-10', 10.00, N'Đã thanh toán', 'NV005')
GO

INSERT INTO BBKIEMKE (SoBBKK, NgayLap, TrangThai, MaNV) VALUES
('BBKK001', '2024-04-01', N'Đã duyệt', 'NV003'),
('BBKK002', '2024-04-15', N'Chờ duyệt', 'NV004')
GO

INSERT INTO CTKIEMKE (SoBBKK, MaSP, SLSoSach, SLThucTe, PhamChat) VALUES
('BBKK001', 'SP001', 10, 10, N'Tốt'),
('BBKK001', 'SP004', 15, 14, N'Tốt'),
('BBKK002', 'SP006', 4, 4, N'Tốt'),
('BBKK002', 'SP008', 50, 52, N'Tốt')
GO

INSERT INTO BBKIEMNGHIEM (SoBBKN, NgayLap, MaNV) VALUES
('BBKN001', '2024-01-11', 'NV003'),
('BBKN002', '2024-02-16', 'NV003')
GO

INSERT INTO CTKIEMNGHIEM (SoBBKN, MaSP, SLChungTu, SLDungQuyCach, SLLoi, GhiChu) VALUES
('BBKN001', 'SP001', 10, 10, 0, N'Đạt yêu cầu'),
('BBKN001', 'SP004', 15, 15, 0, N'Đạt yêu cầu'),
('BBKN002', 'SP006', 4, 3, 1, N'1 cái bị trầy mặt thân đàn'),
('BBKN002', 'SP008', 50, 50, 0, N'Đạt yêu cầu')
GO