namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleted_RowVersion_AttractionReservationWorker_table : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Attraction_Reservation_Worker", "RowVersion");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Attraction_Reservation_Worker", "RowVersion", c => c.Binary());
        }
    }
}
