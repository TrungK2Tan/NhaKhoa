namespace NhaKhoa.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DanhGia")]
    public partial class DanhGia
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DanhGia()
        {
            TinTucs = new HashSet<TinTuc>();
        }

        [Key]
        public int Id_danhgia { get; set; }

        public string Noidung { get; set; }

        public string Saodanhgia { get; set; }

        public DateTime? Ngaydanhgia { get; set; }

        [StringLength(128)]
        public string Id_Benhnhan { get; set; }

        [StringLength(128)]
        public string Id_Nhasi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TinTuc> TinTucs { get; set; }
    }
}
