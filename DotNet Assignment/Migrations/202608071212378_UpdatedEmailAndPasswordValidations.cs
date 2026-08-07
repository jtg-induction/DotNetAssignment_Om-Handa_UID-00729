namespace DotNet_Assignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedEmailAndPasswordValidations : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Users", "IX_User_Email");
            AlterColumn("dbo.Users", "Email", c => c.String(nullable: false, maxLength: 255));
            CreateIndex("dbo.Users", "Email", unique: true, name: "IX_User_Email");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Users", "IX_User_Email");
            AlterColumn("dbo.Users", "Email", c => c.String(nullable: false, maxLength: 100));
            CreateIndex("dbo.Users", "Email", unique: true, name: "IX_User_Email");
        }
    }
}
