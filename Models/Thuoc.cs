namespace NhaKhoa.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Thuoc")]
    public partial class Thuoc
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Thuoc()
        {
            ChiTietThuocs = new HashSet<ChiTietThuoc>();
        }

        [Key]
        public int Id_thuoc { get; set; }

        public string Tenthuoc { get; set; }

        public string Mota { get; set; }

        public double? Gia { get; set; }

        public DateTime? NgaySX { get; set; }

        public DateTime? HanSD { get; set; }

        public int? Soluong { get; set; }

        public string Thanhphan { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietThuoc> ChiTietThuocs { get; set; }
    }
}
