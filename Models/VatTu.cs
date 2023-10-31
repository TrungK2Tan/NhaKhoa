namespace NhaKhoa.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("VatTu")]
    public partial class VatTu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VatTu()
        {
            VatTuSuDungs = new HashSet<VatTuSuDung>();
        }

        [Key]
        public int Id_Vattu { get; set; }

        public string TenVatTu { get; set; }

        public string Motavattu { get; set; }

        public double? Giavattu { get; set; }

        public DateTime? NgaySX { get; set; }

        public DateTime? HanSD { get; set; }

        public string Chatlieu { get; set; }

        public int? Soluong { get; set; }

        [StringLength(128)]
        public string Id_nhanvien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VatTuSuDung> VatTuSuDungs { get; set; }
    }
}
