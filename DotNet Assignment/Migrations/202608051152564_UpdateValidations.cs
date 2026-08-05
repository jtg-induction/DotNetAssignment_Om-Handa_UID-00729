namespace DotNet_Assignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UpdateValidations : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RefreshTokens", "UserId", "dbo.Users");
            DropForeignKey("dbo.OrderedItems", "MenuItemId", "dbo.MenuItems");
            DropForeignKey("dbo.OrderedItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.RestaurantOwners", "RestaurantId", "dbo.Restaurants");
            DropForeignKey("dbo.Orders", "RestaurantId", "dbo.Restaurants");
            DropForeignKey("dbo.MenuItems", "RestaurantId", "dbo.Restaurants");
            DropForeignKey("dbo.UserAddresses", "UserId", "dbo.Users");
            DropForeignKey("dbo.RestaurantOwners", "UserId", "dbo.Users");
            DropForeignKey("dbo.Orders", "UserId", "dbo.Users");

            DropIndex("dbo.RefreshTokens", new[] { "UserId" });

            RenameColumn("dbo.MenuItems", "Id", "MenuItemId");
            RenameColumn("dbo.OrderedItems", "Id", "OrderedItemId");
            RenameColumn("dbo.Orders", "Id", "OrderId");
            RenameColumn("dbo.Orders", "Total_Price", "TotalPrice");
            RenameColumn("dbo.Restaurants", "Id", "RestaurantId");
            RenameColumn("dbo.Users", "Id", "UserId");
            RenameColumn("dbo.UserAddresses", "Id", "UserAddressId");

            AlterColumn("dbo.Orders", "DeliveryAddress", c => c.String(nullable: false));

            DropPrimaryKey("dbo.MenuItems");
            DropPrimaryKey("dbo.OrderedItems");
            DropPrimaryKey("dbo.Orders");
            DropPrimaryKey("dbo.Restaurants");
            DropPrimaryKey("dbo.Users");
            DropPrimaryKey("dbo.UserAddresses");

            AddPrimaryKey("dbo.MenuItems", "MenuItemId");
            AddPrimaryKey("dbo.OrderedItems", "OrderedItemId");
            AddPrimaryKey("dbo.Orders", "OrderId");
            AddPrimaryKey("dbo.Restaurants", "RestaurantId");
            AddPrimaryKey("dbo.Users", "UserId");
            AddPrimaryKey("dbo.UserAddresses", "UserAddressId");

            AddForeignKey("dbo.OrderedItems", "MenuItemId", "dbo.MenuItems", "MenuItemId");
            AddForeignKey("dbo.OrderedItems", "OrderId", "dbo.Orders", "OrderId", cascadeDelete: true);
            AddForeignKey("dbo.RestaurantOwners", "RestaurantId", "dbo.Restaurants", "RestaurantId", cascadeDelete: true);
            AddForeignKey("dbo.Orders", "RestaurantId", "dbo.Restaurants", "RestaurantId");
            AddForeignKey("dbo.MenuItems", "RestaurantId", "dbo.Restaurants", "RestaurantId", cascadeDelete: true);
            AddForeignKey("dbo.UserAddresses", "UserId", "dbo.Users", "UserId", cascadeDelete: true);
            AddForeignKey("dbo.RestaurantOwners", "UserId", "dbo.Users", "UserId");
            AddForeignKey("dbo.Orders", "UserId", "dbo.Users", "UserId");

            DropTable("dbo.RefreshTokens");
        }

        public override void Down()
        {
            CreateTable(
                "dbo.RefreshTokens",
                c => new
                {
                    RefreshTokenId = c.Guid(nullable: false),
                    Token = c.String(),
                    ExpiresAt = c.DateTime(nullable: false),
                    CreatedAt = c.DateTime(nullable: false),
                    UserId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.RefreshTokenId);

            AlterColumn("dbo.Orders", "DeliveryAddress", c => c.String());

            RenameColumn("dbo.UserAddresses", "UserAddressId", "Id");
            RenameColumn("dbo.Users", "UserId", "Id");
            RenameColumn("dbo.Restaurants", "RestaurantId", "Id");
            RenameColumn("dbo.Orders", "TotalPrice", "Total_Price");
            RenameColumn("dbo.Orders", "OrderId", "Id");
            RenameColumn("dbo.OrderedItems", "OrderedItemId", "Id");
            RenameColumn("dbo.MenuItems", "MenuItemId", "Id");

            DropPrimaryKey("dbo.UserAddresses");
            DropPrimaryKey("dbo.Users");
            DropPrimaryKey("dbo.Restaurants");
            DropPrimaryKey("dbo.Orders");
            DropPrimaryKey("dbo.OrderedItems");
            DropPrimaryKey("dbo.MenuItems");

            AddPrimaryKey("dbo.UserAddresses", "Id");
            AddPrimaryKey("dbo.Users", "Id");
            AddPrimaryKey("dbo.Restaurants", "Id");
            AddPrimaryKey("dbo.Orders", "Id");
            AddPrimaryKey("dbo.OrderedItems", "Id");
            AddPrimaryKey("dbo.MenuItems", "Id");

            CreateIndex("dbo.RefreshTokens", "UserId");
            AddForeignKey("dbo.Orders", "UserId", "dbo.Users", "Id");
            AddForeignKey("dbo.RestaurantOwners", "UserId", "dbo.Users", "Id");
            AddForeignKey("dbo.UserAddresses", "UserId", "dbo.Users", "Id", cascadeDelete: true);
            AddForeignKey("dbo.MenuItems", "RestaurantId", "dbo.Restaurants", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Orders", "RestaurantId", "dbo.Restaurants", "Id");
            AddForeignKey("dbo.RestaurantOwners", "RestaurantId", "dbo.Restaurants", "Id", cascadeDelete: true);
            AddForeignKey("dbo.OrderedItems", "OrderId", "dbo.Orders", "Id", cascadeDelete: true);
            AddForeignKey("dbo.OrderedItems", "MenuItemId", "dbo.MenuItems", "Id");
            AddForeignKey("dbo.RefreshTokens", "UserId", "dbo.Users", "Id", cascadeDelete: true);
        }
    }
}
