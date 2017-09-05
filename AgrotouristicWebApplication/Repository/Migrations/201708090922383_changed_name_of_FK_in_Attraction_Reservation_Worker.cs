namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changed_name_of_FK_in_Attraction_Reservation_Worker : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Attractions_Reservations_Workers", new[] { "Worker_Id" });
            DropColumn("dbo.Attractions_Reservations_Workers", "WorkerId");
            RenameColumn(table: "dbo.Attractions_Reservations_Workers", name: "Worker_Id", newName: "WorkerId");
            AlterColumn("dbo.Attractions_Reservations_Workers", "WorkerId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Attractions_Reservations_Workers", "WorkerId");
            AddForeignKey("dbo.Attractions_Reservations_Workers", "WorkerId", "dbo.AspNetUsers", "Id", true, "FK_dbo.Attraction_Reservation_Worker_WorkerId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Attractions_Reservations_Workers", new[] { "WorkerId" });
            AlterColumn("dbo.Attractions_Reservations_Workers", "WorkerId", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Attractions_Reservations_Workers", name: "WorkerId", newName: "Worker_Id");
            AddColumn("dbo.Attractions_Reservations_Workers", "WorkerId", c => c.Int(nullable: false));
            CreateIndex("dbo.Attractions_Reservations_Workers", "Worker_Id");
            DropForeignKey("dbo.Attractions_Reservations_Workers", "FK_dbo.Attraction_Reservation_Worker_WorkerId");
        }
    }
}
