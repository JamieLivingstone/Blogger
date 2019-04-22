// <auto-generated />
using System;
using Blogger.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Blogger.Infrastructure.Migrations {
    [DbContext (typeof (ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot {
        protected override void BuildModel (ModelBuilder modelBuilder) {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation ("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation ("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation ("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity ("Blogger.Core.Entities.ApplicationUser", b => {
                b.Property<string> ("Id")
                    .ValueGeneratedOnAdd ();

                b.Property<int> ("AccessFailedCount");

                b.Property<string> ("Bio");

                b.Property<string> ("ConcurrencyStamp")
                    .IsConcurrencyToken ();

                b.Property<string> ("Email")
                    .HasMaxLength (256);

                b.Property<bool> ("EmailConfirmed");

                b.Property<string> ("Image");

                b.Property<bool> ("LockoutEnabled");

                b.Property<DateTimeOffset?> ("LockoutEnd");

                b.Property<string> ("NormalizedEmail")
                    .HasMaxLength (256);

                b.Property<string> ("NormalizedUserName")
                    .HasMaxLength (256);

                b.Property<string> ("PasswordHash");

                b.Property<string> ("PhoneNumber");

                b.Property<bool> ("PhoneNumberConfirmed");

                b.Property<string> ("SecurityStamp");

                b.Property<bool> ("TwoFactorEnabled");

                b.Property<string> ("UserName")
                    .HasMaxLength (256);

                b.HasKey ("Id");

                b.HasIndex ("NormalizedEmail")
                    .HasName ("EmailIndex");

                b.HasIndex ("NormalizedUserName")
                    .IsUnique ()
                    .HasName ("UserNameIndex");

                b.ToTable ("AspNetUsers");
            });

            modelBuilder.Entity ("Blogger.Core.Entities.Article", b => {
                b.Property<int> ("Id")
                    .ValueGeneratedOnAdd ();

                b.Property<string> ("AuthorId");

                b.Property<string> ("Body");

                b.Property<DateTime> ("CreatedAt");

                b.Property<string> ("Description");

                b.Property<string> ("Slug");

                b.Property<string> ("Title");

                b.Property<DateTime> ("UpdatedAt");

                b.HasKey ("Id");

                b.HasIndex ("AuthorId");

                b.ToTable ("Articles");
            });

            modelBuilder.Entity ("Blogger.Core.Entities.Comment", b => {
                b.Property<int> ("Id")
                    .ValueGeneratedOnAdd ();

                b.Property<int> ("ArticleId");

                b.Property<string> ("AuthorId");

                b.Property<string> ("Body");

                b.Property<DateTime> ("CreatedAt");

                b.Property<DateTime> ("UpdatedAt");

                b.HasKey ("Id");

                b.HasIndex ("ArticleId");

                b.HasIndex ("AuthorId");

                b.ToTable ("Comments");
            });

            modelBuilder.Entity ("Blogger.Core.Entities.Follower", b => {
                b.Property<string> ("TargetId");

                b.Property<string> ("ObserverId");

                b.Property<int> ("Id");

                b.HasKey ("TargetId", "ObserverId");

                b.HasIndex ("ObserverId");

                b.ToTable ("Followers");
            });

            modelBuilder.Entity ("Microsoft.AspNetCore.Identity.IdentityRole", b => {
                b.Property<string> ("Id")
                    .ValueGeneratedOnAdd ();

                b.Property<string> ("ConcurrencyStamp")
                    .IsConcurrencyToken ();

                b.Property<string> ("Name")
                    .HasMaxLength (256);

                b.Property<string> ("NormalizedName")
                    .HasMaxLength (256);

                b.HasKey ("Id");

                b.HasIndex ("NormalizedName")
                    .IsUnique ()
                    .HasName ("RoleNameIndex");

                b.ToTable ("AspNetRoles");
            });

            modelBuilder.Entity ("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b => {
                b.Property<int> ("Id")
                    .ValueGeneratedOnAdd ();

                b.Property<string> ("ClaimType");

                b.Property<string> ("ClaimValue");

                b.Property<string> ("RoleId")
                    .IsRequired ();

                b.HasKey ("Id");

                b.HasIndex ("RoleId");

                b.ToTable ("AspNetRoleClaims");
            });

            modelBuilder.Entity ("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b => {
                b.Property<int> ("Id")
                    .ValueGeneratedOnAdd ();

                b.Property<string> ("ClaimType");

                b.Property<string> ("ClaimValue");

                b.Property<string> ("UserId")
                    .IsRequired ();

                b.HasKey ("Id");

                b.HasIndex ("UserId");

                b.ToTable ("AspNetUserClaims");
            });

            modelBuilder.Entity ("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b => {
                b.Property<string> ("LoginProvider");

                b.Property<string> ("ProviderKey");

                b.Property<string> ("ProviderDisplayName");

                b.Property<string> ("UserId")
                    .IsRequired ();

                b.HasKey ("LoginProvider", "ProviderKey");

                b.HasIndex ("UserId");

                b.ToTable ("AspNetUserLogins");
            });

            modelBuilder.Entity ("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b => {
                b.Property<string> ("UserId");

                b.Property<string> ("RoleId");

                b.HasKey ("UserId", "RoleId");

                b.HasIndex ("RoleId");

                b.ToTable ("AspNetUserRoles");
            });

            modelBuilder.Entity ("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b => {
                b.Property<string> ("UserId");

                b.Property<string> ("LoginProvider");

                b.Property<string> ("Name");

                b.Property<string> ("Value");

                b.HasKey ("UserId", "LoginProvider", "Name");

                b.ToTable ("AspNetUserTokens");
            });

            modelBuilder.Entity ("Blogger.Core.Entities.Article", b => {
                b.HasOne ("Blogger.Core.Entities.ApplicationUser", "Author")
                    .WithMany ("Articles")
                    .HasForeignKey ("AuthorId")
                    .OnDelete (DeleteBehavior.Cascade);
            });

            modelBuilder.Entity ("Blogger.Core.Entities.Comment", b => {
                b.HasOne ("Blogger.Core.Entities.Article", "Article")
                    .WithMany ("Comments")
                    .HasForeignKey ("ArticleId")
                    .OnDelete (DeleteBehavior.Cascade);

                b.HasOne ("Blogger.Core.Entities.ApplicationUser", "Author")
                    .WithMany ()
                    .HasForeignKey ("AuthorId");
            });

            modelBuilder.Entity ("Blogger.Core.Entities.Follower", b => {
                b.HasOne ("Blogger.Core.Entities.ApplicationUser", "Observer")
                    .WithMany ()
                    .HasForeignKey ("ObserverId")
                    .OnDelete (DeleteBehavior.Cascade);

                b.HasOne ("Blogger.Core.Entities.ApplicationUser", "Target")
                    .WithMany ()
                    .HasForeignKey ("TargetId")
                    .OnDelete (DeleteBehavior.Cascade);
            });

            modelBuilder.Entity ("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b => {
                b.HasOne ("Microsoft.AspNetCore.Identity.IdentityRole")
                    .WithMany ()
                    .HasForeignKey ("RoleId")
                    .OnDelete (DeleteBehavior.Cascade);
            });

            modelBuilder.Entity ("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b => {
                b.HasOne ("Blogger.Core.Entities.ApplicationUser")
                    .WithMany ()
                    .HasForeignKey ("UserId")
                    .OnDelete (DeleteBehavior.Cascade);
            });

            modelBuilder.Entity ("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b => {
                b.HasOne ("Blogger.Core.Entities.ApplicationUser")
                    .WithMany ()
                    .HasForeignKey ("UserId")
                    .OnDelete (DeleteBehavior.Cascade);
            });

            modelBuilder.Entity ("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b => {
                b.HasOne ("Microsoft.AspNetCore.Identity.IdentityRole")
                    .WithMany ()
                    .HasForeignKey ("RoleId")
                    .OnDelete (DeleteBehavior.Cascade);

                b.HasOne ("Blogger.Core.Entities.ApplicationUser")
                    .WithMany ()
                    .HasForeignKey ("UserId")
                    .OnDelete (DeleteBehavior.Cascade);
            });

            modelBuilder.Entity ("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b => {
                b.HasOne ("Blogger.Core.Entities.ApplicationUser")
                    .WithMany ()
                    .HasForeignKey ("UserId")
                    .OnDelete (DeleteBehavior.Cascade);
            });
#pragma warning restore 612, 618
        }
    }
}