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
        [Key]
        public int Id_dichvu { get; set; }

        public string Tendichvu { get; set; }

        public string Giadichvu { get; set; }

        public string Mota { get; set; }

        public string HinhAnh { get; set; }
    }
}
