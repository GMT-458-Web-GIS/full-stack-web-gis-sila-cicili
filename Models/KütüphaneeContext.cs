using System;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Models; // ⚠️ BU ÇOK ÖNEMLİ
using NetTopologySuite.Geometries; // ⚠️ BU DA ÇOK ÖNEMLİ

namespace LibrarySystem.Models
{
    public partial class KütüphaneeContext : DbContext
    {
        public KütüphaneeContext(DbContextOptions<KütüphaneeContext> options)
            : base(options)
        {
        }

        // Tabloların
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Loan> Loans { get; set; }
        public virtual DbSet<Request> Requests { get; set; }
        public virtual DbSet<User> Users { get; set; }
        
        // Yeni Tablo
        public virtual DbSet<LibraryBranch> LibraryBranches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- HARİTA AYARI ---
            modelBuilder.Entity<LibraryBranch>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Location).HasColumnType("geometry(Point, 4326)");
            });

            // --- MEVCUT TABLO AYARLARI (Dokunma) ---
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.BookId).HasName("books_pkey");
                entity.ToTable("books");
                entity.Property(e => e.BookId).HasColumnName("book_id");
                entity.Property(e => e.Author).HasMaxLength(100).HasColumnName("author");
                entity.Property(e => e.Category).HasMaxLength(50).HasColumnName("category");
                entity.Property(e => e.CurrentStock).HasDefaultValue(1).HasColumnName("current_stock");
                entity.Property(e => e.DateAdded).HasDefaultValueSql("CURRENT_TIMESTAMP").HasColumnName("date_added");
                entity.Property(e => e.Title).HasMaxLength(200).HasColumnName("title");
                entity.Property(e => e.TotalStock).HasDefaultValue(1).HasColumnName("total_stock");
            });

            modelBuilder.Entity<Loan>(entity =>
            {
                entity.HasKey(e => e.LoanId).HasName("loans_pkey");
                entity.ToTable("loans");
                entity.Property(e => e.LoanId).HasColumnName("loan_id");
                entity.Property(e => e.BookId).HasColumnName("book_id");
                entity.Property(e => e.BorrowDate).HasDefaultValueSql("CURRENT_DATE").HasColumnName("borrow_date");
                entity.Property(e => e.DueDate).HasColumnName("due_date");
                entity.Property(e => e.ReturnDate).HasColumnName("return_date");
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValueSql("'active'::character varying").HasColumnName("status");
                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Book).WithMany(p => p.Loans).HasForeignKey(d => d.BookId);
                entity.HasOne(d => d.User).WithMany(p => p.Loans).HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("users_pkey");
                entity.ToTable("users");
                entity.HasIndex(e => e.Email, "users_email_key").IsUnique();
                entity.HasIndex(e => e.Username, "users_username_key").IsUnique();
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.AccountStatus).HasMaxLength(20).HasDefaultValueSql("'active'::character varying").HasColumnName("account_status");
                entity.Property(e => e.Address).HasColumnName("address");
                entity.Property(e => e.Email).HasMaxLength(100).HasColumnName("email");
                entity.Property(e => e.FirstName).HasMaxLength(30).HasColumnName("first_name");
                entity.Property(e => e.LastName).HasMaxLength(30).HasColumnName("last_name");
                entity.Property(e => e.PasswordHash).HasMaxLength(255).HasColumnName("password_hash");
                entity.Property(e => e.Phone).HasMaxLength(15).HasColumnName("phone");
                entity.Property(e => e.RegistrationDate).HasDefaultValueSql("CURRENT_TIMESTAMP").HasColumnName("registration_date");
                entity.Property(e => e.Role).HasMaxLength(20).HasDefaultValueSql("'student'::character varying").HasColumnName("role");
                entity.Property(e => e.Username).HasMaxLength(30).HasColumnName("username");
            });
            
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}