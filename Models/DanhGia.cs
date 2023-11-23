namespace NhaKhoa.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DanhGia")]
    public partial class DanhGia
    {
        [Key]
        public int Id_danhgia { get; set; }

        public string Noidung { get; set; }

        public double? Saodanhgia { get; set; }

        public DateTime? Ngaydanhgia { get; set; }

        [StringLength(128)]
        public string Id_Benhnhan { get; set; }

        public int? Id_tintuc { get; set; }

        public virtual TinTuc TinTuc { get; set; }
    }
}
