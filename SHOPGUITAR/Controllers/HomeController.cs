using BCrypt;
using SHOPGUITAR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SHOPGUITAR.Controllers
{
    public class HomeController : Controller
    {
        private SHOPGUITARSINHVIENEntities db = new SHOPGUITARSINHVIENEntities();
        public ActionResult Index(string maLoai = null)
        {
            var model = new HomeModel();
            model.DanhSachLoai = db.DANHMUCSANPHAMs.ToList();

            if (!string.IsNullOrEmpty(maLoai))
            {
                model.DanhSachSanPham = db.SANPHAMs
                    .Where(s => s.MaLoaiSP == maLoai)
                    .ToList();
                model.LoaiDangChon = maLoai;
            }
            else
            {
                model.DanhSachSanPham = db.SANPHAMs.ToList();
            }

            return View(model);
        }
    }
}