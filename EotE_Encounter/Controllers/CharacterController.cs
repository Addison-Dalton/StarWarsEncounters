﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EotE_Encounter.Models;
using EotE_Encounter.Data;

namespace EotE_Encounter.Controllers
{
    public class CharacterController : Controller
    {

        public CharacterController()
        {
        }

        public ActionResult CreateCharacter(string encounterName, List<Character> encounterCharacters)
        {
            Encounter encounter = new Encounter() { Name = encounterName, Characters = encounterCharacters };
            ViewBag.EncounterName = encounter.Name;
            ViewBag.EncounterCharacters = encounter.Characters;
            return PartialView("Add");
        }
        
        public ActionResult Add(Character character, string encounterName, List<Character> encounterCharacters)
        {
            if (ModelState.IsValid)
            {
                character.SetIniativeScore();

                //if added character has a greater iniativeScore than the current character with greatest iniativeScore, then set added character turn to true

                if(encounterCharacters.Count <= 0)
                {
                    character.Turn = true;
                    character.Id = 1;
                }
                else
                {
                    //give new character an ID that is +1 of the current MAX id of the all the characters.
                    character.Id = encounterCharacters.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
                    if (character.IniativeScore > encounterCharacters.OrderByDescending(c => c.IniativeScore).First().IniativeScore)
                    {
                        foreach(Character characterInEncounter in encounterCharacters)
                        {
                            characterInEncounter.Turn = false;
                        }
                        character.Turn = true;
                    }
                }
                encounterCharacters.Add(character);

                Encounter encounter = new Encounter() { Name = encounterName, Characters = encounterCharacters };
                TempData["encounter"] = Newtonsoft.Json.JsonConvert.SerializeObject(encounter);
                return RedirectToAction("Details", "Encounter", new {encounter = (Encounter) null });
            }
            return PartialView();
        }


        public ActionResult Edit(Character character, Encounter encounter)
        {
            if (ModelState.IsValid)
            {
                Character oldCharacter = encounter.Characters.Where(c => c.Id.Equals(character.Id)).SingleOrDefault();
                var oldCharacterIndex = encounter.Characters.IndexOf(oldCharacter);
                oldCharacter.Name = character.Name;
                oldCharacter.Notes = character.Notes;
                //if the number if triupmhs, successes, or advantages have changed, update the IniativeScore
                if (oldCharacter.Triumphs != character.Triumphs || oldCharacter.Succeses != character.Succeses || oldCharacter.Advantages != character.Advantages)
                {
                    oldCharacter.Triumphs = character.Triumphs;
                    oldCharacter.Succeses = character.Succeses;
                    oldCharacter.Advantages = character.Advantages;
                    oldCharacter.SetIniativeScore();
                    if (oldCharacter.IniativeScore >= encounter.Characters.OrderByDescending(c => c.IniativeScore).First().IniativeScore)
                    {
                        encounter.Characters.OrderByDescending(c => c.IniativeScore).First().Turn = false;
                        oldCharacter.Turn = true;
                    }
                }
                encounter.Characters[oldCharacterIndex] = oldCharacter;

                return RedirectToAction("Details", "Encounter", encounter);
            }
            return PartialView("Details", character);
        }

        public ActionResult Details(int characterId, Encounter encounter)
        {
            Character character = encounter.Characters.Where(c => c.Id.Equals(characterId)).SingleOrDefault();
            ViewBag.Encounter = encounter;
            //_context.Entry(character).Reload();
            return PartialView("Details", character);
        }

        public ActionResult Delete(int characterId, Encounter encounter)
        {
            Character character = encounter.Characters.Where(c => c.Id == characterId).SingleOrDefault();
            List<Character> characters = encounter.Characters.ToList().OrderByDescending(c => c.IniativeScore).ToList();
            int characterIndex = characters.IndexOf(character);

            if(character.Turn == true)
            {
                if (character.Id != characters.LastOrDefault().Id)
                {
                    characters[characterIndex + 1].Turn = true;
                }
                else
                {
                    characters.FirstOrDefault().Turn = true;
                }
            }

            encounter.Characters.Remove(character);
            return RedirectToAction("Details", "Encounter", encounter);
        }

        public ActionResult ChangeTurn(string direction, Encounter encounter)
        {
            const string NEXT = "next";
            const string PREV = "prev";
            List<Character> characters = encounter.Characters.OrderByDescending(c => c.IniativeScore).ToList();
            Character currentTurnCharacter = encounter.Characters.Where(c => c.Turn == true).SingleOrDefault();
            currentTurnCharacter.Turn = false;
            int currentTurnCharacterIndex = characters.IndexOf(currentTurnCharacter);
            //previous turn
            if (direction == PREV)
            {
                //detect if character is at the top of the list
                if (currentTurnCharacter.Id != characters.FirstOrDefault().Id)
                {
                    characters[currentTurnCharacterIndex - 1].Turn = true;
                }
                else
                {
                    characters.LastOrDefault().Turn = true;
                }
            }else if (direction== NEXT)
            {
                if (currentTurnCharacter.Id != characters.LastOrDefault().Id)
                {
                    characters[currentTurnCharacterIndex + 1].Turn = true;
                }
                else
                {
                    characters.FirstOrDefault().Turn = true;
                }
            }
            encounter.Characters = characters;
            return RedirectToAction("Details", "Encounter", encounter);
        }
    }
}