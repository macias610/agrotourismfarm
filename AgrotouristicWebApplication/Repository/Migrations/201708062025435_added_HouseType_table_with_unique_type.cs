namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_HouseType_table_with_unique_type : DbMigration
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
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Type, unique: true);
            
            AddColumn("dbo.House", "Name", c => c.String(nullable: false, maxLength: 15));
            AddColumn("dbo.House", "HouseTypeId", c => c.Int(nullable: false));
            AlterColumn("dbo.House", "Description", c => c.String(nullable: false, maxLength: 100));
            CreateIndex("dbo.House", "Name", unique: true);
            CreateIndex("dbo.House", "HouseTypeId");
            AddForeignKey("dbo.House", "HouseTypeId", "dbo.HouseType", "Id", cascadeDelete: true);
            DropColumn("dbo.House", "Type");
            DropColumn("dbo.House", "Price");
        }
        
        public override void Down()
        {
            AddColumn("dbo.House", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.House", "Type", c => c.String(nullable: false, maxLength: 15));
            DropForeignKey("dbo.House", "HouseTypeId", "dbo.HouseType");
            DropIndex("dbo.HouseType", new[] { "Type" });
            DropIndex("dbo.House", new[] { "HouseTypeId" });
            DropIndex("dbo.House", new[] { "Name" });
            AlterColumn("dbo.House", "Description", c => c.String(nullable: false, maxLength: 15));
            DropColumn("dbo.House", "HouseTypeId");
            DropColumn("dbo.House", "Name");
            DropTable("dbo.HouseType");
        }
    }
}
