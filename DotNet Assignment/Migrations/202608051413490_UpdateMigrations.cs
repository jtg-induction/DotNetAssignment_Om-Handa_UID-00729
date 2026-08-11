namespace DotNet_Assignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMigrations : DbMigration
    {
        public override void Up()
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
                .PrimaryKey(t => t.RefreshTokenId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RefreshTokens", "UserId", "dbo.Users");
            DropIndex("dbo.RefreshTokens", new[] { "UserId" });
            DropTable("dbo.RefreshTokens");
        }
    }
}
