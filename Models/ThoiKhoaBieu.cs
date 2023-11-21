namespace NhaKhoa.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ThoiKhoaBieu")]
    public partial class ThoiKhoaBieu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ThoiKhoaBieu()
        {
            PhieuDatLiches = new HashSet<PhieuDatLich>();
        }

        [Key]
        public int Id_TKB { get; set; }

        public int? Id_Thu { get; set; }

        public int? Id_Phong { get; set; }

        public int? Id_khunggio { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NgayLamViec { get; set; }

        [StringLength(128)]
        public string Id_Nhasi { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual KhungGio KhungGio { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PhieuDatLich> PhieuDatLiches { get; set; }

        public virtual Phong Phong { get; set; }

        public virtual Thu Thu { get; set; }
    }
}
