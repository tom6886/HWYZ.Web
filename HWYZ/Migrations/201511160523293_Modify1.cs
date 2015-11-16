namespace HWYZ.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Guser", "RoleId", c => c.String(maxLength: 64));
            CreateIndex("dbo.Guser", "RoleId");
            AddForeignKey("dbo.Guser", "RoleId", "dbo.GuserRole", "ID");
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.Guser", "RoleId", "dbo.GuserRole");
            //DropIndex("dbo.Guser", new[] { "RoleId" });
            //DropColumn("dbo.Guser", "RoleId");
        }
    }
}
