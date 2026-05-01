using MechanicalSheets.Api.Enums;
using MechanicalSheets.Api.Models;

namespace MechanicalSheets.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
    
        if (db.DefectCatalogs.Any()) return;

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
            new() { Code = "01-RUG-01", Category = DefectCategoryEnum.Frame, Description = "Presenza di ruggine passante", Gravity = 0 }
,
        };

        db.DefectCatalogs.AddRange(defects);
        await db.SaveChangesAsync();
    }
}