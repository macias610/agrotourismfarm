namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changed_name_of_FK_in_Attraction_Reservation_Worker : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Attraction_Reservation_Worker", new[] { "Worker_Id" });
            DropColumn("dbo.Attraction_Reservation_Worker", "WorkerId");
            RenameColumn(table: "dbo.Attraction_Reservation_Worker", name: "Worker_Id", newName: "WorkerId");
            AlterColumn("dbo.Attraction_Reservation_Worker", "WorkerId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Attraction_Reservation_Worker", "WorkerId");
            AddForeignKey("dbo.Attraction_Reservation_Worker", "WorkerId", "dbo.AspNetUsers", "Id", true, "FK_dbo.Attraction_Reservation_Worker_WorkerId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Attraction_Reservation_Worker", new[] { "WorkerId" });
            AlterColumn("dbo.Attraction_Reservation_Worker", "WorkerId", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Attraction_Reservation_Worker", name: "WorkerId", newName: "Worker_Id");
            AddColumn("dbo.Attraction_Reservation_Worker", "WorkerId", c => c.Int(nullable: false));
            CreateIndex("dbo.Attraction_Reservation_Worker", "Worker_Id");
            DropForeignKey("dbo.Attraction_Reservation_Worker", "FK_dbo.Attraction_Reservation_Worker_WorkerId");
        }
    }
}
