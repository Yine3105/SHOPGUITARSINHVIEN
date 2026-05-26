using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SHOPGUITAR.Models
{
    public class AuthModel
    {
        public class DangNhapModel
        {
            public string TaiKhoan { get; set; }
            public string Password { get; set; }
        }

        public class DangKiModel
        {
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Password { get; set; }
            public string ConfirmPassword { get; set; }

            // Thông tin khách hàng
            public string HoKH { get; set; }
            public string TenKH { get; set; }
            public bool GioiTinhKH { get; set; }
            public DateTime NgaySinhKH { get; set; }
            public string DiaChiKH { get; set; }
            // Ảnh upload qua HttpPostedFileBase
            public HttpPostedFileBase AnhKH { get; set; }
        }
        public class DoiMatKhauModel
        {
            public string MatKhauCu { get; set; }
            public string MatKhauMoi { get; set; }
            public string XacNhanMatKhau { get; set; }
        }
    }
}