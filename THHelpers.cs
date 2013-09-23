using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Media;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Action = Styx.TreeSharp.Action;

namespace TuanHARogue
{
    public partial class Classname
    {
        #region Attackable

        private static bool Attackable(WoWUnit target)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (target == null || !target.IsValid)
                {
                    return false;
                }

                if (target.IsBoss)
                {
                    return true;
                }

                if (!ValidUnit(target))
                {
                    return false;
                }

                if (Invulnerable(target))
                {
                    return false;
                }

                if (DebuffCCBreakonDamage(target))
                {
                    return false;
                }

                if (!IsEnemy(target))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool Attackable(WoWUnit target, int range)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (target == null || !target.IsValid)
                {
                    return false;
                }

                if (!ValidUnit(target))
                {
                    return false;
                }

                if (target.Distance - target.CombatReach - 1 > range)
                {
                    return false;
                }

                //if (range <= 5 && !target.IsWithinMeleeRange)
                //{
                //    return false;
                //}

                if (Invulnerable(target))
                {
                    return false;
                }

                if (DebuffCCBreakonDamage(target))
                {
                    return false;
                }

                if (!IsEnemy(target))
                {
                    return false;
                }

                if (range > 5 && !target.IsWithinMeleeRange)
                {
                    if (target.Distance - target.CombatReach - 1 > range || !target.InLineOfSpellSight)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool AttackableNoCC(WoWUnit target, int range)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (target == null || !target.IsValid)
                {
                    return false;
                }

                if (!ValidUnit(target))
                {
                    return false;
                }

                if (target.Distance > range)
                {
                    return false;
                }

                if (!target.InLineOfSpellSight)
                {
                    return false;
                }

                if (Invulnerable(target))
                {
                    return false;
                }

                if (!IsEnemy(target))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool AttackableNoLoS(WoWUnit target, int range)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (target == null || !target.IsValid)
                {
                    return false;
                }

                if (target.IsBoss)
                {
                    return true;
                }

                if (!ValidUnit(target))
                {
                    return false;
                }

                if (range <= 5 && !target.IsWithinMeleeRange)
                {
                    return false;
                }

                if (range > 5 && target.Distance > range)
                {
                    return false;
                }

                if (Invulnerable(target))
                {
                    return false;
                }

                if (DebuffCCBreakonDamage(target))
                {
                    return false;
                }

                if (!IsEnemy(target))
                {
                    return false;
                }
            }
            return true;
        }

        //prevent double ValidUnit Check
        //private static bool AttackableValid(WoWUnit target)
        //{
        //    //using (StyxWoW.Memory.AcquireFrame())
        //    {
        //        if (target.Distance < 40 && !Invulnerable(target) &&
        //            !DebuffCCBreakonDamage(target) && IsEnemy(target) &&
        //            target.InLineOfSpellSight)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        #endregion

        #region  BattleStandard

        private static Composite UseBattleStandard()
        {
            return
                new Decorator(
                    ret =>
                    THSettings.Instance.HealthStone &&
                    Me.Combat &&
                    Me.HealthPercent < THSettings.Instance.HealthStoneHP &&
                    Me.CurrentTarget != null &&
                    Me.CurrentTarget.CurrentHealth > Me.HealthPercent &&
                    InBattleground,
                    new Action(delegate
                        {
                            if (Me.IsAlliance)
                            {
                                WoWItem bs = Me.BagItems.FirstOrDefault(o => o.Entry == 18606);
                                //18606 Alliance Battle Standard

                                if (!Me.HasAura("Alliance Battle Standard") && bs != null &&
                                    bs.CooldownTimeLeft.TotalMilliseconds <= MyLatency)
                                {
                                    bs.Use();
                                    //Lua.DoString("RunMacroText(\"/5 Used Healthstone\")");
                                    //Lua.DoString("RunMacroText(\"/s Used Battle Standard\")");
                                    Logging.Write("Use Battle Standard at " + Me.HealthPercent + "%");
                                }
                            }

                            if (Me.IsHorde)
                            {
                                WoWItem bs = Me.BagItems.FirstOrDefault(o => o.Entry == 18607);
                                //18606 Horde Battle Standard

                                if (!Me.HasAura("Horde Battle Standard") && bs != null &&
                                    bs.CooldownTimeLeft.TotalMilliseconds <= MyLatency)
                                {
                                    bs.Use();
                                    //Lua.DoString("RunMacroText(\"/5 Used Healthstone\")");
                                    //Lua.DoString("RunMacroText(\"/s Used Battle Standard\")");
                                    Logging.Write("Use Battle Standard at " + Me.HealthPercent + "%");
                                }
                            }

                            return RunStatus.Failure;
                        })
                    );
        }

        #endregion

        #region CalculateTimeConsumed

        private static DateTime TimeConsumed;

        private static void CalculateTimeConsumedStart()
        {
            TimeConsumed = DateTime.Now;
        }

        private static void CalculateTimeConsumedStop(string CallFunction = "TimeConsumed")
        {
            Logging.Write(LogLevel.Diagnostic, "{0} take {1} ms", CallFunction,
                          (DateTime.Now - TimeConsumed).TotalMilliseconds);
        }

        #endregion

        #region CanUseEquippedItem

        //Thank Apoc
        private static bool CanUseEquippedItem(WoWItem item)
        {
            // Check for engineering tinkers!
            var itemSpell = Lua.GetReturnVal<string>("return GetItemSpell(" + item.Entry + ")", 0);
            if (string.IsNullOrEmpty(itemSpell))
                return false;
            return item.Usable && item.Cooldown <= 0;
        }

        #endregion

        #region CastSpell

        private static WoWUnit LastCastUnit;
        private static DateTime LastVanishDefend;
        public static DateTime LastCastTime;
        private static DateTime LastCastStealth;
        private static DateTime LastCastCC;
        public static string LastCastSpell;
        private static string CastReason;
        private static DateTime SpamCastEnd;
        public static bool CastFailed;

        private static void CastSpell(string spellName, WoWUnit u, string reason = "", bool SpamCast = false,
                                      double SpamCastTine = 1000)
        {
            //Logging.Write(LogLevel.Diagnostic, "Trying to cast " + spellName + " on " + u.Name + " (" + reason + ")");

            //Logging.Write("CheapShotCheck " + CheapShotCheck());
            //Logging.Write("CalculateEnergy " + CalculateEnergy(60, MyAuraTimeLeft(115192, Me)));
            //Logging.Write("MyAuraTimeLeft " + MyAuraTimeLeft(115192, Me));
            //Logging.Write("KidneyShotCheck " + KidneyShotCheck());

            LastCastSpell = "";
            CastFailed = false;

            if (u == null ||
                !u.IsValid)
            {
                Logging.Write(LogLevel.Diagnostic, "CastSpell fail: null/invalid");
                return;
            }

            if (SpamCast)
            {
                SpamCastEnd = DateTime.Now + TimeSpan.FromMilliseconds(SpamCastTine);
                while (SpamCastEnd > DateTime.Now)
                {
                    SpellManager.Cast(spellName, u);
                    if (LastCastSpell == spellName || CastFailed)
                    {
                        break;
                    }
                }
            }
            else
            {
                SpellManager.Cast(spellName, u);
                LastCastSpell = spellName;
                LastCastTime = DateTime.Now;
            }

            LastCastUnit = u;

            //Logging.Write("GetPlayerRunicPower: " + GetPlayerRunicPower);
            //Logging.Write("GetPlayerRune: " + GetPlayerRune);

            //Prevent spamming
            //if (_lastCastUnit != u || spellName != _lastCastSpell ||
            //    _lastCastTime + TimeSpan.FromMilliseconds(500) < DateTime.Now)
            //{
            string barTwo = "Energy: " + PlayerEnergy + " + CP: " + PlayerComboPoint;

            string unitLogName;
            if (u == Me)
            {
                unitLogName = "Myself";
            }
            else if (u.IsPlayer)
            {
                unitLogName = u.Class.ToString();
            }
            else
            {
                unitLogName = u.Name;
            }

            CastReason = "";
            CastReason = reason;


            Color colorlog;
            if (u == Me)
            {
                colorlog = Colors.GreenYellow;
            }

            else if (u == Me.CurrentTarget)
            {
                colorlog = Colors.Yellow;
            }
            else
            {
                colorlog = Colors.YellowGreen;
            }

            Logging.Write(colorlog, DateTime.Now.ToString("ss:fff") + " HP: " +
                                    Math.Round(Me.HealthPercent) + "% " + barTwo + " " +
                                    unitLogName + " " +
                                    Math.Round(u.Distance, 2) + "y " +
                                    Math.Round(u.HealthPercent) + "% hp " + spellName + " (" + CastReason + ") ");
            //}


            //TickMilisecond = DateTime.Now;

            //if (!NoGCDSpells.Contains(spellName))
            //{
            //    UpdateGCD();
            //}


            //if (Me.GotAlivePet)
            //{
            //    Logging.Write("Me.GotAlivePet: " + Me.Pet.Name);
            //}
            //if (Me.Minions.Count > 0)
            //{
            //    Logging.Write("Me.Minions.Count " + Me.Minions.Count);
            //}
        }

        #endregion

        #region CanCleanse

        //private static bool CanCleanse(WoWAura aura)
        //{
        //    //if (aura == null)
        //    //    return false;

