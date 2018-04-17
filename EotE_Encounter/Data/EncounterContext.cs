using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EotE_Encounter.Models;
using Microsoft.EntityFrameworkCore;

namespace EotE_Encounter.Data
{
    public class EncounterContext : DbContext
    {
        public EncounterContext(DbContextOptions<EncounterContext> options) : base(options)
        {
            
        }

        public DbSet<Character> Characters { get; set; }
        public DbSet<Encounter> Encounters { get; set; }
    }
}
