using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EotE_Encounter.Models;
using EotE_Encounter.Data;
using Microsoft.EntityFrameworkCore;

namespace EotE_Encounter.Controllers
{
    public class EncounterController : Controller
    {
        public EncounterController()
        {
        }

        /*sets encounter name is "Encounter" if none is given
         *saves encounter to DB
         *
         */
        [HttpPost]
        public ActionResult Create(Encounter encounter)
        {
            //consider doing this in encounter controller
            if (String.IsNullOrWhiteSpace(encounter.Name))
            {
                encounter.Name = "Encounter";
            }

            return RedirectToAction("Details", encounter);
        }

        /* gets the encounter from db based on encounterId.
         * gets list of characters that encounterId's match the encounter.
         * sorts list of characters based on IniativeScore
         */
        public ActionResult Details(Encounter encounter)
        {
            if(encounter.Name == null)
            {
                encounter = Newtonsoft.Json.JsonConvert.DeserializeObject<Encounter>(TempData["encounter"].ToString());
            }
            return PartialView("Details", encounter);
        }

        /*changes the iniative order of the character passed. This is done by swapping the iniativeScore of the 
         * character passed with either the character below or above it, which is passed via the direction variable.
         * It the character is at the top of the list, and moved up then the character is moved to the bottom of the list.
         * The opposite occurs if the character is at the bottom of the list.
         */
        public ActionResult ChangeInitiative(int characterId, string direction, string encounterJSON)
        {
            const string MOVE_UP = "up";
            const string MOVE_DOWN = "down";
            Encounter encounter = Newtonsoft.Json.JsonConvert.DeserializeObject<Encounter>(encounterJSON);
            List<Character> characters = encounter.Characters.OrderByDescending(c => c.IniativeScore).ToList();
            Character movedCharacter = encounter.Characters.Where(c => c.Id == characterId).SingleOrDefault();

            //move character up
            if(direction == MOVE_UP)
            {
                //detect if character is at the top of the list
                if(movedCharacter.Id != characters.FirstOrDefault().Id)
                {
                    short tempInitiativeScore = movedCharacter.IniativeScore;
                    int movedCharacterIndex = characters.IndexOf(movedCharacter);
                    movedCharacter.IniativeScore = characters[movedCharacterIndex - 1].IniativeScore;
                    characters[movedCharacterIndex - 1].IniativeScore = tempInitiativeScore;

                    if(characters[movedCharacterIndex - 1].Turn == true)
                    {
                        characters[movedCharacterIndex - 1].Turn = false;
                        movedCharacter.Turn = true;
                    }
                }
                else
                {
                    
                    for(int i = 1; i < characters.Count; i++)
                    {
                        short tempInitiativeScore = movedCharacter.IniativeScore;
                        movedCharacter.IniativeScore = characters[i].IniativeScore;
                        characters[i].IniativeScore = tempInitiativeScore;
                    }

                    if (movedCharacter.Turn == true)
                    {
                        int movedCharacterIndex = characters.IndexOf(movedCharacter);
                        movedCharacter.Turn = false;
                        characters[movedCharacterIndex + 1].Turn = true;
                    }
                }
            }else if (direction == MOVE_DOWN)
            {
                if(movedCharacter.Id != characters.LastOrDefault().Id)
                {
                    short tempInitiativeScore = movedCharacter.IniativeScore;
                    int movedCharacterIndex = characters.IndexOf(movedCharacter);
                    movedCharacter.IniativeScore = characters[movedCharacterIndex + 1].IniativeScore;
                    characters[movedCharacterIndex + 1].IniativeScore = tempInitiativeScore;

                    if (movedCharacter.Turn == true)
                    {
                        movedCharacter.Turn = false;
                        characters[movedCharacterIndex + 1].Turn = true;
                    }
                }
                else
                {
                    for(int i = characters.Count - 2; i >= 0; i--)
                    {
                        short tempInitiativeScore = movedCharacter.IniativeScore;
                        movedCharacter.IniativeScore = characters[i].IniativeScore;
                        characters[i].IniativeScore = tempInitiativeScore;
                    }
                }
            }
            encounter.Characters = characters;
            TempData["encounter"] = Newtonsoft.Json.JsonConvert.SerializeObject(encounter);
            return RedirectToAction("Details");
        }

        //deletes all characters from encounter
        public ActionResult Clear(string encounterJSON)
        {
            Encounter encounter = Newtonsoft.Json.JsonConvert.DeserializeObject<Encounter>(encounterJSON);
            encounter.Characters.Clear();
            TempData["encounter"] = Newtonsoft.Json.JsonConvert.SerializeObject(encounter);
            return RedirectToAction("Details");

        }
    }
}