using SHOPGUITAR.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using Rotativa;
using System.IO;
using Newtonsoft.Json;

namespace SHOPGUITAR.Controllers
{
    public class BaoCaoNgayViewModel
    {
        public DateTime Ngay { get; set; }
        public int SoDonHang { get; set; }
        public int SoSanPhamBanRa { get; set; }
        public decimal DoanhThu { get; set; }
    }
    public class SanPhamBanChayViewModel
    {
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public string TenLoaiSP { get; set; }
        public decimal GiaNhap { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuongBan { get; set; }
        public decimal DoanhThu { get; set; }
        public decimal LoiNhuan { get; set; }
    }

    public class ThongKeController : Controller
    {
        private SHOPGUITARSINHVIENEntities db = new SHOPGUITARSINHVIENEntities();

        // ================= 1. THỐNG KÊ TỒN KHO =================
        [HttpGet]
        public ActionResult TonKho(string trangThaiHang)
        {
            if (Session["UserRole"] == null ||
                (Session["UserRole"].ToString() != "Admin" && Session["UserRole"].ToString() != "Quản lý" &&
                 Session["UserRole"].ToString() != "Quản lý kho" && Session["UserRole"].ToString() != "Nhân viên kho"))
                return RedirectToAction("Index", "Home");

            var dataTonKho = db.VW_BaoCaoTonKho.AsQueryable();

            if (!string.IsNullOrEmpty(trangThaiHang))
            {
                if (trangThaiHang == "SapHet") dataTonKho = dataTonKho.Where(p => p.TonKhoThucTe > 0 && p.TonKhoThucTe <= 5);
                else if (trangThaiHang == "HetHang") dataTonKho = dataTonKho.Where(p => p.TonKhoThucTe <= 0);
                else if (trangThaiHang == "ConHang") dataTonKho = dataTonKho.Where(p => p.TonKhoThucTe > 5);
            }

            ViewBag.TongSoLuongTon = dataTonKho.Sum(p => (int?)p.TonKhoThucTe) ?? 0;
            ViewBag.SapHetHang = db.VW_BaoCaoTonKho.Count(p => p.TonKhoThucTe > 0 && p.TonKhoThucTe <= 5);
            ViewBag.DaHetHang = db.VW_BaoCaoTonKho.Count(p => p.TonKhoThucTe <= 0);
            ViewBag.TrangThaiDangChon = trangThaiHang;

            return View(dataTonKho.ToList());
        }

