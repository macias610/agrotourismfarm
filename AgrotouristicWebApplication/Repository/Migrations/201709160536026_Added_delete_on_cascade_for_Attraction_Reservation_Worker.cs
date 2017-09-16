namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_delete_on_cascade_for_Attraction_Reservation_Worker : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Attraction_Reservation_Worker", "Attraction_ReservationId", "dbo.Attraction_Reservation");
            AddForeignKey("dbo.Attraction_Reservation_Worker", "Attraction_ReservationId", "dbo.Attraction_Reservation", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Attraction_Reservation_Worker", "Attraction_ReservationId", "dbo.Attraction_Reservation");
            AddForeignKey("dbo.Attraction_Reservation_Worker", "Attraction_ReservationId", "dbo.Attraction_Reservation", "Id");
        }
    }
}
