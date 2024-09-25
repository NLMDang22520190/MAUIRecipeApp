using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MAUIRecipeApp.Models;

public partial class RecipeDbContext : DbContext
{
    public RecipeDbContext()
    {
    }

    public RecipeDbContext(DbContextOptions<RecipeDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FoodRating> FoodRatings { get; set; }

    public virtual DbSet<FoodRecipe> FoodRecipes { get; set; }

    public virtual DbSet<FoodRecipeType> FoodRecipeTypes { get; set; }

    public virtual DbSet<FoodRecipeTypeMapping> FoodRecipeTypeMappings { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<RecipeIngredient> RecipeIngredients { get; set; }

    public virtual DbSet<RecipeVideo> RecipeVideos { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserDetail> UserDetails { get; set; }

    public virtual DbSet<UserSavedRecipe> UserSavedRecipes { get; set; }


    string connectionString = MauiProgram.CreateMauiApp().Configuration.GetConnectionString("DefaultConnection");

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(connectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FoodRating>(entity =>
        {
            entity.HasKey(e => e.Rid).HasName("PK__FoodRati__CAFF413255E19591");

            entity.Property(e => e.Rid).HasColumnName("RID");
            entity.Property(e => e.DateRated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Frid).HasColumnName("FRID");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("isDeleted");
            entity.Property(e => e.Uid).HasColumnName("UID");

            //entity.HasOne(d => d.Fr).WithMany(p => p.FoodRatings)
            //    .HasForeignKey(d => d.Frid)
            //    .HasConstraintName("FK__FoodRating__FRID__7C4F7684");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.FoodRatings)
                .HasForeignKey(d => d.Uid)
                .HasConstraintName("FK__FoodRatings__UID__7B5B524B");
        });

        modelBuilder.Entity<FoodRecipe>(entity =>
        {
            entity.HasKey(e => e.Frid).HasName("PK__FoodReci__9D11117F0D00EFF1");

            entity.Property(e => e.Frid).HasColumnName("FRID");
            entity.Property(e => e.DifficultyLevel).HasMaxLength(50);
            entity.Property(e => e.HealthBenefits).HasMaxLength(255);
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("isDeleted");
            entity.Property(e => e.RecipeName).HasMaxLength(255);
            entity.Property(e => e.UploaderUid).HasColumnName("UploaderUID");

            //entity.HasOne(d => d.UploaderU).WithMany(p => p.FoodRecipes)
            //    .HasForeignKey(d => d.UploaderUid)
            //    .HasConstraintName("FK__FoodRecip__Uploa__5FB337D6");
        });

        modelBuilder.Entity<FoodRecipeType>(entity =>
        {
            entity.HasKey(e => e.Tofid).HasName("PK__FoodReci__07FC9C7E7A407921");

            entity.Property(e => e.Tofid).HasColumnName("TOFID");
            entity.Property(e => e.FoodTypeName).HasMaxLength(100);
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("isDeleted");
        });

        modelBuilder.Entity<FoodRecipeTypeMapping>(entity =>
        {
            entity.HasKey(e => new { e.Frid, e.Tofid }).HasName("PK__FoodReci__6D6ED8B8B232C0AE");

            entity.Property(e => e.Frid).HasColumnName("FRID");
            entity.Property(e => e.Tofid).HasColumnName("TOFID");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("isDeleted");

            //entity.HasOne(d => d.Fr).WithMany(p => p.FoodRecipeTypeMappings)
            //    .HasForeignKey(d => d.Frid)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK__FoodRecipe__FRID__6EF57B66");

            //entity.HasOne(d => d.Tof).WithMany(p => p.FoodRecipeTypeMappings)
            //    .HasForeignKey(d => d.Tofid)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
                //.HasConstraintName("FK__FoodRecip__TOFID__6FE99F9F");
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.Iid).HasName("PK__Ingredie__C4972B4C6465573C");

            entity.Property(e => e.Iid).HasColumnName("IID");
            entity.Property(e => e.IngredientName).HasMaxLength(255);
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("isDeleted");
            entity.Property(e => e.MeasurementUnit).HasMaxLength(50);
        });

        modelBuilder.Entity<RecipeIngredient>(entity =>
        {
            entity.HasKey(e => new { e.Frid, e.Iid }).HasName("PK__RecipeIn__415863CB3319248C");

            entity.Property(e => e.Frid).HasColumnName("FRID");
            entity.Property(e => e.Iid).HasColumnName("IID");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("isDeleted");
            entity.Property(e => e.Quantity).HasColumnType("decimal(10, 2)");

            //entity.HasOne(d => d.Fr).WithMany(p => p.RecipeIngredients)
            //    .HasForeignKey(d => d.Frid)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK__RecipeIngr__FRID__76969D2E");

            //entity.HasOne(d => d.IidNavigation).WithMany(p => p.RecipeIngredients)
            //    .HasForeignKey(d => d.Iid)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK__RecipeIngre__IID__778AC167");
        });

        modelBuilder.Entity<RecipeVideo>(entity =>
        {
            entity.HasKey(e => e.Vid).HasName("PK__RecipeVi__C5DF22BB54DA14C1");

            entity.Property(e => e.Vid).HasColumnName("VID");
            entity.Property(e => e.Frid).HasColumnName("FRID");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("isDeleted");
            entity.Property(e => e.VideoUrl)
                .HasMaxLength(255)
                .HasColumnName("VideoURL");

            //entity.HasOne(d => d.Fr).WithMany(p => p.RecipeVideos)
            //    .HasForeignKey(d => d.Frid)
            //    .HasConstraintName("FK__RecipeVide__FRID__02084FDA");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Uid).HasName("PK__Users__C5B19602BD48CDDD");

            entity.Property(e => e.Uid).HasColumnName("UID");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("isDeleted");
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Provider).HasMaxLength(100);
            entity.Property(e => e.UserType).HasMaxLength(50);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<UserDetail>(entity =>
        {
            entity.HasKey(e => e.Udid).HasName("PK__UserDeta__B1A79DB924A6E9FC");

            entity.Property(e => e.Udid).HasColumnName("UDID");
            entity.Property(e => e.Allergies).HasMaxLength(255);
            entity.Property(e => e.HealthCondition).HasMaxLength(255);
            entity.Property(e => e.Height).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("isDeleted");
            entity.Property(e => e.Uid).HasColumnName("UID");
            entity.Property(e => e.Weight).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.UserDetails)
                .HasForeignKey(d => d.Uid)
                .HasConstraintName("FK__UserDetails__UID__68487DD7");
        });

        modelBuilder.Entity<UserSavedRecipe>(entity =>
        {
            entity.HasKey(e => new { e.Uid, e.Frid }).HasName("PK__UserSave__2C608715414E2E08");

            entity.Property(e => e.Uid).HasColumnName("UID");
            entity.Property(e => e.Frid).HasColumnName("FRID");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("isDeleted");

            //entity.HasOne(d => d.Fr).WithMany(p => p.UserSavedRecipes)
            //    .HasForeignKey(d => d.Frid)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK__UserSavedR__FRID__6477ECF3");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.UserSavedRecipes)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserSavedRe__UID__6383C8BA");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