        // ================= 2. XUẤT EXCEL TỒN KHO =================
        [HttpGet]
        public ActionResult XuatExcelTonKho()
        {
            var data = db.VW_BaoCaoTonKho.ToList();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Tồn Kho");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Mã SP";
                worksheet.Cell(currentRow, 2).Value = "Tên Nhạc Cụ";
                worksheet.Cell(currentRow, 4).Value = "Tồn Kho";
                worksheet.Cell(currentRow, 5).Value = "Trạng Thái";

                var headerRange = worksheet.Range(currentRow, 1, currentRow, 5);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#8B322C");

                foreach (var item in data)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.MaSP;
                    worksheet.Cell(currentRow, 2).Value = item.TenSP ?? "---";
                    worksheet.Cell(currentRow, 4).Value = item.TonKhoThucTe;
                    worksheet.Cell(currentRow, 5).Value = item.TonKhoThucTe <= 0 ? "Hết hàng" : (item.TonKhoThucTe <= 5 ? "Sắp hết" : "An toàn");
                }
                worksheet.Columns().AdjustToContents();
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BaoCaoTonKho_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx");
                }
            }
        }

        // ================= 3. XUẤT PDF TỒN KHO =================
        [HttpGet]
        public ActionResult XuatPdfTonKho()
        {
            ViewBag.NguoiLap = Session["UserName"] != null ? Session["UserName"].ToString() : "..................................";
            return new ViewAsPdf("InBaoCaoTonKho", db.VW_BaoCaoTonKho.ToList())
            {
                FileName = "BieuMau10_BaoCaoTonKho.pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = new Rotativa.Options.Margins(15, 15, 15, 15)
            };
        }

        // ================= 4. BÁO CÁO DOANH THU =================
        [HttpGet]
        public ActionResult DoanhThu(string tuNgay, string denNgay)
        {
            if (Session["UserRole"] == null || (Session["UserRole"].ToString() != "Admin" && Session["UserRole"].ToString() != "Quản lý"))
                return RedirectToAction("Index", "Home");

            DateTime startDate = DateTime.Now.AddDays(-30).Date;
            DateTime endDate = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(tuNgay)) DateTime.TryParseExact(tuNgay, "yyyy-MM-dd", null, DateTimeStyles.None, out startDate);
            if (!string.IsNullOrEmpty(denNgay)) DateTime.TryParseExact(denNgay, "yyyy-MM-dd", null, DateTimeStyles.None, out endDate);

            // RÀNG BUỘC: Nếu đến ngày vượt quá hôm nay, tự động khóa về ngày hôm nay
            if (endDate > DateTime.Now.Date)
            {
                endDate = DateTime.Now.Date;
            }
            endDate = endDate.AddDays(1).AddTicks(-1);

            var danhSachDonHang = db.DONHANGs
                .Where(d => d.NgayDatHang >= startDate && d.NgayDatHang <= endDate && d.TrangThai == "HoanThanh")
                .OrderByDescending(d => d.NgayDatHang)
                .ToList();

            var dicTongTien = new Dictionary<string, decimal>();
            decimal tongDoanhThu = 0;
            foreach (var don in danhSachDonHang)
            {
                decimal tienCuaDon = db.CT_DONHANG
                    .Where(ct => ct.MaDH == don.MaDH)
                    .Sum(ct => ct.SoLuong * ct.DonGia) ;
                dicTongTien[don.MaDH.ToString()] = tienCuaDon;
                tongDoanhThu += tienCuaDon;
            }
            var chartRaw = danhSachDonHang
                .GroupBy(d => d.NgayDatHang.ToString("dd/MM"))
                .Select(g => new
                {
                    Ngay = g.Key,
                    DoanhThuNgay = g.Sum(d => dicTongTien.ContainsKey(d.MaDH.ToString()) ? dicTongTien[d.MaDH.ToString()] : 0)
                })
                .Reverse()
                .ToList();

            ViewBag.ChartLabels = JsonConvert.SerializeObject(chartRaw.Select(x => x.Ngay).ToList());
            ViewBag.ChartData = JsonConvert.SerializeObject(chartRaw.Select(x => x.DoanhThuNgay).ToList());

            ViewBag.TongDoanhThu = tongDoanhThu;
            ViewBag.TongSoDon = danhSachDonHang.Count;
            ViewBag.TrungBinhDon = danhSachDonHang.Count > 0 ? tongDoanhThu / danhSachDonHang.Count : 0;
            ViewBag.TuNgay = startDate.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = endDate.ToString("yyyy-MM-dd");
            ViewBag.DicTongTien = dicTongTien;

            return View(danhSachDonHang);
        }

        // ================= 5. XUẤT PDF DOANH THU =================
        [HttpGet]
        public ActionResult XuatPdfDoanhThu(string tuNgay, string denNgay)
        {
            if (Session["UserRole"] == null || (Session["UserRole"].ToString() != "Admin" && Session["UserRole"].ToString() != "Quản lý"))
                return RedirectToAction("Index", "Home");

            DateTime startDate = DateTime.Now.AddDays(-30).Date;
            DateTime endDate = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(tuNgay)) DateTime.TryParseExact(tuNgay, "yyyy-MM-dd", null, DateTimeStyles.None, out startDate);
            if (!string.IsNullOrEmpty(denNgay)) DateTime.TryParseExact(denNgay, "yyyy-MM-dd", null, DateTimeStyles.None, out endDate);

            // RÀNG BUỘC: Nếu đến ngày vượt quá hôm nay, tự động khóa về ngày hôm nay
            if (endDate > DateTime.Now.Date)
            {
                endDate = DateTime.Now.Date;
            }
            endDate = endDate.AddDays(1).AddTicks(-1);

            var danhSachDonHang = db.DONHANGs.Where(d => d.NgayDatHang >= startDate && d.NgayDatHang <= endDate && d.TrangThai == "HoanThanh").ToList();

            var model = danhSachDonHang
                .GroupBy(d => d.NgayDatHang.Date)
                .Select(g =>
                {
                    var maDHsNgay = g.Select(d => d.MaDH).ToList();
                    int soSanPham = db.CT_DONHANG.Where(ct => maDHsNgay.Contains(ct.MaDH)).Sum(ct => (int?)ct.SoLuong) ?? 0;
                    decimal doanhThu = db.CT_DONHANG.Where(ct => maDHsNgay.Contains(ct.MaDH)).Sum(ct => (decimal?)(ct.SoLuong * ct.DonGia)) ?? 0;

                    return new BaoCaoNgayViewModel
                    {
                        Ngay = g.Key,
                        SoDonHang = g.Count(),
                        SoSanPhamBanRa = soSanPham,
                        DoanhThu = doanhThu
                    };
                })
                .OrderBy(x => x.Ngay)
                .ToList();

            ViewBag.NguoiLap = Session["UserName"] != null ? Session["UserName"].ToString() : "..................................";
            ViewBag.TuNgay = startDate.ToString("dd/MM/yyyy");
            ViewBag.DenNgay = endDate.ToString("dd/MM/yyyy");

            return new ViewAsPdf("InBaoCaoDoanhThu", model)
            {
                FileName = "BaoCaoDoanhThu_" + DateTime.Now.ToString("ddMMyyyy") + ".pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = new Rotativa.Options.Margins(15, 15, 15, 15)
            };
        }

        // ================= 6. BÁN CHẠY (TopSP) =================
        [HttpGet]
        public ActionResult TopSP(string tuNgay, string denNgay)
        {
            if (Session["UserRole"] == null || (Session["UserRole"].ToString() != "Admin" && Session["UserRole"].ToString() != "Quản lý"))
                return RedirectToAction("Index", "Home");

            DateTime startDate = DateTime.Now.AddDays(-30).Date;
            DateTime endDate = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(tuNgay)) DateTime.TryParseExact(tuNgay, "yyyy-MM-dd", null, DateTimeStyles.None, out startDate);
            if (!string.IsNullOrEmpty(denNgay)) DateTime.TryParseExact(denNgay, "yyyy-MM-dd", null, DateTimeStyles.None, out endDate);

            // RÀNG BUỘC: Nếu đến ngày vượt quá hôm nay, tự động khóa về ngày hôm nay
            if (endDate > DateTime.Now.Date)
            {
                endDate = DateTime.Now.Date;
            }
            endDate = endDate.AddDays(1).AddTicks(-1);

            var maDHs = db.DONHANGs.Where(d => d.NgayDatHang >= startDate && d.NgayDatHang <= endDate && d.TrangThai == "HoanThanh").Select(d => d.MaDH).ToList();
            var grouped = db.CT_DONHANG.Where(ct => maDHs.Contains(ct.MaDH)).GroupBy(ct => ct.MaSP).Select(g => new { MaSP = g.Key, SL = g.Sum(x => x.SoLuong), DT = g.Sum(x => x.SoLuong * x.DonGia) }).OrderByDescending(x => x.SL).ToList();

            var list = grouped.Select(x => {
                var sp = db.SANPHAMs.FirstOrDefault(s => s.MaSP == x.MaSP);
                return new SanPhamBanChayViewModel { MaSP = x.MaSP, TenSP = sp?.TenSP, TenLoaiSP = sp?.DANHMUCSANPHAM?.TenLoaiSP, GiaBan = sp?.GiaBan ?? 0, SoLuongBan = x.SL, DoanhThu = x.DT };
            }).ToList();

            ViewBag.TuNgay = startDate.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = endDate.ToString("yyyy-MM-dd");
            return View(list);
        }

        // ================= 7. XUẤT PDF BIỂU MẪU 9 =================
        [HttpGet]
        public ActionResult XuatPdfBanChay(string tuNgay, string denNgay)
        {
            DateTime startDate = DateTime.Now.AddDays(-30).Date;
            DateTime endDate = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(tuNgay)) DateTime.TryParseExact(tuNgay, "yyyy-MM-dd", null, DateTimeStyles.None, out startDate);
            if (!string.IsNullOrEmpty(denNgay)) DateTime.TryParseExact(denNgay, "yyyy-MM-dd", null, DateTimeStyles.None, out endDate);

            // RÀNG BUỘC: Nếu đến ngày vượt quá hôm nay, tự động khóa về ngày hôm nay
            if (endDate > DateTime.Now.Date)
            {
                endDate = DateTime.Now.Date;
            }
            endDate = endDate.AddDays(1).AddTicks(-1);

            var maDHs = db.DONHANGs.Where(d => d.NgayDatHang >= startDate && d.NgayDatHang <= endDate && d.TrangThai == "HoanThanh").Select(d => d.MaDH).ToList();
            var grouped = db.CT_DONHANG.Where(ct => maDHs.Contains(ct.MaDH)).GroupBy(ct => ct.MaSP).Select(g => new { MaSP = g.Key, SL = g.Sum(x => x.SoLuong), DT = g.Sum(x => x.SoLuong * x.DonGia) }).OrderByDescending(x => x.SL).ToList();

            var listBanChay = grouped.Select(x => {
                var sp = db.SANPHAMs.FirstOrDefault(s => s.MaSP == x.MaSP);
                return new SanPhamBanChayViewModel { MaSP = x.MaSP, TenSP = sp?.TenSP, TenLoaiSP = sp?.DANHMUCSANPHAM?.TenLoaiSP, GiaBan = sp?.GiaBan ?? 0, SoLuongBan = x.SL, DoanhThu = x.DT, LoiNhuan = x.DT - ((sp?.GiaNhap ?? 0) * x.SL) };
            }).ToList();

            ViewBag.NguoiLap = Session["UserName"] != null ? Session["UserName"].ToString() : "..................................";
            ViewBag.TuNgay = startDate.ToString("dd/MM/yyyy");
            ViewBag.DenNgay = endDate.ToString("dd/MM/yyyy");
            ViewBag.TongSoDon = maDHs.Count;

            return new Rotativa.ViewAsPdf("InBaoCaoTopSP", listBanChay)
            {
                FileName = "BMTopSanPham" + ".pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageMargins = new Rotativa.Options.Margins(15, 15, 15, 15)
            };
        }

        // ================= 8. LỢI NHUẬN =================
        [HttpGet]
        public ActionResult LoiNhuan(string tuNgay, string denNgay)
        {
            if (Session["UserRole"] == null || (Session["UserRole"].ToString() != "Admin" && Session["UserRole"].ToString() != "Quản lý"))
                return RedirectToAction("Index", "Home");

            DateTime startDate = DateTime.Now.AddDays(-30).Date;
            DateTime endDate = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(tuNgay)) DateTime.TryParseExact(tuNgay, "yyyy-MM-dd", null, DateTimeStyles.None, out startDate);
            if (!string.IsNullOrEmpty(denNgay)) DateTime.TryParseExact(denNgay, "yyyy-MM-dd", null, DateTimeStyles.None, out endDate);

            // RÀNG BUỘC: Nếu đến ngày vượt quá hôm nay, tự động khóa về ngày hôm nay
            if (endDate > DateTime.Now.Date)
            {
                endDate = DateTime.Now.Date;
            }
            endDate = endDate.AddDays(1).AddTicks(-1);

            var maDHs = db.DONHANGs.Where(d => d.NgayDatHang >= startDate && d.NgayDatHang <= endDate && d.TrangThai == "HoanThanh").Select(d => d.MaDH).ToList();
            var grouped = db.CT_DONHANG.Where(ct => maDHs.Contains(ct.MaDH)).GroupBy(ct => ct.MaSP).Select(g => new { MaSP = g.Key, SL = g.Sum(x => x.SoLuong), DT = g.Sum(x => x.SoLuong * x.DonGia) }).ToList();

            var list = grouped.Select(x => {
                var sp = db.SANPHAMs.FirstOrDefault(s => s.MaSP == x.MaSP);
                decimal giaNhap = sp?.GiaNhap ?? 0;
                return new SanPhamBanChayViewModel { MaSP = x.MaSP, TenSP = sp?.TenSP, GiaNhap = giaNhap, GiaBan = sp?.GiaBan ?? 0, SoLuongBan = x.SL, DoanhThu = x.DT, LoiNhuan = x.DT - (giaNhap * x.SL) };
            }).ToList();

            ViewBag.TuNgay = startDate.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = endDate.ToString("yyyy-MM-dd");
            return View(list);
        }

        // ================= 9. XUẤT PDF BIỂU MẪU 11 =================
        [HttpGet]
        public ActionResult XuatPdfLoiNhuan(string tuNgay, string denNgay)
        {
            DateTime startDate = DateTime.Now.AddDays(-30).Date;
            DateTime endDate = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(tuNgay)) DateTime.TryParseExact(tuNgay, "yyyy-MM-dd", null, DateTimeStyles.None, out startDate);
            if (!string.IsNullOrEmpty(denNgay)) DateTime.TryParseExact(denNgay, "yyyy-MM-dd", null, DateTimeStyles.None, out endDate);

            // RÀNG BUỘC: Nếu đến ngày vượt quá hôm nay, tự động khóa về ngày hôm nay
            if (endDate > DateTime.Now.Date)
            {
                endDate = DateTime.Now.Date;
            }
            endDate = endDate.AddDays(1).AddTicks(-1);

            var maDHs = db.DONHANGs.Where(d => d.NgayDatHang >= startDate && d.NgayDatHang <= endDate && d.TrangThai == "HoanThanh").Select(d => d.MaDH).ToList();
            var grouped = db.CT_DONHANG.Where(ct => maDHs.Contains(ct.MaDH)).GroupBy(ct => ct.MaSP).Select(g => new { MaSP = g.Key, SL = g.Sum(x => x.SoLuong), DT = g.Sum(x => x.SoLuong * x.DonGia) }).OrderByDescending(x => x.SL).ToList();

            var listLoiNhuan = grouped.Select(x => {
                var sp = db.SANPHAMs.FirstOrDefault(s => s.MaSP == x.MaSP);
                decimal giaNhap = sp?.GiaNhap ?? 0;
                return new SanPhamBanChayViewModel { MaSP = x.MaSP, TenSP = sp?.TenSP, GiaNhap = giaNhap, GiaBan = sp?.GiaBan ?? 0, SoLuongBan = x.SL, DoanhThu = x.DT, LoiNhuan = x.DT - (giaNhap * x.SL) };
            }).ToList();

            ViewBag.NguoiLap = Session["UserName"] != null ? Session["UserName"].ToString() : "..................................";
            ViewBag.TuNgay = startDate.ToString("dd/MM/yyyy");
            ViewBag.DenNgay = endDate.ToString("dd/MM/yyyy");

            return new ViewAsPdf("InBaoCaoLoiNhuan", listLoiNhuan)
            {
                FileName = "BieuMau11_BaoCaoLoiNhuan_" + DateTime.Now.ToString("ddMMyyyy") + ".pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                PageMargins = new Rotativa.Options.Margins(15, 15, 15, 15)
            };
        }
    }
}