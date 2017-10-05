namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_delete_on_cascade_for_ApplicationUser : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Reservation", new[] { "ClientId" });
            AlterColumn("dbo.Reservation", "ClientId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Reservation", "ClientId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Reservation", new[] { "ClientId" });
            AlterColumn("dbo.Reservation", "ClientId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Reservation", "ClientId");
        }
    }
}
