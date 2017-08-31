namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changed_delete_on_cascade_relations : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reservation", "ClientId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Reservation_House", "ReservationId", "dbo.Reservation");
            DropForeignKey("dbo.Reservation_House", "MealId", "dbo.Meal");
            DropForeignKey("dbo.Reservation_House_Participant", "Reservation_HouseId", "dbo.Reservation_House");
            DropForeignKey("dbo.House", "HouseTypeId", "dbo.HouseType");
            DropForeignKey("dbo.Reservation_House_Participant", "ParticipantId", "dbo.Participant");
            DropIndex("dbo.Reservation", new[] { "ClientId" });
            AlterColumn("dbo.Reservation", "ClientId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Reservation", "ClientId");
            AddForeignKey("dbo.Reservation", "ClientId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Reservation_House", "ReservationId", "dbo.Reservation", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Reservation_House", "MealId", "dbo.Meal", "Id");
            AddForeignKey("dbo.Reservation_House_Participant", "Reservation_HouseId", "dbo.Reservation_House", "Id", cascadeDelete: true);
            AddForeignKey("dbo.House", "HouseTypeId", "dbo.HouseType", "Id");
            AddForeignKey("dbo.Reservation_House_Participant", "ParticipantId", "dbo.Participant", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reservation_House_Participant", "ParticipantId", "dbo.Participant");
            DropForeignKey("dbo.House", "HouseTypeId", "dbo.HouseType");
            DropForeignKey("dbo.Reservation_House_Participant", "Reservation_HouseId", "dbo.Reservation_House");
            DropForeignKey("dbo.Reservation_House", "MealId", "dbo.Meal");
            DropForeignKey("dbo.Reservation_House", "ReservationId", "dbo.Reservation");
            DropForeignKey("dbo.Reservation", "ClientId", "dbo.AspNetUsers");
            DropIndex("dbo.Reservation", new[] { "ClientId" });
            AlterColumn("dbo.Reservation", "ClientId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Reservation", "ClientId");
            AddForeignKey("dbo.Reservation_House_Participant", "ParticipantId", "dbo.Participant", "Id");
            AddForeignKey("dbo.House", "HouseTypeId", "dbo.HouseType", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Reservation_House_Participant", "Reservation_HouseId", "dbo.Reservation_House", "Id");
            AddForeignKey("dbo.Reservation_House", "MealId", "dbo.Meal", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Reservation_House", "ReservationId", "dbo.Reservation", "Id");
            AddForeignKey("dbo.Reservation", "ClientId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
