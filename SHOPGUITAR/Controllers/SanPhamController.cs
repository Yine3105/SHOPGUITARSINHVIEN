using SHOPGUITAR.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SHOPGUITAR.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly SHOPGUITARSINHVIENEntities _context;

        public SanPhamController()
        {
            _context = new SHOPGUITARSINHVIENEntities();
        }

        // GET: SanPham
        public async Task<ActionResult> Index()
        {
            var sanPhams = await _context.SANPHAMs.ToListAsync();

            return View(sanPhams);
        }

        // GET: SanPham/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
                return HttpNotFound();

            var sanPham = await _context.SANPHAMs.FindAsync(id);

            if (sanPham == null)
                return HttpNotFound();

            return View(sanPham);
        }

        // POST: SanPham/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(
            string id,
            SANPHAM sanPham,
            HttpPostedFileBase fileUpload
        )
        {
            if (id != sanPham.MaSP)
                return HttpNotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Upload ảnh mới
                    if (fileUpload != null && fileUpload.ContentLength > 0)
                    {
                        var folder = Server.MapPath("~/images/products");

                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }

                        // Xóa ảnh cũ
                        if (!string.IsNullOrEmpty(sanPham.AnhSP))
                        {
                            var oldPath = Path.Combine(folder, sanPham.AnhSP);

                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }

                        // Tạo tên file mới
                        var extension = Path.GetExtension(fileUpload.FileName);

                        var fileName = Guid.NewGuid().ToString() + extension;

                        var savePath = Path.Combine(folder, fileName);

                        // Lưu file
                        fileUpload.SaveAs(savePath);

                        // Lưu tên vào DB
                        sanPham.AnhSP = fileName;
                    }

                    _context.Entry(sanPham).State = EntityState.Modified;

                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Cập nhật sản phẩm thành công!";

                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }

            return View(sanPham);
        }
        // GET: SanPham/Delete/SP001
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
                return HttpNotFound();

            var sanPham = await _context.SANPHAMs.FindAsync(id);

            if (sanPham == null)
                return HttpNotFound();

            return View(sanPham);
        }
        // POST: SanPham/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var sanPham = await _context.SANPHAMs
                                        .Include(x => x.KHUYENMAIs)
                                        .FirstOrDefaultAsync(x => x.MaSP == id);

            if (sanPham == null)
                return HttpNotFound();

            // Kiểm tra sản phẩm có trong đơn hàng không
            bool daCoDonHang = await _context.CT_DONHANG
                                             .AnyAsync(x => x.MaSP == id);

            if (daCoDonHang)
            {
                TempData["Error"] = "❌ Sản phẩm đang tồn tại trong đơn hàng nên không thể xóa!";
                return RedirectToAction("Index");
            }

            try
            {
                // Xóa liên kết khuyến mãi
                sanPham.KHUYENMAIs.Clear();

                _context.SANPHAMs.Remove(sanPham);

                await _context.SaveChangesAsync();

                TempData["Success"] = "✅ Xóa sản phẩm thành công!";
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                // Bắt lỗi FK constraint (ví dụ vẫn còn trong CT_DONHANG hoặc bảng khác)
                var innerEx = ex.InnerException?.InnerException?.Message ?? ex.Message;

                if (innerEx.Contains("FK") || innerEx.Contains("REFERENCE") || innerEx.Contains("FOREIGN KEY"))
                {
                    TempData["Error"] = "❌ Sản phẩm đang được sử dụng trong dữ liệu khác, không thể xóa!";
                }
                else
                {
                    TempData["Error"] = "❌ Lỗi khi xóa sản phẩm: " + innerEx;
                }
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}