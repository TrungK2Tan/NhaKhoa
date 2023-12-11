using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace NhaKhoa.Models
{
    public partial class NhaKhoaModel : DbContext
    {
        public NhaKhoaModel()
            : base("name=NhaKhoa")
        {
        }

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<ChiTietThuoc> ChiTietThuocs { get; set; }
        public virtual DbSet<DanhGia> DanhGias { get; set; }
        public virtual DbSet<DanhGiaNhaSi> DanhGiaNhaSis { get; set; }
        public virtual DbSet<DichVu> DichVus { get; set; }
        public virtual DbSet<DonThuoc> DonThuocs { get; set; }
        public virtual DbSet<HinhThucThanhToan> HinhThucThanhToans { get; set; }
        public virtual DbSet<KhungGio> KhungGios { get; set; }
        public virtual DbSet<PhieuDatLich> PhieuDatLiches { get; set; }
        public virtual DbSet<Phong> Phongs { get; set; }
        public virtual DbSet<ThoiKhoaBieu> ThoiKhoaBieux { get; set; }
        public virtual DbSet<Thu> Thus { get; set; }
        public virtual DbSet<Thuoc> Thuocs { get; set; }
        public virtual DbSet<TinTuc> TinTucs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.DanhGiaNhaSis)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.Id_Nhasi);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.PhieuDatLiches)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.IdBenhNhan);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.ThoiKhoaBieux)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.Id_Nhasi);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.TinTucs)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.Id_admin);

            modelBuilder.Entity<DonThuoc>()
                .HasMany(e => e.ChiTietThuocs)
                .WithRequired(e => e.DonThuoc)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Thuoc>()
                .HasMany(e => e.ChiTietThuocs)
                .WithRequired(e => e.Thuoc)
                .WillCascadeOnDelete(false);
        }
    }
}
