using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using EotE_Encounter.Models;

namespace EotE_Encounter.Models
{
    public class Encounter
    {
        public Encounter()
        {
            Round = 1;
            Characters = new List<Character>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public byte Round { get; set; }
        [StringLength(2000)]
        public string Notes { get; set; }

        public List<Character> Characters { get; set; } //get this working
    }

}
