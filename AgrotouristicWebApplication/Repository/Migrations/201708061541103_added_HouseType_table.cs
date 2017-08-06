namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_HouseType_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HouseType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(nullable: false, maxLength: 15),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HouseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.House", t => t.HouseId, cascadeDelete: true)
                .Index(t => t.Type, unique: true)
                .Index(t => t.HouseId);
            
            AlterColumn("dbo.House", "Description", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.House", "Type");
            DropColumn("dbo.House", "Price");
        }
        
        public override void Down()
        {
            AddColumn("dbo.House", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.House", "Type", c => c.String(nullable: false, maxLength: 15));
            DropForeignKey("dbo.HouseType", "HouseId", "dbo.House");
            DropIndex("dbo.HouseType", new[] { "HouseId" });
            DropIndex("dbo.HouseType", new[] { "Type" });
            AlterColumn("dbo.House", "Description", c => c.String(nullable: false, maxLength: 15));
            DropTable("dbo.HouseType");
        }
    }
}
