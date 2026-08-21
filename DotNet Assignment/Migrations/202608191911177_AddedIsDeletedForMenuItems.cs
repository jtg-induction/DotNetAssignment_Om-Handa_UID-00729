namespace DotNet_Assignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsDeletedForMenuItems : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MenuItems", "IsDeleted", c => c.Boolean(nullable: false));
            DropColumn("dbo.MenuItems", "InStock");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MenuItems", "InStock", c => c.Boolean(nullable: false));
            DropColumn("dbo.MenuItems", "IsDeleted");
        }
    }
}
