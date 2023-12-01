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
            DonThuocs = new HashSet<DonThuoc>();
        }

        [Key]
        public int Id_Phieudat { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NgayKham { get; set; }

        public double? Gia { get; set; }

        public int? Id_hinhthuc { get; set; }

        [StringLength(128)]
        public string IdNhaSi { get; set; }

        [StringLength(128)]
        public string IdBenhNhan { get; set; }

        public int? Id_TKB { get; set; }

        public int? STT { get; set; }

        public bool? TrangThai { get; set; }

        public bool? TrangThaiThanhToan { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DonThuoc> DonThuocs { get; set; }

        public virtual HinhThucThanhToan HinhThucThanhToan { get; set; }

        public virtual ThoiKhoaBieu ThoiKhoaBieu { get; set; }
    }
}
