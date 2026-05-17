using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SHOPGUITAR.Models
{
    public class HomeModel
    {
        public List<DANHMUCSANPHAM> DanhSachLoai { get; set; }
        public List<SANPHAM> DanhSachSanPham { get; set; }
        public String LoaiDangChon { get; set; }
    }
}