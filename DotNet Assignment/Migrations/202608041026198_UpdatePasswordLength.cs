namespace DotNet_Assignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePasswordLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "Password", c => c.String(nullable: false, maxLength: 255));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "Password", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
