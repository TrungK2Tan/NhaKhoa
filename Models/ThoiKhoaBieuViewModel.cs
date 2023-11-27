using System;
using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NhaKhoa.Models
{
    public class ThoiKhoaBieuViewModel
    {
        public List<Thu> DanhSachThu { get; set; }
        public List<ThoiKhoaBieu> DanhSachThoiKhoaBieu { get; set; }
        public DateTime[] weeks { get; set; }
        public DateTime SelectedWeek { get; set; }

        public List<ThoiKhoaBieu> WorkSchedules { get; set; }
    }
}