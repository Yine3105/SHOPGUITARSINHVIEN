using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SHOPGUITAR.Models
{
    public class DoiMatKhau
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại.")]
        [DataType(DataType.Password)]
        public string MatKhauCu { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string MatKhauMoi { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới.")]
        [DataType(DataType.Password)]
        [Compare("MatKhauMoi", ErrorMessage = "Mật khẩu xác nhận không trùng khớp.")]
        public string XacNhanMatKhau { get; set; }
    }
}