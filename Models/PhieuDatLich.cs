namespace NhaKhoa.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PhieuDatLich")]
    public partial class PhieuDatLich
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PhieuDatLich()
        {
            LichKhams = new HashSet<LichKham>();
            PhiKhams = new HashSet<PhiKham>();
        }

        [Key]
        public int Id_Phieudat { get; set; }

        public DateTime? NgayKham { get; set; }

        public double? Gia { get; set; }

        public int? Id_hinhthuc { get; set; }

        [StringLength(128)]
        public string IdNhaSi { get; set; }

        [StringLength(128)]
        public string IdBenhNhan { get; set; }

        public int? Id_kTKB { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LichKham> LichKhams { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PhiKham> PhiKhams { get; set; }

        public virtual ThoiKhoaBieu ThoiKhoaBieu { get; set; }
    }
}
