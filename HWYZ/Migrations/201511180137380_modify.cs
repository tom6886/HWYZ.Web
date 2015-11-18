namespace HWYZ.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modify : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Store", "Discount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Store", "Discount", c => c.Int(nullable: false));
        }
    }
}
