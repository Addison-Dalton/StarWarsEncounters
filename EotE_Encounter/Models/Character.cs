using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EotE_Encounter.Models
{
    public class Character
    {
        public int Id { get; set; }
        [Required][StringLength(250)]
        public string Name { get; set; }
        public byte Triumphs { get; set; } = 0;
        [Display(Name="Successes")]
        public byte Succeses { get; set; } = 0;
        public byte Advantages { get; set; } = 0;
        public short IniativeScore { get; set; } //this is calculated based on # of Triumphs, successes, and advantages
        public Encounter Encounter { get; set; }
        public bool? Turn { get; set; } //indicates if it is this character's turn or not
        [StringLength(1000)]
        public string Notes { get; set; }

        /*calculates the IniativeScore for a character.
         * This is done by giving triumps, successes, and advantages a multiplier value. Multiple the value by number of each role to get overall score.
         */
        public void SetIniativeScore()
        {
            const short TRIUMPH_MULTIPLIER = 250;
            const short SUCCESS_MULTIPLER = 20;
            const short ADVANTAGE_MULTIPLER = 1;
            short score = 0;

            score = (Int16)((this.Triumphs * TRIUMPH_MULTIPLIER) + (this.Succeses * SUCCESS_MULTIPLER) + (this.Advantages + ADVANTAGE_MULTIPLER));
            this.IniativeScore = score;
        }
    }
}
