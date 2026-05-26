using SHOPGUITAR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SHOPGUITAR.Controllers
{
    public class AdminController : Controller
    {
        private SHOPGUITARSINHVIENEntities db = new SHOPGUITARSINHVIENEntities();

        // 1. Giao diện hiển thị danh sách User
        [HttpGet]
        public ActionResult PhanQuyen()
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var danhSachUsers = db.USERS.ToList();
            var danhSachRoles = new List<string> { "Admin", "Quản lý", "Quản lý kho", "Nhân viên kho", "Nhân viên bán hàng", "Khách hàng" };

            var danhSachStatus = new List<string> { "Đang hoạt động", "Tạm nghỉ", "Nghỉ làm" };

            ViewBag.DanhSachRoles = danhSachRoles;
            ViewBag.DanhSachStatus = danhSachStatus;

            return View(danhSachUsers);
        }

        // 2. Hàm API AJAX cập nhật Quyền (Role)
        [HttpPost]
        public JsonResult CapNhatQuyen(int userId, string newRole)
        {
            try
            {
                var user = db.USERS.FirstOrDefault(u => u.user_id == userId);
                if (user == null) return Json(new { success = false, message = "Không tìm thấy người dùng!" });

                user.user_role = newRole;
                db.SaveChanges();

                return Json(new { success = true, message = "Cập nhật quyền thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // 3. Hàm API AJAX cập nhật Trạng thái (Status) mới thêm
        [HttpPost]
        public JsonResult CapNhatTrangThai(int userId, string newStatus)
        {
            try
            {
                var user = db.USERS.FirstOrDefault(u => u.user_id == userId);
                if (user == null) return Json(new { success = false, message = "Không tìm thấy người dùng!" });

                user.user_status = newStatus;
                db.SaveChanges();

                return Json(new { success = true, message = "Cập nhật trạng thái thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }
}