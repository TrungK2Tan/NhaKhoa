namespace NhaKhoa.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("VatTuSuDung")]
    public partial class VatTuSuDung
    {
        [Key]
        [Column(Order = 0)]
        public int Id_dichvu { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id_Vattu { get; set; }

        public int? Soluong { get; set; }

        public double? Gia { get; set; }

        public virtual DichVu DichVu { get; set; }

        public virtual HoaDon HoaDon { get; set; }

        public virtual HoaDon HoaDon1 { get; set; }

        public virtual VatTu VatTu { get; set; }
    }
}
