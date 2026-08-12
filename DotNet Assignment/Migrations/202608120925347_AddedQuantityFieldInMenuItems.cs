namespace DotNet_Assignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedQuantityFieldInMenuItems : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MenuItems", "QuantityAvailable", c => c.Int(nullable: false));
            CreateIndex("dbo.Users", "PhoneNumber", unique: true, name: "IX_User_PhoneNumber");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Users", "IX_User_PhoneNumber");
            DropColumn("dbo.MenuItems", "QuantityAvailable");
        }
    }
}
