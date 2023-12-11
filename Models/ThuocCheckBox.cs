using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NhaKhoa.Models
{
    // Add this class to your code
    public class ThuocCheckBox
    {
        public int Id_thuoc { get; set; }
        public string Tenthuoc { get; set; }
        public bool Selected { get; set; }
        public int SoLuong { get; set; } // Add this property for quantity
    }


}