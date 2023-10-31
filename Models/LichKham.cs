namespace NhaKhoa.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LichKham")]
    public partial class LichKham
    {
        [Key]
        public int Id_lichkham { get; set; }

        public int? STT { get; set; }

        public bool? TrangThai { get; set; }

        public int? Id_phieudat { get; set; }

        [Required]
        [StringLength(128)]
        public string Id_Benhnhan { get; set; }

        [StringLength(128)]
        public string Id_Nhasi { get; set; }

        public int? Id_Phong { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual PhieuDatLich PhieuDatLich { get; set; }
    }
}
