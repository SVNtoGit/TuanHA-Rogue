using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Styx;
using Styx.Common;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace TuanHARogue
{
    public partial class Classname
    {
        #region BuffBurst

        private static readonly HashSet<string> BuffBurstHS = new HashSet<string>
            {
                "Pillar of Frost",
                "Unholy Frenzy",
                "Incarnation: Chosen of Elune",
                "Nature's Vigil",
                "Berserk ",
                "Bestial Wrath",
                "Rapid Fire",
                "Icy Veins",
                //"Time Warp",
                //"Tigereye Brew",
                "Avenging Wrath",
                "Holy Avenger",
                "Guardian of Ancient Kings",
                "Shadow Dance",
                "Killing Spree",
                "Ascendance",
                //"Bloodlust",
                "Dark Soul: Instability",
                "Recklessness",
                "Skull Banner",
            };

        private static bool BuffBurst(WoWUnit target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            using (StyxWoW.Memory.AcquireFrame())
            {
                if (target.ActiveAuras.Any(a => BuffBurstHS.Contains(a.Value.Name)))
                {
                    //Logging.Write(target.Name + " got BuffHeal");
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region BuffHeal

        private static readonly HashSet<string> BuffHealHS = new HashSet<string>
            {
                "Rejuvenation",
                "Regrowth",
                "Lifebloom",
                "Wild Growth",
//"Recuperate",
                "Riptide",
                "Earth Shield",
                "Earthliving",
                "Renew",
                "Echo of Light",
                "Divine Aegis",
                "Living Seed",
//"Blood Shield",
                "Renewing Mist",
                "Enveloping Mist",
                "Soothing Mist",
                "Zen Sphere",
                "Power Word: Shield",
                "Pain Suppression",
                "Illuminated Healing",
                "Sacred Shield"
                //"Enraged Regeneratione")
            };

        private static bool BuffHeal(WoWUnit target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            using (StyxWoW.Memory.AcquireFrame())
            {
                if (target.ActiveAuras.Any(a => BuffHealHS.Contains(a.Value.Name)))
                {
                    //Logging.Write(target.Name + " got BuffHeal");
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region BuffEnrage

        private static readonly HashSet<string> BuffEnrageHS = new HashSet<string>
            {
                "Enraged",
                "Berserker Rage",
                "Enraged Regeneratione",
            };

        private static bool BuffEnrage(WoWUnit target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            using (StyxWoW.Memory.AcquireFrame())
            {
                if (target.ActiveAuras.Any(
                    a =>
                    BuffEnrageHS.Contains(a.Value.Name) ||
                    a.Value.Spell.DispelType == WoWDispelType.Enrage))
                {
                    //Logging.Write(target.Name + " got BuffHeal");
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region DebuffCC

        private static readonly HashSet<int> DebuffCCHS = new HashSet<int>
            {
                30217, //Adamantite Grenade
                89766, //Axe Toss (Felguard/Wrathguard)
                90337, //Bad Manner (Monkey)
                710, //Banish
                113801, //Bash (Force of Nature - Feral Treants)
                102795, //Bear Hug
                76780, //Bind Elemental
                117526, //Binding Shot
                2094, //Blind
                105421, //Blinding Light
                115752, //Blinding Light (Glyph of Blinding Light)
                123393, //Breath of Fire (Glyph of Breath of Fire)
                126451, //Clash
                122242, //Clash (not sure which one is right)
                67769, //Cobalt Frag Bomb
                118271, //Combustion Impact
                33786, //Cyclone
                113506, //Cyclone (Symbiosis)
                7922, //Charge Stun
                119392, //Charging Ox Wave
                1833, //Cheap Shot
                44572, //Deep Freeze
                54786, //Demonic Leap (Metamorphosis)
                99, //Disorienting Roar
                605, //Dominate Mind
                118895, //Dragon Roar
                31661, //Dragon's Breath
                77505, //Earthquake
                5782, //Fear
                118699, //Fear
                130616, //Fear (Glyph of Fear)
                30216, //Fel Iron Bomb
                105593, //Fist of Justice
                117418, //Fists of Fury
                3355, //Freezing Trap
                91800, //Gnaw
                1776, //Gouge
                853, //Hammer of Justice
                110698, //Hammer of Justice (Paladin)
                51514, //Hex
                2637, //Hibernate
                88625, //Holy Word: Chastise
                119072, //Holy Wrath
                5484, //Howl of Terror
                22703, //Infernal Awakening
                113056, //Intimidating Roar [Cowering in fear] (Warrior)
                113004, //Intimidating Roar [Fleeing in fear] (Warrior)
                5246, //Intimidating Shout (aoe)
                20511, //Intimidating Shout (targeted)
                24394, //Intimidation
                408, //Kidney Shot
                119381, //Leg Sweep
                126246, //Lullaby (Crane)
                22570, //Maim
                115268, //Mesmerize (Shivarra)
                5211, //Mighty Bash
                91797, //Monstrous Blow (Dark Transformation)
                6789, //Mortal Coil
                115078, //Paralysis
                113953, //Paralysis (Paralytic Poison)
                126355, //Paralyzing Quill (Porcupine)
                126423, //Petrifying Gaze (Basilisk)
                118, //Polymorph
                61305, //Polymorph: Black Cat
                28272, //Polymorph: Pig
                61721, //Polymorph: Rabbit
                61780, //Polymorph: Turkey
                28271, //Polymorph: Turtle
                9005, //Pounce
                102546, //Pounce (Incarnation)
                64044, //Psychic Horror
                8122, //Psychic Scream
                113792, //Psychic Terror (Psyfiend)
                118345, //Pulverize
                107079, //Quaking Palm
                13327, //Reckless Charge
                115001, //Remorseless Winter
                20066, //Repentance
                82691, //Ring of Frost
                6770, //Sap
                1513, //Scare Beast
                19503, //Scatter Shot
                132412, //Seduction (Grimoire of Sacrifice)
                6358, //Seduction (Succubus)
                9484, //Shackle Undead
                30283, //Shadowfury
                132168, //Shockwave
                87204, //Sin and Punishment
                104045, //Sleep (Metamorphosis)
                50519, //Sonic Blast (Bat)
                118905, //Static Charge (Capacitor Totem)
                56626, //Sting (Wasp)
                107570, //Storm Bolt
                10326, //Turn Evil
                20549, //War Stomp
                105771, //Warbringer
                19386, //Wyvern Sting
                108194, //Asphyxiate
            };

        private static bool DebuffCC(WoWUnit target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }


            using (StyxWoW.Memory.AcquireFrame())
            {
                if (target.ActiveAuras.Any(a => DebuffCCHS.Contains(a.Value.SpellId)))
                {
                    //Logging.Write(target.Name + " got DebuffCC");
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region DebuffCCDuration

        private static readonly HashSet<int> DebuffCCDurationHS = new HashSet<int>
            {
                30217, //Adamantite Grenade
                89766, //Axe Toss (Felguard/Wrathguard)
                90337, //Bad Manner (Monkey)
                710, //Banish
                113801, //Bash (Force of Nature - Feral Treants)
                102795, //Bear Hug
                76780, //Bind Elemental
                117526, //Binding Shot
                2094, //Blind
                105421, //Blinding Light
                115752, //Blinding Light (Glyph of Blinding Light)
                123393, //Breath of Fire (Glyph of Breath of Fire)
                126451, //Clash
                122242, //Clash (not sure which one is right)
                67769, //Cobalt Frag Bomb
                118271, //Combustion Impact
                33786, //Cyclone
                113506, //Cyclone (Symbiosis)
                7922, //Charge Stun
                119392, //Charging Ox Wave
                1833, //Cheap Shot
                44572, //Deep Freeze
                54786, //Demonic Leap (Metamorphosis)
                99, //Disorienting Roar
                605, //Dominate Mind
                118895, //Dragon Roar
                31661, //Dragon's Breath
                77505, //Earthquake
                5782, //Fear
                118699, //Fear
                130616, //Fear (Glyph of Fear)
                30216, //Fel Iron Bomb
                105593, //Fist of Justice
                117418, //Fists of Fury
                3355, //Freezing Trap
                91800, //Gnaw
                1776, //Gouge
                853, //Hammer of Justice
                110698, //Hammer of Justice (Paladin)
                51514, //Hex
                2637, //Hibernate
                88625, //Holy Word: Chastise
                119072, //Holy Wrath
                5484, //Howl of Terror
                22703, //Infernal Awakening
                113056, //Intimidating Roar [Cowering in fear] (Warrior)
                113004, //Intimidating Roar [Fleeing in fear] (Warrior)
                5246, //Intimidating Shout (aoe)
                20511, //Intimidating Shout (targeted)
                24394, //Intimidation
                408, //Kidney Shot
                119381, //Leg Sweep
                126246, //Lullaby (Crane)
                22570, //Maim
                115268, //Mesmerize (Shivarra)
                5211, //Mighty Bash
                91797, //Monstrous Blow (Dark Transformation)
                6789, //Mortal Coil
                115078, //Paralysis
                113953, //Paralysis (Paralytic Poison)
                126355, //Paralyzing Quill (Porcupine)
                126423, //Petrifying Gaze (Basilisk)
                118, //Polymorph
                61305, //Polymorph: Black Cat
                28272, //Polymorph: Pig
                61721, //Polymorph: Rabbit
                61780, //Polymorph: Turkey
                28271, //Polymorph: Turtle
                9005, //Pounce
                102546, //Pounce (Incarnation)
                64044, //Psychic Horror
                8122, //Psychic Scream
                113792, //Psychic Terror (Psyfiend)
                118345, //Pulverize
                107079, //Quaking Palm
                13327, //Reckless Charge
                115001, //Remorseless Winter
                20066, //Repentance
                82691, //Ring of Frost
                6770, //Sap
                1513, //Scare Beast
                19503, //Scatter Shot
                132412, //Seduction (Grimoire of Sacrifice)
                6358, //Seduction (Succubus)
                9484, //Shackle Undead
                30283, //Shadowfury
                132168, //Shockwave
                87204, //Sin and Punishment
                104045, //Sleep (Metamorphosis)
                50519, //Sonic Blast (Bat)
                118905, //Static Charge (Capacitor Totem)
                56626, //Sting (Wasp)
                107570, //Storm Bolt
                10326, //Turn Evil
                20549, //War Stomp
                105771, //Warbringer
                19386, //Wyvern Sting
                108194, //Asphyxiate
            };

        private static bool DebuffCCDuration(WoWUnit target, double duration)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            using (StyxWoW.Memory.AcquireFrame())
            {
                if (
                    target.ActiveAuras.Any(
                        a =>
                        DebuffCCDurationHS.Contains(a.Value.SpellId) &&
                        a.Value.TimeLeft.TotalMilliseconds > duration))
                {
                    //Logging.Write(target.Name + " got DebuffCCDuration > " + duration);
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region DebuffCCleanseASAP

        private static readonly HashSet<int> DebuffCCleanseASAPHS = new HashSet<int>
            {
                105421, //Blinding Light
                123393, //Breath of Fire (Glyph of Breath of Fire)
                44572, //Deep Freeze
                605, //Dominate Mind
                31661, //Dragon's Breath
                5782, // target.Class != WoWClass.Warrior || //Fear
                118699, // target.Class != WoWClass.Warrior || //Fear
                130616, // target.Class != WoWClass.Warrior || //Fear (Glyph of Fear)
                3355, //Freezing Trap
                853, //Hammer of Justice
                110698, //Hammer of Justice (Paladin)
                2637, //Hibernate
                88625, //Holy Word: Chastise
                119072, //Holy Wrath
                5484, // target.Class != WoWClass.Warrior || //Howl of Terror
                115268, //Mesmerize (Shivarra)
                6789, //Mortal Coil
                115078, //Paralysis
                113953, //Paralysis (Paralytic Poison)
                126355, //Paralyzing Quill (Porcupine)
                118, //Polymorph
                61305, //Polymorph: Black Cat
                28272, //Polymorph: Pig
                61721, //Polymorph: Rabbit
                61780, //Polymorph: Turkey
                28271, //Polymorph: Turtle
                64044, // target.Class != WoWClass.Warrior || //Psychic Horror
                8122, // target.Class != WoWClass.Warrior || //Psychic Scream
                113792, // target.Class != WoWClass.Warrior || //Psychic Terror (Psyfiend)
                107079, //Quaking Palm
                115001, //Remorseless Winter
                20066, // target.Class != WoWClass.Warrior || //Repentance
                82691, //Ring of Frost
                1513, //Scare Beast
                132412, //Seduction (Grimoire of Sacrifice)
                6358, //Seduction (Succubus)
                9484, //Shackle Undead
                30283, //Shadowfury
                87204, //Sin and Punishment
                104045, //Sleep (Metamorphosis)
                118905, //Static Charge (Capacitor Totem)
                10326, //Turn Evil
                19386, //Wyvern Sting //Thank bp423
                118552, //Flesh to Stone" //Thank bp423
                119985, //Dread Spray" //Thank mnipper
                117436, //Lightning Prison
                124863, //Visions of Demise
                123011, //Terrorize (10%)
                123012, //Terrorize (5%)
//Farraki
                136708, //Stone Gaze - Thank Clubwar
                136719, //Blazing Sunlight - Thank Clubwar && bp423
//Gurubashi
                136587, //Venom Bolt Volley - Thank Clubwar
//Drakaki
                136710, //Deadly Plague - Thank Clubwar
//Amani
                136512, //Hex of Confusion - Thank Clubwar
                136857, //Entrapped - Thank amputations
                136185, //Fragile Bones - Thank Sk1vvy 
                136187, //Clouded Mind - Thank Sk1vvy 
                136183, //Dulled Synapses - Thank Sk1vvy 
                136181, //Impaired Eyesight - Thank Sk1vvy 
                138040, //Horrific Visage - Thank macVsog
                117949 //Closed Curcuit
            };

        private static readonly HashSet<int> DebuffCCleanseASAPHSWarrior = new HashSet<int>
            {
                5782,
                118699,
                130616,
                64044,
                8122,
                113792
            };

        private static bool DebuffCCleanseASAP(WoWUnit target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            if (target.IsPlayer &&
                target.Class == WoWClass.Warrior &&
                target.ActiveAuras.Any(
                    a =>
                    DebuffCCleanseASAPHSWarrior.Contains(a.Value.SpellId)))
            {
                return false;
            }


            using (StyxWoW.Memory.AcquireFrame())
            {
                if (target.ActiveAuras.Any(
                    a =>
                    DebuffCCleanseASAPHS.Contains(a.Value.SpellId) &&
                    a.Value.TimeLeft.TotalMilliseconds > 3000))
                {
                    //Logging.Write(target.Name + " got DebuffCCCleanseASAP");
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region DebuffCCBreakonDamage

        private static readonly HashSet<int> DebuffCCBreakonDamageHS = new HashSet<int>
            {
                2094, //Blind
                105421, //Blinding Light
                99, //Disorienting Roar
                31661, //Dragon's Breath
                3355, //Freezing Trap
                1776, //Gouge
                2637, //Hibernate
                115268, //Mesmerize (Shivarra)
                115078, //Paralysis
                //113953, //Paralysis (Paralytic Poison)
                126355, //Paralyzing Quill (Porcupine)
                126423, //Petrifying Gaze (Basilisk)
                118, //Polymorph
                61305, //Polymorph: Black Cat
                28272, //Polymorph: Pig
                61721, //Polymorph: Rabbit
                61780, //Polymorph: Turkey
                28271, //Polymorph: Turtle
                20066, //Repentance
                6770, //Sap
                19503, //Scatter Shot
                132412, //Seduction (Grimoire of Sacrifice)
                6358, //Seduction (Succubus)
                104045, //Sleep (Metamorphosis)
                19386, //Wyvern Sting
            };

        private static bool DebuffCCBreakonDamage(WoWUnit target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }


            using (StyxWoW.Memory.AcquireFrame())
            {
                //if (MyAura(6770, target) &&
                //    target.Distance < 5 &&
                //    PlayerEnergy > Me.MaxEnergy - 20)
                //{
                //    return false;
                //}

                if (target.ActiveAuras.Any(a => DebuffCCBreakonDamageHS.Contains(a.Value.SpellId)))
                {
                    //Logging.Write(target.Name + " got DebuffCCBreakonDamage");
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region DebuffDisarm

        private static readonly HashSet<int> DebuffDisarmHS = new HashSet<int>
            {
                50541, //Clench (Scorpid)
                676, //Disarm
                118093, //Disarm (Voidwalker/Voidlord)
                51722, //Dismantle
                117368, //Grapple Weapon
                126458, //Grapple Weapon (Monk)
                64058, //Psychic Horror
                91644, //Snatch (Bird of Prey)
            };

        private static bool DebuffDisarm(WoWUnit target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            using (StyxWoW.Memory.AcquireFrame())
            {
                if (target.ActiveAuras.Any(
                    a =>
                    a.Value.ApplyAuraType == WoWApplyAuraType.ModDisarm ||
                    a.Value.ApplyAuraType == WoWApplyAuraType.ModDisarmRanged ||
                    a.Value.ApplyAuraType == WoWApplyAuraType.ModDisarmShield ||
                    DebuffDisarmHS.Contains(a.Value.SpellId)))
                {
                    //Logging.Write(target.Name + " got DebuffDisarm");
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region DebuffDoNotHeal

        private static readonly HashSet<string> DebuffDoNotHealHS = new HashSet<string>
            {
                "Dominate Mind",
                "Cyclone",
                "Dissonance Field",
                "Parasitic Growth",
                "Beast of Nightmares",
                "Corrupted Healing", //Grapple Weapon (Monk)
                "Reshape Life"
            };

        private static bool DebuffDoNotHeal(WoWUnit target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            //using (StyxWoW.Memory.AcquireFrame())
            //{
            //    return target.HasAura("Dominate Mind") ||
            //           target.HasAura("Cyclone") ||
            //           target.HasAura(123184) || //Dissonance Field 123184unit.HasAura(123184)
            //           target.HasAura(121949) || // / Parasitic Growth
            //           target.HasAura(137341) || //Beast of Nightmares
            //           target.HasAura("Beast of Nightmares") || //Beast of Nightmares
            //           target.HasAura(137360) || //Corrupted Healing
            //           target.HasAura("Reshape Life");
            //}

            using (StyxWoW.Memory.AcquireFrame())
            {
                if (target.ActiveAuras.Any(a => DebuffDoNotHealHS.Contains(a.Value.Name)))
                {
                    //Logging.Write(target.Name + " got DebuffDisarm");
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region DebuffDoNotCleanse

        private static readonly HashSet<string> DebuffDoNotCleanseHS = new HashSet<string>
            {
                "Flame Shock",
                "Thick Bones",
                "Clear Mind",
                "Keen Eyesight",
                "Improved Synapses",
                "Unstable Affliction", //Grapple Weapon (Monk)
                "Matter Swap",
                "Ionization",
                "Vampiric Touch"
            };

        private static bool DebuffDoNotCleanse(WoWUnit target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            //using (StyxWoW.Memory.AcquireFrame())
            //{
            //    return target.HasAura("Flame Shock") ||
            //           target.HasAura(136184) || //Thick Bones
            //           target.HasAura(136186) || //Clear Mind
            //           target.HasAura(136180) || //Keen Eyesight
            //           target.HasAura(136182) || //Improved Synapses
            //           target.HasAura("Unstable Affliction") ||
            //           target.HasAura(138609) || // Matter Swap
            //           target.HasAura(138732) || //Ionization
            //           target.HasAura("Vampiric Touch");
            //}

            using (StyxWoW.Memory.AcquireFrame())
            {
                if (target.ActiveAuras.Any(a => DebuffDoNotCleanseHS.Contains(a.Value.Name)))
                {
                    //Logging.Write(target.Name + " got DebuffDisarm");
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region DebuffDot

        /// <summary>
        /// Credit worklifebalance http://www.thebuddyforum.com/honorbuddy-forum/developer-forum/113473-wowapplyauratype-periodicleech-temporal-displacement.html#post1107923
        /// </summary>
        private static DateTime DebuffDotLast;

        private static HashSet<WoWApplyAuraType> HS_HasAuraTypeDOT = new HashSet<WoWApplyAuraType>()
            {
                WoWApplyAuraType.PeriodicDamage,
                WoWApplyAuraType.PeriodicDamagePercent,
                WoWApplyAuraType.PeriodicLeech
            };

        private static HashSet<int> HS_HasAuraDOT = new HashSet<int>()
            {
                95223, //Mass Resurrected    
                3674, //Black Arrow
                71328, //Dungeon Cooldown 
                1822, //Rake
                44457, //Living Bomb
                106830, //Thrash
                45181, //Cheated Death  
                11366, //Pyroblast 
                119611, //Renewing Mist 
                118253, //Serpent Sting
                118283, //Ursol's Vortex  
                53651, //Light's Beacon 
                57934, //Tricks of the Trade 
                100340, //CSA Area Trigger Dummy Timer
                57723, //Exhaustion
                57724, //Sated
                80354, //Temporal Displacement
                25771, //Forbearance
                96223, //Run Speed Marker 
            };

        private static bool DebuffDot(WoWUnit target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            //if (DebuffDotLast < DateTime.Now
            //    && (InArena || InBattleground || Me.IsMoving || CurrentTargetAttackable(15))
            //    && CurrentTargetCheckFacing
            //    && !MeMounted
            //    && !Me.Combat
            //    && !Me.HasAura("Stealth")
            //    && !Me.HasAura("Vanish"))
            //{
            //    DebuffDotLast = DateTime.Now + TimeSpan.FromMilliseconds(3000);

            //    var auraList = target.GetAllAuras();


            //    foreach (var aura in target.ActiveAuras)
            //    {
            //        if (aura.Value.ApplyAuraType == WoWApplyAuraType.PeriodicDamage ||
            //            aura.Value.ApplyAuraType == WoWApplyAuraType.PeriodicDamagePercent ||
            //            aura.Value.ApplyAuraType == WoWApplyAuraType.PeriodicLeech)
            //        {
            //            //Logging.Write("Dot Aura " + aura.Value.Name);
            //            if (aura.Value.ApplyAuraType == WoWApplyAuraType.PeriodicDamage)
            //            {
            //                Logging.Write("Dot Aura PeriodicDamage " + aura.Value.Name + " ID: " + aura.Value.SpellId);
            //            }

            //            if (aura.Value.ApplyAuraType == WoWApplyAuraType.PeriodicDamagePercent)
            //            {
            //                Logging.Write("Dot Aura PeriodicDamagePercent " + aura.Value.Name + " ID: " +
            //                              aura.Value.SpellId);
            //            }

            //            if (aura.Value.ApplyAuraType == WoWApplyAuraType.PeriodicLeech)
            //            {
            //                Logging.Write("Dot Aura PeriodicLeech " + aura.Value.Name + " ID: " + aura.Value.SpellId);
            //            }
            //        }
            //    }

            //CalculateTimeConsumedStart();

            foreach (WoWAura aura in target.GetAllAuras())
            {
                if (!aura.IsActive)
                {
                    continue;
                }

                if (HS_HasAuraDOT.Contains(aura.SpellId))
                {
                    continue;
                }

                if (HS_HasAuraTypeDOT.Contains(aura.ApplyAuraType))
                {
                    Logging.Write("DebuffDot {0} {1} ID: {2}", aura.ApplyAuraType, aura.Name, + aura.SpellId);
                    //CalculateTimeConsumedStop("DebuffDot Found New Method");
                    return true;
                }
            }

            //if (target.Debuffs.Any(
            //    a =>
            //    a.Value.SpellId != 95223 && //Mass Resurrected    
            //    a.Value.SpellId != 3674 && //Black Arrow
            //    a.Value.SpellId != 71328 && //Dungeon Cooldown 
            //    a.Value.SpellId != 1822 && //Rake
            //    a.Value.SpellId != 44457 && //Living Bomb
            //    a.Value.SpellId != 106830 && //Thrash
            //    a.Value.SpellId != 45181 && //Cheated Death  
            //    a.Value.SpellId != 11366 && //Pyroblast 
            //    a.Value.SpellId != 119611 && //Renewing Mist 
            //    a.Value.SpellId != 118253 && //Serpent Sting
            //    a.Value.SpellId != 118283 && //Ursol's Vortex  
            //    a.Value.SpellId != 53651 && //Light's Beacon 
            //    a.Value.SpellId != 57934 && //Tricks of the Trade 
            //    a.Value.SpellId != 100340 && //CSA Area Trigger Dummy Timer
            //    a.Value.SpellId != 57723 && //Exhaustion
            //    a.Value.SpellId != 57724 && //Sated
            //    a.Value.SpellId != 80354 && //Temporal Displacement
            //    (a.Value.ApplyAuraType == WoWApplyAuraType.PeriodicDamage ||
            //     a.Value.ApplyAuraType == WoWApplyAuraType.PeriodicDamagePercent ||
            //     a.Value.ApplyAuraType == WoWApplyAuraType.PeriodicLeech)
            //    ))
            //{
            //    CalculateTimeConsumedStop("DebuffDot");
            //    return true;
            //}

            //CalculateTimeConsumedStop("DebuffDot Found Old Method");
            return false;
        }

        #endregion

        #region DebuffFearDuration

        private static readonly
            HashSet<int> DebuffFearDurationHS = new HashSet<int>
                {
                    5782, //Fear
                    118699, //Fear
                    130616, //Fear (Glyph of Fear)
                    5484, //Howl of Terror
                    113056, //Intimidating Roar [Cowering in fear] (Warrior)
                    113004, //Intimidating Roar [Fleeing in fear] (Warrior)
                    5246, //Intimidating Shout (aoe)
                    20511, //Intimidating Shout (targeted)
                    115268, //Mesmerize (Shivarra)
                    64044, //Psychic Horror
                    8122, //Psychic Scream
                    113792, //Psychic Terror (Psyfiend)
                    132412, //Seduction (Grimoire of Sacrifice)
                    6358, //Seduction (Succubus)
                    87204, //Sin and Punishment
                    104045 //Sleep (Metamorphosis)
                };

        private static
            bool DebuffFearDuration
            (WoWUnit target, double duration)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            using (StyxWoW.Memory.AcquireFrame())
            {
                if (target.ActiveAuras.Any(
                    a =>
                    (a.Value.ApplyAuraType == WoWApplyAuraType.ModFear ||
                     a.Value.ApplyAuraType == WoWApplyAuraType.ModCharm ||
                     DebuffFearDurationHS.Contains(a.Value.SpellId)) &&
                    a.Value.TimeLeft.TotalMilliseconds > duration))
                {
                    //Logging.Write(target.Name + " got DebuffCCDuration > " + duration);
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region DebuffRoot

        private static readonly
            HashSet<int> DebuffRootHS = new HashSet<int>
                {
                    96294, //Chains of Ice (Chilblains)
                    116706, //Disable
                    64695, //Earthgrab (Earthgrab Totem)
                    339, //Entangling Roots
                    113770, //Entangling Roots (Force of Nature - Balance Treants)
                    19975, //Entangling Roots (Nature's Grasp)
                    113275, //Entangling Roots (Symbiosis)
                    113275, //Entangling Roots (Symbiosis)
                    19185, //Entrapment
                    33395, //Freeze
                    63685, //Freeze (Frozen Power)
                    39965, //Frost Grenade
                    122, //Frost Nova
                    110693, //Frost Nova (Mage)
                    55536, //Frostweave Net
                    87194, //Glyph of Mind Blast
                    111340, //Ice Ward
                    45334, //Immobilized (Wild Charge - Bear)
                    90327, //Lock Jaw (Dog)
                    102359, //Mass Entanglement
                    128405, //Narrow Escape
                    13099, //Net-o-Matic
                    115197, //Partial Paralysis
                    50245, //Pin (Crab)
                    91807, //Shambling Rush (Dark Transformation)
                    123407, //Spinning Fire Blossom
                    107566, //Staggering Shout
                    54706, //Venom Web Spray (Silithid)
                    114404, //Void Tendril's Grasp
                    4167, //Web (Spider)
                };

        private static
            bool DebuffRoot
            (WoWUnit
                 target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            using (StyxWoW.Memory.AcquireFrame())
            {
                if (target.ActiveAuras.Any(
                    a => a.Value.ApplyAuraType == WoWApplyAuraType.ModRoot || DebuffRootHS.Contains(a.Value.SpellId)))
                {
                    //Logging.Write(target.Name + " got DebuffRoot");
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region DebuffRootCanCloak

        private static readonly
            HashSet<int> DebuffRootCanCloakHS = new HashSet<int>
                {
                    96294, //Chains of Ice (Chilblains)
                    64695, //Earthgrab (Earthgrab Totem)
                    339, //Entangling Roots
                    113770, //Entangling Roots (Force of Nature - Balance Treants)
                    19975, //Entangling Roots (Nature's Grasp)
                    113275, //Entangling Roots (Symbiosis)
                    113275, //Entangling Roots (Symbiosis)
                    19185, //Entrapment
                    33395, //Freeze
                    63685, //Freeze (Frozen Power)
                    122, //Frost Nova
                    110693, //Frost Nova (Mage)
                    87194, //Glyph of Mind Blast
                    111340, //Ice Ward
                    102359, //Mass Entanglement
                    123407, //Spinning Fire Blossom
                    54706, //Venom Web Spray (Silithid)
                    114404, //Void Tendril's Grasp
                    4167, //Web (Spider)
                };

        private static
            bool DebuffRootCanCloak
            (WoWUnit
                 target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            using (StyxWoW.Memory.AcquireFrame())
            {
                if (target.ActiveAuras.Any(
                    a =>
                    a.Value.ApplyAuraType == WoWApplyAuraType.ModRoot &&
                    a.Value.Spell.DispelType == WoWDispelType.Magic ||
                    DebuffRootHS.Contains(a.Value.SpellId)))
                {
                    //Logging.Write(target.Name + " got DebuffRoot");
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region DebuffRootCanCleanse

        private static readonly
            HashSet<int> DebuffRootCanCleanseHS = new HashSet<int>
                {
                    96294, //Chains of Ice (Chilblains)
                    64695, //Earthgrab (Earthgrab Totem)
                    339, //Entangling Roots
                    113770, //Entangling Roots (Force of Nature - Balance Treants)
                    19975, //Entangling Roots (Nature's Grasp)
                    113275, //Entangling Roots (Symbiosis)
                    113275, //Entangling Roots (Symbiosis)
                    19185, //Entrapment
                    33395, //Freeze
                    63685, //Freeze (Frozen Power)
                    122, //Frost Nova
                    110693, //Frost Nova (Mage)
                    87194, //Glyph of Mind Blast
                    111340, //Ice Ward
                    102359, //Mass Entanglement
                    115197, //Partial Paralysis
                    91807, //Shambling Rush (Dark Transformation)
                    123407, //Spinning Fire Blossom
                    54706, //Venom Web Spray (Silithid)
                    114404, //Void Tendril's Grasp
                    4167, //Web (Spider)
                };

        private static
            bool DebuffRootCanCleanse
            (WoWUnit
                 target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            using (StyxWoW.Memory.AcquireFrame())
            {
                return target.ActiveAuras.Any(
                    a =>
                    a.Value.ApplyAuraType == WoWApplyAuraType.ModRoot &&
                    (a.Value.Spell.DispelType == WoWDispelType.Magic ||
                     a.Value.Spell.DispelType == WoWDispelType.Poison) ||
                    DebuffRootCanCleanseHS.Contains(a.Value.SpellId));
            }
        }

        #endregion

        #region DebuffRootorSnare

        private static readonly
            HashSet<int> DebuffRootorSnareHS = new HashSet<int>
                {
                    96294, //Chains of Ice (Chilblains)
                    116706, //Disable
                    64695, //Earthgrab (Earthgrab Totem)
                    339, //Entangling Roots
                    113770, //Entangling Roots (Force of Nature - Balance Treants)
                    19975, //Entangling Roots (Nature's Grasp)
                    113275, //Entangling Roots (Symbiosis)
                    113275, //Entangling Roots (Symbiosis)
                    19185, //Entrapment
                    33395, //Freeze
                    63685, //Freeze (Frozen Power)
                    39965, //Frost Grenade
                    122, //Frost Nova
                    110693, //Frost Nova (Mage)
                    55536, //Frostweave Net
                    87194, //Glyph of Mind Blast
                    111340, //Ice Ward
                    45334, //Immobilized (Wild Charge - Bear)
                    90327, //Lock Jaw (Dog)
                    102359, //Mass Entanglement
                    128405, //Narrow Escape
                    13099, //Net-o-Matic
                    115197, //Partial Paralysis
                    50245, //Pin (Crab)
                    91807, //Shambling Rush (Dark Transformation)
                    123407, //Spinning Fire Blossom
                    107566, //Staggering Shout
                    54706, //Venom Web Spray (Silithid)
                    114404, //Void Tendril's Grasp
                    4167, //Web (Spider)
                    50433, //Ankle Crack (Crocolisk)
                    110300, //Burden of Guilt
                    35101, //Concussive Barrage
                    5116, //Concussive Shot
                    120, //Cone of Cold
                    3409, //Crippling Poison
                    18223, //Curse of Exhaustion
                    45524, //Chains of Ice
                    50435, //Chilblains
                    121288, //Chilled (Frost Armor)
                    1604, //Dazed
                    63529, //Dazed - Avenger's Shield
                    50259, //Dazed (Wild Charge - Cat)
                    26679, //Deadly Throw
                    119696, //Debilitation
                    116095, //Disable
                    123727, //Dizzying Haze
                    3600, //Earthbind (Earthbind Totem)
                    77478, //Earthquake (Glyph of Unstable Earth)
                    123586, //Flying Serpent Kick
                    113092, //Frost Bomb
                    54644, //Frost Breath (Chimaera)
                    8056, //Frost Shock
                    116, //Frostbolt
                    8034, //Frostbrand Attack
                    44614, //Frostfire Bolt
                    61394, //Frozen Wake (Glyph of Freezing Trap)
                    1715, //Hamstring
                    13810, //Ice Trap
                    58180, //Infected Wounds
                    118585, //Leer of the Ox
                    15407, //Mind Flay
                    12323, //Piercing Howl
                    115000, //Remorseless Winter
                    20170, //Seal of Justice
                    47960, //Shadowflame
                    31589, //Slow
                    129923, //Sluggish (Glyph of Hindering Strikes)
                    61391, //Typhoon
                    51490, //Thunderstorm
                    127797, //Ursol's Vortex
                    137637, //Warbringer
                };

        private static
            bool DebuffRootorSnare
            (WoWUnit
                 target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            using (StyxWoW.Memory.AcquireFrame())
            {
                return target.ActiveAuras.Any(
                    a =>
                    a.Value.ApplyAuraType == WoWApplyAuraType.ModRoot ||
                    a.Value.ApplyAuraType == WoWApplyAuraType.ModDecreaseSpeed ||
                    DebuffRootorSnareHS.Contains(a.Value.SpellId));
            }
        }

        private static
            int DebuffRootorSnareCount
            (WoWUnit
                 target)
        {
            return
                target.ActiveAuras.Count(
                    aura =>
                    aura.Value.ApplyAuraType == WoWApplyAuraType.ModRoot ||
                    aura.Value.ApplyAuraType == WoWApplyAuraType.ModDecreaseSpeed ||
                    DebuffRootorSnareHS.Contains(aura.Value.SpellId));
        }

        #endregion

        #region DebuffSilence

        private static readonly
            HashSet<int> DebuffSilenceHS = new HashSet<int>
                {
                    129597, //Arcane Torrent (Chi)
                    25046, //Arcane Torrent (Energy)
                    80483, //Arcane Torrent (Focus)
                    28730, //Arcane Torrent (Mana)
                    69179, //Arcane Torrent (Rage)
                    50613, //Arcane Torrent (Runic Power)
                    31935, //Avenger's Shield
                    114238, //Fae Silence (Glyph of Fae Silence)
                    102051, //Frostjaw (also a root)
                    1330, //Garrote - Silence
                    115782, //Optical Blast (Observer)
                    15487, //Silence
                    18498, //Silenced - Gag Order
                    55021, //Silenced - Improved Counterspell
                    34490, //Silencing Shot
                    81261, //Solar Beam
                    113287, //Solar Beam (Symbiosis)
                    116709, //Spear Hand Strike
                    24259, //Spell Lock (Felhunter)
                    132409, //Spell Lock (Grimoire of Sacrifice)
                    47476, //Strangulate
                    31117, //Unstable Affliction
                };

        private static
            bool DebuffSilence
            (WoWUnit
                 target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }


            using (StyxWoW.Memory.AcquireFrame())
            {
                if (target.ActiveAuras.Any(
                    a =>
                    a.Value.ApplyAuraType == WoWApplyAuraType.ModSilence ||
                    DebuffSilenceHS.Contains(a.Value.SpellId)))
                {
                    //Logging.Write(target.Name + " got DebuffSilence");
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region DebuffSnare

        private static readonly
            HashSet<int> DebuffSnareHS = new HashSet<int>
                {
                    50433, //Ankle Crack (Crocolisk)
                    110300, //Burden of Guilt
                    35101, //Concussive Barrage
                    5116, //Concussive Shot
                    120, //Cone of Cold
                    3409, //Crippling Poison
                    18223, //Curse of Exhaustion
                    45524, //Chains of Ice
                    50435, //Chilblains
                    121288, //Chilled (Frost Armor)
                    1604, //Dazed
                    63529, //Dazed - Avenger's Shield
                    50259, //Dazed (Wild Charge - Cat)
                    26679, //Deadly Throw
                    119696, //Debilitation
                    116095, //Disable
                    123727, //Dizzying Haze
                    3600, //Earthbind (Earthbind Totem)
                    77478, //Earthquake (Glyph of Unstable Earth)
                    123586, //Flying Serpent Kick
                    113092, //Frost Bomb
                    54644, //Frost Breath (Chimaera)
                    8056, //Frost Shock
                    116, //Frostbolt
                    8034, //Frostbrand Attack
                    44614, //Frostfire Bolt
                    61394, //Frozen Wake (Glyph of Freezing Trap)
                    1715, //Hamstring
                    13810, //Ice Trap
                    58180, //Infected Wounds
                    118585, //Leer of the Ox
                    15407, //Mind Flay
                    12323, //Piercing Howl
                    115000, //Remorseless Winter
                    20170, //Seal of Justice
                    47960, //Shadowflame
                    31589, //Slow
                    129923, //Sluggish (Glyph of Hindering Strikes)
                    61391, //Typhoon
                    51490, //Thunderstorm
                    127797, //Ursol's Vortex
                    137637, //Warbringer
                };

        private static
            bool DebuffSnare
            (WoWUnit
                 target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            using (StyxWoW.Memory.AcquireFrame())
            {
                return target.ActiveAuras.Any(
                    a =>
                    a.Value.ApplyAuraType == WoWApplyAuraType.ModDecreaseSpeed ||
                    DebuffSnareHS.Contains(a.Value.SpellId));
            }
        }

        #endregion

        #region DebuffStunDuration

        private static readonly
            HashSet<int> DebuffStunDurationHS = new HashSet<int>
                {
                    89766, //Axe Toss (Felguard/Wrathguard)
                    113801, //Bash (Force of Nature - Feral Treants)
                    102795, //Bear Hug
                    117526, //Binding Shot
                    126451, //Clash
                    122242, //Clash (not sure which one is right)
                    119392, //Charging Ox Wave
                    1833, //Cheap Shot
                    44572, //Deep Freeze
                    54786, //Demonic Leap (Metamorphosis)
                    105593, //Fist of Justice
                    117418, //Fists of Fury
                    91800, //Gnaw
                    853, //Hammer of Justice
                    110698, //Hammer of Justice (Paladin)
                    24394, //Intimidation
                    408, //Kidney Shot
                    119381, //Leg Sweep
                    5211, //Mighty Bash
                    91797, //Monstrous Blow (Dark Transformation)
                    9005, //Pounce
                    102546, //Pounce (Incarnation)
                    115001, //Remorseless Winter
                    82691, //Ring of Frost
                    30283, //Shadowfury
                    132168, //Shockwave
                    118905, //Static Charge (Capacitor Totem)
                    20549, //War Stomp
                    108194 //Asphyxiate
                };

        private static
            bool DebuffStunDuration
            (WoWUnit target, double duration)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            using (StyxWoW.Memory.AcquireFrame())
            {
                return target.ActiveAuras.Any(
                    a =>
                    (a.Value.ApplyAuraType == WoWApplyAuraType.ModStun ||
                     DebuffStunDurationHS.Contains(a.Value.SpellId)) &&
                    a.Value.TimeLeft.TotalMilliseconds > duration);
            }
        }

        private static
            bool DebuffStun
            (WoWUnit
                 target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            using (StyxWoW.Memory.AcquireFrame())
            {
                return target.ActiveAuras.Any(
                    a =>
                    (a.Value.ApplyAuraType == WoWApplyAuraType.ModStun ||
                     DebuffStunDurationHS.Contains(a.Value.SpellId)));
            }
        }

        private static
            double DebuffStunDurationTimeLeft
            (WoWUnit
                 target)
        {
            if (target == null || !target.IsValid)
            {
                return 0;
            }

            using (StyxWoW.Memory.AcquireFrame())
            {
                var auraStun =
                    (from a in target.ActiveAuras
                     where DebuffStunDurationHS.Contains(a.Value.SpellId)
                     orderby a.Value.TimeLeft descending
                     select a).FirstOrDefault();

                return auraStun.Value != null ? auraStun.Value.TimeLeft.TotalMilliseconds : 0;

                //if (auraStun.Value != null)
                //{
                //    return auraStun.Value.TimeLeft.TotalMilliseconds;
                //}
                //return 0;
            }
        }

        #endregion

        #region Invulnerable

        private static readonly
            HashSet<string> InvulnerableHS = new HashSet<string>
                {
                    "Hand of Protection", //Rogue Only
                    "Shield Block", //Rogue Only
                    //"Bladestorm",
                    "Cyclone",
                    //"Desecrated Ground",
                    "Deterrence",
                    "Dispersion",
                    "Divine Shield", //Grapple Weapon (Monk)
                    "Ice Block",
                    "Dematerialize",
                };

        private static
            bool Invulnerable
            (WoWUnit
                 target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            using (StyxWoW.Memory.AcquireFrame())
            {
                if (target.ActiveAuras.Any(a => InvulnerableHS.Contains(a.Value.Name)))
                {
                    //Logging.Write(target.Name + " got DebuffDisarm");
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region InvulnerablePhysic

        private static
            bool InvulnerablePhysic
            (WoWUnit
                 target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            return target.HasAura("Hand of Protection");
        }

        #endregion

        #region InvulnerableSpell

        private static readonly
            HashSet<string> InvulnerableSpellHS = new HashSet<string>
                {
                    "Anti-Magic Shell",
                    "Cloak of Shadows",
                    "Glyph of Ice Block",
                    "Grounding Totem Effect",
                    "Mass Spell Reflection",
                    "Phantasm",
                    "Spell Reflection",
                    "Zen Meditation"
                };

        private static
            bool InvulnerableSpell
            (WoWUnit
                 target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            using (StyxWoW.Memory.AcquireFrame())
            {
                if (target.ActiveAuras.Any(a => InvulnerableSpellHS.Contains(a.Value.Name)))
                {
                    //Logging.Write(target.Name + " got DebuffDisarm");
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region InvulnerableRootandSnare

        private static readonly
            HashSet<string> InvulnerableRootandSnareHS = new HashSet<string>
                {
                    "Master's Call",
                    "Bladestorm",
                    "Hand of Freedom"
                };

        private static
            bool InvulnerableRootandSnare
            (WoWUnit
                 target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            using (StyxWoW.Memory.AcquireFrame())
            {
                if (target.ActiveAuras.Any(a => InvulnerableRootandSnareHS.Contains(a.Value.Name)))
                {
                    //Logging.Write(target.Name + " got DebuffDisarm");
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region InvulnerableStun

        private static readonly
            HashSet<string> InvulnerableStunHS = new HashSet<string>
                {
                    "Icebound Fortitude",
                    "Bladestorm"
                };

        private static
            bool InvulnerableStun
            (WoWUnit
                 target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            using (StyxWoW.Memory.AcquireFrame())
            {
                if (target.ActiveAuras.Any(a => InvulnerableStunHS.Contains(a.Value.Name)))
                {
                    //Logging.Write(target.Name + " got DebuffDisarm");
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region SafeUsingCooldown

        private static readonly
            HashSet<string> SafeUsingCooldownHS = new HashSet<string>
                {
                    "Evasion",
                    "Bladestorm",
                };

        private static
            bool SafeUsingCooldown
            (WoWUnit
                 target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            //if (Me.IsBehind(target))
            //{
            //    return true;
            //}

            using (StyxWoW.Memory.AcquireFrame())
            {
                if (target.ActiveAuras.Any(a => SafeUsingCooldownHS.Contains(a.Value.Name)))
                {
                    //Logging.Write(target.Name + " got DebuffDisarm");
                    return false;
                }
                return true;
            }
        }

        #endregion

        #region WriteDebuff

        private static
            void WriteDebuff
            (WoWUnit
                 target)
        {
            if (target == null || !target.IsValid)
                return;
            foreach (var aura in target.ActiveAuras)
            {
                if (!aura.Value.IsHarmful) continue; //Name: Battle Fatigue (134735)
                if (aura.Value.SpellId == 134735) continue; //Name: Battle Fatigue (134735)
                var spell = aura.Value.Spell;
                Logging.Write("--------" + aura.Value.Name + " (" + aura.Value.SpellId + ")" + " (" + target.Class +
                              ")" +
                              "--------");
                //Logging.Write("Name: " + aura.Value.Name + " (" + aura.Value.SpellId + ")");
                Logging.Write("ApplyAuraType: " + aura.Value.ApplyAuraType);
                Logging.Write("SpellEffects: " + spell.SpellEffects);
                Logging.Write("SpellEffect1: " + spell.SpellEffect1);
                Logging.Write("SpellEffect2: " + spell.SpellEffect2);
                Logging.Write("SpellEffect3: " + spell.SpellEffect3);
            }
        }

        #endregion
    }
}