﻿using System.Collections.Generic;
using Styx.WoWInternals.WoWObjects;

namespace TuanHARogue
{
    public partial class Classname
    {
        //#region InterruptCastNoChannel

        //private static double InterruptCastNoChannel(WoWUnit target)
        //{
        //    if (target == null || !target.IsValid || !target.IsPlayer)
        //    {
        //        return 0;
        //    }
        //    double timeLeft = 0;

        //    if (target.IsCasting && (target.CastingSpell.Name == "Arcane Blast" ||
        //                             target.CastingSpell.Name == "Banish" ||
        //                             target.CastingSpell.Name == "Binding Heal" ||
        //                             target.CastingSpell.Name == "Cyclone" ||
        //                             target.CastingSpell.Name == "Chain Heal" ||
        //                             target.CastingSpell.Name == "Chain Lightning" ||
        //                             target.CastingSpell.Name == "Chi Burst" ||
        //                             target.CastingSpell.Name == "Chaos Bolt" ||
        //                             target.CastingSpell.Name == "Demonic Circle: Summon" ||
        //                             target.CastingSpell.Name == "Denounce" ||
        //                             target.CastingSpell.Name == "Divine Light" ||
        //                             target.CastingSpell.Name == "Divine Plea" ||
        //                             target.CastingSpell.Name == "Dominated Mind" ||
        //                             target.CastingSpell.Name == "Elemental Blast" ||
        //                             target.CastingSpell.Name == "Entangling Roots" ||
        //                             target.CastingSpell.Name == "Enveloping Mist" ||
        //                             target.CastingSpell.Name == "Fear" ||
        //                             target.CastingSpell.Name == "Fireball" ||
        //                             target.CastingSpell.Name == "Flash Heal" ||
        //                             target.CastingSpell.Name == "Flash of Light" ||
        //                             target.CastingSpell.Name == "Frost Bomb" ||
        //                             target.CastingSpell.Name == "Frostjaw" ||
        //                             target.CastingSpell.Name == "Frostbolt" ||
        //                             target.CastingSpell.Name == "Frostfire Bolt" ||
        //                             target.CastingSpell.Name == "Greater Heal" ||
        //                             target.CastingSpell.Name == "Greater Healing Wave" ||
        //                             target.CastingSpell.Name == "Hand of Gul'dan" ||
        //                             target.CastingSpell.Name == "Haunt" ||
        //                             target.CastingSpell.Name == "Heal" ||
        //                             target.CastingSpell.Name == "Healing Surge" ||
        //                             target.CastingSpell.Name == "Healing Touch" ||
        //                             target.CastingSpell.Name == "Healing Wave" ||
        //                             target.CastingSpell.Name == "Hex" ||
        //                             target.CastingSpell.Name == "Holy Fire" ||
        //                             target.CastingSpell.Name == "Holy Light" ||
        //                             target.CastingSpell.Name == "Holy Radiance" ||
        //                             target.CastingSpell.Name == "Hibernate" ||
        //                             target.CastingSpell.Name == "Mass Dispel" ||
        //                             target.CastingSpell.Name == "Mind Spike" ||
        //                             target.CastingSpell.Name == "Immolate" ||
        //                             target.CastingSpell.Name == "Incinerate" ||
        //                             target.CastingSpell.Name == "Lava Burst" ||
        //                             target.CastingSpell.Name == "Lightning Bolt" ||
        //                             target.CastingSpell.Name == "Mind Blast" ||
        //                             target.CastingSpell.Name == "Mind Spike" ||
        //                             target.CastingSpell.Name == "Nourish" ||
        //                             target.CastingSpell.Name == "Polymorph" ||
        //                             target.CastingSpell.Name == "Prayer of Healing" ||
        //                             target.CastingSpell.Name == "Pyroblast" ||
        //                             target.CastingSpell.Name == "Rebirth" ||
        //                             target.CastingSpell.Name == "Regrowth" ||
        //                             target.CastingSpell.Name == "Repentance" ||
        //                             target.CastingSpell.Name == "Scorch" ||
        //                             target.CastingSpell.Name == "Seed of Corruption" ||
        //                             target.CastingSpell.Name == "Shadow Bolt" ||
        //                             target.CastingSpell.Name == "Shackle Undead" ||
        //                             target.CastingSpell.Name == "Smite" ||
        //                             target.CastingSpell.Name == "Soul Fire" ||
        //                             target.CastingSpell.Name == "Starfire" ||
        //                             target.CastingSpell.Name == "Starsurge" ||
        //                             target.CastingSpell.Name == "Surging Mist" ||
        //                             target.CastingSpell.Name == "Transcendence" ||
        //                             target.CastingSpell.Name == "Transcendence: Transfer" ||
        //                             target.CastingSpell.Name == "Unstable Affliction" ||
        //                             target.CastingSpell.Name == "Wrath" ||
        //                             target.CastingSpell.Name == "Vampiric Touch" ||
        //                             target.CastingSpell.Name == "Wrath"))
        //    {
        //        timeLeft = target.CurrentCastTimeLeft.TotalMilliseconds;
        //    }
        //    return timeLeft;
        //}

