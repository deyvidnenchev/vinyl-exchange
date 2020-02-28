﻿using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using VinylExchange.Data.Models;


namespace VinylExchange.Data
{
    public class VinylExchangeDbContext : KeyApiAuthorizationDbContext<VinylExchangeUser, VinylExchangeRole, Guid>
    {
        public VinylExchangeDbContext(
           DbContextOptions options,
           IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
            
        }

        public DbSet<Genre> Genres { get; set; }
        public DbSet<Style> Styles { get; set; }
        public DbSet<StyleRelease> StyleReleases { get; set; }
        public DbSet<Release> Releases { get; set; }
        public DbSet<ReleaseFile> ReleaseFiles { get; set; }
        public DbSet<ShopFile> ShopFiles { get; set; }
        public DbSet<CollectionItem> Collections { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Address> Addresses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Genre>()
                .HasMany(s => s.Styles)
                .WithOne(g => g.Genre)
                .HasForeignKey(g => g.GenreId);

            modelBuilder.Entity<StyleRelease>(styleRelease =>
            {
                styleRelease.HasKey(sr => new { sr.StyleId, sr.ReleaseId });

                styleRelease
                .HasOne(sr => sr.Style)
                .WithMany(s => s.Releases)
                .HasForeignKey(sr => sr.StyleId);

                styleRelease
                .HasOne(sr => sr.Release)
                .WithMany(r => r.Styles)
                .HasForeignKey(sr => sr.ReleaseId);

            });

            modelBuilder.Entity<ReleaseFile>()
                .HasOne(r => r.Release)
                .WithMany(rf => rf.ReleaseFiles)
                .HasForeignKey(r => r.ReleaseId);

            modelBuilder.Entity<ShopFile>()
               .HasOne(sf => sf.Shop)
               .WithMany(s => s.ShopFiles)
               .HasForeignKey(sf => sf.ShopId);

            modelBuilder.Entity<CollectionItem>(collectionItem =>
            {

                collectionItem
                .HasOne(ci => ci.User)
                .WithMany(u => u.Collection)
                .HasForeignKey(ci => ci.UserId);

                collectionItem
                .HasOne(ci => ci.Release)
                .WithMany(r => r.ReleaseCollections)
                .HasForeignKey(ci => ci.ReleaseId);
            });

            modelBuilder.Entity<Sale>(sale =>
            {
                             
                sale.HasOne(s => s.Buyer)
                    .WithMany(b => b.Purchases)
                    .HasForeignKey(s => s.BuyerId)
                    .OnDelete(DeleteBehavior.Restrict);

                sale.HasOne(s => s.Seller)
                    .WithMany(s => s.Sales)
                    .HasForeignKey(s => s.SellerId)
                    .OnDelete(DeleteBehavior.Restrict);

                sale.HasOne(s => s.Shop)
                .WithMany(s => s.Sales)
                .HasForeignKey(s => s.ShopId)
                 .OnDelete(DeleteBehavior.Restrict);

                sale.HasOne(s => s.Release)
                .WithMany(r => r.Sales)
                .HasForeignKey(s => s.ReleaseId);

            });

            modelBuilder.Entity<Address>()
                .HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId);

        }

    }
}
