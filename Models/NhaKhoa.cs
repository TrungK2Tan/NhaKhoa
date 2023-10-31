using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace NhaKhoa.Models
{
    public partial class NhaKhoa : DbContext
    {
        public NhaKhoa()
            : base("name=NhaKhoa")
        {
        }

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<DanhGia> DanhGias { get; set; }
        public virtual DbSet<DichVu> DichVus { get; set; }
        public virtual DbSet<DonThuoc> DonThuocs { get; set; }
        public virtual DbSet<HinhThucThanhToan> HinhThucThanhToans { get; set; }
        public virtual DbSet<HoaDon> HoaDons { get; set; }
        public virtual DbSet<KhungGio> KhungGios { get; set; }
        public virtual DbSet<LichKham> LichKhams { get; set; }
        public virtual DbSet<PhieuDatLich> PhieuDatLiches { get; set; }
        public virtual DbSet<PhiKham> PhiKhams { get; set; }
        public virtual DbSet<Phong> Phongs { get; set; }
        public virtual DbSet<TinTuc> TinTucs { get; set; }
        public virtual DbSet<Thuoc> Thuocs { get; set; }
        public virtual DbSet<VatTu> VatTus { get; set; }
        public virtual DbSet<VatTuSuDung> VatTuSuDungs { get; set; }
        public virtual DbSet<ThoiKhoaBieu> ThoiKhoaBieux { get; set; }

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
                .HasMany(e => e.DonThuocs)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.Id_bacsi);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.HoaDons)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.Id_benhnhan);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.LichKhams)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.Id_Nhasi);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.PhieuDatLiches)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.IdBenhNhan);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.TinTucs)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.Id_admin);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.ThoiKhoaBieux)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.Id_Nhasi);

            modelBuilder.Entity<DichVu>()
                .HasMany(e => e.VatTuSuDungs)
                .WithRequired(e => e.DichVu)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DonThuoc>()
                .HasMany(e => e.Thuocs)
                .WithMany(e => e.DonThuocs)
                .Map(m => m.ToTable("ChiTietThuoc").MapLeftKey("Id_donthuoc").MapRightKey("Id_thuoc"));

            modelBuilder.Entity<HoaDon>()
                .HasMany(e => e.VatTuSuDungs)
                .WithRequired(e => e.HoaDon)
                .HasForeignKey(e => e.Id_dichvu)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HoaDon>()
                .HasMany(e => e.VatTuSuDungs1)
                .WithRequired(e => e.HoaDon1)
                .HasForeignKey(e => e.Id_Vattu)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PhieuDatLich>()
                .HasMany(e => e.PhiKhams)
                .WithRequired(e => e.PhieuDatLich)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PhieuDatLich>()
                .HasOptional(e => e.ThoiKhoaBieu)
                .WithRequired(e => e.PhieuDatLich);

            modelBuilder.Entity<VatTu>()
                .HasMany(e => e.VatTuSuDungs)
                .WithRequired(e => e.VatTu)
                .WillCascadeOnDelete(false);
        }
    }
}
