using SHOPGUITAR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static SHOPGUITAR.Models.AuthModel;

namespace SHOPGUITAR.Controllers
{
    public class AuthController : Controller
    {
        private SHOPGUITARSINHVIENEntities db = new SHOPGUITARSINHVIENEntities();

        // ==================== ĐĂNG KÝ ====================
        [HttpGet]
        public ActionResult DangKy() => View();

        [HttpPost]
        public ActionResult DangKy(AuthModel.DangKiModel model)
        {
            // Kiểm tra tên đăng nhập
            if (string.IsNullOrWhiteSpace(model.UserName))
            { ViewBag.Error = "Vui lòng nhập tên đăng nhập!"; return View(model); }

            if (db.USERS.Any(u => u.user_name == model.UserName))
            { ViewBag.Error = "Tên đăng nhập đã tồn tại!"; return View(model); }

            // Kiểm tra ít nhất email hoặc SĐT
            if (string.IsNullOrWhiteSpace(model.Email) && string.IsNullOrWhiteSpace(model.Phone))
            { ViewBag.Error = "Vui lòng nhập ít nhất email hoặc số điện thoại!"; return View(model); }

            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                if (!model.Email.Contains("@"))
                { ViewBag.Error = "Email không hợp lệ!"; return View(model); }

                if (db.USERS.Any(u => u.user_email == model.Email))
                { ViewBag.Error = "Email đã tồn tại!"; return View(model); }
            }

            if (!string.IsNullOrWhiteSpace(model.Phone))
            {
                if (model.Phone.Length != 10 || !model.Phone.All(char.IsDigit))
                { ViewBag.Error = "Số điện thoại không hợp lệ!"; return View(model); }

                if (db.USERS.Any(u => u.user_phone == model.Phone))
                { ViewBag.Error = "Số điện thoại đã tồn tại!"; return View(model); }
            }

            // Kiểm tra mật khẩu
            if (string.IsNullOrWhiteSpace(model.Password) || model.Password.Length < 6)
            { ViewBag.Error = "Mật khẩu phải có ít nhất 6 ký tự!"; return View(model); }

            if (model.Password != model.ConfirmPassword)
            { ViewBag.Error = "Mật khẩu không khớp!"; return View(model); }

            // Kiểm tra thông tin khách hàng
            if (string.IsNullOrWhiteSpace(model.HoKH))
            { ViewBag.Error = "Vui lòng nhập họ!"; return View(model); }

            if (string.IsNullOrWhiteSpace(model.TenKH))
            { ViewBag.Error = "Vui lòng nhập tên!"; return View(model); }

            // Xử lý ảnh upload
            string tenAnh = null;
            if (model.AnhKH != null && model.AnhKH.ContentLength > 0)
            {
                // Kiểm tra định dạng ảnh
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                if (!allowedTypes.Contains(model.AnhKH.ContentType))
                { ViewBag.Error = "Chỉ chấp nhận file ảnh (jpg, png, gif, webp)!"; return View(model); }

                // Kiểm tra dung lượng (tối đa 2MB)
                if (model.AnhKH.ContentLength > 2 * 1024 * 1024)
                { ViewBag.Error = "Ảnh không được vượt quá 2MB!"; return View(model); }

                // Lưu file vào thư mục Uploads/KhachHang
                string folder = Server.MapPath("~/Images/KhachHang/");
                if (!System.IO.Directory.Exists(folder))
                    System.IO.Directory.CreateDirectory(folder);

                string ext = System.IO.Path.GetExtension(model.AnhKH.FileName);
                tenAnh = "KH_" + DateTime.Now.Ticks + ext;
                model.AnhKH.SaveAs(folder + tenAnh);
            }

            // Tạo user
            var user = new USER
            {
                user_name = model.UserName,
                user_password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                user_email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email,
                user_phone = string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone,
                user_status = "Đang hoạt động",
                user_role = "Khách hàng",
                created_at = DateTime.Now
            };

            db.USERS.Add(user);
            db.SaveChanges();

            int soKH = db.KHACHHANGs.Count() + 1;
            // Tạo khách hàng
            var kh = new KHACHHANG
            {
                MaKH = "KH" + soKH.ToString("D3"),
                user_id = user.user_id,
                HoKH = model.HoKH,
                TenKH = model.TenKH,
                AnhKH = tenAnh,
                DiaChiKH = model.DiaChiKH,
                GioiTinhKH = model.GioiTinhKH,
                NgaySinhKH = model.NgaySinhKH,
                LoaiKH = "Thường"
            };

            db.KHACHHANGs.Add(kh);
            db.SaveChanges();

            TempData["Success"] = "Đăng ký thành công!";
            return RedirectToAction("DangNhap");
        }

        // ==================== ĐĂNG NHẬP ====================
        [HttpGet]
        public ActionResult DangNhap() => View();

        [HttpPost]
        public ActionResult DangNhap(AuthModel.DangNhapModel model)
        {
            if (string.IsNullOrWhiteSpace(model.TaiKhoan))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
                return View(model);
            }

            // Tìm theo username, email hoặc SĐT
            var user = db.USERS.FirstOrDefault(u =>
                u.user_name == model.TaiKhoan ||
                u.user_email == model.TaiKhoan ||
                u.user_phone == model.TaiKhoan
            );

            if (user == null)
            {
                ViewBag.Error = "Tài khoản không tồn tại!";
                return View(model);
            }

            if (user.user_status != "Đang hoạt động")
            {
                ViewBag.Error = "Tài khoản đã bị khóa!";
                return View(model);
            }

            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.user_password))
            {
                ViewBag.Error = "Sai mật khẩu!";
                return View(model);
            }

            user.last_login = DateTime.Now;
            db.SaveChanges();

            Session["UserId"] = user.user_id;
            Session["UserName"] = user.user_name;
            Session["UserRole"] = user.user_role;

            // Test: lấy thẳng không cần kiểm tra role
            var kh = db.KHACHHANGs.FirstOrDefault(k => k.user_id == user.user_id);
            var nv = db.NHANVIENs.FirstOrDefault(n => n.user_id == user.user_id);

            if (kh != null)
                Session["HoTen"] = kh.HoKH + " " + kh.TenKH;
            else if (nv != null)
                Session["HoTen"] = nv.HoNV + " " + nv.TenNV;
            else
                Session["HoTen"] = user.user_name;

            return RedirectToAction("Index", "Home");
        }

        // ==================== ĐĂNG XUẤT ====================
        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("DangNhap");
        }
    }
}