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
            DonThuocs = new HashSet<DonThuoc>();
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

        [StringLength(128)]
        public string Id_nhanvien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DonThuoc> DonThuocs { get; set; }
    }
}
