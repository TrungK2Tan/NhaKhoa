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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TinTuc()
        {
            DanhGias = new HashSet<DanhGia>();
        }

        [Key]
        public int Id_tintuc { get; set; }

        public string Tieude { get; set; }

        public string Noidung { get; set; }

        public string Hinhanh { get; set; }

        public DateTime? Ngaygiotao { get; set; }

        [StringLength(128)]
        public string Id_admin { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DanhGia> DanhGias { get; set; }
    }
}
