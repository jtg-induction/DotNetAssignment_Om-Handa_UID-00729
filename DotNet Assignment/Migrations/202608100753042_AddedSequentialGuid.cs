namespace DotNet_Assignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSequentialGuid : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderedItems", "MenuItemId", "dbo.MenuItems");
            DropForeignKey("dbo.OrderedItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.RestaurantOwners", "RestaurantId", "dbo.Restaurants");
            DropForeignKey("dbo.Orders", "RestaurantId", "dbo.Restaurants");
            DropForeignKey("dbo.MenuItems", "RestaurantId", "dbo.Restaurants");
            DropForeignKey("dbo.RefreshTokens", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserAddresses", "UserId", "dbo.Users");
            DropForeignKey("dbo.RestaurantOwners", "UserId", "dbo.Users");
            DropForeignKey("dbo.Orders", "UserId", "dbo.Users");
            DropPrimaryKey("dbo.MenuItems");
            DropPrimaryKey("dbo.OrderedItems");
            DropPrimaryKey("dbo.Orders");
            DropPrimaryKey("dbo.Restaurants");
            DropPrimaryKey("dbo.Users");
            DropPrimaryKey("dbo.RefreshTokens");
            DropPrimaryKey("dbo.UserAddresses");
            AlterColumn("dbo.MenuItems", "MenuItemId", c => c.Guid(nullable: false, identity: true, defaultValueSql:"NEWSEQUENTIALID()"));
            AlterColumn("dbo.OrderedItems", "OrderedItemId", c => c.Guid(nullable: false, identity: true, defaultValueSql: "NEWSEQUENTIALID()"));
            AlterColumn("dbo.Orders", "OrderId", c => c.Guid(nullable: false, identity: true, defaultValueSql: "NEWSEQUENTIALID()"));
            AlterColumn("dbo.Restaurants", "RestaurantId", c => c.Guid(nullable: false, identity: true, defaultValueSql: "NEWSEQUENTIALID()"));
            AlterColumn("dbo.Users", "UserId", c => c.Guid(nullable: false, identity: true, defaultValueSql: "NEWSEQUENTIALID()"));
            AlterColumn("dbo.RefreshTokens", "RefreshTokenId", c => c.Guid(nullable: false, identity: true, defaultValueSql: "NEWSEQUENTIALID()"));
            AlterColumn("dbo.UserAddresses", "UserAddressId", c => c.Guid(nullable: false, identity: true, defaultValueSql: "NEWSEQUENTIALID()"));
            AddPrimaryKey("dbo.MenuItems", "MenuItemId");
            AddPrimaryKey("dbo.OrderedItems", "OrderedItemId");
            AddPrimaryKey("dbo.Orders", "OrderId");
            AddPrimaryKey("dbo.Restaurants", "RestaurantId");
            AddPrimaryKey("dbo.Users", "UserId");
            AddPrimaryKey("dbo.RefreshTokens", "RefreshTokenId");
            AddPrimaryKey("dbo.UserAddresses", "UserAddressId");
            AddForeignKey("dbo.OrderedItems", "MenuItemId", "dbo.MenuItems", "MenuItemId");
            AddForeignKey("dbo.OrderedItems", "OrderId", "dbo.Orders", "OrderId", cascadeDelete: true);
            AddForeignKey("dbo.RestaurantOwners", "RestaurantId", "dbo.Restaurants", "RestaurantId", cascadeDelete: true);
            AddForeignKey("dbo.Orders", "RestaurantId", "dbo.Restaurants", "RestaurantId");
            AddForeignKey("dbo.MenuItems", "RestaurantId", "dbo.Restaurants", "RestaurantId", cascadeDelete: true);
            AddForeignKey("dbo.RefreshTokens", "UserId", "dbo.Users", "UserId", cascadeDelete: true);
            AddForeignKey("dbo.UserAddresses", "UserId", "dbo.Users", "UserId", cascadeDelete: true);
            AddForeignKey("dbo.RestaurantOwners", "UserId", "dbo.Users", "UserId");
            AddForeignKey("dbo.Orders", "UserId", "dbo.Users", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "UserId", "dbo.Users");
            DropForeignKey("dbo.RestaurantOwners", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserAddresses", "UserId", "dbo.Users");
            DropForeignKey("dbo.RefreshTokens", "UserId", "dbo.Users");
            DropForeignKey("dbo.MenuItems", "RestaurantId", "dbo.Restaurants");
            DropForeignKey("dbo.Orders", "RestaurantId", "dbo.Restaurants");
            DropForeignKey("dbo.RestaurantOwners", "RestaurantId", "dbo.Restaurants");
            DropForeignKey("dbo.OrderedItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.OrderedItems", "MenuItemId", "dbo.MenuItems");
            DropPrimaryKey("dbo.UserAddresses");
            DropPrimaryKey("dbo.RefreshTokens");
            DropPrimaryKey("dbo.Users");
            DropPrimaryKey("dbo.Restaurants");
            DropPrimaryKey("dbo.Orders");
            DropPrimaryKey("dbo.OrderedItems");
            DropPrimaryKey("dbo.MenuItems");
            AlterColumn("dbo.UserAddresses", "UserAddressId", c => c.Guid(nullable: false));
            AlterColumn("dbo.RefreshTokens", "RefreshTokenId", c => c.Guid(nullable: false));
            AlterColumn("dbo.Users", "UserId", c => c.Guid(nullable: false));
            AlterColumn("dbo.Restaurants", "RestaurantId", c => c.Guid(nullable: false));
            AlterColumn("dbo.Orders", "OrderId", c => c.Guid(nullable: false));
            AlterColumn("dbo.OrderedItems", "OrderedItemId", c => c.Guid(nullable: false));
            AlterColumn("dbo.MenuItems", "MenuItemId", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.UserAddresses", "UserAddressId");
            AddPrimaryKey("dbo.RefreshTokens", "RefreshTokenId");
            AddPrimaryKey("dbo.Users", "UserId");
            AddPrimaryKey("dbo.Restaurants", "RestaurantId");
            AddPrimaryKey("dbo.Orders", "OrderId");
            AddPrimaryKey("dbo.OrderedItems", "OrderedItemId");
            AddPrimaryKey("dbo.MenuItems", "MenuItemId");
            AddForeignKey("dbo.Orders", "UserId", "dbo.Users", "UserId");
            AddForeignKey("dbo.RestaurantOwners", "UserId", "dbo.Users", "UserId");
            AddForeignKey("dbo.UserAddresses", "UserId", "dbo.Users", "UserId", cascadeDelete: true);
            AddForeignKey("dbo.RefreshTokens", "UserId", "dbo.Users", "UserId", cascadeDelete: true);
            AddForeignKey("dbo.MenuItems", "RestaurantId", "dbo.Restaurants", "RestaurantId", cascadeDelete: true);
            AddForeignKey("dbo.Orders", "RestaurantId", "dbo.Restaurants", "RestaurantId");
            AddForeignKey("dbo.RestaurantOwners", "RestaurantId", "dbo.Restaurants", "RestaurantId", cascadeDelete: true);
            AddForeignKey("dbo.OrderedItems", "OrderId", "dbo.Orders", "OrderId", cascadeDelete: true);
            AddForeignKey("dbo.OrderedItems", "MenuItemId", "dbo.MenuItems", "MenuItemId");
        }
    }
}
