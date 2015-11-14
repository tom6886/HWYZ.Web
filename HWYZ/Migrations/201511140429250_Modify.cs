namespace HWYZ.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GuserRole",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 64),
                        RoleName = c.String(nullable: false, maxLength: 64),
                        RoleVal = c.String(nullable: false, maxLength: 64),
                        Status = c.Int(nullable: false),
                        Creator = c.String(maxLength: 50),
                        CreateTime = c.DateTime(nullable: false),
                        CreatorID = c.String(maxLength: 64),
                        ModifyTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.GuserRole");
        }
    }
}
