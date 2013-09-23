using System;
using System.Collections.Generic;
using Styx.Common;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace TuanHARogue
{
    public partial class Classname
    {
        #region Disorients

        private static readonly HashSet<int> DisorientsHS = new HashSet<int>
            {
                //-- Druid
                2637, // "disorient", -- Hibernate
                99, // "disorient", -- Disorienting Roar (talent)
                //-- Hunter
                3355, // "disorient", -- Freezing Trap
                19386, // "disorient", -- Wyvern Sting
                //-- Mage
                118, // "disorient", -- Polymorph
                28272, // "disorient", -- Polymorph (pig)
                28271, // "disorient", -- Polymorph (turtle)
                61305, // "disorient", -- Polymorph (black cat)
                61025, // "disorient", -- Polymorph (serpent) -- FIXME: gone ?
                61721, // "disorient", -- Polymorph (rabbit)
                61780, // "disorient", -- Polymorph (turkey)
                82691, // "disorient", -- Ring of Frost
                //-- Monk
                115078, // "disorient", -- Paralysis
                //-- Paladin
                20066, // "disorient", -- Repentance
                //-- Priest
                9484, // "disorient", -- Shackle Undead
                //-- Rogue
                1776, // "disorient", -- Gouge
                6770, // "disorient", -- Sap
                //-- Shaman
                51514, // "disorient", -- Hex
                //-- Pandaren
                107079, // "disorient", -- Quaking Palm
            };

        private static Dictionary<WoWUnit, DateTime> DisorientsExpire = new Dictionary<WoWUnit, DateTime>();
        private static Dictionary<WoWUnit, int> DisorientsLevel = new Dictionary<WoWUnit, int>();

        private static void DisorientsAdd(WoWUnit unit)
        {
            if (!DisorientsExpire.ContainsKey(unit))
            {
                //Logging.Write("Add {0} to DisorientsExpire expire at {1}", SafeName(unit),
                //DateTime.Now + TimeSpan.FromSeconds(14));
                DisorientsExpire.Add(unit, DateTime.Now + TimeSpan.FromSeconds(14));
            }
            else
            {
                //Logging.Write("{0} already exist, remove it", unit.Name);
                DisorientsExpire.Remove(unit);
                //Logging.Write("Update {0} to DisorientsExpire expire in {1}", SafeName(unit),
                //DateTime.Now + TimeSpan.FromSeconds(14));
                DisorientsExpire.Add(unit, DateTime.Now + TimeSpan.FromSeconds(14));
            }

            if (!DisorientsLevel.ContainsKey(unit))
            {
                DisorientsLevel.Add(unit, 1);
                Logging.Write("Disorients DR Level {1} on {0}", SafeName(unit), 1);
            }
            else
            {
                var currentlevel = DisorientsLevel[unit];
                //Logging.Write("{0} already exist, remove it", SafeName(unit));
                DisorientsLevel.Remove(unit);
                DisorientsLevel.Add(unit, currentlevel + 1);
                Logging.Write("Disorients DR Level {1} on {0}", SafeName(unit), currentlevel + 1);
            }
        }

        private static List<WoWUnit> DisorientsRemoveList = new List<WoWUnit>();

        private static void DisorientsRemove()
        {
            DisorientsRemoveList.Clear();
            foreach (var unit in DisorientsExpire)
            {
                if (unit.Value < DateTime.Now)
                {
                    DisorientsRemoveList.Add(unit.Key);
                }
            }

            foreach (var unit in DisorientsRemoveList)
            {
                if (unit == null || !unit.IsValid)
                {
                    continue;
                }

                if (DisorientsExpire.ContainsKey(unit))
                {
                    DisorientsExpire.Remove(unit);
                }

                if (DisorientsLevel.ContainsKey(unit))
                {
                    DisorientsLevel.Remove(unit);
                }
                Logging.Write("Disorients DR expired on {0}", SafeName(unit));
            }
        }

        private static int DisorientsLevelGet(WoWUnit unit)
        {
            if (unit == null || !unit.IsValid)
            {
                return 0;
            }

            if (DisorientsLevel.ContainsKey(unit))
            {
                return DisorientsLevel[unit];
            }
            return 0;
        }

        #endregion

        #region ControlStun

        private static readonly HashSet<int> ControlStunHS = new HashSet<int>
            {
                //Death Knight
                108194, // "ctrlstun", // Asphyxiate (talent)
                91800, // "ctrlstun", // Gnaw (Ghoul)
                91797, // "ctrlstun", // Monstrous Blow (Dark Transformation Ghoul)
                115001, // "ctrlstun", // Remorseless Winter (talent)
                // Druid
                22570, // "ctrlstun", // Maim
                9005, // "ctrlstun", // Pounce
                5211, // "ctrlstun", // Mighty Bash (talent)
                102795, // "ctrlstun", // Bear Hug
                113801,
                // "ctrlstun", // Bash (treants in feral spec) (Bugged by blizzard - it instantly applies all 3 levels of DR right now, making any target instantly immune to ctrlstuns)
                // Hunter
                24394, // "ctrlstun", // Intimidation
                90337, // "ctrlstun", // Bad Manner (Monkey)
                50519, // "ctrlstun", // Sonic Blast (Bat)
                //  56626,// "ctrlstun", // Sting (Wasp) //FIXME: this doesn't share with ctrlstun anymore. Unknown what it is right now, so watch for it on www.arenajunkies.com/topic/227748-mop-diminishing-returns-updating-the-list
                117526, // "ctrlstun", // Binding Shot (talent)
                96201, // "ctrlstun", // Web Wrap (Shale Spider)
                // Mage
                44572, // "ctrlstun", // Deep Freeze
                118271, // "ctrlstun", // Combustion Impact (Combustion; Fire)
                // Monk
                119392, // "ctrlstun", // Charging Ox Wave (talent)
                119381, // "ctrlstun", // Leg Sweep (talent)
                122242, // "ctrlstun", // Clash (Brewmaster)
                120086, // "ctrlstun", // Fists of Fury (Windwalker)
                // Paladin
                853, // "ctrlstun", // Hammer of Justice
                110698, // "ctrlstun", // Hammer of Justice (Symbiosis)
                119072, // "ctrlstun", // Holy Wrath (Protection)
                105593, // "ctrlstun", // Fist of Justice (talent)
                115752, // "ctrlstun", // Glyph of Blinding Light
                // Priest
                //  88625,// "ctrlstun", // Holy Word: Chastise //FIXME: this doesn't share with ctrlstun anymore. Unknown what it is right now, so watch for it on www.arenajunkies.com/topic/227748-mop-diminishing-returns-updating-the-list
                // Rogue
                1833, // "ctrlstun", // Cheap Shot
                408, // "ctrlstun", // Kidney Shot
                // Shaman
                118905, // "ctrlstun", // Static Charge (Capacitor Totem)
                // Warlock
                30283, // "ctrlstun", // Shadowfury
                89766, // "ctrlstun", // Axe Toss (Felguard)
                //  22703,// "ctrlstun", // Infernal Awakening (Infernal) // According to the DR thread on AJ, this doesn't have DR at all.
                // Warrior
                132168, // "ctrlstun", // Shockwave
                105771, // "ctrlstun", // Warbringer (talent)
                // Tauren
                20549, // "ctrlstun", // War Stomp
            };

        private static Dictionary<WoWUnit, DateTime> ControlStunExpire = new Dictionary<WoWUnit, DateTime>();
        private static Dictionary<WoWUnit, int> ControlStunLevel = new Dictionary<WoWUnit, int>();

        private static void ControlStunAdd(WoWUnit unit)
        {
            if (!ControlStunExpire.ContainsKey(unit))
            {
                //Logging.Write("Add {0} to ControlStunExpire expire at {1}", SafeName(unit),
                //              (DateTime.Now + TimeSpan.FromSeconds(14)).ToString("ss:fff"));
                ControlStunExpire.Add(unit, DateTime.Now + TimeSpan.FromSeconds(14));
            }
            else
            {
                //Logging.Write("ControlStunExpire: {0} already exist, remove it", SafeName(unit));
                ControlStunExpire.Remove(unit);

                //foreach (var unitleft in ControlStunExpire)
                //{
                //    Logging.Write("ControlStunExpire still Contain {0} - {1}", unitleft.Key.Name, unitleft.Value);
                //}

                //Logging.Write("Update {0} to ControlStunExpire expire in {1}", SafeName(unit),
                //              (DateTime.Now + TimeSpan.FromSeconds(14)).ToString("ss:fff"));
                ControlStunExpire.Add(unit, DateTime.Now + TimeSpan.FromSeconds(14));
            }

            if (!ControlStunLevel.ContainsKey(unit))
            {
                ControlStunLevel.Add(unit, 1);
                Logging.Write("ControlStun DR Level {1} on {0}", SafeName(unit), 1);
            }
            else
            {
                var currentlevel = ControlStunLevel[unit];
                //Logging.Write("ControlStunLevel {0} already exist, remove it", SafeName(unit));
                ControlStunLevel.Remove(unit);

                //foreach (var unitleft in ControlStunLevel)
                //{
                //    Logging.Write("ControlStunExpire still Contain {0} - {1}", unitleft.Key.Name, unitleft.Value);
                //}

                ControlStunLevel.Add(unit, currentlevel + 1);
                Logging.Write("ControlStun DR Level {1} on {0}", SafeName(unit), currentlevel + 1);
            }
        }

        private static List<WoWUnit> ControlStunRemoveList = new List<WoWUnit>();

        private static void ControlStunRemove()
        {
            ControlStunRemoveList.Clear();
            foreach (var unit in ControlStunExpire)
            {
                if (unit.Value < DateTime.Now)
                {
                    ControlStunRemoveList.Add(unit.Key);
                }
            }

            foreach (var unit in ControlStunRemoveList)
            {
                if (unit == null || !unit.IsValid)
                {
                    continue;
                }

                if (ControlStunExpire.ContainsKey(unit))
                {
                    ControlStunExpire.Remove(unit);
                }

                if (ControlStunLevel.ContainsKey(unit))
                {
                    ControlStunLevel.Remove(unit);
                }
                Logging.Write("ControlStun DR expired on {0}", SafeName(unit));
            }
        }

        private static int ControlStunLevelGet(WoWUnit unit)
        {
            if (unit == null || !unit.IsValid)
            {
                return 0;
            }

            if (ControlStunLevel.ContainsKey(unit))
            {
                return ControlStunLevel[unit];
            }
            return 0;
        }

        #endregion

        #region HandleCombatLogTH

        private static void HandleCombatLogTH(object sender, LuaEventArgs args)
        {
            var e = new CombatLogEventArgs(args.EventName, args.FireTimeStamp, args.Args);

            if (e.DestUnit == null ||
                e.Event != "SPELL_AURA_REFRESH" &&
                e.Event != "SPELL_AURA_APPLIED" ||
                !IsEnemy(e.DestUnit) ||
                !DisorientsHS.Contains(e.SpellId) &&
                !ControlStunHS.Contains(e.SpellId))
                return;

            if (DisorientsHS.Contains(e.SpellId))
            {
                DisorientsAdd(e.DestUnit);
                //Logging.Write(DateTime.Now.ToString("ss:fff") + " " +
                //              e.DestName +
                //              " got DisorientsHS "
                //              + e.SpellName);
            }

            if (ControlStunHS.Contains(e.SpellId))
            {
                ControlStunAdd(e.DestUnit);
                //Logging.Write(DateTime.Now.ToString("ss:fff") + " " +
                //              e.DestName +
                //              " got ControlStunHS "
                //              + e.SpellName);
            }

            //Logging.Write("[CombatLogAll] " + e.Event + " - " + e.SourceName + " - " + e.SpellName);

            //switch (e.Event)
            //{
            //        //default:
            //        //    Logging.Write("[CombatLog] filter out this event - " + e.Event + " - " + e.SourceName + " - " +
            //        //                  e.SpellName);
            //        //    break;

            //    case "SPELL_AURA_APPLIED":
            //        Logging.Write(e.DestName + " got SPELL_AURA_APPLIED " + e.SpellName);
            //        break;
            //        //case "SPELL_AURA_REFRESH":
            //        //    Logging.Write(e.SourceUnit.Name + " got SPELL_AURA_REFRESH " + e.SpellName);
            //        //    break;
            //        //case "SPELL_AURA_REMOVED":
            //        //    Logging.Write(e.SourceUnit.Name + " got SPELL_AURA_REFRESH " + e.SpellName);
            //        //    break;
            //}
        }

        #endregion

        #region AttachCombatLogEventTH

        private static bool CombatLogAttachedTH;

        private static void AttachCombatLogEventTH()
        {
            if (CombatLogAttachedTH)
                return;

            Lua.Events.AttachEvent("COMBAT_LOG_EVENT_UNFILTERED", HandleCombatLogTH);

            string filterCriteria =
                "return args[2] == 'SPELL_AURA_APPLIED')";
            //+ " or args[2] == 'SPELL_AURA_REFRESH'"
            //+ " or args[2] == 'SPELL_AURA_REMOVED')";

            //string filterCriteria =
            //    "return args[4] == UnitGUID('player')"
            //    + " and (args[2] == 'SPELL_MISSED'"
            //    + " or args[2] == 'RANGE_MISSED'"
            //    + " or args[2] == 'SPELL_AURA_APPLIED'"
            //    + " or args[2] == 'SPELL_AURA_REFRESH'"
            //    + " or args[2] == 'SPELL_AURA_REMOVED'"
            //    + " or args[2] == 'SWING_MISSED'"
            //    + " or args[2] == 'SPELL_CAST_START'"
            //    + " or args[2] == 'SPELL_CAST_SUCCESS'"
            //    + " or args[2] == 'SPELL_CAST_FAILED')";

            if (!Lua.Events.AddFilter("COMBAT_LOG_EVENT_UNFILTERED", filterCriteria))
            {
                Logging.Write(
                    "ERROR: Could not add combat log event filter! - Performance may be horrible, and things may not work properly!");
            }

            Logging.Write("Attached AttachCombatLogEventTH");

            CombatLogAttachedTH = true;
        }

        #endregion

        #region DetachCombatLogEventTH

        private static void DetachCombatLogEventTH()
        {
            if (!CombatLogAttachedTH)
                return;

            Logging.Write("DetachCombatLogEventTH");
            Logging.Write("Removed combat log filter");
            Lua.Events.RemoveFilter("COMBAT_LOG_EVENT_UNFILTERED");
            Logging.Write("Detached combat log");
            Lua.Events.DetachEvent("COMBAT_LOG_EVENT_UNFILTERED", HandleCombatLogTH);

            CombatLogAttachedTH = false;
        }

        #endregion
    }
}