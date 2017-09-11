namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_delete_on_cascade_for_AttractionReservation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Attraction_Reservation", "ReservationId", "dbo.Reservation");
            AddForeignKey("dbo.Attraction_Reservation", "ReservationId", "dbo.Reservation", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Attraction_Reservation", "ReservationId", "dbo.Reservation");
            AddForeignKey("dbo.Attraction_Reservation", "ReservationId", "dbo.Reservation", "Id");
        }
    }
}
