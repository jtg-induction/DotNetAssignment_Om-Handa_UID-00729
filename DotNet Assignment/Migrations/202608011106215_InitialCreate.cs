namespace DotNet_Assignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MenuItems",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 255),
                        Rating = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Category = c.String(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        InStock = c.Boolean(nullable: false),
                        RestaurantId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Restaurants", t => t.RestaurantId, cascadeDelete: true)
                .Index(t => t.RestaurantId);
            
            CreateTable(
                "dbo.OrderedItems",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Quantity = c.Int(nullable: false),
                        ItemPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedAt = c.DateTime(nullable: false),
                        OrderId = c.Guid(nullable: false),
                        MenuItemId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MenuItems", t => t.MenuItemId)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.MenuItemId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Total_Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.Int(nullable: false),
                        DeliveryAddress = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        UserId = c.Guid(nullable: false),
                        RestaurantId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Restaurants", t => t.RestaurantId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RestaurantId);
            
            CreateTable(
                "dbo.Restaurants",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(nullable: false, maxLength: 100),
                        Street = c.String(nullable: false, maxLength: 255),
                        Landmark = c.String(maxLength: 255),
                        City = c.String(nullable: false),
                        State = c.String(nullable: false),
                        Pincode = c.String(nullable: false, maxLength: 6),
                        Rating = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        IsOpen = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RestaurantOwners",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        RestaurantId = c.Guid(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RestaurantId })
                .ForeignKey("dbo.Restaurants", t => t.RestaurantId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RestaurantId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Email = c.String(nullable: false, maxLength: 100),
                        Password = c.String(nullable: false, maxLength: 50),
                        Name = c.String(nullable: false, maxLength: 100),
                        PhoneNumber = c.String(nullable: false, maxLength: 10),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Role = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Email, unique: true, name: "IX_User_Email");
            
            CreateTable(
                "dbo.UserAddresses",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        HouseNumber = c.String(),
                        Street = c.String(nullable: false, maxLength: 100),
                        Landmark = c.String(),
                        City = c.String(nullable: false),
                        State = c.String(nullable: false),
                        Pincode = c.String(nullable: false, maxLength: 6),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        UserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MenuItems", "RestaurantId", "dbo.Restaurants");
            DropForeignKey("dbo.OrderedItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "UserId", "dbo.Users");
            DropForeignKey("dbo.Orders", "RestaurantId", "dbo.Restaurants");
            DropForeignKey("dbo.RestaurantOwners", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserAddresses", "UserId", "dbo.Users");
            DropForeignKey("dbo.RestaurantOwners", "RestaurantId", "dbo.Restaurants");
            DropForeignKey("dbo.OrderedItems", "MenuItemId", "dbo.MenuItems");
            DropIndex("dbo.UserAddresses", new[] { "UserId" });
            DropIndex("dbo.Users", "IX_User_Email");
            DropIndex("dbo.RestaurantOwners", new[] { "RestaurantId" });
            DropIndex("dbo.RestaurantOwners", new[] { "UserId" });
            DropIndex("dbo.Orders", new[] { "RestaurantId" });
            DropIndex("dbo.Orders", new[] { "UserId" });
            DropIndex("dbo.OrderedItems", new[] { "MenuItemId" });
            DropIndex("dbo.OrderedItems", new[] { "OrderId" });
            DropIndex("dbo.MenuItems", new[] { "RestaurantId" });
            DropTable("dbo.UserAddresses");
            DropTable("dbo.Users");
            DropTable("dbo.RestaurantOwners");
            DropTable("dbo.Restaurants");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderedItems");
            DropTable("dbo.MenuItems");
        }
    }
}
