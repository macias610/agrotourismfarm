namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RowVersion_AttractionReservationWorker_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Attraction_Reservation_Worker", "RowVersion", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Attraction_Reservation_Worker", "RowVersion");
        }
    }
}
