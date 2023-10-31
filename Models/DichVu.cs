namespace NhaKhoa.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DichVu")]
    public partial class DichVu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DichVu()
        {
            VatTuSuDungs = new HashSet<VatTuSuDung>();
        }

        [Key]
        public int Id_dichvu { get; set; }

        public string Tendichvu { get; set; }

        public string Giadichvu { get; set; }

        public string Mota { get; set; }

        public int? Id_Vattu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VatTuSuDung> VatTuSuDungs { get; set; }
    }
}
