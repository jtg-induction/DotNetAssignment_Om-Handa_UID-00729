using DotNet_Assignment.Models.Entities;
using System.Data.Entity;

namespace DotNet_Assignment.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext() : base("name= RestaurantPortal") { }

        public DbSet<User> Users { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderedItem> OrderedItems { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<RestaurantOwner> RestaurantsOwner { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAddress>()
                .HasRequired(a => a.User)
                .WithMany(u => u.UserAddresses)
                .HasForeignKey(a => a.UserId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<RestaurantOwner>()
                .HasRequired(ro => ro.User)
                .WithMany(u => u.RestaurantOwners)
                .HasForeignKey(ro => ro.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RestaurantOwner>()
                .HasRequired(ro => ro.Restaurant)
                .WithMany(r => r.RestaurantOwners)
                .HasForeignKey(ro => ro.RestaurantId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Order>()
                .HasRequired(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>()
                .HasRequired(o => o.Restaurant)
                .WithMany(r => r.Orders)
                .HasForeignKey(o => o.RestaurantId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MenuItem>()
                .HasRequired(mi => mi.Restaurant)
                .WithMany(r => r.MenuItems)
                .HasForeignKey(mi => mi.RestaurantId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<OrderedItem>()
                .HasRequired(oi => oi.Order)
                .WithMany(o => o.OrderedItems)
                .HasForeignKey(oi => oi.OrderId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<OrderedItem>()
                .HasRequired(oi => oi.MenuItem)
                .WithMany(mi => mi.OrderedItems)
                .HasForeignKey(oi => oi.MenuItemId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RefreshToken>()
                .HasRequired(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .WillCascadeOnDelete(true);

            base.OnModelCreating(modelBuilder);
        }
    }
}
