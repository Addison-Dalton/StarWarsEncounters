using System;
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
        private readonly EncounterContext _context;

        public CharacterController(EncounterContext context)
        {
            _context = context;
        }

        public ActionResult CreateCharacter(int encounterId)
        {
            ViewBag.EncounterId = encounterId;
            return PartialView("Add");
        }
        
        public ActionResult Add(Character character, int encounterId)
        {
            if (ModelState.IsValid)
            {
                character.EncounterId = encounterId;
                character.Encounter = _context.Encounters.Where(e => e.Id.Equals(encounterId)).SingleOrDefault();
                character.SetIniativeScore();
                //if added character has a greater iniativeScore than the current character with greatest iniativeScore, then set added character turn to true

                if(_context.Characters.ToList().Count <= 0)
                {
                    character.Turn = true;
                }
                else
                {
                    List<Character> characters = _context.Characters.ToList();
                    if (character.IniativeScore > _context.Characters.OrderByDescending(c => c.IniativeScore).First().IniativeScore)
                    {
                        foreach(Character characterInDB in characters)
                        {
                            characterInDB.Turn = false;
                        }
                        character.Turn = true;
                    }
                }
                _context.Characters.Add(character);
                _context.SaveChanges();
                return RedirectToAction("Details", "Encounter", new {encounterId});
            }
            return PartialView();
        }


        public ActionResult Edit(Character character)
        {
            if (ModelState.IsValid)
            {
                Character oldCharacter = _context.Characters.Where(c => c.Id.Equals(character.Id)).SingleOrDefault();
                
                //if the number if triupmhs, successes, or advantages have changed, update the IniativeScore
                if(oldCharacter.Triumphs != character.Triumphs || oldCharacter.Succeses != character.Succeses || oldCharacter.Advantages != character.Advantages)
                {
                    _context.Entry(oldCharacter).CurrentValues.SetValues(character);
                    oldCharacter.SetIniativeScore();
                    if (oldCharacter.IniativeScore >= _context.Characters.OrderByDescending(c => c.IniativeScore).First().IniativeScore)
                    {
                        _context.Characters.OrderByDescending(c => c.IniativeScore).First().Turn = false;
                        oldCharacter.Turn = true;
                    }
                }
                else
                {
                    _context.Entry(oldCharacter).CurrentValues.SetValues(character);
                }
                _context.SaveChanges();

                return RedirectToAction("Details", "Encounter", new {encounterId = character.EncounterId });
            }
            return PartialView("Details", character);
        }

        public ActionResult Details(int characterId)
        {
            Character character = _context.Characters.Where(c => c.Id.Equals(characterId)).SingleOrDefault();
            _context.Entry(character).Reload();
            return PartialView("Details", character);
        }

        public ActionResult Delete(int characterId)
        {
            Character character = _context.Characters.Where(c => c.Id == characterId).SingleOrDefault();
            List<Character> characters = _context.Characters.ToList().OrderByDescending(c => c.IniativeScore).ToList();
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

            _context.Characters.Remove(character);
            _context.SaveChanges();
            return RedirectToAction("Details", "Encounter", new { encounterId = character.EncounterId });
        }

        public ActionResult ChangeTurn(string direction)
        {
            const string NEXT = "next";
            const string PREV = "prev";
            List<Character> characters = _context.Characters.OrderByDescending(c => c.IniativeScore).ToList();
            Character currentTurnCharacter = _context.Characters.Where(c => c.Turn == true).SingleOrDefault();
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
            _context.SaveChanges();
            return RedirectToAction("Details", "Encounter", new { encounterId = currentTurnCharacter.EncounterId });
        }
    }
}