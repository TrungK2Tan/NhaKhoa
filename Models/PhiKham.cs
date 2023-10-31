namespace NhaKhoa.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PhiKham")]
    public partial class PhiKham
    {
        [Key]
        [Column(Order = 0)]
        public string Id_benhnhan { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id_phieudat { get; set; }

        public double? ChiPhiKham { get; set; }

        public int? Id_hinhthuc { get; set; }

        public bool? Trangthai { get; set; }

        public virtual HinhThucThanhToan HinhThucThanhToan { get; set; }

        public virtual PhieuDatLich PhieuDatLich { get; set; }
    }
}
