namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_Reservation_History_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reservation_History",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        OverallCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReservedHouses = c.String(nullable: false),
                        ReservedAttractions = c.String(nullable: false),
                        ClientId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ClientId)
                .Index(t => t.ClientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reservation_History", "ClientId", "dbo.AspNetUsers");
            DropIndex("dbo.Reservation_History", new[] { "ClientId" });
            DropTable("dbo.Reservation_History");
        }
    }
}
