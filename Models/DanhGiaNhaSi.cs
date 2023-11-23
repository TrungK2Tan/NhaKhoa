namespace NhaKhoa.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DanhGiaNhaSi")]
    public partial class DanhGiaNhaSi
    {
        [Key]
        public int Id_danhgianhasi { get; set; }

        public string Noidung { get; set; }

        public double? SaoDanhGia { get; set; }

        public DateTime? NgayDanhGia { get; set; }

        [StringLength(128)]
        public string Id_Benhnhan { get; set; }

        [StringLength(128)]
        public string Id_Nhasi { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }
    }
}
