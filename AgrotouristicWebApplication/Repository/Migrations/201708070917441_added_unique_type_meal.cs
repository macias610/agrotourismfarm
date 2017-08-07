namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_unique_type_meal : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Meal", "Type", c => c.String(nullable: false, maxLength: 100));
            CreateIndex("dbo.Meal", "Type", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Meal", new[] { "Type" });
            AlterColumn("dbo.Meal", "Type", c => c.String(nullable: false));
        }
    }
}
