namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removed_table_ReservationHouseParticipant_changed_delete_on_cascade : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reservation_House_Participant", "ParticipantId", "dbo.Participant");
            DropForeignKey("dbo.Reservation_House_Participant", "Reservation_HouseId", "dbo.Reservation_Houses");
            DropIndex("dbo.Reservation_House_Participant", new[] { "ParticipantId" });
            DropIndex("dbo.Reservation_House_Participant", new[] { "Reservation_HouseId" });
            AddColumn("dbo.Participant", "Reservation_HouseId", c => c.Int(nullable: false));
            CreateIndex("dbo.Participant", "Reservation_HouseId");
            AddForeignKey("dbo.Participant", "Reservation_HouseId", "dbo.Reservation_Houses", "Id", cascadeDelete: true);
            DropTable("dbo.Reservation_House_Participant");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Reservation_House_Participant",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParticipantId = c.Int(nullable: false),
                        Reservation_HouseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.Participant", "Reservation_HouseId", "dbo.Reservation_Houses");
            DropIndex("dbo.Participant", new[] { "Reservation_HouseId" });
            DropColumn("dbo.Participant", "Reservation_HouseId");
            CreateIndex("dbo.Reservation_House_Participant", "Reservation_HouseId");
            CreateIndex("dbo.Reservation_House_Participant", "ParticipantId");
            AddForeignKey("dbo.Reservation_House_Participant", "Reservation_HouseId", "dbo.Reservation_Houses", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Reservation_House_Participant", "ParticipantId", "dbo.Participant", "Id", cascadeDelete: true);
        }
    }
}
