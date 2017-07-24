namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        Name = c.String(),
                        Surname = c.String(),
                        BirthDate = c.DateTime(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Reservation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        DeadlinePayment = c.DateTime(nullable: false),
                        Status = c.String(nullable: false, maxLength: 15),
                        OverallCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ClientId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.Attraction_Reservation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AttractionId = c.Int(nullable: false),
                        ReservationId = c.Int(nullable: false),
                        TermAffair = c.DateTime(nullable: false),
                        QuantityParticipant = c.Int(nullable: false),
                        OverallCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Attraction", t => t.AttractionId)
                .ForeignKey("dbo.Reservation", t => t.ReservationId)
                .Index(t => t.AttractionId)
                .Index(t => t.ReservationId);
            
            CreateTable(
                "dbo.Attraction",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 15),
                        Description = c.String(nullable: false, maxLength: 15),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Discount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Attraction_Reservation_Worker",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WorkerId = c.Int(nullable: false),
                        Attraction_ReservationId = c.Int(nullable: false),
                        Worker_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Attraction_Reservation", t => t.Attraction_ReservationId)
                .ForeignKey("dbo.Worker", t => t.Worker_Id)
                .Index(t => t.Attraction_ReservationId)
                .Index(t => t.Worker_Id);
            
            CreateTable(
                "dbo.Reservation_House",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HouseId = c.Int(nullable: false),
                        ReservationId = c.Int(nullable: false),
                        MealId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.House", t => t.HouseId)
                .ForeignKey("dbo.Meal", t => t.MealId, cascadeDelete: true)
                .ForeignKey("dbo.Reservation", t => t.ReservationId)
                .Index(t => t.HouseId)
                .Index(t => t.ReservationId)
                .Index(t => t.MealId);
            
            CreateTable(
                "dbo.House",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 15),
                        Type = c.String(nullable: false, maxLength: 15),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Meal",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Reservation_House_Participant",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParticipantId = c.Int(nullable: false),
                        Reservation_HouseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Participant", t => t.ParticipantId)
                .ForeignKey("dbo.Reservation_House", t => t.Reservation_HouseId)
                .Index(t => t.ParticipantId)
                .Index(t => t.Reservation_HouseId);
            
            CreateTable(
                "dbo.Participant",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 15),
                        Surname = c.String(nullable: false, maxLength: 15),
                        BirthDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.Client",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Worker",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        HireDate = c.DateTime(nullable: false),
                        Profession = c.String(nullable: false, maxLength: 15),
                        Salary = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Worker", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Client", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Reservation_House_Participant", "Reservation_HouseId", "dbo.Reservation_House");
            DropForeignKey("dbo.Reservation_House_Participant", "ParticipantId", "dbo.Participant");
            DropForeignKey("dbo.Reservation_House", "ReservationId", "dbo.Reservation");
            DropForeignKey("dbo.Reservation_House", "MealId", "dbo.Meal");
            DropForeignKey("dbo.Reservation_House", "HouseId", "dbo.House");
            DropForeignKey("dbo.Reservation", "ClientId", "dbo.Client");
            DropForeignKey("dbo.Attraction_Reservation", "ReservationId", "dbo.Reservation");
            DropForeignKey("dbo.Attraction_Reservation_Worker", "Worker_Id", "dbo.Worker");
            DropForeignKey("dbo.Attraction_Reservation_Worker", "Attraction_ReservationId", "dbo.Attraction_Reservation");
            DropForeignKey("dbo.Attraction_Reservation", "AttractionId", "dbo.Attraction");
            DropIndex("dbo.Worker", new[] { "Id" });
            DropIndex("dbo.Client", new[] { "Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Reservation_House_Participant", new[] { "Reservation_HouseId" });
            DropIndex("dbo.Reservation_House_Participant", new[] { "ParticipantId" });
            DropIndex("dbo.Reservation_House", new[] { "MealId" });
            DropIndex("dbo.Reservation_House", new[] { "ReservationId" });
            DropIndex("dbo.Reservation_House", new[] { "HouseId" });
            DropIndex("dbo.Attraction_Reservation_Worker", new[] { "Worker_Id" });
            DropIndex("dbo.Attraction_Reservation_Worker", new[] { "Attraction_ReservationId" });
            DropIndex("dbo.Attraction_Reservation", new[] { "ReservationId" });
            DropIndex("dbo.Attraction_Reservation", new[] { "AttractionId" });
            DropIndex("dbo.Reservation", new[] { "ClientId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropTable("dbo.Worker");
            DropTable("dbo.Client");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Participant");
            DropTable("dbo.Reservation_House_Participant");
            DropTable("dbo.Meal");
            DropTable("dbo.House");
            DropTable("dbo.Reservation_House");
            DropTable("dbo.Attraction_Reservation_Worker");
            DropTable("dbo.Attraction");
            DropTable("dbo.Attraction_Reservation");
            DropTable("dbo.Reservation");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
        }
    }
}
