namespace NhaKhoa.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HoaDon")]
    public partial class HoaDon
    {
        [Key]
        public int Id_hoadon { get; set; }

        public DateTime? NgayGio { get; set; }

        public double? TongTien { get; set; }

        public int? Id_donthuoc { get; set; }

        public int? id_hinhthuc { get; set; }

        [StringLength(128)]
        public string Id_benhnhan { get; set; }

        [StringLength(128)]
        public string Id_bacsi { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual DonThuoc DonThuoc { get; set; }

        public virtual HinhThucThanhToan HinhThucThanhToan { get; set; }
    }
}
