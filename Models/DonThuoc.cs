namespace NhaKhoa.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DonThuoc")]
    public partial class DonThuoc
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DonThuoc()
        {
            HoaDons = new HashSet<HoaDon>();
            Thuocs = new HashSet<Thuoc>();
        }

        [Key]
        public int Id_donthuoc { get; set; }

        public string Mota { get; set; }

        public int? Soluong { get; set; }

        public int? Id_thuoc { get; set; }

        public int? Id_phieudat { get; set; }

        public string Chandoan { get; set; }

        public virtual PhieuDatLich PhieuDatLich { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HoaDon> HoaDons { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Thuoc> Thuocs { get; set; }
    }
}
