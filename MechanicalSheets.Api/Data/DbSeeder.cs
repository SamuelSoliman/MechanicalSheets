using MechanicalSheets.Api.Enums;
using MechanicalSheets.Api.Models;

namespace MechanicalSheets.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        // Seed catalogo difetti
        if (!db.DefectCatalogs.Any())
        {
            var defects = new List<DefectCatalog>
            {
                new() { Code = "01-CAR-01", Category = DefectCategoryEnum.Bodywork, Description = "Alterazione trattamento verniciante", Gravity = 1 },
                new() { Code = "01-CAR-02", Category = DefectCategoryEnum.Bodywork, Description = "Ossidazione/corrosione lamiera", Gravity = 4 },
                new() { Code = "01-CAR-03", Category = DefectCategoryEnum.Bodywork, Description = "Deformazione pannelli", Gravity = 4 },
                new() { Code = "01-CAR-05", Category = DefectCategoryEnum.Bodywork, Description = "Difetti saldatura strutturale", Gravity = 4 },
                new() { Code = "01-CAR-06", Category = DefectCategoryEnum.Bodywork, Description = "Fuori allineamento geometrico", Gravity = 5 },
                new() { Code = "01-ILL-08", Category = DefectCategoryEnum.Lighting, Description = "Gruppo ottico danneggiato o appannato", Gravity = 2 },
                new() { Code = "01-ILL-09", Category = DefectCategoryEnum.Lighting, Description = "Illuminazione insufficiente o assente", Gravity = 2 },
                new() { Code = "01-ILL-10", Category = DefectCategoryEnum.Lighting, Description = "Cavi impianto danneggiati", Gravity = 2 },
                new() { Code = "01-ILL-12", Category = DefectCategoryEnum.Lighting, Description = "Impianti non protetti o scoperti", Gravity = 2 },
                new() { Code = "01-ILL-13", Category = DefectCategoryEnum.Lighting, Description = "Connettori scollegati o ossidati", Gravity = 2 },
                new() { Code = "01-FIS-01", Category = DefectCategoryEnum.Fasteners, Description = "Ossidazione bulloneria", Gravity = 2 },
                new() { Code = "01-FIS-02", Category = DefectCategoryEnum.Fasteners, Description = "Corrosione staffaggi", Gravity = 4 },
                new() { Code = "01-FIS-04", Category = DefectCategoryEnum.Fasteners, Description = "Deformazione supporti", Gravity = 4 },
                new() { Code = "01-FIS-07", Category = DefectCategoryEnum.Fasteners, Description = "Umidità vano motore (endoscopia)", Gravity = 2 },
                new() { Code = "01-TEL-01", Category = DefectCategoryEnum.Frame, Description = "Ammaloramento struttura portante", Gravity = 2 },
                new() { Code = "01-TEL-02", Category = DefectCategoryEnum.Frame, Description = "Fessure o cricche sul telaio", Gravity = 4 },
                new() { Code = "01-TEL-03", Category = DefectCategoryEnum.Frame, Description = "Ristagno d'acqua nel sottoscocca", Gravity = 2 },
                new() { Code = "01-TEL-04", Category = DefectCategoryEnum.Frame, Description = "Erosione rivestimento antirombo", Gravity = 3 },
                new() { Code = "01-RUG-01", Category = DefectCategoryEnum.Frame, Description = "Presenza di ruggine passante", Gravity = 0 },
              
            };

            db.DefectCatalogs.AddRange(defects);
            await db.SaveChangesAsync();
        }

        // Seed API key
        if (!db.ApiKeys.Any())
        {
            var keyPlain = "test-api-key-12345";
            var keyHash = Convert.ToHexString(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(keyPlain)
                )
            ).ToLower();

            db.ApiKeys.Add(new ApiKey
            {
                Name = "Sistema ERP Test",
                KeyHash = keyHash,
                IsActive = true
            });

            await db.SaveChangesAsync();
            Console.WriteLine($"[SEED] API Key di test: {keyPlain}");
        }

        // Seed utenti
        if (!db.Users.Any())
        {
            var users = new List<User>
            {
                new() { Name = "Mario Rossi",    Email = "mario.rossi@test.com",    Password = BCrypt.Net.BCrypt.HashPassword("password123"), Role = "mechanic" },
                new() { Name = "John Doe",       Email = "john.doe@test.com",        Password = BCrypt.Net.BCrypt.HashPassword("password123"), Role = "mechanic" },
                new() { Name = "Luca Bianchi",   Email = "luca.bianchi@test.com",    Password = BCrypt.Net.BCrypt.HashPassword("password123"), Role = "mechanic" },
                new() { Name = "Anna Verdi",     Email = "anna.verdi@test.com",      Password = BCrypt.Net.BCrypt.HashPassword("password123"), Role = "mechanic" },
                new() { Name = "Manager One",    Email = "manager@test.com",         Password = BCrypt.Net.BCrypt.HashPassword("password123"), Role = "manager" },
                new() { Name = "Manager Two",    Email = "manager2@test.com",        Password = BCrypt.Net.BCrypt.HashPassword("password123"), Role = "manager" },
            };

            db.Users.AddRange(users);
            await db.SaveChangesAsync();

            Console.WriteLine("[SEED] Utenti creati");
        }

      
        if (!db.Sheets.Any())
        {
           
            var mario  = db.Users.First(u => u.Email == "mario.rossi@test.com");
            var john   = db.Users.First(u => u.Email == "john.doe@test.com");
            var luca   = db.Users.First(u => u.Email == "luca.bianchi@test.com");
            var anna   = db.Users.First(u => u.Email == "anna.verdi@test.com");
            var manager = db.Users.First(u => u.Email == "manager@test.com");

            var d1  = db.DefectCatalogs.First(d => d.Code == "01-CAR-01");
            var d2  = db.DefectCatalogs.First(d => d.Code == "01-CAR-02");
            var d3  = db.DefectCatalogs.First(d => d.Code == "01-ILL-08");
            var d4  = db.DefectCatalogs.First(d => d.Code == "01-FIS-01");
            var d5  = db.DefectCatalogs.First(d => d.Code == "01-TEL-01");
            var d6  = db.DefectCatalogs.First(d => d.Code == "01-TEL-02");
            var d7  = db.DefectCatalogs.First(d => d.Code == "01-RUG-01");
            var d8  = db.DefectCatalogs.First(d => d.Code == "01-CAR-03");
            var d9  = db.DefectCatalogs.First(d => d.Code == "01-ILL-09");
           

           
            var sheet1 = new Sheet
            {
                Code = "VEI-001",
                Brand = "Fiat",
                Vehicle = "500",
                InspectionDate = new DateOnly(2026, 4, 10),
                SheetStatus = SheetStatusEnum.Draft,
                CreatedById = mario.Id,
            };
            db.Sheets.Add(sheet1);
            await db.SaveChangesAsync();

            db.SheetTechnicians.AddRange(
                new SheetTechnician { SheetId = sheet1.Id, UserId = mario.Id },
                new SheetTechnician { SheetId = sheet1.Id, UserId = john.Id }
            );
            db.SheetDefectItems.AddRange(
                new SheetDefectItem { SheetId = sheet1.Id, DefectCatalogId = d1.Id, IsSeen = true,  ExtentLow = true, IntensityLow = true },
                new SheetDefectItem { SheetId = sheet1.Id, DefectCatalogId = d2.Id, IsSeen = true,  ExtentMedium = true, IntensityMedium = true, Notes = "Corrosione visibile sul paraurti" }
            );
            await db.SaveChangesAsync();

           
            var sheet2 = new Sheet
            {
                Code = "VEI-002",
                Brand = "Volkswagen",
                Vehicle = "Golf",
                InspectionDate = new DateOnly(2026, 4, 12),
                SheetStatus = SheetStatusEnum.Submitted,
                SubmittedAt = DateTime.UtcNow.AddDays(-3),
                CreatedById = john.Id,
            };
            db.Sheets.Add(sheet2);
            await db.SaveChangesAsync();

            db.SheetTechnicians.Add(
                new SheetTechnician { SheetId = sheet2.Id, UserId = john.Id }
            );
            db.SheetDefectItems.AddRange(
                new SheetDefectItem { SheetId = sheet2.Id, DefectCatalogId = d3.Id, IsSeen = true,  ExtentLow = true, IntensityLow = true },
                new SheetDefectItem { SheetId = sheet2.Id, DefectCatalogId = d4.Id, IsSeen = true,  ExtentMedium = true },
                new SheetDefectItem { SheetId = sheet2.Id, DefectCatalogId = d5.Id, IsSeen = false, IsNa = true }
            );
            await db.SaveChangesAsync();

          
            var sheet3 = new Sheet
            {
                Code = "VEI-003",
                Brand = "BMW",
                Vehicle = "Serie 3",
                InspectionDate = new DateOnly(2026, 4, 14),
                SheetStatus = SheetStatusEnum.Approved,
                SubmittedAt = DateTime.UtcNow.AddDays(-5),
                ReviewedAt = DateTime.UtcNow.AddDays(-2),
                ReviewedById = manager.Id,
                CreatedById = luca.Id,
            };
            db.Sheets.Add(sheet3);
            await db.SaveChangesAsync();

            db.SheetTechnicians.AddRange(
                new SheetTechnician { SheetId = sheet3.Id, UserId = luca.Id },
                new SheetTechnician { SheetId = sheet3.Id, UserId = anna.Id }
            );
            db.SheetDefectItems.AddRange(
                new SheetDefectItem { SheetId = sheet3.Id, DefectCatalogId = d6.Id, IsSeen = true, ExtentHigh = true, IntensityHigh = true, IsPs = true },
                new SheetDefectItem { SheetId = sheet3.Id, DefectCatalogId = d7.Id, IsSeen = true,  Notes = "Ruggine passante sul sottoscocca" },
                new SheetDefectItem { SheetId = sheet3.Id, DefectCatalogId = d8.Id, IsSeen = true,  ExtentMedium = true, IntensityLow = true },
                new SheetDefectItem { SheetId = sheet3.Id, DefectCatalogId = d9.Id, IsSeen = false, IsNr = true }
            );
            await db.SaveChangesAsync();

          
            var sheet4 = new Sheet
            {
                Code = "VEI-004",
                Brand = "Toyota",
                Vehicle = "Yaris",
                InspectionDate = new DateOnly(2026, 4, 16),
                SheetStatus = SheetStatusEnum.Rejected,
                SubmittedAt = DateTime.UtcNow.AddDays(-4),
                ReviewedAt = DateTime.UtcNow.AddDays(-1),
                ReviewedById = manager.Id,
                RejectionNote = "Mancano foto per i difetti carrozzeria",
                CreatedById = anna.Id,
            };
            db.Sheets.Add(sheet4);
            await db.SaveChangesAsync();

            db.SheetTechnicians.Add(
                new SheetTechnician { SheetId = sheet4.Id, UserId = anna.Id }
            );
            db.SheetDefectItems.AddRange(
                new SheetDefectItem { SheetId = sheet4.Id, DefectCatalogId = d1.Id, IsSeen = true, ExtentLow = true },
                new SheetDefectItem { SheetId = sheet4.Id, DefectCatalogId = d9.Id, IsSeen = true, Notes = "Connettori scollegati o ossidato" }
            );
            await db.SaveChangesAsync();

          
            var sheet5 = new Sheet
            {
                Code = "VEI-005",
                Brand = "Renault",
                Vehicle = "Clio",
                InspectionDate = new DateOnly(2026, 4, 20),
                SheetStatus = SheetStatusEnum.Draft,
                CreatedById = mario.Id,
            };
            db.Sheets.Add(sheet5);
            await db.SaveChangesAsync();

            db.SheetTechnicians.AddRange(
                new SheetTechnician { SheetId = sheet5.Id, UserId = mario.Id },
                new SheetTechnician { SheetId = sheet5.Id, UserId = luca.Id }
            );
            db.SheetDefectItems.AddRange(
                new SheetDefectItem { SheetId = sheet5.Id, DefectCatalogId = d1.Id, IsSeen = true,  ExtentLow = true, IntensityLow = true },
                new SheetDefectItem { SheetId = sheet5.Id, DefectCatalogId = d2.Id, IsSeen = true, ExtentMedium = true, IntensityMedium = true },
                new SheetDefectItem { SheetId = sheet5.Id, DefectCatalogId = d3.Id, IsSeen = true,  ExtentLow = true },
                new SheetDefectItem { SheetId = sheet5.Id, DefectCatalogId = d4.Id, IsSeen = false, IsNp = true },
                new SheetDefectItem { SheetId = sheet5.Id, DefectCatalogId = d5.Id, IsSeen = true,  ExtentMedium = true, IntensityLow = true },
                new SheetDefectItem { SheetId = sheet5.Id, DefectCatalogId = d6.Id, IsSeen = true,  ExtentHigh = true, IntensityHigh = true, IsPs = true, Notes = "Cricca sul longherone destro" }
            );
            await db.SaveChangesAsync();

           
            var sheet6 = new Sheet
            {
                Code = "VEI-006",
                Brand = "Ford",
                Vehicle = "Focus",
                InspectionDate = new DateOnly(2026, 3, 28),
                SheetStatus = SheetStatusEnum.Rejected,
                SubmittedAt = DateTime.UtcNow.AddDays(-10),
                ReviewedAt = DateTime.UtcNow.AddDays(-7),
                ReviewedById = manager.Id,
                CreatedById = luca.Id,
            };
            db.Sheets.Add(sheet6);
            await db.SaveChangesAsync();

            db.SheetTechnicians.Add(
                new SheetTechnician { SheetId = sheet6.Id, UserId = luca.Id }
            );
            db.SheetDefectItems.AddRange(
                new SheetDefectItem { SheetId = sheet6.Id, DefectCatalogId = d7.Id, IsSeen = true, Notes = "Ruggine passante trattata" },
                new SheetDefectItem { SheetId = sheet6.Id, DefectCatalogId = d9.Id, IsSeen = false, IsNr = true },
                new SheetDefectItem { SheetId = sheet6.Id, DefectCatalogId = d3.Id, IsSeen = true }
            );
            await db.SaveChangesAsync();

            Console.WriteLine("[SEED] Schede e difetti creati");
        }
    }
}