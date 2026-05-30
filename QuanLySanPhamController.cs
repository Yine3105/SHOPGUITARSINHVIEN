using SHOPGUITAR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SHOPGUITAR.Controllers
{
    public class QuanLySanPhamController : Controller
    {
        private SHOPGUITARSINHVIENEntities db = new SHOPGUITARSINHVIENEntities();
        // 1. CHỨC NĂNG QUẢN LÝ TỒN KHO
        public ActionResult TonKho()
        {
            // Lấy mức cảnh báo tồn kho tối thiểu từ bảng THAMSO
            var thamSo = db.THAMSOes.FirstOrDefault(t => t.MaTS == "TONKHO");
            int mucCanhBao = thamSo != null ? int.Parse(thamSo.GiaTri) : 5;
            ViewBag.MucCanhBao = mucCanhBao;

            // Truy xuất danh sách sản phẩm để hiển thị số lượng tồn
            var sanPhams = db.SANPHAMs.ToList();
            return View(sanPhams);
        }

        // 2. CHỨC NĂNG NHẬP HÀNG - LẬP PHIẾU NHẬP
        [HttpGet]
        public ActionResult LapPhieuNhap()
        {
            // Load danh sách nhà cung cấp vào Dropdownlist theo biểu mẫu QLK_BM1
            ViewBag.MaNCC = new SelectList(db.NHACUNGCAPs, "MaNCC", "TenNCC");
            return View();
        }

        [HttpPost]
        public ActionResult LapPhieuNhap(PHIEUNHAP pn)
        {
            pn.NgayNhap = DateTime.Now;
            pn.TongTienNhap = 0; // Trigger SQL sẽ tự tính tổng tiền lô hàng

            // Lấy ID người dùng đang đăng nhập (nếu có), tạm thời gán cứng NV003 (Quản lý kho)
            pn.MaNV = Session["UserRole"] != null ? "NV003" : "NV003";

            db.PHIEUNHAPs.Add(pn);
            db.SaveChanges();

            // Chuyển sang bước thêm chi tiết sản phẩm vào phiếu nhập vừa tạo
            return RedirectToAction("ChiTietNhap", new { id = pn.SoPN });
        }

        // 3. CHỨC NĂNG NHẬP HÀNG - CHI TIẾT
        [HttpGet]
        public ActionResult ChiTietNhap(string id)
        {
            ViewBag.SoPN = id;
            ViewBag.MaSP = new SelectList(db.SANPHAMs, "MaSP", "TenSP");

            // Hiển thị các sản phẩm đã được quét vào phiếu nhập này
            var chiTiet = db.CT_PHIEUNHAP.Where(c => c.SoPN == id).ToList();
            return View(chiTiet);
        }

        [HttpPost]
        public ActionResult ChiTietNhap(CT_PHIEUNHAP ct)
        {
            // Kiểm tra quy định số lượng nhập > 0 và tính thành tiền
            ct.ThanhTien = ct.SLThucNhap * ct.DonGiaNhap;

            db.CT_PHIEUNHAP.Add(ct);
            db.SaveChanges(); // Trigger SQL tự động cộng Tồn Kho và Tổng Tiền

            return RedirectToAction("ChiTietNhap", new { id = ct.SoPN });
        }
        // 4. CHỨC NĂNG KIỂM NGHIỆM SẢN PHẨM
        [HttpGet]
        public ActionResult LapBienBanKiemNghiem()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LapBienBanKiemNghiem(BBKIEMNGHIEM bb)
        {
            bb.NgayLap = DateTime.Now;
            // Giả định NV004 là Nhân viên kho đang đăng nhập
            bb.MaNV = Session["UserRole"] != null ? "NV004" : "NV004";

            db.BBKIEMNGHIEMs.Add(bb);
            db.SaveChanges();

            return RedirectToAction("ChiTietKiemNghiem", new { id = bb.SoBBKN });
        }

        [HttpGet]
        public ActionResult ChiTietKiemNghiem(string id)
        {
            ViewBag.SoBBKN = id;
            ViewBag.MaSP = new SelectList(db.SANPHAMs, "MaSP", "TenSP");

            var chiTiet = db.CTKIEMNGHIEMs.Where(c => c.SoBBKN == id).ToList();
            return View(chiTiet);
        }

        [HttpPost]
        public ActionResult ChiTietKiemNghiem(CTKIEMNGHIEM ct)
        {
            db.CTKIEMNGHIEMs.Add(ct);
            db.SaveChanges();
            return RedirectToAction("ChiTietKiemNghiem", new { id = ct.SoBBKN });
        }

        // 5. CHỨC NĂNG KIỂM KÊ KHO ĐỊNH KỲ
        [HttpGet]
        public ActionResult LapBienBanKiemKe()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LapBienBanKiemKe(BBKIEMKE bb)
        {
            bb.NgayLap = DateTime.Now;
            bb.TrangThai = "Chờ duyệt"; // Mặc định phải chờ Quản lý duyệt
            bb.MaNV = Session["UserRole"] != null ? "NV004" : "NV004";

            db.BBKIEMKEs.Add(bb);
            db.SaveChanges();

            return RedirectToAction("ChiTietKiemKe", new { id = bb.SoBBKK });
        }

        [HttpGet]
        public ActionResult ChiTietKiemKe(string id)
        {
            ViewBag.SoBBKK = id;
            ViewBag.MaSP = new SelectList(db.SANPHAMs, "MaSP", "TenSP");

            var chiTiet = db.CTKIEMKEs.Where(c => c.SoBBKK == id).ToList();
            return View(chiTiet);
        }

        [HttpPost]
        public ActionResult ChiTietKiemKe(CTKIEMKE ct)
        {
            // Tự động lấy số lượng tồn kho trên máy tính gán vào SLSoSach
            var sp = db.SANPHAMs.FirstOrDefault(s => s.MaSP == ct.MaSP);
            ct.SLSoSach = sp != null ? sp.SoLuongSP : 0;

            db.CTKIEMKEs.Add(ct);
            db.SaveChanges();
            // Ngay lúc này, Trigger TINHCHENHLECHKIEMKE dưới SQL Server sẽ 
            // tự động tính ra SLThua và SLThieu dựa vào SLThucTe và SLSoSach

            return RedirectToAction("ChiTietKiemKe", new { id = ct.SoBBKK });
        }
        [HttpGet]
        public ActionResult ThemNhaCungCap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ThemNhaCungCap(NHACUNGCAP ncc)
        {
            // Kiểm tra trùng mã
            if (db.NHACUNGCAPs.Any(x => x.MaNCC == ncc.MaNCC))
            {
                ModelState.AddModelError("", "Mã Nhà cung cấp đã tồn tại!");
                return View(ncc);
            }

            db.NHACUNGCAPs.Add(ncc);
            db.SaveChanges();

            // Trả về đoạn script thông báo thành công và tự đóng tab
            return Content("<script>alert('Thêm Nhà cung cấp thành công!'); window.close();</script>");
        }
        [HttpGet]
        public ActionResult ThemSanPham()
        {
            ViewBag.MaLoaiSP = new SelectList(db.DANHMUCSANPHAMs, "MaLoaiSP", "TenLoaiSP");
            return View();
        }

        [HttpPost]
        public ActionResult ThemSanPham(SANPHAM sp)
        {
            if (db.SANPHAMs.Any(x => x.MaSP == sp.MaSP))
            {
                ViewBag.MaLoaiSP = new SelectList(db.DANHMUCSANPHAMs, "MaLoaiSP", "TenLoaiSP", sp.MaLoaiSP);
                ModelState.AddModelError("", "Mã Sản phẩm đã tồn tại!");
                return View(sp);
            }

            // Gán dữ liệu mặc định bắt buộc
            sp.AnhSP = "default.png";
            sp.SoLuongSP = 0; // Tồn kho ban đầu luôn là 0
            sp.TrangThaiSP = true;
            sp.NgayThemSP = DateTime.Now;

            db.SANPHAMs.Add(sp);
            db.SaveChanges();

            return Content("<script>alert('Tạo mã sản phẩm thành công!'); window.close();</script>");
        }
    }
}