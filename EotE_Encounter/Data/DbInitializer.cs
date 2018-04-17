using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EotE_Encounter.Models;
using EotE_Encounter.Data;
using Microsoft.EntityFrameworkCore;

namespace EotE_Encounter.Data
{
    public static class DbInitializer
    {
        //method to clear tables of any data
        public static void Initialize(EncounterContext context)
        {
            context.Database.EnsureCreated();

            if (context.Characters.Any())
            {
                context.Database.ExecuteSqlCommand("DELETE FROM Characters");
            }
            context.SaveChanges();

            if (context.Encounters.Any())
            {
                context.Database.ExecuteSqlCommand("DELETE FROM Encounters");
            }
            context.SaveChanges();
        }
    }
}
