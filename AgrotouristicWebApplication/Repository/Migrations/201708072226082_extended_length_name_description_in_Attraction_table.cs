namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class extended_length_name_description_in_Attraction_table : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Attraction", "Name", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.Attraction", "Description", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Attraction", "Description", c => c.String(nullable: false, maxLength: 15));
            AlterColumn("dbo.Attraction", "Name", c => c.String(nullable: false, maxLength: 15));
        }
    }
}
