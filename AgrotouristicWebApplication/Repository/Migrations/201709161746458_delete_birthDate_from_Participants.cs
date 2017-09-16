namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class delete_birthDate_from_Participants : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Participant", "BirthDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Participant", "BirthDate", c => c.DateTime(nullable: false));
        }
    }
}