        //#endregion

        //#region InterruptCastChannel

        //private static double InterruptCastChannel(WoWUnit target)
        //{
        //    if (target == null || !target.IsValid || !target.IsPlayer)
        //    {
        //        return 0;
        //    }
        //    double timeLeft = 0;

        //    if (target.IsChanneling && (target.ChanneledSpell.Name == "Drain Life" ||
        //                                target.ChanneledSpell.Name == "Hymn of Hope" ||
        //                                target.ChanneledSpell.Name == "Blizzard" ||
        //                                target.ChanneledSpell.Name == "Arcane Barrage" ||
        //                                target.ChanneledSpell.Name == "Evocation" ||
        //                                target.ChanneledSpell.Name == "Mana Tea" ||
        //                                target.ChanneledSpell.Name == "Crackling Jade Lightning" ||
        //                                target.ChanneledSpell.Name == "Malefic Grasp" ||
        //                                target.ChanneledSpell.Name == "Hellfire" ||
        //                                target.ChanneledSpell.Name == "Rain of Fire" ||
        //                                target.ChanneledSpell.Name == "Harvest Life" ||
        //                                target.ChanneledSpell.Name == "Health Funnel" ||
        //                                target.ChanneledSpell.Name == "Drain Soul" ||
        //                                target.ChanneledSpell.Name == "Arcane Missiles" ||
        //                                target.ChanneledSpell.Name == "Mind Flay" ||
        //                                target.ChanneledSpell.Name == "Penance" ||
        //                                target.ChanneledSpell.Name == "Soothing Mist" ||
        //                                target.ChanneledSpell.Name == "Tranquility" ||
        //                                target.ChanneledSpell.Name == "Drain Life"))
        //    {
        //        timeLeft = target.CurrentChannelTimeLeft.TotalMilliseconds;
        //    }

        //    return timeLeft;
        //}

        //#endregion

        #region InterruptCheck

        private static readonly HashSet<int> DoNotInterrupt = new HashSet<int>
            {
                3648, //Hearthstone
                77767, //Cobra Shot
                56641, //Steady Shot
            };


        private static bool InterruptCheck(WoWUnit unit, double millisecondsleft, bool includeUninterruptable = true)
        {
            if (unit == null ||
                !unit.IsValid ||
                !unit.Combat ||
                !unit.IsCasting && !unit.IsChanneling ||
                !includeUninterruptable && !unit.CanInterruptCurrentSpellCast ||
                unit.IsCasting && DoNotInterrupt.Contains(unit.CastingSpellId) ||
                unit.IsChanneling && DoNotInterrupt.Contains(unit.ChanneledCastingSpellId) ||
                unit.IsCasting && unit.CurrentCastTimeLeft.TotalMilliseconds > millisecondsleft + MyLatency)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}