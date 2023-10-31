namespace NhaKhoa.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TinTuc")]
    public partial class TinTuc
    {
        [Key]
        public int Id_tintuc { get; set; }

        public string Tieude { get; set; }

        public string Noidung { get; set; }

        public string Hinhanh { get; set; }

        public DateTime? Ngaygiotao { get; set; }

        [StringLength(128)]
        public string Id_admin { get; set; }

        [StringLength(128)]
        public string Id_Benhnhan { get; set; }

        public int? Thich { get; set; }

        public int? Id_danhgia { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual DanhGia DanhGia { get; set; }
    }
}
