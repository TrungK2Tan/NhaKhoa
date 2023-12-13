using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NhaKhoa.Models
{
    public class LapDonThuocViewModel
    {
        public DonThuoc DonThuoc { get; set; }
        public List<ThuocCheckBox> Thuocs { get; set; }
        public string SelectedMedicines { get; set; }
    }

}