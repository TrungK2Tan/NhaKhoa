namespace NhaKhoa.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ThoiKhoaBieu")]
    public partial class ThoiKhoaBieu
    {
        [Key]
        public int Id_TKB { get; set; }

        public string TenTKB { get; set; }

        public int? Id_Phong { get; set; }

        [StringLength(128)]
        public string Id_Nhasi { get; set; }

        public int? Id_khunggio { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual KhungGio KhungGio { get; set; }

        public virtual PhieuDatLich PhieuDatLich { get; set; }

        public virtual Phong Phong { get; set; }
    }
}
