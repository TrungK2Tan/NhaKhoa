namespace NhaKhoa.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Phong")]
    public partial class Phong
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Phong()
        {
            ThoiKhoaBieux = new HashSet<ThoiKhoaBieu>();
        }

        [Key]
        public int Id_Phong { get; set; }

        public string TenPhong { get; set; }

        public string HinhAnh { get; set; }

        public string Mota { get; set; }

        public bool? Trangthai { get; set; }

        [StringLength(128)]
        public string Id_NhaSi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ThoiKhoaBieu> ThoiKhoaBieux { get; set; }
    }
}
