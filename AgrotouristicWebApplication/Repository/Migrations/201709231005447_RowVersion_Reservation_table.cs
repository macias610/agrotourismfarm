namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RowVersion_Reservation_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservation", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservation", "RowVersion");
        }
    }
}