        //    //if (aura.Spell.DispelType == WoWDispelType.Disease || aura.Spell.DispelType == WoWDispelType.Magic ||
        //    //    aura.Spell.DispelType == WoWDispelType.Poison)
        //    if (aura.Spell.DispelType != WoWDispelType.None)
        //    {
        //        //Logging.Write("CanCleanse: " + aura.Name + " - " + aura.SpellId);
        //        return true;
        //    }
        //    return false;
        //}

        #endregion

        #region ConstantFace

        //private static void ConstantFace(WoWUnit unit)
        //{
        //    if (!IsOverrideModeOn && !Me.IsSafelyFacing(unit))
        //    {
        //        WoWMovement.ConstantFace(unit.Guid);
        //    }
        //}

        #endregion

        #region CountDebuff

        private static double CountDebuff(WoWUnit u)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (u == null || !u.IsValid || !u.IsAlive)
                    return 0;

                int numberofDebuff =
                    u.Debuffs.Values.Count(
                        debuff =>
                        (debuff.Spell.DispelType == WoWDispelType.Magic ||
                         debuff.Spell.DispelType == WoWDispelType.Poison ||
                         debuff.Spell.DispelType == WoWDispelType.Disease));

                return numberofDebuff;
            }
        }

        private static double CountDebuffAll(WoWUnit u)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (u == null || !u.IsValid || !u.IsAlive)
                    return 0;

                int numberofDebuff =
                    u.Debuffs.Values.Count();

                return numberofDebuff;
            }
        }

        private static double CountDebuffMagic(WoWUnit u)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (u == null || !u.IsValid || !u.IsAlive)
                    return 0;

                int numberofDebuff =
                    u.Debuffs.Values.Count(
                        debuff =>
                        (debuff.Spell.DispelType == WoWDispelType.Magic));

                return numberofDebuff;
            }
        }

        private static double CountDebuffRootandSnareMechanic(WoWUnit u)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (u == null || !u.IsValid || !u.IsAlive)
                    return 0;

                return
                    u.ActiveAuras.Values.Count(
                        debuff =>
                        (debuff.Spell.Mechanic == WoWSpellMechanic.Snared ||
                         debuff.Spell.Mechanic == WoWSpellMechanic.Rooted));
            }
        }

        #endregion

        #region CountDPSTarget

        private static int CountDPSTarget(WoWUnit target)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (InArena || InBattleground)
                {
                    return NearbyUnFriendlyPlayers.Count(
                        unit =>
                        unit.IsValid &&
                        unit.CurrentTarget != null &&
                        unit.CurrentTarget.IsValid &&
                        unit.CurrentTarget == target &&
                        (TalentSort(unit) >= 2 &&
                         TalentSort(unit) <= 3 &&
                         unit.Location.Distance(target.Location) < 40 ||
                         TalentSort(unit) == 1 &&
                         unit.Location.Distance(target.Location) < 15) &&
                        !DebuffCC(unit));
                }
                return NearbyUnFriendlyUnits.Count(
                    unit =>
                    unit.IsValid &&
                    unit.CurrentTarget != null &&
                    unit.CurrentTarget.IsValid &&
                    unit.CurrentTarget == target &&
                    !DebuffCC(unit));
            }
        }

        #endregion

        #region CountMagicDPSTarget

        private static int CountMagicDPSTarget(WoWUnit target)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (InArena || InBattleground)
                {
                    return NearbyUnFriendlyPlayers.Count(
                        unit =>
                        unit.IsValid &&
                        unit.CurrentTarget != null &&
                        unit.CurrentTarget.IsValid &&
                        unit.CurrentTarget == target &&
                        TalentSort(unit) == 3 &&
                        unit.Location.Distance(target.Location) < 40 &&
                        !DebuffCC(unit));
                }
                return NearbyUnFriendlyUnits.Count(
                    unit =>
                    unit.IsValid &&
                    unit.CurrentTarget != null &&
                    unit.CurrentTarget.IsValid &&
                    unit.CurrentTarget == target &&
                    unit.PowerType == WoWPowerType.Mana &&
                    unit.Location.Distance(target.Location) < 40 &&
                    !DebuffCC(unit));
            }
        }

        #endregion

        #region CountMeleeDPSTarget

        private static int CountMeleeDPSTarget(WoWUnit target, double distance)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (InArena || InBattleground)
                {
                    return NearbyUnFriendlyPlayers.Count(
                        unit =>
                        unit.IsValid &&
                        unit.CurrentTarget != null &&
                        unit.CurrentTarget.IsValid &&
                        unit.CurrentTarget == target &&
                        TalentSort(unit) == 1 &&
                        unit.Location.Distance(target.Location) <= distance &&
                        !DebuffCC(unit));
                }
                return NearbyUnFriendlyUnits.Count(
                    unit =>
                    unit.IsValid &&
                    unit.CurrentTarget != null &&
                    unit.CurrentTarget.IsValid &&
                    unit.CurrentTarget == target &&
                    unit.Location.Distance(target.Location) <= distance &&
                    !DebuffCC(unit));
            }
        }

        #endregion

        #region CountMeleeFriendDPSTarget

        private static int CountMeleeFriendDPSTarget(WoWUnit target, double distance)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (InArena || InBattleground)
                {
                    return RaidPartyMembers.Count(
                        unit =>
                        unit.IsValid &&
                        unit.CurrentTarget != null &&
                        unit.CurrentTarget.IsValid &&
                        unit.CurrentTarget == target &&
                        TalentSort(unit) == 1 &&
                        unit.Location.Distance(target.Location) <= distance &&
                        !DebuffCC(unit));
                }
                return RaidPartyMembers.Count(
                    unit =>
                    unit.IsValid &&
                    unit.CurrentTarget != null &&
                    unit.CurrentTarget.IsValid &&
                    unit.CurrentTarget == target &&
                    unit.Location.Distance(target.Location) <= distance &&
                    !DebuffCC(unit));
            }
        }

        #endregion

        #region CountPhysicDPSTarget

        private static int CountPhysicDPSTarget(WoWUnit target)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (InArena || InBattleground)
                {
                    return NearbyUnFriendlyPlayers.Count(
                        unit =>
                        unit.IsValid &&
                        unit.CurrentTarget != null &&
                        unit.CurrentTarget.IsValid &&
                        unit.CurrentTarget == target &&
                        (TalentSort(unit) == 2 &&
                         unit.Location.Distance(target.Location) < 40 ||
                         TalentSort(unit) == 1 &&
                         unit.Location.Distance(target.Location) < 25) &&
                        !DebuffCC(unit));
                }
                return NearbyUnFriendlyUnits.Count(
                    unit =>
                    unit.IsValid &&
                    unit.CurrentTarget != null &&
                    unit.CurrentTarget.IsValid &&
                    unit.CurrentTarget == target &&
                    unit.Location.Distance(target.Location) < 15 &&
                    !DebuffCC(unit));
            }
        }

        #endregion

        #region CountEnemyNear

        private static double CountEnemyNear(WoWUnit unitCenter, float distance)
        {
            return
                NearbyUnFriendlyUnits.Where(
                    unit =>
                    unit.IsValid &&
                    unitCenter.Location.Distance(unit.Location) <= distance &&
                    (IsDummy(unit) ||
                     unit.Combat &&
                     !unit.IsPet &&
                     (unit.CurrentTarget != null && unit.CurrentTarget == Me ||
                      unit.IsTargetingMyPartyMember ||
                      unit.IsTargetingMyRaidMember))).Aggregate
                    <WoWUnit, double>(0, (current, unit) => current + 1);
        }

        #endregion

        #region CountEnemyNearNoCC

        private static double CountEnemyNearNoCC(WoWUnit unitCenter, float distance)
        {
            return
                NearbyUnFriendlyUnits.Where(
                    unit =>
                    unit.IsValid &&
                    unitCenter.Location.Distance(unit.Location) <= distance &&
                    (IsDummy(unit) ||
                     unit.Combat &&
                     !unit.IsPet &&
                     !DebuffCC(unit) &&
                     (unit.CurrentTarget != null && unit.CurrentTarget == Me ||
                      unit.IsTargetingMyPartyMember ||
                      unit.IsTargetingMyRaidMember))).Aggregate
                    <WoWUnit, double>(0, (current, unit) => current + 1);
        }

        #endregion

        #region CountEnemyNearNoCCBreakonDamge

        private static double CountEnemyNearNoCCBreakonDamge(WoWUnit unitCenter, float distance)
        {
            if (unitCenter == null || unitCenter.IsValid)
            {
                return 0;
            }
            return
                NearbyUnFriendlyUnits.Where(
                    unit =>
                    unit.IsValid &&
                    unitCenter.Location.Distance(unit.Location) <= distance &&
                    DebuffCCBreakonDamage(unit)).Aggregate
                    <WoWUnit, double>(0, (current, unit) => current + 1);
        }

        #endregion

        #region CountEnemyFromPoint

        //private static double CountEnemyFromPoint(WoWPoint pointCenter, float radius)
        //{
        //    return
        //        NearbyUnFriendlyUnits.Where(
        //            unit =>
        //            pointCenter.Distance(unit.Location) <= radius &&
        //            unit.Combat).Aggregate
        //            <WoWUnit, double>(0, (current, unit) => current + 1);
        //}

        #endregion

        #region CountEneyTargettingUnit

        private static double CountEneyTargettingUnit(WoWUnit target, float distance)
        {
            return
                NearbyUnFriendlyUnits.Where(
                    unit =>
                    unit.IsValid &&
                    unit.Distance < distance &&
                    unit.CurrentTarget != null &&
                    unit.CurrentTarget.IsValid &&
                    unit.Level <= target.Level + 3 &&
                    //unit.Combat && 
                    !unit.IsPet &&
                    unit.CurrentTarget == target //&&
                    //DebuffCC(unit) <= MyLatency
                    ).Aggregate
                    <WoWUnit, double>(0, (current, unit) => current + 1);
        }

        #endregion

        #region CountEneyTargettingUnitInLoS

        private static double CountEneyTargettingUnitInLoSMe(float distance)
        {
            return
                NearbyUnFriendlyUnits.Where(
                    unit =>
                    unit.IsValid &&
                    unit.Distance < distance &&
                    unit.CurrentTarget != null &&
                    unit.CurrentTarget.IsValid &&
                    unit.Level <= Me.Level + 3 &&
                    //unit.Combat && 
                    !unit.IsPet &&
                    unit.CurrentTarget == Me &&
                    !DebuffCC(unit) &&
                    unit.InLineOfSpellSight
                    ).Aggregate
                    <WoWUnit, double>(0, (current, unit) => current + 1);
        }

        #endregion

        #region CurrentTargetCheck

        private static WoWUnit CurrentTargetCheckLast;
        private static double CurrentTargetCheckDist;
        private static bool CurrentTargetCheckIsWithinMeleeRange;
        private static bool CurrentTargetCheckDebuffCCBreakonDamage;
        private static bool CurrentTargetCheckInvulnerable;
        private static bool CurrentTargetCheckIsEnemy;
        private static bool CurrentTargetCheckInLineOfSpellSight;
        private static bool CurrentTargetCheckFacing;

        private static void CurrentTargetCheck()
        {
            CurrentTargetCheckLast = null;
            CurrentTargetCheckDist = 1000;
            CurrentTargetCheckIsWithinMeleeRange = false;
            CurrentTargetCheckDebuffCCBreakonDamage = false;
            CurrentTargetCheckInvulnerable = false;
            CurrentTargetCheckIsEnemy = false;
            CurrentTargetCheckInLineOfSpellSight = false;
            CurrentTargetCheckFacing = false;

            if (Me.CurrentTarget != null && Me.CurrentTarget.IsValid)
            {
                CurrentTargetCheckLast = Me.CurrentTarget;
                CurrentTargetCheckFacing = Me.IsFacing(Me.CurrentTarget);
                CurrentTargetCheckDist = Me.CurrentTarget.Distance - Me.CurrentTarget.CombatReach - 1;
                CurrentTargetCheckIsWithinMeleeRange = Me.CurrentTarget.IsWithinMeleeRange;
                CurrentTargetCheckDebuffCCBreakonDamage = DebuffCCBreakonDamage(Me.CurrentTarget);
                CurrentTargetCheckInvulnerable = Invulnerable(Me.CurrentTarget);
                CurrentTargetCheckIsEnemy = IsEnemy(Me.CurrentTarget);
                if (CurrentTargetCheckIsWithinMeleeRange)
                {
                    CurrentTargetCheckInLineOfSpellSight = true;
                }
                else if (Me.CurrentTarget.InLineOfSpellSight)
                {
                    CurrentTargetCheckInLineOfSpellSight = true;
                }
            }
        }

        private static bool CurrentTargetAttackable(double distance)
        {
            if (Me.CurrentTarget == null ||
                !Me.CurrentTarget.IsValid ||
                !Me.CurrentTarget.IsAlive ||
                Me.CurrentTarget.IsDead ||
                Blacklist.Contains(Me.CurrentTarget.Guid, BlacklistFlags.All))
            {
                return false;
            }

            if (Me.CurrentTarget != CurrentTargetCheckLast)
            {
                CurrentTargetCheck();
            }

            if (CurrentTargetCheckDebuffCCBreakonDamage ||
                CurrentTargetCheckInvulnerable ||
                !CurrentTargetCheckIsEnemy ||
                !CurrentTargetCheckInLineOfSpellSight ||
                !CurrentTargetCheckIsWithinMeleeRange &&
                CurrentTargetCheckDist > distance)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region DistanceCheck

        private static double DistanceCheck(WoWUnit target)
        {
            if (target == null || !target.IsValid)
            {
                return 1000;
            }
            //if (target.IsWithinMeleeRange)
            //{
            //    return 5;
            //}
            //return target.Distance;
            return target.Distance - target.CombatReach - 1;
        }

        #endregion

        #region FacingOverride

        private static bool FacingOverride(WoWUnit unit)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (unit == null)
                {
                    return false;
                }

                if (Me.IsFacing(unit))
                {
                    return true;
                }

                if (THSettings.Instance.AutoFace &&
                    (!IsOverrideModeOn ||
                     !Me.IsMoving))
                {
                    return true;
                }

                return false;
            }
        }

        #endregion

        #region GatherData

        //private readonly HashSet<int> _spellIDGather = new HashSet<int>
        //    {
        //        11196,
        //    };

        //public List<string> DebuffandCastCollected = new List<string>();

        //private string GetCreateClassName(ulong createdbyguid)
        //{
        //    //using (StyxWoW.Memory.AcquireFrame())
        //    {
        //        WoWPlayer unitcreateaura = (from unit in ObjectManager.GetObjectsOfType<WoWPlayer>(true, true)
        //                                    where unit.Guid == createdbyguid
        //                                    select unit).FirstOrDefault();

        //        if (unitcreateaura != null && unitcreateaura.IsPlayer)
        //        {
        //            return unitcreateaura.Class.ToString();
        //        }
        //        if (unitcreateaura != null && unitcreateaura.IsPet)
        //        {
        //            return unitcreateaura.Class.ToString() + " (Pet)";
        //        }
        //    }
        //    return "";
        //}

        //private static void GatherData()
        //{
        //    //using (StyxWoW.Memory.AcquireFrame())
        //    {
        //        List<WoWUnit> allUnit = ObjectManager.GetObjectsOfType<WoWUnit>(true, true);

        //        foreach (WoWUnit unit in allUnit)
        //        {
        //            foreach (var aura in unit.Debuffs)
        //            {
        //                if (!_spellIDGather.Contains(aura.Value.SpellId) &&
        //                    GetCreateClassName(aura.Value.CreatorGuid) != "")
        //                {
        //                    _spellIDGather.Add(aura.Value.SpellId);
        //                    //Logging.Write(GetCreateClassName(aura.Value.CreatorGuid) + "|" + "Debuff" + "|" + aura.Value.SpellId + "|" + aura.Value.Name + "|" + aura.Value.Spell.DispelType + "|" + aura.Value.Spell.Mechanic + "|" + aura.Value.Spell.BaseCooldown);
        //                    DebuffandCastCollected.Add(GetCreateClassName(aura.Value.CreatorGuid) + "|" + "Debuff" + "|" +
        //                                               aura.Value.SpellId + "|" + aura.Value.Name + "|" +
        //                                               aura.Value.Spell.DispelType + "|" + aura.Value.Spell.Mechanic +
        //                                               "|" + aura.Value.Spell.BaseCooldown);
        //                }
        //            }

        //            if (unit.IsCasting && !_spellIDGather.Contains(unit.CastingSpellId) &&
        //                GetCreateClassName(unit.Guid) != "")
        //            {
        //                _spellIDGather.Add(unit.CastingSpellId);
        //                //Logging.Write(unit.Class.ToString() + "|" + "Casting" + "|" + unit.CastingSpellId + "|" + unit.CastingSpell.Name + "|" +unit.CastingSpell.DispelType + "|" + unit.CastingSpell.Mechanic + "|" + unit.CastingSpell.BaseCooldown);
        //                DebuffandCastCollected.Add(unit.Class.ToString() + "|" + "Casting" + "|" + unit.CastingSpellId +
        //                                           "|" + unit.CastingSpell.Name + "|" + unit.CastingSpell.DispelType +
        //                                           "|" + unit.CastingSpell.Mechanic + "|" +
        //                                           unit.CastingSpell.BaseCooldown);
        //            }

        //            //foreach (var aura in unit.Buffs)
        //            //{
        //            //    if (!_spellIDGather.Contains(aura.Value.SpellId))
        //            //    {
        //            //        _spellIDGather.Add(aura.Value.SpellId);
        //            //        Logging.Write("Buff," + aura.Value.SpellId + "," + aura.Value.Name + "," + aura.Value.Spell.DispelType + "," + aura.Value.Spell.Mechanic + "," + aura.Value.Spell.BaseCooldown);
        //            //    }
        //            //}
        //        }
        //    }
        //}

        #endregion

        #region GCD

        private static DateTime _gcdReady;

        //private static void UpdateGCDEvent(object sender, LuaEventArgs raw)
        //{
        //    var args = raw.Args;
        //    var player = Convert.ToString(args[0]);

        //    // Not me ... Im out!
        //    if (player != "player")
        //    {
        //        return;
        //    }
        //    var spellID = Convert.ToInt32(args[4]);
        //    var spellName = WoWSpell.FromId(spellID).Name;

        //}

        //public static void UpdateGCD()
        //{
        //    //using (StyxWoW.Memory.AcquireFrame())
        //    {
        //        _gcdReady = DateTime.Now + SpellManager.GlobalCooldownLeft;
        //    }
        //}

        private static bool GCDL()
        {
            if ((DateTime.Now - LastCastTime).TotalMilliseconds < MyLatency &&
                SpellManager.GlobalCooldownLeft.TotalMilliseconds > MyLatency)
            {
                //Logging.Write("GCDL TRUE");
                return true;
            }
            return false;
        }

        private static bool Casting()
        {
            if (Me.IsCasting)
            {
                return true;
            }

            if (Me.IsChanneling)
            {
                return true;
            }

            return false;
        }


        private static bool CastingorGCDL()
        {
            return Casting() || GCDL();
        }

        #endregion

        #region GetAsyncKeyState

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);

        #endregion

        #region GetCurrentSpec

        public static string GetCurrentSpec()
        {
            if (Me.Specialization == WoWSpec.RogueAssassination)
            {
                return "Assassination";
            }
            if (Me.Specialization == WoWSpec.RogueSubtlety)
            {
                return "Subtlety";
            }
            else if (Me.Specialization == WoWSpec.RogueCombat)
            {
                return "Combat";
            }
            Logging.Write("No Specialization detected");
            return "No Specialization detected";
        }

        #endregion

        #region GetSpellCooldown

        private static TimeSpan GetSpellCooldown(string spell, int indetermValue = int.MaxValue)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                SpellFindResults sfr;
                if (SpellManager.FindSpell(spell, out sfr))
                    return (sfr.Override ?? sfr.Original).CooldownTimeLeft;

                if (indetermValue == int.MaxValue)
                    return TimeSpan.MaxValue;
            }
            return TimeSpan.FromSeconds(indetermValue);
        }

        #endregion

        #region GetUnitAuras

        //private static double AuraCCDuration;
        //private static double AuraCCDurationMax;
        //public WoWAura AuraCC;

        //private static double AuraCCBreakonDamageDuration;
        //private static double AuraCCBreakonDamageDurationMax;
        //public WoWAura AuraCCBreakonDamage;

        //private static double AuraDisarmDuration;
        //private static double AuraDisarmDurationMax;
        //public WoWAura AuraDisarm;

        //private static double AuraImmuneDuration;
        //private static double AuraImmuneDurationMax;
        //public WoWAura AuraImmune;

        //private static double AuraImmunePhysicDuration;
        //private static double AuraImmunePhysicDurationMax;
        //public WoWAura AuraImmunePhysic;

        //private static double AuraImmuneSpellDuration;
        //private static double AuraImmuneSpellDurationMax;
        //public WoWAura AuraImmuneSpell;

        //private static double AuraRootDuration;
        //private static double AuraRootDurationMax;
        //public WoWAura AuraRoot;

        //private static double AuraSilenceDuration;
        //private static double AuraSilenceDurationMax;
        //public WoWAura AuraSilence;

        //private static double AuraSnareDuration;
        //private static double AuraSnareDurationMax;
        //public WoWAura AuraSnare;

        //public WoWAura AuraCleanseDoNot;
        //public WoWAura AuraHealDoNot;
        //public int NumberofDebuff;

        //private static bool GetUnitAuras(WoWUnit u)
        //{
        //    if (u == null || !u.IsValid || !u.IsAlive)
        //        return false;

        //    AuraCC = null;
        //    AuraCCDuration = 0;
        //    AuraCCDurationMax = 0;

        //    AuraCCBreakonDamage = null;
        //    AuraCCBreakonDamageDuration = 0;
        //    AuraCCBreakonDamageDurationMax = 0;

        //    AuraDisarm = null;
        //    AuraDisarmDuration = 0;
        //    AuraDisarmDurationMax = 0;

        //    AuraImmune = null;
        //    AuraImmuneDuration = 0;
        //    AuraImmuneDurationMax = 0;

        //    AuraImmuneSpell = null;
        //    AuraImmuneSpellDuration = 0;
        //    AuraImmuneSpellDurationMax = 0;

        //    AuraImmunePhysic = null;
        //    AuraImmunePhysicDuration = 0;
        //    AuraImmunePhysicDurationMax = 0;

        //    AuraRoot = null;
        //    AuraRootDuration = 0;
        //    AuraRootDurationMax = 0;

        //    AuraSilence = null;
        //    AuraSilenceDuration = 0;
        //    AuraSilenceDurationMax = 0;

        //    AuraSnare = null;
        //    AuraSnareDuration = 0;
        //    AuraSnareDurationMax = 0;

        //    NumberofDebuff = 0;
        //    AuraCleanseDoNot = null;
        //    AuraHealDoNot = null;

        //    foreach (var aura in u.GetAllAuras())
        //    {
        //        //Count Number of Debuff
        //        if (aura.IsHarmful &&
        //            (aura.Spell.DispelType == WoWDispelType.Disease ||
        //             aura.Spell.DispelType == WoWDispelType.Magic ||
        //             aura.Spell.DispelType == WoWDispelType.Poison))
        //        {
        //            NumberofDebuff = NumberofDebuff + 1;
        //        }

        //        //Find out if AuraCleanseDoNot exits
        //        if (ListCleanseDoNot.Contains("[" + aura.SpellId + "]"))
        //        {
        //            AuraCleanseDoNot = aura.;
        //        }

        //        //Find out if AuraHealDoNot exits
        //        if (ListHealDoNot.Contains("[" + aura.SpellId + "]"))
        //        {
        //            AuraHealDoNot = aura.;
        //        }

        //        if (ListCC.Contains("[" + aura.SpellId + "]"))
        //        {
        //            AuraCCDuration = aura.TimeLeft.TotalMilliseconds;
        //            if (AuraCCDuration > AuraCCDurationMax)
        //            {
        //                AuraCC = aura.;
        //                AuraCCDurationMax = AuraCCDuration;
        //            }
        //        }

        //        if (ListCCBreakonDamage.Contains("[" + aura.SpellId + "]"))
        //        {
        //            AuraCCBreakonDamageDuration = aura.TimeLeft.TotalMilliseconds;
        //            if (AuraCCBreakonDamageDuration > AuraCCBreakonDamageDurationMax)
        //            {
        //                AuraCCBreakonDamage = aura.;
        //                AuraCCBreakonDamageDurationMax = AuraCCBreakonDamageDuration;
        //            }
        //        }

        //        if (ListDisarm.Contains("[" + aura.SpellId + "]"))
        //        {
        //            AuraDisarmDuration = aura.TimeLeft.TotalMilliseconds;
        //            if (AuraDisarmDuration > AuraDisarmDurationMax)
        //            {
        //                AuraDisarm = aura.;
        //                AuraDisarmDurationMax = AuraDisarmDuration;
        //            }
        //        }

        //        if (ListImmune.Contains("[" + aura.SpellId + "]"))
        //        {
        //            AuraImmuneDuration = aura.TimeLeft.TotalMilliseconds;
        //            if (AuraImmuneDuration > AuraImmuneDurationMax)
        //            {
        //                AuraImmune = aura.;
        //                AuraImmuneDurationMax = AuraImmuneDuration;
        //            }
        //        }

        //        if (ListImmuneSpell.Contains("[" + aura.SpellId + "]"))
        //        {
        //            AuraImmuneSpellDuration = aura.TimeLeft.TotalMilliseconds;
        //            if (AuraImmuneSpellDuration > AuraImmuneSpellDurationMax)
        //            {
        //                AuraImmuneSpell = aura.;
        //                AuraImmuneSpellDurationMax = AuraImmuneSpellDuration;
        //            }
        //        }

        //        if (ListImmunePhysic.Contains("[" + aura.SpellId + "]"))
        //        {
        //            AuraImmunePhysicDuration = aura.TimeLeft.TotalMilliseconds;
        //            if (AuraImmunePhysicDuration > AuraImmunePhysicDurationMax)
        //            {
        //                AuraImmunePhysic = aura.;
        //                AuraImmunePhysicDurationMax = AuraImmunePhysicDuration;
        //            }
        //        }

        //        if (ListRoot.Contains("[" + aura.SpellId + "]"))
        //        {
        //            AuraRootDuration = aura.TimeLeft.TotalMilliseconds;
        //            if (AuraRootDuration > AuraRootDurationMax)
        //            {
        //                AuraRoot = aura.;
        //                AuraRootDurationMax = AuraRootDuration;
        //            }
        //        }

        //        if (ListSilence.Contains("[" + aura.SpellId + "]"))
        //        {
        //            AuraSilenceDuration = aura.TimeLeft.TotalMilliseconds;
        //            if (AuraSilenceDuration > AuraSilenceDurationMax)
        //            {
        //                AuraSilence = aura.;
        //                AuraSilenceDurationMax = AuraSilenceDuration;
        //            }
        //        }

        //        if (ListSnare.Contains("[" + aura.SpellId + "]"))
        //        {
        //            AuraSnareDuration = aura.TimeLeft.TotalMilliseconds;
        //            if (AuraSnareDuration > AuraSnareDurationMax)
        //            {
        //                AuraSnare = aura.;
        //                AuraSnareDurationMax = AuraSnareDuration;
        //            }
        //        }
        //    }
        //    return true;
        //}

        #endregion

        #region GetAllMyAuras

        private static DateTime _lastGetAllMyAuras = DateTime.Now;

        private static void DumpAuras()
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (_lastGetAllMyAuras.AddSeconds(10) < DateTime.Now)
                {
                    int i = 1;
                    Logging.Write(LogLevel.Diagnostic, "----------------------------------");
                    foreach (WoWAura aura in Me.GetAllAuras())
                    {
                        Logging.Write(LogLevel.Diagnostic,
                                      i + ". Me.GetAllAuras Name: " + aura.Name + " - SpellId: " + aura.SpellId);
                        i = i + 1;
                    }
                    Logging.Write(LogLevel.Diagnostic, "----------------------------------");

                    i = 1;
                    Logging.Write(LogLevel.Diagnostic, "----------------------------------");
                    foreach (var aura in Me.Auras)
                    {
                        Logging.Write(LogLevel.Diagnostic,
                                      i + ". Me.Auras - Name: " + aura.Value.Name + " - SpellId: " + aura.Value.SpellId);
                        i = i + 1;
                    }
                    Logging.Write(LogLevel.Diagnostic, "----------------------------------");

                    i = 1;
                    Logging.Write(LogLevel.Diagnostic, "----------------------------------");
                    foreach (var aura in Me.ActiveAuras)
                    {
                        Logging.Write(LogLevel.Diagnostic,
                                      i + ". Me.ActiveAuras - Name: " + aura.Value.Name + " - SpellId: " +
                                      aura.Value.SpellId);
                        i = i + 1;
                    }
                    Logging.Write(LogLevel.Diagnostic, "----------------------------------");

                    i = 1;
                    Logging.Write(LogLevel.Diagnostic, "----------------------------------");
                    foreach (var aura in Me.PassiveAuras)
                    {
                        Logging.Write(LogLevel.Diagnostic,
                                      i + ". Me.PassiveAuras - Name: " + aura.Value.Name + " - SpellId: " +
                                      aura.Value.SpellId);
                        i = i + 1;
                    }
                    Logging.Write(LogLevel.Diagnostic, "----------------------------------");

                    _lastGetAllMyAuras = DateTime.Now;
                }
            }
        }

        #endregion

        #region GetMaxDistance

        private float GetMaxDistance(WoWUnit target)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (target != null)
                {
                    return (float) Math.Max(0f, target.Distance2D - target.BoundingRadius);
                }
                return 0;
            }
        }

        #endregion

        #region GetMinDistance

        private float GetMinDistance(WoWUnit target)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (target != null)
                {
                    return (float) Math.Max(0f, target.Distance2D - target.BoundingRadius);
                }
                return 123456;
            }
        }

        #endregion

        #region GetUnitAttckingMyMinion

        private static WoWUnit _unitAttckingMyMinion;

        private static bool GetUnitAttckingMyMinion()
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                _unitAttckingMyMinion = null;
                _unitAttckingMyMinion = (from unit in NearbyUnFriendlyUnits
                                         where unit.Combat
                                         where unit.CurrentTarget != null
                                         where unit.CurrentTarget.IsValid
                                         where unit.CurrentTarget.CreatedByUnitGuid == Me.Guid
                                         select unit).FirstOrDefault();
                return _unitAttckingMyMinion != null;
            }
        }

        #endregion

        #region GetUnitNear

        private static WoWUnit _unitNear;

        private static bool GetUnitNear()
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                _unitNear = null;
                if (InBattleground || InArena)
                {
                    _unitNear = (from unit in NearbyUnFriendlyPlayers
                                 where unit.IsValid
                                 //orderby unit.ThreatInfo.RawPercent ascending
                                 orderby unit.CurrentHealth ascending
                                 //where Me.Combat
                                 where Attackable(unit, 20)
                                 select unit).FirstOrDefault();

                    if (_unitNear == null)
                    {
                        _unitNear = (from unit in NearbyUnFriendlyPlayers
                                     where unit.IsValid
                                     //orderby unit.ThreatInfo.RawPercent ascending
                                     orderby unit.CurrentHealth ascending
                                     //where Me.Combat
                                     where Attackable(unit, 30)
                                     select unit).FirstOrDefault();
                    }

                    if (_unitNear == null)
                    {
                        _unitNear = (from unit in NearbyUnFriendlyPlayers
                                     where unit.IsValid
                                     //orderby unit.ThreatInfo.RawPercent ascending
                                     orderby unit.CurrentHealth ascending
                                     //where Me.Combat
                                     where Attackable(unit, 40)
                                     select unit).FirstOrDefault();
                    }
                }

                if (_unitNear == null && !InBattleground && !InArena)
                {
                    _unitNear = (from unit in NearbyUnFriendlyUnits
                                 where unit.IsValid
                                 //orderby unit.ThreatInfo.RawPercent ascending
                                 orderby unit.CurrentHealth descending
                                 where Me.Combat
                                 where !unit.IsSafelyBehind(Me)
                                 where (unit.CurrentTarget != null && unit.CurrentTarget == Me ||
                                        unit.IsTargetingMyPartyMember ||
                                        unit.IsTargetingMyRaidMember ||
                                        unit.IsTargetingMeOrPet)
                                 where Attackable(unit, 40)
                                 select unit).FirstOrDefault();
                }

                return _unitNear != null;
            }
        }

        #endregion

        #region GetBestTarget

        private static WoWUnit BestTarget;

        private static bool GetBestTarget()
        {
            if (InBattleground || InArena)
            {
                BestTarget = (from unit in NearbyUnFriendlyPlayers
                              where unit.IsValid &&
                                    Attackable(unit, 10)
                              orderby unit.CurrentHealth ascending
                              select unit).FirstOrDefault();

                if (BestTarget == null)
                {
                    BestTarget = (from unit in NearbyUnFriendlyPlayers
                                  where unit.IsValid &&
                                        Attackable(unit, 20)
                                  orderby unit.CurrentHealth ascending
                                  select unit).FirstOrDefault();
                }

                if (BestTarget == null)
                {
                    BestTarget = (from unit in NearbyUnFriendlyPlayers
                                  where unit.IsValid &&
                                        Attackable(unit, 40)
                                  orderby unit.CurrentHealth ascending
                                  select unit).FirstOrDefault();
                }
            }

            if (BestTarget == null && !InBattleground && !InArena)
            {
                BestTarget = (from unit in NearbyUnFriendlyUnits
                              where unit.IsValid &&
                                    //orderby unit.ThreatInfo.RawPercent ascending
                                    Me.Combat &&
                                    !unit.IsSafelyBehind(Me) &&
                                    (unit.IsTargetingMyPartyMember || unit.IsTargetingMyRaidMember ||
                                     unit.IsTargetingMeOrPet) &&
                                    Attackable(unit, 40)
                              orderby unit.CurrentHealth descending
                              select unit).FirstOrDefault();
            }
            return BestTarget != null;
        }

        #endregion

        #region GlobalCheck

        private static bool MeMounted;
        private static bool FacingOverrideMeCurrentTarget;
        private static bool MeCombat;
        private static bool InArena;
        private static bool InBattleground;
        private static bool InInstance;
        private static bool InDungeon;
        private static bool InRaid;
        private static DateTime MeCurrentMapCheckTime;

        private static void GlobalCheck()
        {
            MeMounted = Me.Mounted;

            MeCombat = Me.Combat;

            FacingOverrideMeCurrentTarget = FacingOverride(Me.CurrentTarget);


            if (MeCurrentMapCheckTime < DateTime.Now || Me.IsResting)
            {
                MeCurrentMapCheckTime = DateTime.Now + TimeSpan.FromSeconds(30);

                InArena = Me.CurrentMap.IsArena;

                InBattleground = Me.CurrentMap.IsBattleground;

                InInstance = Me.CurrentMap.IsInstance;

                InDungeon = Me.CurrentMap.IsDungeon;

                InRaid = Me.CurrentMap.IsRaid;
            }
        }

        #endregion

        #region IndexToKeys

        private static Keys KeyTwo;

        private static Keys IndexToKeys(int index)
        {
            switch (index)
            {
                case 1:
                    return Keys.A;
                case 2:
                    return Keys.B;
                case 3:
                    return Keys.C;
                case 4:
                    return Keys.D;
                case 5:
                    return Keys.E;
                case 6:
                    return Keys.F;
                case 7:
                    return Keys.G;
                case 8:
                    return Keys.H;
                case 9:
                    return Keys.I;
                case 10:
                    return Keys.J;
                case 11:
                    return Keys.K;
                case 12:
                    return Keys.L;
                case 13:
                    return Keys.M;
                case 14:
                    return Keys.N;
                case 15:
                    return Keys.O;
                case 16:
                    return Keys.P;
                case 17:
                    return Keys.Q;
                case 18:
                    return Keys.R;
                case 19:
                    return Keys.S;
                case 20:
                    return Keys.T;
                case 21:
                    return Keys.U;
                case 22:
                    return Keys.V;
                case 23:
                    return Keys.W;
                case 24:
                    return Keys.X;
                case 25:
                    return Keys.Y;
                case 26:
                    return Keys.Z;
                case 27:
                    return Keys.D1;
                case 28:
                    return Keys.D2;
                case 29:
                    return Keys.D3;
                case 30:
                    return Keys.D4;
                case 31:
                    return Keys.D5;
                case 32:
                    return Keys.D6;
                case 33:
                    return Keys.D7;
                case 34:
                    return Keys.D8;
                case 35:
                    return Keys.D9;
                case 36:
                    return Keys.D0;
                case 37:
                    return Keys.Up;
                case 38:
                    return Keys.Down;
                case 39:
                    return Keys.Left;
                case 40:
                    return Keys.Right;
            }
            return Keys.None;
        }

        #endregion

        #region IsAttacking

        //private static bool IsAttacking()
        //{
        //    return Me.IsCasting && Me.CastingSpell.Name == "Denounce";
        //}

        #endregion

        #region IsDummy

        private static bool IsDummy(WoWUnit target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }
            return target.Entry == 31146 || // Raider's
                   target.Entry == 67127 || // Shine Training Dummy
                   target.Entry == 46647 || // 81-85
                   target.Entry == 32546 || // Ebon Knight's (DK)
                   target.Entry == 31144 || // 79-80
                   target.Entry == 32543 || // Veteran's (Eastern Plaguelands)
                   target.Entry == 32667 || // 70
                   target.Entry == 32542 || // 65 EPL
                   target.Entry == 32666 || // 60
                   target.Entry == 30527; // ?? Boss one (no idea?)
        }

        #endregion

        #region IsEnemy

        //public WoWUnit MyPartyorRaidUnit;

        private static bool IsMyPartyRaidMember(WoWUnit u)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (u == null || !u.IsValid)
                {
                    return false;
                }

                if (!u.IsPlayer)
                {
                    if (RaidPartyMembers.Contains(u.CreatedByUnit))
                    {
                        return true;
                    }
                }
                else
                {
                    if (RaidPartyMembers.Contains(u))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        //private  bool UnitIsInMyPartyOrRaid(WoWUnit target)
        //{
        //    //using (StyxWoW.Memory.AcquireFrame())
        //    {
        //        MyPartyorRaidUnit = (from unit in ObjectManager.GetObjectsOfType<WoWPlayer>(true, true)
        //                             where unit.IsInMyPartyOrRaid
        //                             where unit.Guid == target.Guid || unit.Guid == target.CreatedByUnitGuid
        //                             select unit).FirstOrDefault();
        //    }
        //    return MyPartyorRaidUnit != null;
        //}

        private static bool IsEnemy(WoWUnit target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            //if (target.Entry == 62442 && !target.Attackable) //tsulong
            //{
            //    return false;
            //}

            //if (target.HasAura("Reshape Life") || //Reshape Life
            //    target.HasAura("Convert")) //Convert
            //{
            //    return true;
            //}

            if (IsMyPartyRaidMember(target))
            {
                return false;
            }

            if (InArena || InBattleground)
            {
                return true;
            }

            if (!target.IsFriendly || IsDummy(target) && Me.Combat && Me.IsFacing(target))
            {
                return true;
            }
            return false;
        }

        #endregion

        #region IsMeorFocus

        //private  bool IsMeorFocus(WoWUnit target)
        //{
        //    if (target == Me ||
        //        target == Me.FocusedUnit && ValidUnit(Me.FocusedUnit) && !IsEnemy(Me.FocusedUnit) ||
        //        target == Me.CurrentTarget && ValidUnit(Me.CurrentTarget) && !IsEnemy(Me.CurrentTarget) ||
        //        InArena)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        #endregion

        #region MeIsTank

        private static bool MeIsTank
        {
            get
            {
                //return (StyxWoW.Me.Role & WoWPartyMember.GroupRole.Tank) != 0 ||
                //        Tanks.All(t => !t.IsAlive) && StyxWoW.Me.HasAura("Bear Form");
                return (StyxWoW.Me.Role & WoWPartyMember.GroupRole.Tank) != 0;
            }
        }

        #endregion

        #region MyAura

        private static bool MyAura(string auraName, WoWUnit u)
        {
            using (StyxWoW.Memory.AcquireFrame())
            {
                if (u == null || !u.IsValid || !u.IsAlive || !u.HasAura(auraName))
                    return false;

                bool aura = u.GetAllAuras().Any(a => a.Name == auraName && a.CreatorGuid == Me.Guid);

                return aura;
                //return u.ActiveAuras.Any(a => a.Value.Name == auraName && a.Value.CreatorGuid == Me.Guid);
            }
        }


        private static bool MyAura(int auraID, WoWUnit u)
        {
            using (StyxWoW.Memory.AcquireFrame())
            {
                if (u == null || !u.IsValid || !u.IsAlive || !u.HasAura(auraID))
                    return false;

                bool aura = u.GetAllAuras().Any(a => a.SpellId == auraID && a.CreatorGuid == Me.Guid);

                return aura;

                //return u.ActiveAuras.Any(a => a.Value.SpellId == auraID && a.Value.CreatorGuid == Me.Guid);
            }
        }

        #endregion

        #region MyAuraTimeLeft

        private static double MyAuraTimeLeft(string auraName, WoWUnit u)
        {
            using (StyxWoW.Memory.AcquireFrame())
            {
                if (u == null || !u.IsValid || !u.IsAlive || !u.HasAura(auraName))
                    return 0;

                WoWAura aura = u.GetAllAuras().FirstOrDefault(a => a.Name == auraName && a.CreatorGuid == Me.Guid);

                if (aura == null)
                {
                    return 0;
                }
                return aura.TimeLeft.TotalMilliseconds;
            }
        }

        private static double MyAuraTimeLeft(int auraID, WoWUnit u)
        {
            using (StyxWoW.Memory.AcquireFrame())
            {
                if (u == null || !u.IsValid || !u.IsAlive || !u.HasAura(auraID))
                    return 0;

                WoWAura aura = u.GetAllAuras().FirstOrDefault(a => a.SpellId == auraID && a.CreatorGuid == Me.Guid);

                if (aura == null)
                {
                    return 0;
                }
                return aura.TimeLeft.TotalMilliseconds;
            }
        }

        #endregion

        #region MyAuraStackCount

        private static double MyAuraStackCount(string auraName, WoWUnit u)
        {
            using (StyxWoW.Memory.AcquireFrame())
            {
                if (u == null || !u.IsValid || !u.IsAlive || !u.HasAura(auraName))
                    return 0;

                WoWAura aura = u.ActiveAuras.Values.FirstOrDefault(a => a.Name == auraName && a.CreatorGuid == u.Guid);

                if (aura == null)
                {
                    return 0;
                }
                return aura.StackCount;
            }
        }

        private static double MyAuraStackCount(int auraID, WoWUnit u)
        {
            using (StyxWoW.Memory.AcquireFrame())
            {
                if (u == null || !u.IsValid || !u.IsAlive || !u.HasAura(auraID))
                    return 0;

                WoWAura aura = u.ActiveAuras.Values.FirstOrDefault(a => a.SpellId == auraID && a.CreatorGuid == u.Guid);

                if (aura == null)
                {
                    return 0;
                }
                return aura.StackCount;
            }
        }

        #endregion

        #region LogTest

        //private Composite LogTest(string message)
        //{
        //    return new Action(delegate
        //                          {
        //                              Logging.Write(LogLevel.Diagnostic, "LogTest: " + message);
        //                              return RunStatus.Failure;
        //                          });
        //}

        #endregion

        #region UnitAuraTimeLeft

        private static double UnitAuraTimeLeft(string auraName, WoWUnit u)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (u == null || !u.IsValid || !u.IsAlive)
                    return 0;

                WoWAura aura = u.ActiveAuras.Values.FirstOrDefault(a => a.Name == auraName && a.CreatorGuid == u.Guid);

                if (aura == null)
                {
                    return 0;
                }
                return aura.TimeLeft.TotalMilliseconds;
            }
        }

        private static double UnitAuraTimeLeft(int auraID, WoWUnit u)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (u == null)
                    return 0;

                WoWAura aura = u.ActiveAuras.Values.FirstOrDefault(a => a.SpellId == auraID && a.CreatorGuid == u.Guid);

                if (aura == null)
                {
                    return 0;
                }
                return aura.TimeLeft.TotalMilliseconds;
            }
        }

        #endregion

        #region SafelyFacingTarget

        private static void SafelyFacingTarget(WoWUnit unit)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (THSettings.Instance.AutoFace && !IsOverrideModeOn && !Me.IsSafelyFacing(unit))
                {
                    Me.SetFacing(unit.Location);
                    //unit.Face();
                }
            }
        }

        #endregion

        #region SetAutoAttack

        private static Composite SetAutoAttack()
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                return new Action(delegate
                    {
                        if (Me.IsAutoAttacking &&
                            (!THSettings.Instance.AutoAttack ||
                             !Attackable(Me.CurrentTarget, 30))) // ||
                            //Me.CurrentTarget.HasAura("Water Shield") &&
                            //InArena))
                        {
                            Logging.Write("Stop Attack");
                            Lua.DoString("RunMacroText('/stopattack');");
                        }

                        //Disable this, it conflic with the targeting system
                        if (THSettings.Instance.AutoAttack &&
                            !Me.IsAutoAttacking &&
                            (Me.HasAura(115192) ||
                             !IsStealthedNoSD(Me)) &&
                            Attackable(Me.CurrentTarget, 10))
                        {
                            Logging.Write("Start Attack");
                            Lua.DoString("RunMacroText('/startattack');");
                        }

                        //Logging.Write(Me.IsAutoAttacking ? "Auto Attack ON" : "Auto Attack OFF");

                        return RunStatus.Failure;
                    });
            }
        }

        #endregion

        #region TalentSort

        private static byte TalentSort(WoWUnit target)
        {
            if (target == null)
            {
                return 0;
            }

            if (!target.IsPlayer)
            {
                return 0;
            }

            if (target.Class == WoWClass.DeathKnight)
            {
                return 1;
            }

            if (target.Class == WoWClass.Druid)
            {
                if (target.Buffs.ContainsKey("Moonkin Form"))
                    return 3;
                if (target.MaxMana < target.MaxHealth/2 ||
                    (target.Buffs.ContainsKey("Leader of the Pack") &&
                     target.Buffs["Leader of the Pack"].CreatorGuid == target.Guid))
                    return 1;
                return 4;
            }

            if (target.Class == WoWClass.Hunter)
            {
                return 2;
            }

            if (target.Class == WoWClass.Mage)
            {
                return 3;
            }

            if (target.Class == WoWClass.Monk)
            {
                if (target.HasAura("Stance of the Wise Serpent"))
                    return 4;
                return 1;
            }

            if (target.Class == WoWClass.Paladin)
            {
                if (target.MaxMana > target.MaxMana/2)
                    return 4;
                return 1;
            }

            if (target.Class == WoWClass.Priest)
            {
                if (target.HasAura("Shadowform"))
                    return 3;
                return 4;
            }

            if (target.Class == WoWClass.Rogue)
            {
                return 1;
            }

            if (target.Class == WoWClass.Shaman)
            {
                if (target.MaxMana < target.MaxHealth/2)
                    return 1;
                if (target.Buffs.ContainsKey("Elemental Oath") &&
                    target.Buffs["Elemental Oath"].CreatorGuid == target.Guid)
                    return 3;
                return 4;
            }

            if (target.Class == WoWClass.Warlock)
            {
                return 3;
            }

            if (target.Class == WoWClass.Warrior)
            {
                return 1;
            }

            return 0;
        }

        private static byte TalentSortSimple(WoWUnit target)
        {
            byte sortSimple = TalentSort(target);

            if (sortSimple == 4)
            {
                return 4;
            }

            if (sortSimple < 4 && sortSimple > 0)
            {
                return 1;
            }

            return 0;
        }

        private static string TalentSortRole(WoWUnit target)
        {
            switch (TalentSort(target))
            {
                case 1:
                    return "Melee";
                case 2:
                    return "Range DPS";
                case 3:
                    return "Range DPS";
                case 4:
                    return "Healer";
            }
            return "Unknow";
        }

        #endregion

        #region TalentValue

        private static int TalentValue(int blood, int frost, int unholy)
        {
            return GetCurrentSpec() == "Blood" ? blood : (GetCurrentSpec() == "Unholy" ? unholy : frost);
        }

        private static bool TalentValue(bool blood, bool frost, bool unholy)
        {
            return GetCurrentSpec() == "Blood" ? blood : (GetCurrentSpec() == "Unholy" ? unholy : frost);
        }

        #endregion

        #region UpdateStatusEvent

        private static void UpdateStatusEvent(object sender, LuaEventArgs args)
        {
            THSettings.Instance.UpdateStatus = true;
        }

        #endregion

        #region OnBotStartedEvent

        private static void OnBotStartedEvent(object o)
        {
            if (Me.Inventory.Equipped.Hands != null &&
                Me.Inventory.Equipped.Hands.Name.Contains("Malevolent Gladiator's Scaled Gauntlets"))
            {
            }
            Logging.Write("----------------------------------");
            Logging.Write("Update Status on Bot Start");
            Logging.Write("----------------------------------");

            THSettings.Instance.Pause = false;
            THSettings.Instance.UpdateStatus = true;
        }

        #endregion

        #region SafeName

        private static string SafeName(WoWUnit unit)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (unit == null || !unit.IsValid)
                {
                    return "null or invalid";
                }
                if (unit == Me)
                {
                    return "Myself";
                }
                if (unit.IsPlayer)
                {
                    return unit.Class.ToString();
                }
                if (unit.IsPet)
                {
                    return unit.CreatureType.ToString();
                }
            }
            return unit.Name;
        }

        #endregion

        #region SpecialUnit

        //private static readonly HashSet<uint> SpecialUnit = new HashSet<uint>
        //    {
        //        60491, //Sha of Anger
        //        62346, //Galleon
        //        60410, //Elegon
        //        60776, //Empyreal Focus
        //        60793, //Celestial Protector
        //        60913, //Energy Charge
        //        60143, //garajal-the-spiritbinder
        //        60412, //Empyreal Focus
        //        63667, //garalon
        //    };

        #endregion

        #region UpdateEventHandler

        private static void UpdateEventHandler()
        {
            //Logging.Write(LogLevel.Diagnostic, "UpdateEventHandler");

            if (!CombatLogAttachedTH &&
                (InBattleground ||
                 InArena) &&
                (TreeRoot.Current.Name == "Tyrael" ||
                 TreeRoot.Current.Name == "LazyRaider" ||
                 TreeRoot.Current.Name == "Raid Bot" ||
                 TreeRoot.Current.Name == "Combat Bot"))
            {
                AttachCombatLogEventTH();
                //Logging.Write(LogLevel.Diagnostic, "AttachCombatLogEvent");
            }
            if (CombatLogAttachedTH &&
                !InBattleground &&
                !InArena)
            {
                DetachCombatLogEventTH();
            }

            if (!EventHandlers.CombatLogAttached &&
                !InBattleground &&
                !InArena &&
                !InDungeon &&
                !InRaid &&
                (TreeRoot.Current.Name != "Tyrael" &&
                 TreeRoot.Current.Name != "LazyRaider" &&
                 TreeRoot.Current.Name != "Raid Bot" &&
                 TreeRoot.Current.Name != "Combat Bot"))
            {
                EventHandlers.AttachCombatLogEvent();
                //Logging.Write(LogLevel.Diagnostic, "AttachCombatLogEvent");
            }

            if (EventHandlers.CombatLogAttached &&
                (InBattleground ||
                 InArena ||
                 InDungeon ||
                 InRaid))
            {
                EventHandlers.DetachCombatLogEvent();
            }
        }

        private static Composite UpdateEventHandlerComp()
        {
            return new Action(
                ret =>
                    {
                        //AttachCombatLogEventTH();
                        //DetachCombatLogEventTH();
                        UpdateEventHandler();
                        return RunStatus.Failure;
                    }
                );
        }

        #endregion

        #region UpdateCurrentMap

        //public string CurrentMap;

        //private static void UpdateCurrentMapEvent(BotEvents.Player.MapChangedEventArgs args)
        //{
        //    THSettings.Instance.UpdateStatus = true;
        //}

        //private static void UpdateCurrentMap()
        //{
        //    if (InArena)
        //    {
        //        CurrentMap = "Arena";
        //    }
        //    else if (InBattleground && Me.IsFFAPvPFlagged)
        //    {
        //        CurrentMap = "Rated Battleground";
        //    }
        //    else if (InBattleground)
        //    {
        //        CurrentMap = "Battleground";
        //    }
        //    else if (InDungeon)
        //    {
        //        CurrentMap = "Dungeon";
        //    }
        //    else if (InRaid)
        //    {
        //        CurrentMap = "Raid";
        //    }
        //    else
        //    {
        //        CurrentMap = "World";
        //    }

        //    Logging.Write("----------------------------------");
        //    Logging.Write("CurrentMap: " + CurrentMap);
        //    Logging.Write("----------------------------------");
        //}

        #endregion

        #region UpdateMyLatency

        private static double MyLatency;

        private static void UpdateMyLatency()
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (THSettings.Instance.LagTolerance)
                {
                    //If SLagTolerance enabled, start casting next spell MyLatency Millisecond before GlobalCooldown ready.

                    MyLatency = (StyxWoW.WoWClient.Latency);

                    //Use here because Lag Tolerance cap at 400
                    //Logging.Write("----------------------------------");
                    //Logging.Write("MyLatency: " + MyLatency);
                    //Logging.Write("----------------------------------");

                    if (MyLatency > 400)
                    {
                        //Lag Tolerance cap at 400
                        MyLatency = 400;
                    }
                }
                else
                {
                    MyLatency = 0;
                }
            }
        }

        #endregion

        #region UpdateMyTalent

        private static string _hasTalent = "";

        private static void UpdateMyTalentEvent(object sender, LuaEventArgs args)
        {
            UpdateMyTalent();
        }

        private static void UpdateMyTalent()
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                _hasTalent = "";
                for (int i = 1; i <= 18; i++)
                {
                    _hasTalent = _hasTalent +
                                 Lua.GetReturnVal<String>(
                                     string.Format(
                                         "local t= select(5,GetTalentInfo({0})) if t == true then return '['..select(1,GetTalentInfo({0}))..'] ' end return nil",
                                         i), 0);
                }

                Logging.Write("----------------------------------");
                Logging.Write("Talent:");
                Logging.Write(_hasTalent);
                Logging.Write("----------------------------------");
                _hasTalent = "";
            }
        }

        #endregion

        #region UpdateRaidPartyMembers

        private static IEnumerable<WoWPartyMember> GroupMembers
        {
            get { return !Me.GroupInfo.IsInRaid ? Me.GroupInfo.PartyMembers : Me.GroupInfo.RaidMembers; }
        }

        private static void UpdateRaidPartyMembersEvent(object sender, LuaEventArgs args)
        {
            UpdateRaidPartyMembers();
        }

        private static DateTime UpdateRaidPartyMembersLast;

        //private static Composite UpdateRaidPartyMembersComp()
        //{
        //    return new Decorator(
        //        ret =>
        //        UpdateRaidPartyMembersLast < DateTime.Now ||
        //        Me.HasAura("Arena Preparation") ||
        //        Me.HasAura("Preparation"),
        //        new Action(delegate
        //            {
        //                UpdateRaidPartyMembers();
        //                UpdateRaidPartyMembersLast = DateTime.Now + TimeSpan.FromSeconds(10);
        //                return RunStatus.Failure;
        //            })
        //        );
        //}

        private static readonly List<WoWPlayer> RaidPartyMembers = new List<WoWPlayer>();

        private static void UpdateRaidPartyMembers(bool writelog = true)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                RaidPartyMembers.Clear();
                //Logging.Write("Debug " + GroupHealingMembers.Count() + " Members");

                foreach (var woWPartyMember in GroupMembers)
                {
                    if (woWPartyMember.ToPlayer() != null)
                    {
                        //Logging.Write("Add " + woWPartyMember.ToPlayer().Class + " to RaidPartyMembers");
                        RaidPartyMembers.Add(woWPartyMember.ToPlayer());
                    }
                }

                if (!RaidPartyMembers.Contains(Me))
                {
                    RaidPartyMembers.Add(Me);
                }

                if (writelog && RaidPartyMembers.Any())
                {
                    Logging.Write("----------------------------------");
                    Logging.Write("RaidPartyMembers: " + RaidPartyMembers.Count() + " Members");

                    foreach (WoWPlayer member in RaidPartyMembers)
                    {
                        Logging.Write(LogLevel.Diagnostic, "{0} ({3}) - {1} k / {2} k hp", SafeName(member),
                                      Math.Round((double) (member.CurrentHealth/1000), 0),
                                      Math.Round((double) (member.MaxHealth/1000), 0),
                                      TalentSortRole(member));
                    }

                    Logging.Write("----------------------------------");
                }
            }
        }

        #endregion

        #region UseHealthstone

        private static
            Composite UseHealthstone()
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                return
                    new Decorator(
                        ret =>
                        THSettings.Instance.HealthStone &&
                        Me.Combat &&
                        Me.HealthPercent < THSettings.Instance.HealthStoneHP &&
                        Me.CurrentTarget != null &&
                        Me.CurrentTarget.CurrentHealth > Me.HealthPercent,
                        new Action(delegate
                            {
                                WoWItem hs = Me.BagItems.FirstOrDefault(o => o.Entry == 5512);
                                //5512 Healthstone
                                if (hs != null && hs.CooldownTimeLeft.TotalMilliseconds <= MyLatency)
                                {
                                    hs.Use();
                                    //Lua.DoString("RunMacroText(\"/s Used Healthstone\")");
                                    Logging.Write("Use Healthstone at " + Me.HealthPercent + "%");
                                }
                                return RunStatus.Failure;
                            })
                        );
            }
        }

        #endregion

        #region UpdateMyGlyph

        private static void UpdateMyGlyphEvent(object sender, LuaEventArgs args)
        {
            UpdateMyGlyph();
        }

        //private static readonly HashSet<string> NoGCDSpells = new HashSet<string> { };
        private static string HasGlyph;
        private static string HasGlyphName;

        private static void UpdateMyGlyph()
        {
            HasGlyph = "";
            HasGlyphName = "";
            //using (StyxWoW.Memory.AcquireFrame())
            {
                var glyphCount = Lua.GetReturnVal<int>("return GetNumGlyphSockets()", 0);

                if (glyphCount != 0)
                {
                    for (int i = 1; i <= glyphCount; i++)
                    {
                        string lua =
                            String.Format(
                                "local enabled, glyphType, glyphTooltipIndex, glyphSpellID, icon = GetGlyphSocketInfo({0});if (enabled) then return glyphSpellID else return 0 end",
                                i);

                        var glyphSpellId = Lua.GetReturnVal<int>(lua, 0);

                        try
                        {
                            if (glyphSpellId > 0)
                            {
                                HasGlyphName = HasGlyphName + "[" + (WoWSpell.FromId(glyphSpellId)) + " - " +
                                               glyphSpellId +
                                               "] ";
                                HasGlyph = HasGlyph + "[" + glyphSpellId + "] ";
                            }
                            else
                            {
                                Logging.Write("Glyphdetection - No Glyph in slot " + i);
                                //TreeRoot.Stop();
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging.Write("We couldn't detect your Glyphs");
                            Logging.Write("Report this message to us: " + ex);
                            //TreeRoot.Stop();
                        }
                    }
                }

                Logging.Write("----------------------------------");
                Logging.Write("Glyph:");
                Logging.Write(HasGlyphName);
                Logging.Write("----------------------------------");

                if (HasGlyph.Contains("42955"))
                {
                    AmbushBonusRange = 5;
                }
                else
                {
                    AmbushBonusRange = 0;
                }
            }
        }

        #endregion

        #region UpdateStatus

        private static string UseRotation;
        private static double MaxComboPoint = 5;

        private static void UpdateStatus()
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (THSettings.Instance.UpdateStatus)
                    if (Me.IsValid)
                    {
                        {
                            //Reset BurstLast on start
                            BurstLast = DateTime.Now;

                            //if (THSettings.Instance.LastSavedSpec != GetCurrentSpec())
                            //{
                            //    Logging.Write("----------------------------------");
                            //    Logging.Write("You just change specialization to " + GetCurrentSpec() + ". ");
                            //    Logging.Write("Please select the appropriate settings");
                            //    Logging.Write("----------------------------------");

                            //    OnButtonPress();
                            //}

                            Logging.Write("----------------------------------");
                            Logging.Write("Building Rotation base on current Talents and Glyphs......");
                            Logging.Write("----------------------------------");
                            Logging.Write("");

                            //if (TreeRoot.Current.Name != "Questing" && 
                            //    TreeRoot.Current.Name != "Grind Bot" &&
                            //    TreeRoot.Current.Name != "LazyRaider" && 
                            //    TreeRoot.Current.Name != "BGBuddy" &&
                            //    TreeRoot.Current.Name != "DungeonBuddy" && 
                            //    TreeRoot.Current.Name != "Mixed Mode" && 
                            //    GetAsyncKeyState(Keys.F10) == 0)
                            //{
                            //    Logging.Write("This CC only work with DungeonBuddy - LazyRaider - Questing - Grind Bot.");
                            //    Logging.Write("Custom Class Stop.");

                            //    TreeRoot.Stop();
                            //}

                            if (TreeRoot.Current.Name == "LazyRaider" && !THSettings.Instance.AutoMove)
                            {
                                THSettings.Instance.AutoMove = false;
                                //Logging.Write(LogLevel.Diagnostic,
                                //              "EnableMovement: " + THSettings.Instance.EnableMovement);
                            }
                            //else
                            //{
                            //    THSettings.Instance.SAutoMove = true;
                            //    THSettings.Instance.SAutoTarget = true;
                            //Logging.Write(LogLevel.Diagnostic,
                            //              "EnableMovement: " + THSettings.Instance.EnableMovement);
                            //}

                            //if (Me.Level < 85)
                            //{
                            //    Logging.Write("This CC only work on Level 85-90.");
                            //    //TreeRoot.Stop();
                            //    return;
                            //}

                            UpdateRaidPartyMembers();

                            //UpdateEventHandler();

                            UpdateMyTalent();

                            UpdateMyGlyph();

                            UpdateMyLatency();

                            _gcdReady = DateTime.Now;
                            Logging.Write("----------------------------------");
                            Logging.Write("Building Rotation Completed");
                            Logging.Write("----------------------------------");
                            Logging.Write("");

                            Logging.Write("----------------------------------");
                            Logging.Write("Hold 1 second Control + " + IndexToKeys(THSettings.Instance.PauseKey) +
                                          " To Toggle Pause Mode.");
                            Logging.Write("----------------------------------");
                            Logging.Write("");

                            //Logging.Write("----------------------------------");
                            //Logging.Write("Press Control + " + IndexToKeys(THSettings.Instance.AoEKey) +
                            //              " To Toggle Pause Mode.");
                            //Logging.Write("----------------------------------");
                            //Logging.Write("");

                            if (THSettings.Instance.BurstKey > 6)
                            {
                                Logging.Write("----------------------------------");
                                Logging.Write("Hold 1 second Control + " + IndexToKeys(THSettings.Instance.BurstKey - 6) +
                                              " To Toggle Burst Mode");
                                Logging.Write("----------------------------------");
                                Logging.Write("");
                            }

                            //CheckEnvironmentLast = DateTime.Now;

                            switch (Me.Specialization)
                            {
                                case WoWSpec.RogueAssassination:
                                    UseRotation = "RogueAssassination";
                                    break;
                                case WoWSpec.RogueCombat:
                                    UseRotation = "RogueCombat";
                                    break;
                                case WoWSpec.RogueSubtlety:
                                    UseRotation = "RogueSubtlety";
                                    break;
                                default:
                                    UseRotation = "RogueSubtlety";
                                    break;
                            }

                            if (SpellManager.HasSpell("Anticipation"))
                            {
                                MaxComboPoint = 10;
                            }
                            else
                            {
                                MaxComboPoint = 5;
                            }
                            THSettings.Instance.UpdateStatus = false;
                        }
                    }
            }
        }

        #endregion

        #region ValidUnit

        public static bool ValidUnit(WoWUnit u)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (u != null &&
                    u.IsValid &&
                    u.Attackable &&
                    u.CanSelect &&
                    u.IsAlive &&
                    !u.IsCritter &&
                    !Blacklist.Contains(u.Guid, BlacklistFlags.All))
                {
                    return true;
                }

                return false;
                //if (u == null || !u.IsValid || Blacklist.Contains(u.Guid, BlacklistFlags.All) || !u.Attackable ||
                //    !u.IsAlive || !u.CanSelect || u.IsDead ||
                //    u.IsCritter && (u.CurrentTarget == null || u.CurrentTarget != null && u.CurrentTarget != Me) ||
                //    u.IsNonCombatPet)
                //{
                //    return false;
                //}
                //return true;
            }
        }

        #endregion
    }
}