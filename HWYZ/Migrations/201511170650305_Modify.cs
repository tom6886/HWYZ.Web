namespace HWYZ.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Store",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 64),
                        StoreName = c.String(nullable: false, maxLength: 64),
                        StoreCode = c.String(nullable: false, maxLength: 64),
                        Province = c.String(nullable: false, maxLength: 64),
                        City = c.String(nullable: false, maxLength: 64),
                        County = c.String(nullable: false, maxLength: 64),
                        Address = c.String(nullable: false, maxLength: 100),
                        Lng = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Lat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Recommender = c.String(maxLength: 100),
                        Presider = c.String(maxLength: 100),
                        Tel = c.String(maxLength: 100),
                        StoreType = c.Int(nullable: false),
                        Discount = c.Int(nullable: false),
                        Alipay = c.String(maxLength: 100),
                        WeiXin = c.String(maxLength: 100),
                        Bank = c.String(maxLength: 100),
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
            DropTable("dbo.Store");
        }
    }
}
