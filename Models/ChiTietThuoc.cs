namespace NhaKhoa.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ChiTietThuoc")]
    public partial class ChiTietThuoc
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id_thuoc { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id_donthuoc { get; set; }

        public int? soluong { get; set; }

        public double? Gia { get; set; }

        public virtual DonThuoc DonThuoc { get; set; }

        public virtual Thuoc Thuoc { get; set; }
    }
}
