using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.CommonBot.Inventory;
using Styx.Helpers;
using Styx.Pathing;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Action = Styx.TreeSharp.Action;

namespace TuanHARogue
{
    public partial class Classname
    {
        #region AdrenalineRush

        private static Composite AdrenalineRush()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.AdrenalineRush &&
                (THSettings.Instance.ShadowBladesCD ||
                 THSettings.Instance.ShadowBladesBurst &&
                 Burst) &&
                SpellManager.HasSpell("Adrenaline Rush") &&
                !MeMounted &&
                CurrentTargetAttackable(5) &&
                Me.IsFacing(Me.CurrentTarget) &&
                PlayerEnergy < Me.MaxEnergy - 20 &&
                SpellManager.CanCast("Adrenaline Rush"),
                new Action(delegate
                    {
                        CastSpell("Adrenaline Rush", Me, "AdrenalineRush");
                        return RunStatus.Failure;
                    })
                );
        }

        #endregion

        #region Ambush

        private static bool AmbushCheck()
        {
            if (!SpellManager.HasSpell("Ambush"))
            {
                return false;
            }

            if (!IsStealthed(Me))
            {
                return false;
            }

            if (Me.CurrentTarget == null)
            {
                return false;
            }

            if (!Me.IsBehind(Me.CurrentTarget))
            {
                return false;
            }

            if (PlayerEnergy > THSettings.Instance.ShadowDanceEnergy)
            {
                return false;
            }
            return true;
        }

        private static bool AmbushShadowDanceCheck()
        {
            if (!Me.HasAura("Shadow Dance") || !Me.IsBehind(Me.CurrentTarget))
            {
                return false;
            }
            return true;
        }

        private static double AmbushBonusRange;

        private static Composite AmbushOld()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    (THSettings.Instance.AttackOOC || Me.Combat) &&
                    THSettings.Instance.Ambush &&
                    !SpellManager.HasSpell("Cloak and Dagger") &&
                    SpellManager.HasSpell("Ambush") &&
                    !MeMounted &&
                    !CheapShotCheck() &&
                    IsStealthed(Me) &&
                    CurrentTargetAttackable(5 + AmbushBonusRange) &&
                    PlayerComboPoint < MaxComboPoint &&
                    Me.IsBehind(Me.CurrentTarget) &&
                    FacingOverrideMeCurrentTarget,
                    new Action(delegate
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            if (!SpellManager.CanCast("Ambush"))
                            {
                                return RunStatus.Success;
                            }
                            CastSpell("Ambush", Me.CurrentTarget, "Ambush");
                            return RunStatus.Success;
                        })),
                new Decorator(
                    ret =>
                    (THSettings.Instance.AttackOOC || Me.Combat) &&
                    THSettings.Instance.Ambush &&
                    SpellManager.HasSpell("Cloak and Dagger") &&
                    SpellManager.HasSpell("Ambush") &&
                    !MeMounted &&
                    !CheapShotCheck() &&
                    IsStealthedNoSD(Me) &&
                    CurrentTargetAttackable(30) &&
                    PlayerComboPoint < MaxComboPoint &&
                    FacingOverrideMeCurrentTarget &&
                    (DebuffCC(Me.CurrentTarget) ||
                     Me.IsBehind(Me.CurrentTarget) ||
                     Me.CurrentTarget.IsPlayer ||
                     !Me.CurrentTarget.IsPlayer &&
                     (Me.CurrentTarget.CurrentTarget == null ||
                      Me.CurrentTarget.CurrentTarget != null &&
                      Me.CurrentTarget.CurrentTarget != Me)),
                    new Action(delegate
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            if (!SpellManager.CanCast("Ambush"))
                            {
                                return RunStatus.Success;
                            }
                            CastSpell("Ambush", Me.CurrentTarget, "Ambush Cloak and Dagger");
                            return RunStatus.Success;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Ambush &&
                    SpellManager.HasSpell("Cloak and Dagger") &&
                    SpellManager.HasSpell("Ambush") &&
                    !MeMounted &&
                    !CheapShotCheck() &&
                    IsShadowDance(Me) &&
                    CurrentTargetAttackable(5 + AmbushBonusRange) &&
                    PlayerComboPoint < MaxComboPoint &&
                    FacingOverrideMeCurrentTarget &&
                    Me.IsBehind(Me.CurrentTarget) &&
                    (DebuffCC(Me.CurrentTarget) ||
                     Me.IsBehind(Me.CurrentTarget) ||
                     Me.CurrentTarget.IsPlayer ||
                     !Me.CurrentTarget.IsPlayer &&
                     (Me.CurrentTarget.CurrentTarget == null ||
                      Me.CurrentTarget.CurrentTarget != null &&
                      Me.CurrentTarget.CurrentTarget != Me)),
                    new Action(delegate
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            if (!SpellManager.CanCast("Ambush"))
                            {
                                return RunStatus.Success;
                            }
                            CastSpell("Ambush", Me.CurrentTarget, "Ambush IsShadowDance");
                            return RunStatus.Success;
                        }))
                );
        }

        private static Composite AmbushCaD()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.Ambush &&
                SpellManager.HasSpell("Ambush") &&
                SpellManager.HasSpell("Cloak and Dagger") &&
                !MeMounted &&
                CurrentTargetAttackable(30) &&
                FacingOverrideMeCurrentTarget &&
                IsStealthedNoSD(Me) &&
                //PlayerComboPoint < MaxComboPoint &&
                (DebuffCC(Me.CurrentTarget) ||
                 Me.IsBehind(Me.CurrentTarget) ||
                 Me.CurrentTarget.IsPlayer ||
                 !Me.CurrentTarget.IsPlayer &&
                 (Me.CurrentTarget.CurrentTarget == null ||
                  Me.CurrentTarget.CurrentTarget != null &&
                  Me.CurrentTarget.CurrentTarget != Me)) &&
                //!CheapShotCheck() &&
                //CalculateEnergy(60, MyAuraTimeLeft(115192, Me)) > 30 &&
                SpellManager.CanCast("Ambush"),
                new Action(delegate
                    {
                        SafelyFacingTarget(Me.CurrentTarget);
                        CastSpell("Ambush", Me.CurrentTarget, "Ambush Cloak and Dagger");
                    }))
                ;
        }

        private static Composite Ambush()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.Ambush &&
                SpellManager.HasSpell("Ambush") &&
                !MeMounted &&
                CurrentTargetAttackable(5 + AmbushBonusRange) &&
                FacingOverrideMeCurrentTarget &&
                IsStealthed(Me) &&
                //PlayerComboPoint < MaxComboPoint &&
                Me.IsBehind(Me.CurrentTarget) &&
                //!CheapShotCheck() &&
                //CalculateEnergy(60, MyAuraTimeLeft(115192, Me)) > 30 &&
                SpellManager.CanCast("Ambush"),
                new Action(delegate
                    {
                        SafelyFacingTarget(Me.CurrentTarget);
                        CastSpell("Ambush", Me.CurrentTarget, "OpenerRotation: Ambush");
                    }));
        }

        #endregion

        #region AttackOOC

        private static Composite StopAttackOOC()
        {
            return new Decorator(
                ret =>
                !THSettings.Instance.AttackOOC &&
                !Me.Combat &&
                Me.IsAutoAttacking,
                new Action(
                    ret =>
                        {
                            Logging.Write("Stop Attack Out of Combat");
                            if (Me.IsAutoAttacking)
                            {
                                Lua.DoString("RunMacroText('/stopattack');");
                            }
                            return RunStatus.Success;
                        })
                );
        }

        #endregion

        #region AttackASAP

        private static WoWUnit UnitAttackASAP;

        private static bool GetUnitAttackASAP()
        {
            if (!InArena && !InBattleground)
            {
                return false;
            }

            UnitAttackASAP = null;

            //using (StyxWoW.Memory.AcquireFrame())
            {
                //Spirit Link Totem 53006
                //Earthgrab Totem 60561
                //Earthbind Totem 2630
                //Windwalk Totem 59717
                //Mana Tide Totem 10467
                //Healing Tide Totem 59764 
                //Capacitor Totem 61245 
                UnitAttackASAP = NearbyUnFriendlyUnits.FirstOrDefault(
                    unit => unit != null && unit.IsValid &&
                            FacingOverride(unit) &&
                            (unit.IsPlayer && unit.IsCasting &&
                             (unit.CastingSpell.Id == 98322 || unit.CastingSpell.Id == 98323 ||
                              unit.CastingSpell.Id == 98324) || //Capturing
                             unit.Entry == 59190 || //Psyfiend
                             unit.IsTotem &&
                             (unit.Entry == 53006 ||
                              unit.Entry == 60561 ||
                              unit.Entry == 2630 ||
                              unit.Entry == 59717 ||
                              unit.Entry == 10467 ||
                              unit.Entry == 59764 ||
                              unit.Entry == 61245)) &&
                            Attackable(unit, 30));

                //UnitAttackASAP = (from unit in NearbyUnFriendlyUnits
                //                  where unit != null &&
                //                        unit.IsValid &&
                //                        FacingOverride(unit) &&
                //                        Attackable(unit, 30) &&
                //                        //Spirit Link Totem 53006
                //                        //Earthgrab Totem 60561
                //                        //Earthbind Totem 2630
                //                        //Windwalk Totem 59717
                //                        //Mana Tide Totem 10467
                //                        //Healing Tide Totem 59764 
                //                        //Capacitor Totem 61245 
                //                        (unit.IsPlayer &&
                //                         unit.IsCasting &&
                //                         (unit.CastingSpell.Id == 98322 || //Capturing
                //                          unit.CastingSpell.Id == 98323 || //Capturing
                //                          unit.CastingSpell.Id == 98324) || //Capturing
                //                         unit.Entry == 59190 || //Psyfiend
                //                         //unit.Entry == 65282 && Attackable(unit, 5) || //Void Tendril
                //                         unit.IsTotem &&
                //                         (unit.Entry == 53006 ||
                //                          unit.Entry == 60561 ||
                //                          unit.Entry == 2630 ||
                //                          unit.Entry == 59717 ||
                //                          unit.Entry == 10467 ||
                //                          unit.Entry == 59764 ||
                //                          unit.Entry == 61245))
                //                  select unit).
                //    FirstOrDefault();
            }

            //if (UnitAttackASAP == null)
            //{
            //    //using (StyxWoW.Memory.AcquireFrame())
            //    {
            //        UnitAttackASAP = (from unit in NearbyUnFriendlyUnits
            //                          where unit!=null && unit.IsValid
            //                          where unit.IsPet && unit.IsCasting
            //                          where Attackable(unit, 40)
            //                          where Me.IsFacing(unit)
            //                          where
            //                              //Mesmerize || Seduction
            //                              (unit.CastingSpell.Id == 115268 ||
            //                               unit.CastingSpell.Id == 6358)
            //                          select unit).FirstOrDefault();
            //    }
            //}

            return UnitAttackASAP != null;
        }

        private static Composite AttackASAP()
        {
            return new PrioritySelector(
                //new Decorator(
                //    ret =>
                //    THSettings.Instance.AttackASAP &&
                //    !CurrentTargetAttackable(50) &&
                //    DebuffRoot(Me) &&
                //    GetUnitAttackASAP() &&
                //    UnitAttackASAP != null &&
                //    UnitAttackASAP.IsValid &&
                //    UnitAttackASAP.Entry == 65282,
                //    new Action(
                //        ret =>
                //            {
                //                Logging.Write("Target Void Tendril - You are rooted lol");
                //                UnitAttackASAP.Target();
                //                return RunStatus.Failure;
                //            })
                //    ),
                new Decorator(
                    ret =>
                    (THSettings.Instance.AttackOOC || Me.Combat) &&
                    THSettings.Instance.AttackASAP &&
                    SpellManager.HasSpell("Gouge") &&
                    !IsStealthed(Me) &&
                    GetUnitAttackASAP() &&
                    UnitAttackASAP != null &&
                    UnitAttackASAP.IsValid &&
                    UnitAttackASAP.Entry != 59190 &&
                    UnitAttackASAP.IsWithinMeleeRange &&
                    //UnitAttackASAP.IsCasting &&
                    (HasGlyph.Contains("56809") || UnitAttackASAP.IsFacing(Me)) &&
                    SpellManager.CanCast("Gouge"),
                    new Action(
                        ret =>
                            {
                                SafelyFacingTarget(UnitAttackASAP);
                                CastSpell("Gouge", UnitAttackASAP, "Gouge: Psyfiend");
                            })
                    ),
                new Decorator(
                    ret =>
                    (THSettings.Instance.AttackOOC || Me.Combat) &&
                    THSettings.Instance.AttackASAP &&
                    SpellManager.HasSpell("Fan of Knives") &&
                    !IsStealthed(Me) &&
                    GetUnitAttackASAP() &&
                    UnitAttackASAP != null &&
                    UnitAttackASAP.IsValid &&
                    UnitAttackASAP.IsPlayer &&
                    DistanceCheck(UnitAttackASAP) <= 10 &&
                    SpellManager.CanCast("Fan of Knives"),
                    new Action(
                        ret =>
                            {
                                SafelyFacingTarget(UnitAttackASAP);
                                CastSpell("Fan of Knives", Me, "Fan of Knives: Flag Capper");
                            })
                    ),
                new Decorator(
                    ret =>
                    (THSettings.Instance.AttackOOC || Me.Combat) &&
                    THSettings.Instance.AttackASAP &&
                    SpellManager.HasSpell("Throw") &&
                    !IsStealthed(Me) &&
                    !CurrentTargetAttackable(5) &&
                    GetUnitAttackASAP() &&
                    UnitAttackASAP != null &&
                    UnitAttackASAP.IsValid &&
                    UnitAttackASAP.IsTotem &&
                    SpellManager.CanCast("Throw"),
                    new Action(
                        ret =>
                            {
                                SafelyFacingTarget(UnitAttackASAP);
                                CastSpell("Throw", UnitAttackASAP, "Throw: Flag Capper or Totem");
                            })
                    )
                )
                ;
        }

        #endregion

        #region Backstab

        private static Composite Backstab()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.Backstab &&
                BurstCooldownEnergyPool < DateTime.Now &&
                SpellManager.HasSpell("Backstab") &&
                !MeMounted &&
                (PlayerComboPoint < MaxComboPoint ||
                 PlayerEnergy >= Me.MaxEnergy) &&
                CurrentTargetAttackable(5) &&
                PoolEnergyPassCheck() &&
                FacingOverrideMeCurrentTarget &&
                Me.IsBehind(Me.CurrentTarget) &&
                //!AmbushCheck() &&
                SpellManager.CanCast("Backstab"),
                new Action(delegate
                    {
                        SafelyFacingTarget(Me.CurrentTarget);
                        CastSpell("Backstab", Me.CurrentTarget, "Backstab");
                    })
                );
        }

        #endregion

        #region BladeFlurry

        private static Composite BladeFlurry()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.BladeFlurry &&
                SpellManager.HasSpell("Blade Flurry") &&
                !MeMounted &&
                CurrentTargetAttackable(10) &&
                Me.IsFacing(Me.CurrentTarget) &&
                SpellManager.CanCast("Blade Flurry"),
                new Action(delegate
                    {
                        CastSpell("Blade Flurry", Me, "Blade Flurry");
                        return RunStatus.Failure;
                    })
                );
        }

        private static Composite BladeFlurryCancel()
        {
            return new Decorator(
                ret =>
                Me.HasAura("Blade Flurry"),
                new Action(delegate
                    {
                        Lua.DoString("RunMacroText('/cancelaura " + WoWSpell.FromId(13877) + "');");
                        //Lua.DoString("CancelUnitBuff(\"Player\",\"Righteous Fury\")");
                        return RunStatus.Failure;
                    })
                );
        }

        #endregion

        #region BloodFury

        private static Composite BloodFury()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.AutoRacial &&
                    //!Casting() &&
                    Me.Combat &&
                    CurrentTargetAttackable(50) &&
                    SpellManager.HasSpell("Blood Fury") &&
                    (Me.GetAuraById(51271) != null || //Pillar of Frost
                     Me.GetAuraById(49016) != null) && //Unholy Frenzy
                    SpellManager.CanCast("Blood Fury"),
                    new Action(
                        ret =>
                            {
                                //Logging.Write(LogLevel.Diagnostic, "BloodFury");
                                CastSpell("Blood Fury", Me);
                                return RunStatus.Failure;
                            })),
                //Berserking
                new Decorator(
                    ret =>
                    THSettings.Instance.AutoRacial &&
                    //!Casting() &&
                    Me.Combat &&
                    CurrentTargetAttackable(50) &&
                    SpellManager.HasSpell("Berserking") &&
                    (Me.GetAuraById(51271) != null || //Pillar of Frost
                     Me.GetAuraById(49016) != null) && //Unholy Frenzy
                    SpellManager.CanCast("Berserking"),
                    new Action(
                        ret =>
                            {
                                //Logging.Write(LogLevel.Diagnostic, "BloodFury");
                                CastSpell("Berserking", Me);
                                return RunStatus.Failure;
                            }))
                );
        }

        #endregion

        #region BurstofSpeed

        private static Composite BurstofSpeed()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.BurstofSpeed &&
                    SpellManager.HasSpell("Burst of Speed") &&
                    !MeMounted &&
                    Me.IsMoving &&
                    //!Me.HasAura("Burst of Speed") &&
                    MyAuraTimeLeft("Burst of Speed", Me) <= THSettings.Instance.BurstofSpeedRenew &&
                    CurrentTargetAttackable(40) &&
                    Me.CurrentTarget.Distance - Me.CurrentTarget.CombatReach >=
                    THSettings.Instance.BurstofSpeedDistance &&
                    !Me.CurrentTarget.IsWithinMeleeRange &&
                    FacingOverrideMeCurrentTarget &&
                    //!DebuffRoot(Me) &&
                    SpellManager.CanCast("Burst of Speed"),
                    new Action(delegate
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Burst of Speed", Me, "BurstofSpeed");
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.BurstofSpeedEnergy &&
                    SpellManager.HasSpell("Burst of Speed") &&
                    !MeMounted &&
                    Me.IsMoving &&
                    PlayerEnergy >= THSettings.Instance.BurstofSpeedEnergyNumber &&
                    //!Me.HasAura("Burst of Speed") &&
                    MyAuraTimeLeft("Burst of Speed", Me) <= THSettings.Instance.BurstofSpeedRenew &&
                    //(Me.CurrentTarget == null ||
                    //CurrentTargetAttackable(10) ||
                    // CurrentTargetAttackable(40) &&
                    // !Me.CurrentTarget.IsWithinMeleeRange) &&
                    SpellManager.CanCast("Burst of Speed"),
                    new Action(delegate
                        {
                            CastSpell("Burst of Speed", Me, "BurstofSpeedEnergy");
                            return RunStatus.Failure;
                        }))
                );
        }

        #endregion

        #region CastonGround

        private static DateTime SpamUntil;
        private static bool PendingSpellActivated;
        private static WoWPoint LaunchTrapDropLocation;
        private static DateTime LastCastontheGround;

        private static void CastonGround(string SpellName, WoWUnit target, string reason)
        {
            Logging.Write(LogLevel.Diagnostic,
                          "Trying to Cast " + SpellName + " on " + SafeName(target) + " reason " + reason);

            SpamUntil = DateTime.Now + GetSpellCooldown(SpellName) + TimeSpan.FromMilliseconds(400);
            PendingSpellActivated = false;
            if (Me.IsCasting)
            {
                SpellManager.StopCasting();
            }

            while (SpamUntil > DateTime.Now)
            {
                CastSpell(SpellName, target, reason);
                Logging.Write(LogLevel.Diagnostic,
                              "Casted " + SpellName + " on " + SafeName(target) + " reason " + reason);

                if (Me.CurrentPendingCursorSpell != null)
                {
                    Logging.Write(LogLevel.Diagnostic,
                                  "Trying to ClickRemoteLocation " + SpellName + " on " + SafeName(target) + " reason " +
                                  reason);
                    PendingSpellActivated = true;
                    ObjectManager.Update();
                    LaunchTrapDropLocation = target.Location;
                    SpellManager.ClickRemoteLocation(LaunchTrapDropLocation);
                }

                //if (PendingSpellActivated && Me.CurrentPendingCursorSpell == null)
                //{
                //    Logging.Write(LogLevel.Diagnostic,
                //                  "Done ClickRemoteLocation " + SpellName + " on " + target + " reason " + reason +
                //                  " breaking the loop");
                //    break;
                //}

                //if (SpamUntil > DateTime.Now)
                //{
                //    break;
                //}
            }
            if (Me.CurrentPendingCursorSpell != null)
            {
                Logging.Write(LogLevel.Diagnostic,
                              "Fail ClickRemoteLocation - run SpellStopTargeting - breaking the loop");
                Lua.DoString("SpellStopTargeting()");
            }
            else
            {
                Logging.Write(LogLevel.Diagnostic,
                              "Successfully Cast " + SpellName + " on " + SafeName(target) + " reason " + reason);
                LastCastontheGround = DateTime.Now;
            }
            LastCastontheGround = DateTime.Now;
        }

        private static void CastonGroundDistract(string SpellName, WoWUnit target, string reason)
        {
            if (target == null || !target.IsValid)
            {
                return;
            }

            //Logging.Write(LogLevel.Diagnostic,
            //              "Trying to Cast " + SpellName + " on " + SafeName(target) + " reason " + reason);

            SpamUntil = DateTime.Now + GetSpellCooldown(SpellName) + TimeSpan.FromMilliseconds(400);
            PendingSpellActivated = false;
            if (Me.IsCasting)
            {
                SpellManager.StopCasting();
            }

            while (SpamUntil > DateTime.Now)
            {
                CastSpell(SpellName, target, reason);
                Logging.Write(LogLevel.Diagnostic,
                              "Casted " + SpellName + " on " + SafeName(target) + " reason " + reason);

                if (Me.HasPendingSpell(1725))
                {
                    Logging.Write(LogLevel.Diagnostic,
                                  "Trying to ClickRemoteLocation " + SpellName + " on " + SafeName(target) + " reason " +
                                  reason);
                    PendingSpellActivated = true;
                    ObjectManager.Update();
                    //LaunchTrapDropLocation = WoWMathHelper.CalculatePointBehind(target.Location, target.Rotation, 2);
                    LaunchTrapDropLocation = WoWMathHelper.CalculatePointFrom(Me.Location, target.Location, 5);
                    SpellManager.ClickRemoteLocation(LaunchTrapDropLocation);
                }

                //if (PendingSpellActivated && !Me.HasPendingSpell(1725))
                //{
                //    Logging.Write(LogLevel.Diagnostic,
                //                  "Done ClickRemoteLocation " + SpellName + " on " + target + " reason " + reason +
                //                  " breaking the loop");
                //    break;
                //}
            }

            if (Me.HasPendingSpell(1725))
            {
                Logging.Write(LogLevel.Diagnostic,
                              "Fail ClickRemoteLocation - run SpellStopTargeting - breaking the loop");
                Lua.DoString("SpellStopTargeting()");
            }
            else
            {
                Logging.Write(LogLevel.Diagnostic,
                              "Successfully Cast " + SpellName + " on " + SafeName(target) + " reason " + reason);
                LastCastontheGround = DateTime.Now;
            }
        }

        #endregion

        #region CalculateDropLocation

        private static WoWPoint CalculateDropLocation(WoWUnit target)
        {
            WoWPoint dropLocation;

            if (!target.IsMoving || target.MovementInfo.MovingBackward)
            {
                dropLocation = target.Location;
            }
            else
            {
                dropLocation = WoWMathHelper.CalculatePointBehind(target.Location, target.Rotation,
                                                                  -(5 + target.MovementInfo.RunSpeed));
            }

            return dropLocation;
        }

        #endregion

        #region CancelAura

        public static void CancelAura(int HealthPercent)
        {
            WoWAura a = Me.GetAuraByName("Conversion");
            if (Me.HealthPercent >= HealthPercent && a != null && a.Cancellable)
            {
                a.TryCancelAura();
            }
        }

        #endregion

        #region CheapShot

        private static DateTime CheapShotLast;

        private static bool CheapShotCheck()
        {
            if (!THSettings.Instance.CheapShot)
            {
                return false;
            }

            if (!SpellManager.HasSpell("Cheap Shot"))
            {
                return false;
            }

            if (!IsStealthed(Me))
            {
                return false;
            }

            if (!CurrentTargetAttackable(5))
            {
                return false;
            }

            if (ControlStunLevelGet(Me.CurrentTarget) >= THSettings.Instance.CheapShotStunLevel)
            {
                return false;
            }

            if (DebuffCCDuration(Me.CurrentTarget, 1500))
            {
                return false;
            }

            if (MyAuraTimeLeft(115192, Me) > 1500)
            {
                return false;
            }
            return true;
        }

        private static double CalculateEnergy(int energyUsed, double millisecondleft)
        {
            if (!Me.HasAura(115192))
            {
                return PlayerEnergy;
            }
            return PlayerEnergy + (millisecondleft/100) - energyUsed;
        }

        private static Composite CheapShotOld()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    (THSettings.Instance.AttackOOC || Me.Combat) &&
                    THSettings.Instance.CheapShot &&
                    !SpellManager.HasSpell("Cloak and Dagger") &&
                    SpellManager.HasSpell("Cheap Shot") &&
                    LastCastCC + TimeSpan.FromMilliseconds(2000) < DateTime.Now &&
                    !MeMounted &&
                    IsStealthed(Me) &&
                    CurrentTargetAttackable(5) &&
                    !AmbushShadowDanceCheck() &&
                    PlayerComboPoint < MaxComboPoint &&
                    FacingOverrideMeCurrentTarget &&
                    !DebuffCC(Me.CurrentTarget) &&
                    !InvulnerableStun(Me.CurrentTarget) &&
                    (TalentSort(Me.CurrentTarget) < 3 ||
                     TalentSort(Me.CurrentTarget) > 2 &&
                     !DebuffSilence(Me.CurrentTarget)) &&
                    SpellManager.CanCast("Cheap Shot"),
                    new Action(delegate
                        {
                            //Logging.Write("THSettings.Instance.AttackOOC " + THSettings.Instance.AttackOOC);
                            //Logging.Write("Me.Combat " + Me.Combat);
                            SafelyFacingTarget(Me.CurrentTarget);
                            LastCastCC = DateTime.Now;
                            CastSpell("Cheap Shot", Me.CurrentTarget, "CheapShot");
                        })),
                new Decorator(
                    ret =>
                    (THSettings.Instance.AttackOOC || Me.Combat) &&
                    THSettings.Instance.CheapShot &&
                    SpellManager.HasSpell("Cloak and Dagger") &&
                    SpellManager.HasSpell("Cheap Shot") &&
                    LastCastCC + TimeSpan.FromMilliseconds(1000) < DateTime.Now &&
                    !MeMounted &&
                    IsStealthedNoSD(Me) &&
                    CurrentTargetAttackable(30) &&
                    PlayerComboPoint < MaxComboPoint &&
                    FacingOverrideMeCurrentTarget &&
                    !DebuffCC(Me.CurrentTarget) &&
                    SpellManager.CanCast("Cheap Shot"),
                    new Action(delegate
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            LastCastCC = DateTime.Now;
                            CastSpell("Cheap Shot", Me.CurrentTarget, "CheapShot Cloak and Dagger");
                        })),
                new Decorator(
                    ret =>
                    (THSettings.Instance.AttackOOC || Me.Combat) &&
                    THSettings.Instance.CheapShot &&
                    SpellManager.HasSpell("Cloak and Dagger") &&
                    SpellManager.HasSpell("Cheap Shot") &&
                    LastCastCC + TimeSpan.FromMilliseconds(1000) < DateTime.Now &&
                    !MeMounted &&
                    IsShadowDance(Me) &&
                    CurrentTargetAttackable(5) &&
                    PlayerComboPoint < MaxComboPoint &&
                    FacingOverrideMeCurrentTarget &&
                    !DebuffCC(Me.CurrentTarget) &&
                    SpellManager.CanCast("Cheap Shot"),
                    new Action(delegate
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            LastCastCC = DateTime.Now;
                            CastSpell("Cheap Shot", Me.CurrentTarget, "CheapShot IsShadowDance");
                        }))
                );
        }

        private static Composite CheapShotCaD()
        {
            //return new Decorator(
            //    ret =>
            //    (THSettings.Instance.AttackOOC || Me.Combat) &&
            //    THSettings.Instance.CheapShot &&
            //    SpellManager.HasSpell("Cheap Shot") &&
            //    SpellManager.HasSpell("Cloak and Dagger") &&
            //    !MeMounted &&
            //    CurrentTargetAttackable(30) &&
            //    FacingOverrideMeCurrentTarget &&
            //    !Me.CurrentTarget.IsBoss &&
            //    !Me.CurrentTarget.IsMechanical &&
            //    IsStealthedNoSD(Me) &&
            //    !DebuffCCDuration(Me.CurrentTarget, 1500) &&
            //    !InvulnerableStun(Me.CurrentTarget),// &&
            //    //(!SpellManager.HasSpell("Subterfuge") &&
            //    // SpellManager.CanCast("Cheap Shot") ||
            //    // Me.HasAura(115192) &&
            //    // MyAuraTimeLeft(115192, Me) < 1500),
            //    new Action(delegate
            //        {
            //            SafelyFacingTarget(Me.CurrentTarget);
            //            while (MyAuraTimeLeft(115192, Me) > 0 || !Me.HasAura(115192))
            //            {
            //                if (DebuffStun(Me.CurrentTarget))
            //                {
            //                    Logging.Write("DebuffStun Break");
            //                    break;
            //                }

            //                if (!IsStealthed(Me))
            //                {
            //                    Logging.Write("!IsStealthed Break");
            //                    break;
            //                }

            //                if (MyAuraTimeLeft(115192, Me) < 1000 &&
            //                    SpellManager.CanCast("Cheap Shot"))
            //                {
            //                    CastSpell("Cheap Shot", Me.CurrentTarget, "Cheap Shot Cloak and Dagger");
            //                }
            //            }
            //        }));

            return new Decorator(
                ret =>
                (THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.CheapShot &&
                CheapShotLast < DateTime.Now &&
                SpellManager.HasSpell("Cheap Shot") &&
                SpellManager.HasSpell("Cloak and Dagger") &&
                !MeMounted &&
                CurrentTargetAttackable(30) &&
                FacingOverrideMeCurrentTarget &&
                ControlStunLevelGet(Me.CurrentTarget) < THSettings.Instance.CheapShotStunLevel &&
                !Me.CurrentTarget.IsBoss &&
                !Me.CurrentTarget.IsMechanical &&
                IsStealthedNoSD(Me) &&
                !DebuffStun(Me.CurrentTarget) &&
                !InvulnerableStun(Me.CurrentTarget) &&
                SpellManager.CanCast("Cheap Shot"), // &&
                //(!SpellManager.HasSpell("Subterfuge") &&
                // SpellManager.CanCast("Cheap Shot") ||
                // Me.HasAura(115192) &&
                // MyAuraTimeLeft(115192, Me) < 1500),
                new Action(delegate
                    {
                        //Logging.Write(LogLevel.Diagnostic, "Current ControlStun DR Level {0}",
                        //              ControlStunLevelGet(Me.CurrentTarget));

                        SafelyFacingTarget(Me.CurrentTarget);

                        //while (DebuffCCDuration(Me.CurrentTarget, THSettings.Instance.KidneyShotStunLeft) &&
                        //       IsStealthedNoSD(Me))
                        //{
                        //    Logging.Write("Waiting Stun Over");
                        //}

                        CheapShotLast = DateTime.Now + TimeSpan.FromMilliseconds(2000);
                        CastSpell("Cheap Shot", Me.CurrentTarget, "Cheap Shot Cloak and Dagger");
                    }));
        }

        private static Composite CheapShot()
        {
            //return new Decorator(
            //    ret =>
            //    (THSettings.Instance.AttackOOC || Me.Combat) &&
            //    THSettings.Instance.CheapShot &&
            //    SpellManager.HasSpell("Cheap Shot") &&
            //    !MeMounted &&
            //    CurrentTargetAttackable(5) &&
            //    FacingOverrideMeCurrentTarget &&
            //    !Me.CurrentTarget.IsBoss &&
            //    !Me.CurrentTarget.IsMechanical &&
            //    IsStealthed(Me) &&
            //    !DebuffCCDuration(Me.CurrentTarget, 1500) &&
            //    !InvulnerableStun(Me.CurrentTarget), // &&
            //    //(!SpellManager.HasSpell("Subterfuge") &&
            //    // SpellManager.CanCast("Cheap Shot") ||
            //    // Me.HasAura(115192) &&
            //    // MyAuraTimeLeft(115192, Me) < 1500),
            //    new Action(delegate
            //        {
            //            SafelyFacingTarget(Me.CurrentTarget);
            //            while (MyAuraTimeLeft(115192, Me) > 0 || !Me.HasAura(115192))
            //            {
            //                if (DebuffStun(Me.CurrentTarget))
            //                {
            //                    Logging.Write("DebuffStun Break");
            //                    break;
            //                }

            //                if (!IsStealthed(Me))
            //                {
            //                    Logging.Write("!IsStealthed Break");
            //                    break;
            //                }

            //                if (MyAuraTimeLeft(115192, Me) < 1000 &&
            //                    SpellManager.CanCast("Cheap Shot"))
            //                {
            //                    CastSpell("Cheap Shot", Me.CurrentTarget, "OpenerRotation: Cheap Shot");
            //                }
            //            }
            //        }));
            return new Decorator(
                ret =>
                (THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.CheapShot &&
                CheapShotLast < DateTime.Now &&
                SpellManager.HasSpell("Cheap Shot") &&
                !MeMounted &&
                CurrentTargetAttackable(5) &&
                FacingOverrideMeCurrentTarget &&
                ControlStunLevelGet(Me.CurrentTarget) < THSettings.Instance.CheapShotStunLevel &&
                !Me.CurrentTarget.IsBoss &&
                !Me.CurrentTarget.IsMechanical &&
                IsStealthed(Me) &&
                !DebuffStun(Me.CurrentTarget) &&
                !InvulnerableStun(Me.CurrentTarget) &&
                SpellManager.CanCast("Cheap Shot"), // &&
                //(!SpellManager.HasSpell("Subterfuge") &&
                // SpellManager.CanCast("Cheap Shot") ||
                // Me.HasAura(115192) &&
                // MyAuraTimeLeft(115192, Me) < 1500),
                new Action(delegate
                    {
                        //Logging.Write(LogLevel.Diagnostic, "Current ControlStun DR Level {0}",
                        //              ControlStunLevelGet(Me.CurrentTarget));

                        SafelyFacingTarget(Me.CurrentTarget);

                        //while (DebuffCCDuration(Me.CurrentTarget, THSettings.Instance.KidneyShotStunLeft) &&
                        //       IsStealthed(Me))
                        //{
                        //    Logging.Write("Waiting Stun Over");
                        //}

                        CheapShotLast = DateTime.Now + TimeSpan.FromMilliseconds(2000);
                        CastSpell("Cheap Shot", Me.CurrentTarget, "OpenerRotation: Cheap Shot");
                    }));
        }

        #endregion

        #region CheapShotInterrupt

        private static bool GetUnitInterruptCheapShot()
        {
            UnitInterrupt = null;

            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (InBattleground || InArena)
                {
                    UnitInterrupt = NearbyUnFriendlyPlayers.FirstOrDefault(
                        unit => unit != null && unit.IsValid &&
                                (THSettings.Instance.InterruptAll ||
                                 THSettings.Instance.InterruptTarget &&
                                 Me.CurrentTarget != null &&
                                 unit == Me.CurrentTarget ||
                                 THSettings.Instance.InterruptFocus &&
                                 Me.FocusedUnit != null &&
                                 unit == Me.FocusedUnit) &&
                                unit.IsWithinMeleeRange &&
                                FacingOverride(unit) &&
                                ControlStunLevelGet(UnitInterrupt) < 3 &&
                                InterruptCheck(unit, THSettings.Instance.CheapShotInterruptTimeLeft) &&
                                Attackable(unit, 5));

                    //UnitInterrupt = (from unit in NearbyUnFriendlyPlayers
                    //                 where unit != null &&
                    //                       unit.IsValid &&
                    //                       (THSettings.Instance.InterruptAll ||
                    //                        THSettings.Instance.InterruptTarget &&
                    //                        Me.CurrentTarget != null &&
                    //                        unit == Me.CurrentTarget ||
                    //                        THSettings.Instance.InterruptFocus &&
                    //                        Me.FocusedUnit != null &&
                    //                        unit == Me.FocusedUnit) &&
                    //                       unit.IsWithinMeleeRange &&
                    //                       FacingOverride(unit) &&
                    //                       ControlStunLevelGet(UnitInterrupt) < 3 &&
                    //                       InterruptCheck(unit, THSettings.Instance.CheapShotInterruptTimeLeft, true) &&
                    //                       Attackable(unit, 5)
                    //                 orderby unit.CurrentCastTimeLeft ascending
                    //                 select unit).FirstOrDefault();
                }
                    //PvE Search
                else if (!Me.IsInInstance || InDungeon || InRaid)
                {
                    UnitInterrupt = NearbyUnFriendlyUnits.FirstOrDefault(
                        unit => unit != null && unit.IsValid &&
                                unit.Combat &&
                                FacingOverride(unit) &&
                                ControlStunLevelGet(UnitInterrupt) < 3 &&
                                unit.IsWithinMeleeRange &&
                                InterruptCheck(unit,
                                               THSettings.Instance.CheapShotInterruptTimeLeft) &&
                                Attackable(unit, 5));

                    //UnitInterrupt = (from unit in NearbyUnFriendlyUnits
                    //                 where unit != null &&
                    //                       unit.IsValid &&
                    //                       //where !unit.IsBoss
                    //                       unit.Combat &&
                    //                       FacingOverride(unit) &&
                    //                       ControlStunLevelGet(UnitInterrupt) < 3 &&
                    //                       unit.IsWithinMeleeRange &&
                    //                       InterruptCheck(unit,
                    //                                      THSettings.Instance.CheapShotInterruptTimeLeft, true) &&
                    //                       Attackable(unit, 40)
                    //                 orderby unit.CurrentCastTimeLeft ascending
                    //                 select unit).FirstOrDefault();
                }
            }

            return UnitInterrupt != null;
        }

        private static void CheapShotInterruptVoid()
        {
            if ((THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.CheapShotInterrupt &&
                LastInterrupt + TimeSpan.FromMilliseconds(1000) < DateTime.Now &&
                SpellManager.HasSpell("Cheap Shot") &&
                !MeMounted &&
                IsStealthed(Me) &&
                GetUnitInterruptCheapShot() &&
                UnitInterrupt != null &&
                UnitInterrupt.IsValid &&
                SpellManager.CanCast("Cheap Shot"))
            {
                //Logging.Write(LogLevel.Diagnostic, "Current ControlStun DR Level {0}",
                //              ControlStunLevelGet(UnitInterrupt));

                SafelyFacingTarget(UnitInterrupt);
                CastSpell("Cheap Shot", UnitInterrupt, "CheapShotInterrupgt", true);
                LastInterrupt = DateTime.Now;
            }
        }

        #endregion

        #region CloakofShadows

        private static Composite CloakofShadows()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.CloakofShadowsRoot &&
                    SpellManager.HasSpell("Cloak of Shadows") &&
                    !MeMounted &&
                    !Me.HasAura("Cloak of Shadows") &&
                    CurrentTargetAttackable(30) &&
                    DebuffRootCanCloak(Me) &&
                    SpellManager.CanCast("Cloak of Shadows"),
                    new Action(delegate
                        {
                            CastSpell("Cloak of Shadows", Me, "CloakofShadowsRoot");
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.CloakofShadows &&
                    SpellManager.HasSpell("Cloak of Shadows") &&
                    !MeMounted &&
                    Me.Combat &&
                    Me.HealthPercent <= THSettings.Instance.CloakofShadowsHP &&
                    !Me.HasAura("Cloak of Shadows") &&
                    CountMagicDPSTarget(Me) > 0 &&
                    SpellManager.CanCast("Cloak of Shadows"),
                    new Action(delegate
                        {
                            CastSpell("Cloak of Shadows", Me, "CloakofShadowsHP");
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.CloakofShadowsTarget &&
                    SpellManager.HasSpell("Cloak of Shadows") &&
                    !MeMounted &&
                    Me.Combat &&
                    !Me.HasAura("Cloak of Shadows") &&
                    CountMagicDPSTarget(Me) >= THSettings.Instance.CloakofShadowsTargetUnit &&
                    SpellManager.CanCast("Cloak of Shadows"),
                    new Action(delegate
                        {
                            CastSpell("Cloak of Shadows", Me, "CloakofShadowsTargetUnit");
                            return RunStatus.Failure;
                        }))
                //new Decorator(
                //    ret =>
                //    THSettings.Instance.CloakofShadowsTarget &&
                //    SpellManager.HasSpell("Cloak of Shadows") &&
                //    !MeMounted &&
                //    !Me.HasAura("Cloak of Shadows") &&
                //    !DebuffDot(Me) &&
                //    MyAuraTimeLeft("Vanish", Me)<500 &&
                //    SpellManager.CanCast("Cloak of Shadows"),
                //    new Action(delegate
                //        {
                //            CastSpell("Cloak of Shadows", Me, "CloakofShadows Vanish");
                //            return RunStatus.Failure;
                //        }))
                );
        }

        #endregion

        #region CombatReadiness

        private static DateTime DodgeBuffTime;

        private static Composite CombatReadiness()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.CombatReadiness &&
                    SpellManager.HasSpell("Combat Readiness") &&
                    DodgeBuffTime + TimeSpan.FromSeconds(10) < DateTime.Now &&
                    !MeMounted &&
                    Me.Combat &&
                    Me.HealthPercent <= THSettings.Instance.CombatReadinessHP &&
                    !Me.HasAura("Combat Readiness") &&
                    CountPhysicDPSTarget(Me) > 0 &&
                    SpellManager.CanCast("Combat Readiness"),
                    new Action(delegate
                        {
                            DodgeBuffTime = DateTime.Now;
                            CastSpell("Combat Readiness", Me, "CombatReadinessHP");
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.CombatReadinessTarget &&
                    SpellManager.HasSpell("Combat Readiness") &&
                    DodgeBuffTime + TimeSpan.FromSeconds(10) < DateTime.Now &&
                    !MeMounted &&
                    Me.Combat &&
                    !Me.HasAura("Combat Readiness") &&
                    CountPhysicDPSTarget(Me) >= THSettings.Instance.CombatReadinessTargetUnit &&
                    SpellManager.CanCast("Combat Readiness"),
                    new Action(delegate
                        {
                            DodgeBuffTime = DateTime.Now;
                            CastSpell("Combat Readiness", Me, "CombatReadinessHPTargetUnit");
                            return RunStatus.Failure;
                        }))
                );
        }

        #endregion

        #region CrimsonTempest

        private static double CountCrimsonTempest(WoWUnit unitCenter, float distance)
        {
            return
                NearbyUnFriendlyUnits.Where(
                    unit => unit != null && unit.IsValid &&
                            unit != null &&
                            unit.IsValid &&
                            unitCenter.Location.Distance(unit.Location) <= distance &&
                            MyAuraTimeLeft("Crimson Tempest", unit) < 2000 &&
                            (IsDummy(unit) ||
                             unit.Combat &&
                             !unit.IsPet &&
                             (unit.IsTargetingMyPartyMember ||
                              unit.IsTargetingMyRaidMember ||
                              unit.IsTargetingMeOrPet))).Aggregate
                    <WoWUnit, double>(0, (current, unit) => current + 1);
        }

        private static bool CrimsonTempestCheck()
        {
            if (THSettings.Instance.CrimsonTempest &&
                SpellManager.HasSpell("Crimson Tempest") &&
                !MeMounted &&
                (Me.RawComboPoints >= THSettings.Instance.CrimsonTempestCP ||
                 PlayerComboPoint >= THSettings.Instance.CrimsonTempestCP) &&
                CurrentTargetAttackable(5) &&
                CountCrimsonTempest(Me, 8) >= THSettings.Instance.UnittoStartAoE)
            {
                return true;
            }
            return false;
        }

        private static Composite CrimsonTempest()
        {
            return new Decorator(
                ret =>
                CrimsonTempestCheck() &&
                SpellManager.CanCast("Crimson Tempest"),
                new Action(delegate { CastSpell("Crimson Tempest", Me.CurrentTarget, "CrimsonTempest"); })
                );
        }

        #endregion

        #region DeadlyThrow

        private static Composite DeadlyThrow()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    (THSettings.Instance.AttackOOC || Me.Combat) &&
                    THSettings.Instance.DeadlyThrowFinish &&
                    SpellManager.HasSpell("Deadly Throw") &&
                    !MeMounted &&
                    PlayerComboPoint >= THSettings.Instance.DeadlyThrowFinishCP &&
                    CurrentTargetAttackable(30) &&
                    Me.CurrentTarget.Distance - Me.CurrentTarget.CombatReach >=
                    THSettings.Instance.DeadlyThrowFinishDistance &&
                    FacingOverrideMeCurrentTarget &&
                    !IsStealthed(Me) &&
                    SpellManager.CanCast("Deadly Throw"),
                    new Action(delegate
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Deadly Throw", Me.CurrentTarget, "DeadlyThrowFinish");
                        })),
                new Decorator(
                    ret =>
                    (THSettings.Instance.AttackOOC || Me.Combat) &&
                    THSettings.Instance.DeadlyThrowSlow &&
                    SpellManager.HasSpell("Deadly Throw") &&
                    !MeMounted &&
                    PlayerComboPoint > 0 &&
                    CurrentTargetAttackable(30) &&
                    Me.CurrentTarget.Distance - Me.CurrentTarget.CombatReach >=
                    THSettings.Instance.DeadlyThrowFinishDistance &&
                    !DebuffRootorSnare(Me.CurrentTarget) &&
                    FacingOverrideMeCurrentTarget &&
                    !IsStealthed(Me) &&
                    SpellManager.CanCast("Deadly Throw"),
                    new Action(delegate
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Deadly Throw", Me.CurrentTarget, "DeadlyThrowSlow");
                        }))
                );
        }

        #endregion

        #region DeadlyThrowInterrupt

        private static bool GetUnitInterruptDeadlyThrow()
        {
            UnitInterrupt = null;

            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (InBattleground || InArena)
                {
                    UnitInterrupt = NearbyUnFriendlyPlayers.FirstOrDefault(
                        unit => unit != null && unit.IsValid &&
                                (THSettings.Instance.InterruptAll ||
                                 THSettings.Instance.InterruptTarget &&
                                 Me.CurrentTarget != null &&
                                 unit == Me.CurrentTarget ||
                                 THSettings.Instance.InterruptFocus &&
                                 Me.FocusedUnit != null &&
                                 unit == Me.FocusedUnit) &&
                                FacingOverride(unit) &&
                                !unit.IsWithinMeleeRange &&
                                InterruptCheck(unit, THSettings.Instance.DeadlyThrowInterruptTimeLeft, false) &&
                                Attackable(unit, 30));

                    //UnitInterrupt = (from unit in NearbyUnFriendlyPlayers
                    //                 where unit != null &&
                    //                       unit.IsValid &&
                    //                       (THSettings.Instance.InterruptAll ||
                    //                        THSettings.Instance.InterruptTarget &&
                    //                        Me.CurrentTarget != null &&
                    //                        unit == Me.CurrentTarget ||
                    //                        THSettings.Instance.InterruptFocus &&
                    //                        Me.FocusedUnit != null &&
                    //                        unit == Me.FocusedUnit) &&
                    //                       FacingOverride(unit) &&
                    //                       !unit.IsWithinMeleeRange &&
                    //                       InterruptCheck(unit, THSettings.Instance.DeadlyThrowInterruptTimeLeft, false) &&
                    //                       Attackable(unit, 30)
                    //                 orderby unit.CurrentCastTimeLeft ascending
                    //                 select unit).FirstOrDefault();
                }
                    //PvE Search
                else if (!Me.IsInInstance || InDungeon || InRaid)
                {
                    UnitInterrupt = NearbyUnFriendlyUnits.FirstOrDefault(
                        unit => unit != null && unit.IsValid &&
                                unit.Combat &&
                                FacingOverride(unit) &&
                                !unit.IsWithinMeleeRange &&
                                InterruptCheck(unit,
                                               THSettings.Instance.DeadlyThrowInterruptTimeLeft, false) &&
                                Attackable(unit, 30));

                    //UnitInterrupt = (from unit in NearbyUnFriendlyUnits
                    //                 where unit != null &&
                    //                       unit.IsValid &&
                    //                       //where !unit.IsBoss
                    //                       unit.Combat &&
                    //                       FacingOverride(unit) &&
                    //                       !unit.IsWithinMeleeRange &&
                    //                       InterruptCheck(unit,
                    //                                      THSettings.Instance.DeadlyThrowInterruptTimeLeft, false) &&
                    //                       Attackable(unit, 30)
                    //                 orderby unit.CurrentCastTimeLeft ascending
                    //                 select unit).FirstOrDefault();
                }
            }

            return UnitInterrupt != null;
        }

        private static void DeadlyThrowVoid()
        {
            if ((THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.DeadlyThrowInterrupt &&
                LastInterrupt + TimeSpan.FromMilliseconds(1000) < DateTime.Now &&
                SpellManager.HasSpell("Deadly Throw") &&
                !MeMounted &&
                Math.Abs(PlayerComboPoint - 3) < 1 &&
                GetUnitInterruptDeadlyThrow() &&
                UnitInterrupt != null &&
                UnitInterrupt.IsValid &&
                SpellManager.CanCast("DeadlyThrow"))
            {
                SafelyFacingTarget(UnitInterrupt);
                CastSpell("DeadlyThrow", UnitInterrupt, "DeadlyThrow Interrupt");
                LastInterrupt = DateTime.Now;
            }
        }

        #endregion

        #region DisarmTrap

        private static WoWObject EnemyTrap;

        private static bool GetEnemyTrap()
        {
            //Snake Trap 183957
            //Freeezing Trap 2561
            //Explosive Trap 164839
            //Ice Trap 164639

            EnemyTrap = null;

            EnemyTrap =
                ObjectManager.GetObjectsOfTypeFast<WoWObject>()
                             .FirstOrDefault(
                                 p =>
                                 p.Distance <= 20 &&
                                 (p.Entry == 183957 ||
                                  p.Entry == 2561 ||
                                  p.Entry == 164839 ||
                                  p.Entry == 164639) &&
                                 Me.IsFacing(p));

            //if (EnemyTrap != null)
            //{
            //    Logging.Write("Found Trap " + EnemyTrap.Name);
            //}

            return EnemyTrap != null;
        }

        private static Composite DisarmTrap()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.DisarmTrap &&
                (InArena || InBattleground) &&
                SpellManager.HasSpell("Disarm Trap") &&
                !CurrentTargetAttackable(10) &&
                IsStealthedNoSD(Me) &&
                !Me.IsMoving &&
                GetEnemyTrap() &&
                EnemyTrap != null &&
                EnemyTrap.IsValid,
                new Action(
                    ret =>
                        {
                            Logging.Write(LogLevel.Diagnostic, "Trying Disarm Trap " + EnemyTrap.Name);
                            Me.SetFacing(EnemyTrap.Location);
                            EnemyTrap.Interact();
                            return RunStatus.Failure;
                        })
                );
        }

        #endregion

        #region Dismantle

        private static WoWUnit UnitDismantle;

        private static bool GetUnitDismantleCooddown()
        {
            UnitDismantle = null;

            //Logging.Write("UnitDismantle: Search");

            if (InBattleground || InArena)
            {
                UnitDismantle = NearbyUnFriendlyPlayers.FirstOrDefault(
                    unit => unit != null && unit.IsValid &&
                            FacingOverride(unit) &&
                            TalentSort(unit) <= 2 &&
                            !DebuffDisarm(unit) &&
                            !DebuffCC(unit) &&
                            BuffBurst(unit) &&
                            SafeUsingCooldown(unit) &&
                            Attackable(unit, 5));

                //UnitDismantle = (from unit in NearbyUnFriendlyPlayers
                //                 where unit != null &&
                //                       unit.IsValid &&
                //                       FacingOverride(unit) &&
                //                       TalentSort(unit) <= 2 &&
                //                       !DebuffDisarm(unit) &&
                //                       !DebuffCC(unit) &&
                //                       BuffBurst(unit) &&
                //                       SafeUsingCooldown(unit) &&
                //                       Attackable(unit, 5)
                //                 select unit).FirstOrDefault();
            }

            return UnitDismantle != null;
        }

        private static bool GetUnitDismantle()
        {
            UnitDismantle = null;

            //Logging.Write("UnitDismantle: Search");

            if (InBattleground || InArena)
            {
                UnitDismantle = NearbyUnFriendlyPlayers.FirstOrDefault(
                    unit => unit != null && unit.IsValid &&
                            FacingOverride(unit) &&
                            TalentSort(unit) <= 2 &&
                            !DebuffDisarm(unit) &&
                            !DebuffCC(unit) &&
                            SafeUsingCooldown(unit) &&
                            Attackable(unit, 5));

                //UnitDismantle = (from unit in NearbyUnFriendlyPlayers
                //                 where unit != null &&
                //                       unit.IsValid &&
                //                       FacingOverride(unit) &&
                //                       TalentSort(unit) <= 2 &&
                //                       !DebuffDisarm(unit) &&
                //                       !DebuffCC(unit) &&
                //                       SafeUsingCooldown(unit) &&
                //                       Attackable(unit, 5)
                //                 orderby unit.CurrentHealth descending
                //                 select unit).FirstOrDefault();
            }
            else
            {
                UnitDismantle = NearbyUnFriendlyUnits.FirstOrDefault(
                    unit => unit != null && unit.IsValid &&
                            FacingOverride(unit) &&
                            !unit.IsBoss &&
                            (unit.IsHumanoid ||
                             unit.IsDragon) &&
                            WorthyTarget(unit, 0.75) &&
                            unit.Combat &&
                            !DebuffDisarm(unit) &&
                            (unit.CurrentTarget != null && unit.CurrentTarget == Me ||
                             unit.IsTargetingMyPartyMember ||
                             unit.IsTargetingMyRaidMember ||
                             unit.IsTargetingMeOrPet) &&
                            Attackable(unit, 5));

                //UnitDismantle = (from unit in NearbyUnFriendlyUnits
                //                 where unit != null &&
                //                       unit.IsValid &&
                //                       FacingOverride(unit) &&
                //                       !unit.IsBoss &&
                //                       (unit.IsHumanoid ||
                //                        unit.IsDragon) &&
                //                       WorthyTarget(unit, 0.75) &&
                //                       unit.Combat &&
                //                       !DebuffDisarm(unit) &&
                //                       (unit.CurrentTarget != null && unit.CurrentTarget == Me ||
                //                        unit.IsTargetingMyPartyMember ||
                //                        unit.IsTargetingMyRaidMember ||
                //                        unit.IsTargetingMeOrPet) &&
                //                       Attackable(unit, 5)
                //                 orderby unit.CurrentHealth descending
                //                 select unit).FirstOrDefault();
            }

            //if (UnitDismantle != null)
            //{
            //    Logging.Write("UnitDismantle: " + UnitDismantle.Name +
            //                  " Dist: " + UnitDismantle.Distance +
            //                  " CombatReach: " + UnitDismantle.CombatReach);
            //}

            return UnitDismantle != null;
        }

        private static DateTime DismantleLast;

        private static Composite Dismantle()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.DistmantleCooldown &&
                    SpellManager.HasSpell("Dismantle") &&
                    !MeMounted &&
                    Me.Combat &&
                    GetUnitDismantleCooddown() &&
                    SpellManager.CanCast("Dismantle"),
                    new Action(delegate
                        {
                            SafelyFacingTarget(UnitDismantle);
                            DismantleLast = DateTime.Now;
                            CastSpell("Dismantle", UnitDismantle, "DistmantleCooldown");
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Dismantle &&
                    SpellManager.HasSpell("Dismantle") &&
                    DismantleLast + TimeSpan.FromMilliseconds(10000) < DateTime.Now &&
                    !MeMounted &&
                    Me.Combat &&
                    Me.HealthPercent <= THSettings.Instance.DismantleHP &&
                    !IsStealthed(Me) &&
                    GetUnitDismantle() &&
                    SpellManager.CanCast("Dismantle"),
                    new Action(delegate
                        {
                            SafelyFacingTarget(UnitDismantle);
                            DismantleLast = DateTime.Now;
                            CastSpell("Dismantle", UnitDismantle, "DismantleHP");
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.DismantleTarget &&
                    SpellManager.HasSpell("Dismantle") &&
                    DismantleLast + TimeSpan.FromMilliseconds(10000) < DateTime.Now &&
                    !MeMounted &&
                    Me.Combat &&
                    CountDPSTarget(Me) >= THSettings.Instance.DismantleTargetUnit &&
                    GetUnitDismantle() &&
                    SpellManager.CanCast("Dismantle"),
                    new Action(delegate
                        {
                            SafelyFacingTarget(UnitDismantle);
                            DismantleLast = DateTime.Now;
                            CastSpell("Dismantle", UnitDismantle, "DismantleTargetUnit");
                        }))
                );
        }

        #endregion

        #region Dispatch

        private static Composite Dispatch()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.Dispatch &&
                (BurstCooldownEnergyPool < DateTime.Now ||
                 Me.HasAura(121153)) &&
                SpellManager.HasSpell("Dispatch") &&
                !MeMounted &&
                PlayerComboPoint < MaxComboPoint &&
                CurrentTargetAttackable(5) &&
                (Me.CurrentTarget.HealthPercent <= 35 ||
                 Me.HasAura(121153)) &&
                //PoolEnergyPassCheck() &&
                FacingOverrideMeCurrentTarget &&
                //!AmbushCheck() &&
                SpellManager.CanCast("Dispatch"),
                new Action(delegate
                    {
                        SafelyFacingTarget(Me.CurrentTarget);
                        CastSpell("Dispatch", Me.CurrentTarget, "Dispatch");
                    })
                )
                ;
        }

        #endregion

        #region Distract

        private static WoWUnit UnitRogueDruidInStealth;

        private static bool GetUnitUnitRogueDruidInStealth()
        {
            UnitRogueDruidInStealth = null;

            UnitRogueDruidInStealth = NearbyUnFriendlyPlayers.FirstOrDefault(
                unit => unit != null && unit.IsValid &&
                        unit.Distance <= 20 &&
                        (unit.Class == WoWClass.Rogue ||
                         unit.Class == WoWClass.Druid &&
                         TalentSort(unit) == 1) &&
                        !unit.Combat &&
                        //IsStealthed(unit) &&
                        DisorientsLevelGet(unit) <= THSettings.Instance.SapLevel &&
                        MyAuraTimeLeft(6770, unit) < 500 &&
                        !DebuffCCDuration(Me.CurrentTarget, 500) &&
                        unit.InLineOfSpellSight);

            //UnitRogueDruidInStealth = (from unit in NearbyUnFriendlyPlayers
            //                           where unit != null &&
            //                                 unit.IsValid &&
            //                                 unit.Distance <= 20 &&
            //                                 (unit.Class == WoWClass.Rogue ||
            //                                  unit.Class == WoWClass.Druid &&
            //                                  TalentSort(unit) == 1) &&
            //                                 !unit.Combat &&
            //                                 //IsStealthed(unit) &&
            //                                 DisorientsLevelGet(unit) <= THSettings.Instance.SapLevel &&
            //                                 MyAuraTimeLeft(6770, unit) < 500 &&
            //                                 !DebuffCCDuration(Me.CurrentTarget, 500) &&
            //                                 unit.InLineOfSpellSight
            //                           orderby unit.Distance ascending
            //                           select unit).FirstOrDefault();

            return UnitRogueDruidInStealth != null;
        }


        private static Composite Distract()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.Distract &&
                    SpellManager.HasSpell("Distract") &&
                    IsStealthedNoSD(Me) &&
                    GetUnitUnitRogueDruidInStealth() &&
                    UnitRogueDruidInStealth != null &&
                    UnitRogueDruidInStealth.IsValid &&
                    SpellManager.CanCast("Distract"),
                    new Action(delegate { CastonGroundDistract("Distract", UnitRogueDruidInStealth, "Distract"); })
                    ),
                new Decorator(
                    ret =>
                    THSettings.Instance.Distract &&
                    SpellManager.HasSpell("Distract") &&
                    Me.CurrentEnergy > Me.MaxEnergy - 20 &&
                    CurrentTargetAttackable(30) &&
                    !Me.CurrentTarget.Combat &&
                    DistanceCheck(Me.CurrentTarget) > 10 &&
                    Me.CurrentTarget.IsMoving &&
                    !Me.CurrentTarget.IsFacing(Me) &&
                    SpellManager.CanCast("Distract"),
                    new Action(delegate { CastonGroundDistract("Distract", Me.CurrentTarget, "Distract"); })
                    )
                );
        }

        #endregion

        #region DRTraker

        private static readonly Dictionary<int, DateTime> IncapacitatesList = new Dictionary<int, DateTime>();

        //private List<> IncapacitatesList = new List<int unitid, DateTime Expire>();

        private static Composite IncapacitatesTracker()
        {
            return new Action(delegate { return RunStatus.Failure; });
        }

        #endregion

        #region Evasion

        private static Composite Evasion()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.Evasion &&
                    SpellManager.HasSpell("Evasion") &&
                    DodgeBuffTime + TimeSpan.FromSeconds(15) < DateTime.Now &&
                    !MeMounted &&
                    Me.Combat &&
                    Me.HealthPercent <= THSettings.Instance.EvasionHP &&
                    !Me.HasAura("Evasion") &&
                    CountPhysicDPSTarget(Me) > 0 &&
                    SpellManager.CanCast("Evasion"),
                    new Action(delegate
                        {
                            DodgeBuffTime = DateTime.Now;
                            CastSpell("Evasion", Me, "EvasionHP");
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.EvasionTarget &&
                    SpellManager.HasSpell("Evasion") &&
                    DodgeBuffTime + TimeSpan.FromSeconds(15) < DateTime.Now &&
                    !MeMounted &&
                    Me.Combat &&
                    !Me.HasAura("Evasion") &&
                    CountPhysicDPSTarget(Me) >= THSettings.Instance.EvasionTargetUnit &&
                    SpellManager.CanCast("Evasion"),
                    new Action(delegate
                        {
                            DodgeBuffTime = DateTime.Now;
                            CastSpell("Evasion", Me, "EvasionHPTargetUnit");
                            return RunStatus.Failure;
                        }))
                );
        }

        #endregion

        #region Envenom

        private static Composite Envenom()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.Eviscerate &&
                SpellManager.HasSpell("Envenom") &&
                !MeMounted &&
                CurrentTargetAttackable(5) &&
                FacingOverrideMeCurrentTarget &&
                PlayerComboPoint >= THSettings.Instance.EviscerateCP &&
                !RuptureWait() &&
                (!KidneyShotCheck() ||
                 !IsDummy(Me.CurrentTarget) &&
                 Me.CurrentTarget.CurrentHealth < Me.MaxHealth/5) &&
                SpellManager.CanCast("Envenom"),
                new Action(delegate
                    {
                        //KidneyShotCheckLog();
                        SafelyFacingTarget(Me.CurrentTarget);
                        CastSpell("Envenom", Me.CurrentTarget, "Envenom");
                    })
                );
        }

        private static Composite EnvenomRefreshSnD()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.Eviscerate &&
                SpellManager.HasSpell("Envenom") &&
                SpellManager.HasSpell("Cut to the Chase") &&
                !MeMounted &&
                CurrentTargetAttackable(5) &&
                FacingOverrideMeCurrentTarget &&
                MyAura("Slice and Dice", Me) &&
                MyAuraTimeLeft("Slice and Dice", Me) < 3000 &&
                !KidneyShotCheck() &&
                SpellManager.CanCast("Envenom"),
                new Action(delegate
                    {
                        //KidneyShotCheckLog();
                        SafelyFacingTarget(Me.CurrentTarget);
                        CastSpell("Envenom", Me.CurrentTarget, "EnvenomRefreshSnD");
                    })
                );
        }

        #endregion

        #region Eviscerate

        private static bool RuptureWait()
        {
            if (!THSettings.Instance.Rupture)
            {
                return false;
            }

            if (!SpellManager.HasSpell("Rupture"))
            {
                return false;
            }

            if (!CurrentTargetAttackable(10))
            {
                return false;
            }

            if (!WorthyTarget(Me.CurrentTarget))
            {
                return false;
            }

            if (Me.CurrentTarget.IsPlayer)
            {
                return false;
            }

            if (Me.CurrentTarget.IsPet)
            {
                return false;
            }

            if (PlayerComboPoint > 6)
            {
                return false;
            }

            if (MyAuraTimeLeft("Rupture", Me.CurrentTarget) > 6000)
            {
                return false;
            }

            return true;
        }

        private static Composite Eviscerate()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.Eviscerate &&
                SpellManager.HasSpell("Eviscerate") &&
                !MeMounted &&
                PlayerComboPoint >= THSettings.Instance.EviscerateCP &&
                CurrentTargetAttackable(5) &&
                FacingOverrideMeCurrentTarget &&
                !RuptureWait() &&
                (!KidneyShotCheck() ||
                 !IsDummy(Me.CurrentTarget) &&
                 Me.CurrentTarget.CurrentHealth < Me.MaxHealth/5) &&
                SpellManager.CanCast("Eviscerate"),
                new Action(delegate
                    {
                        SafelyFacingTarget(Me.CurrentTarget);
                        CastSpell("Eviscerate", Me.CurrentTarget, "Eviscerate");
                    })
                );
        }

        #endregion

        #region ExposeArmor

        private static Composite ExposeArmor()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.ExposeArmor &&
                BurstCooldownEnergyPool < DateTime.Now &&
                SpellManager.HasSpell("Expose Armor") &&
                !MeMounted &&
                PlayerComboPoint < MaxComboPoint &&
                CurrentTargetAttackable(5) &&
                !Me.CurrentTarget.HasAura(91021) && //Find Weakness
                (Me.CurrentTarget.Class == WoWClass.DeathKnight ||
                 Me.CurrentTarget.Class == WoWClass.Paladin ||
                 Me.CurrentTarget.Class == WoWClass.Warrior) &&
                PoolEnergyPassCheck() &&
                FacingOverrideMeCurrentTarget &&
                (!IsStealthed(Me) ||
                 Me.Combat &&
                 IsStealthed(Me) &&
                 PlayerEnergy > 90) &&
                (!Me.CurrentTarget.HasAura(113746) ||
                 Me.CurrentTarget.HasAura(113746) &&
                 Me.CurrentTarget.GetAuraById(113746).StackCount < 3 ||
                 Me.CurrentTarget.HasAura(113746) &&
                 Me.CurrentTarget.GetAuraById(113746).TimeLeft.TotalMilliseconds < 7000) &&
                SpellManager.CanCast("Expose Armor"),
                new Action(delegate
                    {
                        SafelyFacingTarget(Me.CurrentTarget);
                        CastSpell("Expose Armor", Me.CurrentTarget, "ExposeArmor");
                    })
                )
                ;
        }

        #endregion

        #region FanofKnives

        private static Composite FanofKnives()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.FanofKnives &&
                SpellManager.HasSpell("Fan of Knives") &&
                !MeMounted &&
                PlayerComboPoint < MaxComboPoint &&
                !CrimsonTempestCheck() &&
                CountEnemyNear(Me, 10) >= THSettings.Instance.FanofKnivesUnit &&
                PoolEnergyPassCheck() &&
                SpellManager.CanCast("Fan of Knives"),
                new Action(delegate { CastSpell("Fan of Knives", Me, "FanofKnives"); })
                );
        }

        #endregion

        #region FacingTarget

        private static Composite FacingTarget()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.AutoFace &&
                Me.Combat &&
                !MeMounted &&
                !IsOverrideModeOn &&
                CurrentTargetAttackable(60) &&
                !CurrentTargetCheckFacing &&
                (Me.CurrentTarget.IsTargetingMeOrPet ||
                 Me.CurrentTarget.IsTargetingMyPartyMember ||
                 Me.CurrentTarget.IsTargetingMyRaidMember),
                new Action(delegate
                    {
                        Me.SetFacing(Me.CurrentTarget);
                        return RunStatus.Failure;
                    })
                );
        }

        #endregion

        #region Feint

        private static Composite Feint()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.Feint &&
                    BurstCooldownEnergyPool < DateTime.Now &&
                    SpellManager.HasSpell("Feint") &&
                    SpellManager.HasSpell("Elusiveness") &&
                    !MeMounted &&
                    Me.Combat &&
                    Me.HealthPercent <= THSettings.Instance.FeintHP &&
                    !Me.HasAura("Feint") &&
                    (PlayerEnergy > 70 ||
                     !Attackable(Me.CurrentTarget, 10) ||
                     Attackable(Me.CurrentTarget, 10) &&
                     Me.CurrentTarget.CurrentHealth > Me.CurrentHealth) &&
                    //MyAuraTimeLeft("Feint", Me) < 1000 &&
                    !IsStealthed(Me) &&
                    CountDPSTarget(Me) > 0 &&
                    SpellManager.CanCast("Feint"),
                    new Action(delegate { CastSpell("Feint", Me, "FeintHP"); })),
                new Decorator(
                    ret =>
                    THSettings.Instance.FeintTarget &&
                    SpellManager.HasSpell("Feint") &&
                    SpellManager.HasSpell("Elusiveness") &&
                    !MeMounted &&
                    Me.Combat &&
                    CountDPSTarget(Me) >= THSettings.Instance.FeintTargetUnit &&
                    !Me.HasAura("Feint") &&
                    //MyAuraTimeLeft("Feint", Me) < 1000 &&
                    !IsStealthed(Me) &&
                    SpellManager.CanCast("Feint"),
                    new Action(delegate { CastSpell("Feint", Me, "FeintTargetUnit"); }))
                );
        }

        #endregion

        #region Garrote

        private static DateTime GarroteLast;

        private static Composite GarroteOld()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    (THSettings.Instance.AttackOOC || Me.Combat) &&
                    THSettings.Instance.Garrote &&
                    GarroteLast < DateTime.Now &&
                    !SpellManager.HasSpell("Cloak and Dagger") &&
                    SpellManager.HasSpell("Garrote") &&
                    !MeMounted &&
                    IsStealthed(Me) &&
                    CurrentTargetAttackable(5) &&
                    !AmbushShadowDanceCheck() &&
                    WorthyTarget(Me.CurrentTarget, 0.5) &&
                    PlayerComboPoint < MaxComboPoint &&
                    //Me.IsBehind(Me.CurrentTarget) &&
                    FacingOverrideMeCurrentTarget &&
                    MyAuraTimeLeft(703, Me.CurrentTarget) < 2000 &&
                    SpellManager.CanCast("Garrote"),
                    new Action(delegate
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Garrote", Me.CurrentTarget, "Garrote");
                            GarroteLast = DateTime.Now + TimeSpan.FromMilliseconds(2000);
                        })),
                new Decorator(
                    ret =>
                    (THSettings.Instance.AttackOOC || Me.Combat) &&
                    THSettings.Instance.Garrote &&
                    GarroteLast < DateTime.Now &&
                    SpellManager.HasSpell("Cloak and Dagger") &&
                    SpellManager.HasSpell("Garrote") &&
                    !MeMounted &&
                    IsStealthedNoSD(Me) &&
                    CurrentTargetAttackable(30) &&
                    PlayerComboPoint < MaxComboPoint &&
                    FacingOverrideMeCurrentTarget &&
                    MyAuraTimeLeft(703, Me.CurrentTarget) < 2000 &&
                    (DebuffCC(Me.CurrentTarget) ||
                     Me.IsBehind(Me.CurrentTarget) ||
                     Me.CurrentTarget.IsPlayer ||
                     !Me.CurrentTarget.IsPlayer &&
                     (Me.CurrentTarget.CurrentTarget == null ||
                      Me.CurrentTarget.CurrentTarget != null &&
                      Me.CurrentTarget.CurrentTarget != Me)) &&
                    SpellManager.CanCast("Garrote"),
                    new Action(delegate
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Garrote", Me.CurrentTarget, "Garrote Cloak and Dagger");
                            GarroteLast = DateTime.Now + TimeSpan.FromMilliseconds(2000);
                        })),
                new Decorator(
                    ret =>
                    (THSettings.Instance.AttackOOC || Me.Combat) &&
                    THSettings.Instance.Garrote &&
                    GarroteLast < DateTime.Now &&
                    SpellManager.HasSpell("Cloak and Dagger") &&
                    SpellManager.HasSpell("Garrote") &&
                    !MeMounted &&
                    IsShadowDance(Me) &&
                    CurrentTargetAttackable(30) &&
                    PlayerComboPoint < MaxComboPoint &&
                    FacingOverrideMeCurrentTarget &&
                    MyAuraTimeLeft(703, Me.CurrentTarget) < 2000 &&
                    (DebuffCC(Me.CurrentTarget) ||
                     Me.IsBehind(Me.CurrentTarget) ||
                     Me.CurrentTarget.IsPlayer ||
                     !Me.CurrentTarget.IsPlayer &&
                     (Me.CurrentTarget.CurrentTarget == null ||
                      Me.CurrentTarget.CurrentTarget != null &&
                      Me.CurrentTarget.CurrentTarget != Me)) &&
                    SpellManager.CanCast("Garrote"),
                    new Action(delegate
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            GarroteLast = DateTime.Now + TimeSpan.FromMilliseconds(2000);
                            CastSpell("Garrote", Me.CurrentTarget, "Garrote IsShadowDance");
                        }))
                );
        }

        private static Composite GarroteCaD()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.Garrote &&
                GarroteLast < DateTime.Now &&
                SpellManager.HasSpell("Garrote") &&
                SpellManager.HasSpell("Cloak and Dagger") &&
                !MeMounted &&
                CurrentTargetAttackable(30) &&
                FacingOverrideMeCurrentTarget &&
                IsStealthedNoSD(Me) &&
                //WorthyTarget(Me.CurrentTarget, 0.5) &&
                (!MyAura(703, Me.CurrentTarget) ||
                 !MyAura(703, Me.CurrentTarget) &&
                 TalentSort(Me.CurrentTarget) >= 3) &&
                !CheapShotCheck() &&
                SpellManager.CanCast("Garrote"),
                new Action(delegate
                    {
                        SafelyFacingTarget(Me.CurrentTarget);
                        GarroteLast = DateTime.Now + TimeSpan.FromMilliseconds(2000);
                        CastSpell("Garrote", Me.CurrentTarget, "Garrote Cloak and Dagger");
                    }));
        }

        private static Composite Garrote()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.Garrote &&
                GarroteLast < DateTime.Now &&
                SpellManager.HasSpell("Garrote") &&
                !MeMounted &&
                CurrentTargetAttackable(5) &&
                FacingOverrideMeCurrentTarget &&
                IsStealthed(Me) &&
                //WorthyTarget(Me.CurrentTarget, 0.5) &&
                (!MyAura(703, Me.CurrentTarget) ||
                 !MyAura(703, Me.CurrentTarget) &&
                 TalentSort(Me.CurrentTarget) >= 3) &&
                !CheapShotCheck() &&
                SpellManager.CanCast("Garrote"),
                new Action(delegate
                    {
                        GarroteLast = DateTime.Now + TimeSpan.FromMilliseconds(2000);
                        SafelyFacingTarget(Me.CurrentTarget);
                        CastSpell("Garrote", Me.CurrentTarget, "Garrote");
                    }));
        }

        #endregion

        #region GougeInterrupt

        private static bool GetUnitInterruptGouge()
        {
            UnitInterrupt = null;

            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (InBattleground || InArena)
                {
                    UnitInterrupt = NearbyUnFriendlyPlayers.FirstOrDefault(
                        unit => unit != null && unit.IsValid &&
                                (THSettings.Instance.InterruptAll ||
                                 THSettings.Instance.InterruptTarget &&
                                 Me.CurrentTarget != null &&
                                 unit == Me.CurrentTarget ||
                                 THSettings.Instance.InterruptFocus &&
                                 Me.FocusedUnit != null &&
                                 unit == Me.FocusedUnit) &&
                                unit.IsWithinMeleeRange &&
                                FacingOverride(unit) &&
                                (HasGlyph.Contains("56809") || unit.IsFacing(Me)) &&
                                DisorientsLevelGet(Me.CurrentTarget) < 3 &&
                                InterruptCheck(unit, THSettings.Instance.GougeTimeLeft, true) &&
                                Attackable(unit, 5));

                    //UnitInterrupt = (from unit in NearbyUnFriendlyPlayers
                    //                 where unit != null &&
                    //                       unit.IsValid &&
                    //                       (THSettings.Instance.InterruptAll ||
                    //                        THSettings.Instance.InterruptTarget &&
                    //                        Me.CurrentTarget != null &&
                    //                        unit == Me.CurrentTarget ||
                    //                        THSettings.Instance.InterruptFocus &&
                    //                        Me.FocusedUnit != null &&
                    //                        unit == Me.FocusedUnit) &&
                    //                       unit.IsWithinMeleeRange &&
                    //                       FacingOverride(unit) &&
                    //                       (HasGlyph.Contains("56809") || unit.IsFacing(Me)) &&
                    //                       DisorientsLevelGet(Me.CurrentTarget) < 3 &&
                    //                       InterruptCheck(unit, THSettings.Instance.GougeTimeLeft, true) &&
                    //                       Attackable(unit, 5)
                    //                 orderby unit.CurrentCastTimeLeft ascending
                    //                 select unit).FirstOrDefault();
                }
                    //PvE Search
                else if (!Me.IsInInstance || InDungeon || InRaid)
                {
                    UnitInterrupt = NearbyUnFriendlyUnits.FirstOrDefault(
                        unit => unit != null && unit.IsValid &&
                                unit.Combat &&
                                FacingOverride(unit) &&
                                (HasGlyph.Contains("56809") || unit.IsFacing(Me)) &&
                                unit.IsWithinMeleeRange &&
                                InterruptCheck(unit,
                                               THSettings.Instance.GougeTimeLeft, true) &&
                                Attackable(unit, 5));

                    //UnitInterrupt = (from unit in NearbyUnFriendlyUnits
                    //                 where unit != null &&
                    //                       unit.IsValid &&
                    //                       //where !unit.IsBoss
                    //                       unit.Combat &&
                    //                       FacingOverride(unit) &&
                    //                       (HasGlyph.Contains("56809") || unit.IsFacing(Me)) &&
                    //                       unit.IsWithinMeleeRange &&
                    //                       InterruptCheck(unit,
                    //                                      THSettings.Instance.GougeTimeLeft, true) &&
                    //                       Attackable(unit, 40)
                    //                 orderby unit.CurrentCastTimeLeft ascending
                    //                 select unit).FirstOrDefault();
                }
            }

            return UnitInterrupt != null;
        }

        private static void GougeVoid()
        {
            if ((THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.Gouge &&
                LastInterrupt + TimeSpan.FromMilliseconds(1000) < DateTime.Now &&
                SpellManager.HasSpell("Gouge") &&
                !MeMounted &&
                GetUnitInterruptGouge() &&
                UnitInterrupt != null &&
                UnitInterrupt.IsValid &&
                SpellManager.CanCast("Gouge"))
            {
                SafelyFacingTarget(UnitInterrupt);
                CastSpell("Gouge", UnitInterrupt, "Gouge", true);
                LastInterrupt = DateTime.Now;
            }
        }

        #endregion

        #region Gouge

        private static WoWUnit UnitGougePet;

        private static bool GetUnitGougePet()
        {
            UnitGougePet = null;

            UnitGougePet = NearbyUnFriendlyUnits.FirstOrDefault(
                unit => unit != null && unit.IsValid &&
                        unit.IsPet &&
                        unit.IsWithinMeleeRange &&
                        unit.CreatedByUnit == Me.CurrentTarget &&
                        FacingOverride(unit) &&
                        (HasGlyph.Contains("56809") || unit.IsFacing(Me)));

            //UnitGougePet = (from unit in NearbyUnFriendlyUnits
            //                where unit != null &&
            //                      unit.IsValid &&
            //                      unit.IsPet &&
            //                      unit.IsWithinMeleeRange &&
            //                      unit.CreatedByUnit == Me.CurrentTarget &&
            //                      FacingOverride(unit) &&
            //                      (HasGlyph.Contains("56809") || unit.IsFacing(Me))
            //                select unit).FirstOrDefault();

            return UnitGougePet != null;
        }

        private static WoWUnit UnitGougeCooldown;

        private static bool GetUnitGougeCooldownCooddown()
        {
            UnitGougeCooldown = null;

            //Logging.Write("UnitGougeCooldown: Search");

            if (InBattleground || InArena)
            {
                UnitGougeCooldown = NearbyUnFriendlyPlayers.FirstOrDefault(
                    unit => unit != null && unit.IsValid &&
                            FacingOverride(unit) &&
                            (HasGlyph.Contains("56809") || unit.IsFacing(Me)) &&
                            (unit.CurrentTarget == null ||
                             unit.CurrentTarget != null &&
                             unit.CurrentTarget != Me ||
                             unit.CurrentTarget == null &&
                             unit.CurrentTarget == Me &&
                             unit.CurrentHealth > Me.CurrentHealth) &&
                            TalentSort(unit) <= 3 &&
                            BuffBurst(unit) &&
                            Attackable(unit, 5));

                //UnitGougeCooldown = (from unit in NearbyUnFriendlyPlayers
                //                     where unit != null &&
                //                           unit.IsValid &&
                //                           FacingOverride(unit) &&
                //                           (HasGlyph.Contains("56809") || unit.IsFacing(Me)) &&
                //                           (unit.CurrentTarget == null ||
                //                            unit.CurrentTarget != null &&
                //                            unit.CurrentTarget != Me ||
                //                            unit.CurrentTarget == null &&
                //                            unit.CurrentTarget == Me &&
                //                            unit.CurrentHealth > Me.CurrentHealth) &&
                //                           TalentSort(unit) <= 3 &&
                //                           BuffBurst(unit) &&
                //                           Attackable(unit, 5)
                //                     select unit).FirstOrDefault();
            }

            return UnitGougeCooldown != null;
        }

        private static WoWUnit UnitGougeHelpFriend;

        private static bool GeUnitGougeHelpFriend()
        {
            UnitGougeHelpFriend = null;

            UnitGougeHelpFriend = NearbyUnFriendlyPlayers.FirstOrDefault(
                unit => unit != null && unit.IsValid &&
                        unit.IsWithinMeleeRange &&
                        FacingOverride(unit) &&
                        (HasGlyph.Contains("56809") || unit.IsFacing(Me)) &&
                        unit.CurrentTarget != null &&
                        RaidPartyMembers.Contains(unit.CurrentTarget) &&
                        unit.CurrentTarget.HealthPercent <= THSettings.Instance.GougeHelpFriendHP &&
                        !DebuffDisarm(unit) &&
                        !DebuffCC(unit) &&
                        Attackable(unit, 5));

            //UnitGougeHelpFriend = (from unit in NearbyUnFriendlyPlayers
            //                       where unit != null &&
            //                             unit.IsValid &&
            //                             unit.IsWithinMeleeRange &&
            //                             FacingOverride(unit) &&
            //                             (HasGlyph.Contains("56809") || unit.IsFacing(Me)) &&
            //                             unit.CurrentTarget != null &&
            //                             RaidPartyMembers.Contains(unit.CurrentTarget) &&
            //                             unit.CurrentTarget.HealthPercent <= THSettings.Instance.GougeHelpFriendHP &&
            //                             !DebuffDisarm(unit) &&
            //                             !DebuffCC(unit) &&
            //                             Attackable(unit, 5)
            //                       orderby unit.CurrentHealth descending
            //                       select unit).FirstOrDefault();

            return UnitGougeHelpFriend != null;
        }

        private static Composite GougeHelpFriend()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.GougeHelpFriend &&
                    SpellManager.HasSpell("Gouge") &&
                    !IsStealthed(Me) &&
                    Me.Combat &&
                    GeUnitGougeHelpFriend() &&
                    UnitGougeHelpFriend != null &&
                    UnitGougeHelpFriend.IsValid &&
                    //FacingOverride(UnitGougePet) &&
                    SpellManager.CanCast("Gouge"),
                    new Action(
                        ret =>
                            {
                                SafelyFacingTarget(UnitGougeHelpFriend);
                                CastSpell("Gouge", UnitGougePet, "GougeHelpFriend");
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.GougeOffensiveCooldown &&
                    SpellManager.HasSpell("Gouge") &&
                    !IsStealthed(Me) &&
                    Me.Combat &&
                    GetUnitGougeCooldownCooddown() &&
                    UnitGougeCooldown != null &&
                    UnitGougeCooldown.IsValid &&
                    //FacingOverride(UnitGougePet) &&
                    SpellManager.CanCast("Gouge"),
                    new Action(
                        ret =>
                            {
                                SafelyFacingTarget(UnitGougeCooldown);
                                CastSpell("Gouge", UnitGougeCooldown, "GougeOffensiveCooldown");
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.GougePet &&
                    SpellManager.HasSpell("Gouge") &&
                    !IsStealthed(Me) &&
                    Me.Combat &&
                    CurrentTargetAttackable(40) &&
                    PlayerComboPoint < MaxComboPoint &&
                    (Me.CurrentTarget.Class == WoWClass.DeathKnight ||
                     Me.CurrentTarget.Class == WoWClass.Hunter ||
                     Me.CurrentTarget.Class == WoWClass.Mage ||
                     Me.CurrentTarget.Class == WoWClass.Priest ||
                     Me.CurrentTarget.Class == WoWClass.Shaman) &&
                    GetUnitGougePet() &&
                    UnitGougePet != null &&
                    UnitGougePet.IsValid &&
                    //FacingOverride(UnitGougePet) &&
                    SpellManager.CanCast("Gouge"),
                    new Action(
                        ret =>
                            {
                                SafelyFacingTarget(UnitGougePet);
                                CastSpell("Gouge", UnitGougePet, "GougePet");
                            }))
                );
        }

        #endregion

        #region Hemorrhage

        private static DateTime HemorrhageDebuffLast;

        private static Composite HemorrhageDebuff()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.Hemorrhage &&
                HemorrhageDebuffLast < DateTime.Now &&
                BurstCooldownEnergyPool < DateTime.Now &&
                SpellManager.HasSpell("Hemorrhage") &&
                !MeMounted &&
                (PlayerComboPoint < MaxComboPoint ||
                 PlayerEnergy >= Me.MaxEnergy) &&
                PoolEnergyPassCheck() &&
                CurrentTargetAttackable(5) &&
                WorthyTarget(Me.CurrentTarget, 0.5) &&
                FacingOverrideMeCurrentTarget &&
                //!AmbushCheck() &&
                MyAuraTimeLeft(89775, Me.CurrentTarget) < 2000 &&
                SpellManager.CanCast("Hemorrhage"),
                new Action(delegate
                    {
                        SafelyFacingTarget(Me.CurrentTarget);
                        HemorrhageDebuffLast = DateTime.Now + TimeSpan.FromMilliseconds(3000);
                        CastSpell("Hemorrhage", Me.CurrentTarget, "HemorrhageDebuff");
                    })
                );
        }

        private static Composite Hemorrhage()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.Hemorrhage &&
                BurstCooldownEnergyPool + TimeSpan.FromMilliseconds(5000) < DateTime.Now &&
                SpellManager.HasSpell("Hemorrhage") &&
                !MeMounted &&
                (PlayerComboPoint < MaxComboPoint ||
                 PlayerEnergy >= Me.MaxEnergy) &&
                CurrentTargetAttackable(5) &&
                PoolEnergyPassCheck() &&
                FacingOverrideMeCurrentTarget &&
                //!AmbushCheck() &&
                (!THSettings.Instance.Backstab ||
                 !Me.IsBehind(Me.CurrentTarget)) &&
                SpellManager.CanCast("Hemorrhage"),
                new Action(delegate
                    {
                        SafelyFacingTarget(Me.CurrentTarget);
                        CastSpell("Hemorrhage", Me.CurrentTarget, "Hemorrhage");
                    })
                );
        }

        #endregion

        #region IsStealthed

        private static bool IsStealthed(WoWUnit target)
        {
            using (StyxWoW.Memory.AcquireFrame())
            {
                //if (target == Me &&
                //    LastCastStealth + TimeSpan.FromMilliseconds(500) > DateTime.Now ||
                if (target.HasAura("Stealth") ||
                    target.HasAura("Vanish") ||
                    target.HasAura("Shadow Dance") ||
                    target.HasAura("Prowl") ||
                    target.IsStealthed)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsStealthedNoSD(WoWUnit target)
        {
            using (StyxWoW.Memory.AcquireFrame())
            {
                //if (target == Me &&
                //    LastCastStealth + TimeSpan.FromMilliseconds(500) > DateTime.Now ||
                if (target.HasAura("Stealth") ||
                    target.HasAura("Vanish") ||
                    target.HasAura("Prowl") ||
                    target.IsStealthed)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsShadowDance(WoWUnit target)
        {
            using (StyxWoW.Memory.AcquireFrame())
            {
                if (target.HasAura("Shadow Dance"))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region KidneyShot

        private static bool KidneyShotCheck()
        {
            if (!THSettings.Instance.KidneyShot ||
                !Me.Combat ||
                !SpellManager.HasSpell("Kidney Shot") ||
                Me.CurrentTarget == null ||
                !Me.CurrentTarget.IsValid ||
                InvulnerableStun(Me.CurrentTarget) ||
                !SafeUsingCooldown(Me.CurrentTarget) ||
                ControlStunLevelGet(Me.CurrentTarget) >= THSettings.Instance.KidneyShotStunLevel ||
                DebuffCCDuration(Me.CurrentTarget, 5000) ||
                GetSpellCooldown("Kidney Shot") > TimeSpan.FromMilliseconds(4000))
            {
                return false;
            }

            return true;
        }

        private static void KidneyShotCheckLog()
        {
            if (KidneyShotCheck())
            {
                Logging.Write("KidneyShotCheck() True");
                Logging.Write("--------------------------------------");
                Logging.Write("Me.Combat " + Me.Combat);
                Logging.Write("InvulnerableStun " + InvulnerableStun(Me.CurrentTarget));
                Logging.Write("SafeUsingCooldown " + SafeUsingCooldown(Me.CurrentTarget));
                Logging.Write("DebuffCCDuration(Me.CurrentTarget, 4000) " + DebuffCCDuration(Me.CurrentTarget, 4000));
                Logging.Write("GetSpellCooldown(Kidney Shot) " + GetSpellCooldown("Kidney Shot").TotalMilliseconds);
            }
            else
            {
                Logging.Write("KidneyShotCheck() False");
                Logging.Write("--------------------------------------");
                Logging.Write("Me.Combat " + Me.Combat);
                Logging.Write("InvulnerableStun " + InvulnerableStun(Me.CurrentTarget));
                Logging.Write("SafeUsingCooldown " + SafeUsingCooldown(Me.CurrentTarget));
                Logging.Write("DebuffCCDuration(Me.CurrentTarget, 4000) " + DebuffCCDuration(Me.CurrentTarget, 4000));
                Logging.Write("GetSpellCooldown(Kidney Shot) " + GetSpellCooldown("Kidney Shot").TotalMilliseconds);
            }
        }

        private static DateTime TimeBreakCC;

        private static Composite KidneyShot()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.KidneyShot &&
                SpellManager.HasSpell("Kidney Shot") &&
                //LastCastCC + TimeSpan.FromMilliseconds(2000) < DateTime.Now &&
                !MeMounted &&
                PlayerComboPoint >= THSettings.Instance.KidneyShotCP &&
                CurrentTargetAttackable(5) &&
                FacingOverrideMeCurrentTarget &&
                ControlStunLevelGet(Me.CurrentTarget) < THSettings.Instance.KidneyShotStunLevel &&
                //!IsStealthed(Me) &&
                !DebuffCCDuration(Me.CurrentTarget, 1000) &&
                !InvulnerableStun(Me.CurrentTarget) &&
                SafeUsingCooldown(Me.CurrentTarget) &&
                SpellManager.CanCast("Kidney Shot"),
                new Action(delegate
                    {
                        //Logging.Write(LogLevel.Diagnostic, "Current ControlStun DR Level {0}",
                        //              ControlStunLevelGet(Me.CurrentTarget));

                        SafelyFacingTarget(Me.CurrentTarget);
                        TimeBreakCC = DateTime.Now +
                                      TimeSpan.FromMilliseconds(DebuffStunDurationTimeLeft(Me.CurrentTarget));
                        while (DateTime.Now <
                               TimeBreakCC - TimeSpan.FromMilliseconds(THSettings.Instance.KidneyShotStunLeft))
                        {
                            //Logging.Write("Target still being stunned for " +
                            //              (TimeBreakCC - DateTime.Now).TotalMilliseconds + " ms. Just Wait a bit!");
                            Logging.Write("Target still being stunned, just Wait a bit!");
                        }
                        LastCastCC = DateTime.Now;
                        CastSpell("Kidney Shot", Me.CurrentTarget, "KidneyShot");
                    })
                );
        }

        #endregion

        #region KidneyShotInterrupt

        private static bool GetUnitInterruptKidney()
        {
            UnitInterrupt = null;

            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (InBattleground || InArena)
                {
                    UnitInterrupt = NearbyUnFriendlyPlayers.FirstOrDefault(
                        unit => unit != null && unit.IsValid &&
                                (THSettings.Instance.InterruptAll ||
                                 THSettings.Instance.InterruptTarget &&
                                 Me.CurrentTarget != null &&
                                 unit == Me.CurrentTarget ||
                                 THSettings.Instance.InterruptFocus &&
                                 Me.FocusedUnit != null &&
                                 unit == Me.FocusedUnit) &&
                                unit.IsWithinMeleeRange &&
                                FacingOverride(unit) &&
                                InterruptCheck(unit, THSettings.Instance.KidneyShotInterruptTimeLeft, true) &&
                                Attackable(unit, 5));

                    //UnitInterrupt = (from unit in NearbyUnFriendlyPlayers
                    //                 where unit != null &&
                    //                       unit.IsValid &&
                    //                       (THSettings.Instance.InterruptAll ||
                    //                        THSettings.Instance.InterruptTarget &&
                    //                        Me.CurrentTarget != null &&
                    //                        unit == Me.CurrentTarget ||
                    //                        THSettings.Instance.InterruptFocus &&
                    //                        Me.FocusedUnit != null &&
                    //                        unit == Me.FocusedUnit) &&
                    //                       unit.IsWithinMeleeRange &&
                    //                       FacingOverride(unit) &&
                    //                       InterruptCheck(unit, THSettings.Instance.KidneyShotInterruptTimeLeft, true) &&
                    //                       Attackable(unit, 5)
                    //                 orderby unit.CurrentCastTimeLeft ascending
                    //                 select unit).FirstOrDefault();
                }
                    //PvE Search
                else if (!Me.IsInInstance || InDungeon || InRaid)
                {
                    UnitInterrupt = NearbyUnFriendlyUnits.FirstOrDefault(
                        unit => unit != null && unit.IsValid &&
                                unit.Combat &&
                                FacingOverride(unit) &&
                                unit.IsWithinMeleeRange &&
                                InterruptCheck(unit,
                                               THSettings.Instance.KidneyShotInterruptTimeLeft, true) &&
                                Attackable(unit, 5));

                    //UnitInterrupt = (from unit in NearbyUnFriendlyUnits
                    //                 where unit != null &&
                    //                       unit.IsValid &&
                    //                       //where !unit.IsBoss
                    //                       unit.Combat &&
                    //                       FacingOverride(unit) &&
                    //                       unit.IsWithinMeleeRange &&
                    //                       InterruptCheck(unit,
                    //                                      THSettings.Instance.KidneyShotInterruptTimeLeft, true) &&
                    //                       Attackable(unit, 40)
                    //                 orderby unit.CurrentCastTimeLeft ascending
                    //                 select unit).FirstOrDefault();
                }
            }

            return UnitInterrupt != null;
        }

        private static void KidneyShotInterruptVoid()
        {
            if ((THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.KidneyShotInterrupt &&
                LastInterrupt + TimeSpan.FromMilliseconds(1000) < DateTime.Now &&
                SpellManager.HasSpell("Kidney Shot") &&
                !MeMounted &&
                PlayerComboPoint > 0 &&
                GetUnitInterruptKidney() &&
                UnitInterrupt != null &&
                UnitInterrupt.IsValid &&
                ControlStunLevelGet(Me.CurrentTarget) < 3 &&
                SpellManager.CanCast("Kidney Shot"))
            {
                //Logging.Write(LogLevel.Diagnostic, "Current ControlStun DR Level {0}",
                //              ControlStunLevelGet(UnitInterrupt));

                SafelyFacingTarget(UnitInterrupt);
                CastSpell("Kidney Shot", UnitInterrupt, "KidneyShotInterrupt", true);
                LastInterrupt = DateTime.Now;
            }
        }

        #endregion

        #region Kick

        private static DateTime LastInterrupt = DateTime.Now;
        private static WoWUnit UnitInterrupt;

        private static bool GetUnitInterruptKick()
        {
            UnitInterrupt = null;

            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (InBattleground || InArena)
                {
                    UnitInterrupt = NearbyUnFriendlyPlayers.FirstOrDefault(
                        unit => unit != null && unit.IsValid &&
                                (THSettings.Instance.InterruptAll ||
                                 THSettings.Instance.InterruptTarget &&
                                 Me.CurrentTarget != null &&
                                 unit == Me.CurrentTarget ||
                                 THSettings.Instance.InterruptFocus &&
                                 Me.FocusedUnit != null &&
                                 unit == Me.FocusedUnit) &&
                                unit.IsWithinMeleeRange &&
                                FacingOverride(unit) &&
                                InterruptCheck(unit, THSettings.Instance.KickTimeLeft + 1000, false) &&
                                Attackable(unit, 5));

                    //UnitInterrupt = (from unit in NearbyUnFriendlyPlayers
                    //                 where unit != null &&
                    //                       unit.IsValid &&
                    //                       (THSettings.Instance.InterruptAll ||
                    //                        THSettings.Instance.InterruptTarget &&
                    //                        Me.CurrentTarget != null &&
                    //                        unit == Me.CurrentTarget ||
                    //                        THSettings.Instance.InterruptFocus &&
                    //                        Me.FocusedUnit != null &&
                    //                        unit == Me.FocusedUnit) &&
                    //                       unit.IsWithinMeleeRange &&
                    //                       FacingOverride(unit) &&
                    //                       InterruptCheck(unit, THSettings.Instance.KickTimeLeft + 1000, false) &&
                    //                       Attackable(unit, 5)
                    //                 orderby unit.CurrentCastTimeLeft ascending
                    //                 select unit).FirstOrDefault();
                }
                    //PvE Search
                else if (!Me.IsInInstance || InDungeon || InRaid)
                {
                    UnitInterrupt = NearbyUnFriendlyUnits.FirstOrDefault(
                        unit => unit != null && unit.IsValid &&
                                unit.Combat &&
                                FacingOverride(unit) &&
                                unit.IsWithinMeleeRange &&
                                InterruptCheck(unit,
                                               THSettings.Instance.KickTimeLeft + 1000, false) &&
                                Attackable(unit, 5));

                    //UnitInterrupt = (from unit in NearbyUnFriendlyUnits
                    //                 where unit != null &&
                    //                       unit.IsValid &&
                    //                       //where !unit.IsBoss
                    //                       unit.Combat &&
                    //                       FacingOverride(unit) &&
                    //                       unit.IsWithinMeleeRange &&
                    //                       InterruptCheck(unit,
                    //                                      THSettings.Instance.KickTimeLeft + 1000, false) &&
                    //                       Attackable(unit, 40)
                    //                 orderby unit.CurrentCastTimeLeft ascending
                    //                 select unit).FirstOrDefault();
                }
            }

            return UnitInterrupt != null;
        }

        private static void KickVoid()
        {
            if ((THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.Kick &&
                LastInterrupt + TimeSpan.FromMilliseconds(1000) < DateTime.Now &&
                SpellManager.HasSpell("Kick") &&
                !MeMounted &&
                GetUnitInterruptKick() &&
                UnitInterrupt != null &&
                UnitInterrupt.IsValid &&
                //GetSpellCooldown("Kick") < TimeSpan.FromMilliseconds(1000))
                SpellManager.CanCast("Kick"))
            {
                while (UnitInterrupt.IsCasting &&
                       UnitInterrupt.CurrentCastTimeLeft.TotalMilliseconds > THSettings.Instance.KickTimeLeft)
                {
                    Logging.Write(LogLevel.Diagnostic, "Waiting for Kick");
                }

                if (UnitInterrupt.IsCasting || UnitInterrupt.IsChanneling)
                {
                    SafelyFacingTarget(UnitInterrupt);
                    CastSpell("Kick", UnitInterrupt, "Kick");
                    LastInterrupt = DateTime.Now;
                }
            }
        }

        #endregion

        #region KillingSpree

        private static Composite KillingSpree()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.KillingSpree &&
                SpellManager.HasSpell("Killing Spree") &&
                (THSettings.Instance.KillingSpreeCD ||
                 THSettings.Instance.KillingSpreeBurst &&
                 Burst) &&
                !MeMounted &&
                Me.Combat &&
                CurrentTargetAttackable(10) &&
                WorthyTarget(Me.CurrentTarget) &&
                SpellManager.CanCast("Killing Spree"),
                new Action(delegate { CastSpell("Killing Spree", Me, "KillingSpree"); })
                );
        }

        #endregion

        #region MarkedforDeath

        private static DateTime AddComboLast;

        private static Composite MarkedforDeath()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.MarkedforDeath &&
                AddComboLast + TimeSpan.FromMilliseconds(2000) < DateTime.Now &&
                SpellManager.HasSpell("Marked for Death") &&
                !MeMounted &&
                Me.Combat &&
                CurrentTargetAttackable(10) &&
                (THSettings.Instance.MarkedforDeathCD ||
                 THSettings.Instance.MarkedforDeathBurst &&
                 Burst ||
                 THSettings.Instance.MarkedforDeathLow &&
                 Me.CurrentTarget.HealthPercent <= THSettings.Instance.MarkedforDeathLowHP) &&
                PlayerComboPoint < 3 &&
                !IsStealthedNoSD(Me) &&
                SpellManager.CanCast("Marked for Death"),
                new Action(delegate
                    {
                        AddComboLast = DateTime.Now;
                        CastSpell("Marked for Death", Me.CurrentTarget, "MarkedforDeath");
                        return RunStatus.Failure;
                    })
                );
        }

        #endregion

        #region Mutilate

        private static Composite Mutilate()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.Mutilate &&
                BurstCooldownEnergyPool < DateTime.Now &&
                SpellManager.HasSpell("Mutilate") &&
                !MeMounted &&
                (PlayerComboPoint < MaxComboPoint ||
                 PlayerEnergy >= Me.MaxEnergy) &&
                CurrentTargetAttackable(5) &&
                Me.CurrentTarget.HealthPercent > 35 &&
                !Me.HasAura(121153) &&
                FacingOverrideMeCurrentTarget &&
                //!AmbushCheck() &&
                SpellManager.CanCast("Mutilate"),
                new Action(delegate
                    {
                        SafelyFacingTarget(Me.CurrentTarget);
                        CastSpell("Mutilate", Me.CurrentTarget, "Mutilate");
                    })
                );
        }

        #endregion

        #region OpenerRotation

        private static DateTime SubterfugeTimeEnd;


        private static Composite OpenerRotation()
        {
            return new PrioritySelector(
                //WriteDebugSubterfuge(),
                new Decorator(
                    ret =>
                    (THSettings.Instance.AttackOOC || Me.Combat) &&
                    SpellManager.HasSpell("Cloak and Dagger") &&
                    !MeMounted &&
                    CurrentTargetAttackable(20) &&
                    FacingOverrideMeCurrentTarget &&
                    IsStealthedNoSD(Me),
                    new PrioritySelector(
                        new Decorator(
                            ret =>
                            THSettings.Instance.CheapShot &&
                            SpellManager.HasSpell("Cheap Shot") &&
                            !DebuffCCDuration(Me.CurrentTarget, 1500) &&
                            !InvulnerableStun(Me.CurrentTarget) &&
                            (!SpellManager.HasSpell("Subterfuge") ||
                             Me.HasAura(115192) &&
                             MyAuraTimeLeft(115192, Me) < 1000),
                            new Action(delegate
                                {
                                    SafelyFacingTarget(Me.CurrentTarget);

                                    while (MyAuraTimeLeft(115192, Me) > MyLatency)
                                    {
                                        if (DebuffStun(Me.CurrentTarget))
                                        {
                                            Logging.Write("DebuffStun Break");
                                            break;
                                        }

                                        if (!IsStealthed(Me))
                                        {
                                            Logging.Write("!IsStealthed Break");
                                            break;
                                        }

                                        if (MyAuraTimeLeft(115192, Me) < MyLatency + 300 &&
                                            SpellManager.CanCast("Cheap Shot"))
                                        {
                                            CastSpell("Cheap Shot", Me.CurrentTarget,
                                                      "OpenerRotation: Cheap Shot Cloak and Dagger");
                                        }
                                    }
                                    return RunStatus.Success;
                                })),
                        new Decorator(
                            ret =>
                            THSettings.Instance.Garrote &&
                            SpellManager.HasSpell("Garrote") &&
                            //WorthyTarget(Me.CurrentTarget, 0.5) &&
                            !MyAura(703, Me.CurrentTarget) &&
                            !CheapShotCheck() &&
                            SpellManager.CanCast("Garrote"),
                            new Action(delegate
                                {
                                    SafelyFacingTarget(Me.CurrentTarget);
                                    CastSpell("Garrote", Me.CurrentTarget,
                                              "OpenerRotation: Garrote Cloak and Dagger");
                                    return RunStatus.Success;
                                })),
                        new Decorator(
                            ret =>
                            THSettings.Instance.Ambush &&
                            SpellManager.HasSpell("Ambush") &&
                            //PlayerComboPoint < MaxComboPoint &&
                            (DebuffCC(Me.CurrentTarget) ||
                             Me.IsBehind(Me.CurrentTarget) ||
                             Me.CurrentTarget.IsPlayer ||
                             !Me.CurrentTarget.IsPlayer &&
                             (Me.CurrentTarget.CurrentTarget == null ||
                              Me.CurrentTarget.CurrentTarget != null &&
                              Me.CurrentTarget.CurrentTarget != Me)) &&
                            !CheapShotCheck() &&
                            CalculateEnergy(60, MyAuraTimeLeft(115192, Me)) > 30 &&
                            SpellManager.CanCast("Ambush"),
                            new Action(delegate
                                {
                                    Logging.Write("MyAuraTimeLeft(115192, Me) " + MyAuraTimeLeft(115192, Me));
                                    SafelyFacingTarget(Me.CurrentTarget);
                                    CastSpell("Ambush", Me.CurrentTarget,
                                              "OpenerRotation: Ambush Cloak and Dagger");
                                    return RunStatus.Success;
                                }))
                        )),
                new Decorator(
                    ret =>
                    (THSettings.Instance.AttackOOC || Me.Combat) &&
                    !SpellManager.HasSpell("Cloak and Dagger") &&
                    !MeMounted &&
                    CurrentTargetAttackable(5 + AmbushBonusRange) &&
                    FacingOverrideMeCurrentTarget &&
                    IsStealthed(Me),
                    new PrioritySelector(
                        new Decorator(
                            ret =>
                            THSettings.Instance.CheapShot &&
                            CurrentTargetAttackable(5) &&
                            SpellManager.HasSpell("Cheap Shot") &&
                            !DebuffCCDuration(Me.CurrentTarget, 1500) &&
                            !InvulnerableStun(Me.CurrentTarget) &&
                            (!SpellManager.HasSpell("Subterfuge") ||
                             Me.HasAura(115192) &&
                             MyAuraTimeLeft(115192, Me) < 1000),
                            new Action(delegate
                                {
                                    SafelyFacingTarget(Me.CurrentTarget);
                                    while (MyAuraTimeLeft(115192, Me) > 0)
                                    {
                                        if (DebuffStun(Me.CurrentTarget))
                                        {
                                            Logging.Write("DebuffStun Break");
                                            break;
                                        }

                                        if (!IsStealthed(Me))
                                        {
                                            Logging.Write("!IsStealthed Break");
                                            break;
                                        }

                                        if (MyAuraTimeLeft(115192, Me) < 500 &&
                                            SpellManager.CanCast("Cheap Shot"))
                                        {
                                            CastSpell("Cheap Shot", Me.CurrentTarget,
                                                      "OpenerRotation: Cheap Shot");
                                        }
                                    }
                                    return RunStatus.Success;
                                })),
                        new Decorator(
                            ret =>
                            THSettings.Instance.Garrote &&
                            CurrentTargetAttackable(5) &&
                            SpellManager.HasSpell("Garrote") &&
                            //WorthyTarget(Me.CurrentTarget, 0.5) &&
                            !MyAura(703, Me.CurrentTarget) &&
                            !CheapShotCheck() &&
                            SpellManager.CanCast("Garrote"),
                            new Action(delegate
                                {
                                    SafelyFacingTarget(Me.CurrentTarget);
                                    CastSpell("Garrote", Me.CurrentTarget,
                                              "OpenerRotation: Garrote");
                                    return RunStatus.Success;
                                })),
                        new Decorator(
                            ret =>
                            THSettings.Instance.Ambush &&
                            CurrentTargetAttackable(5 + AmbushBonusRange) &&
                            SpellManager.HasSpell("Ambush") &&
                            //PlayerComboPoint < MaxComboPoint &&
                            Me.IsBehind(Me.CurrentTarget) &&
                            !CheapShotCheck() &&
                            CalculateEnergy(60, MyAuraTimeLeft(115192, Me)) > 30 &&
                            SpellManager.CanCast("Ambush"),
                            new Action(delegate
                                {
                                    //Logging.Write("MyAuraTimeLeft(115192, Me) " + MyAuraTimeLeft(115192, Me));
                                    SafelyFacingTarget(Me.CurrentTarget);
                                    CastSpell("Ambush", Me.CurrentTarget,
                                              "OpenerRotation: Ambush");
                                    return RunStatus.Success;
                                }))
                        ))
                );
        }

        private static Composite OpenerStopCheck()
        {
            //return new Decorator(
            //    ret =>
            //    THSettings.Instance.CheapShot &&
            //    SpellManager.HasSpell("Cheap Shot") &&
            //    CalculateEnergy(40, MyAuraTimeLeft(115192, Me)) < 40,
            return new Action(delegate
                {
                    if (THSettings.Instance.CheapShot &&
                        SpellManager.HasSpell("Cheap Shot") &&
                        Me.Combat &&
                        Attackable(Me.CurrentTarget, 5) &&
                        ControlStunLevelGet(Me.CurrentTarget) < THSettings.Instance.CheapShotStunLevel &&
                        PlayerEnergy < Me.MaxEnergy - 20 &&
                        IsStealthedNoSD(Me))
                    {
                        return RunStatus.Success;
                    }
                    return RunStatus.Failure;
                });
        }

        #endregion

        #region PickPocket

        private static DateTime PickPocketLast;

        private static Composite PickPocket()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.PickPocket &&
                PickPocketLast < DateTime.Now &&
                SpellManager.HasSpell("Pick Pocket") &&
                !MeMounted &&
                //!Me.Combat &&
                IsStealthedNoSD(Me) &&
                CurrentTargetAttackable(5) &&
                Me.CurrentTarget.IsHumanoid &&
                SpellManager.CanCast("Pick Pocket"),
                new Action(delegate
                    {
                        PickPocketLast = DateTime.Now + TimeSpan.FromMilliseconds(10000);
                        SafelyFacingTarget(Me.CurrentTarget);
                        CastSpell("Pick Pocket", Me.CurrentTarget, "PickPocket");
                        return RunStatus.Failure;
                    })
                );
        }

        #endregion

        #region PendingSpellRemove

        private static Stopwatch PendingSpellStopWatch = new Stopwatch();

        private static void PendingSpellRemove()
        {
            if (Me.CurrentPendingCursorSpell != null &&
                !PendingSpellStopWatch.IsRunning)
            {
                PendingSpellStopWatch.Reset();
                PendingSpellStopWatch.Start();
            }

            if (Me.CurrentPendingCursorSpell != null &&
                PendingSpellStopWatch.IsRunning &&
                PendingSpellStopWatch.Elapsed.TotalMilliseconds > 2000)
            {
                Lua.DoString("SpellStopTargeting()");
                //Lua.DoString("RunMacroText(\"/cast Jab\")"); //Shirt
                PendingSpellStopWatch.Stop();
            }
        }

        #endregion

        #region PlayerEnergy

        private static double PlayerEnergy
        {
            get
            {
                try
                {
                    using (StyxWoW.Memory.AcquireFrame())
                    {
                        var currentEnergy =
                            Lua.GetReturnVal<int>("return UnitPower(\"player\", SPELL_POWER_ENERGY );", 0);
                        //var currentRunicPower = Me.CurrentRunicPower;
                        return currentEnergy;
                    }
                }
                catch
                {
                    Logging.Write("Failed in PlayerEnergy");
                    return 0;
                }
            }
        }

        private static double PlayerComboPoint
        {
            get
            {
                try
                {
                    using (StyxWoW.Memory.AcquireFrame())
                    {
                        //var currentComboPoint = Lua.GetReturnVal<int>("return GetComboPoints(\"player\");", 0);
                        var currentComboPoint = Me.ComboPoints;

                        if (Me.HasAura(115189))
                        {
                            currentComboPoint = (int) (currentComboPoint + Me.GetAuraById(115189).StackCount);
                        }

                        if (LastCastSpell == "Marked for Death" &&
                            GetSpellCooldown("Marked for Death").TotalMilliseconds > 58)
                        {
                            currentComboPoint = 5;
                        }
                        return currentComboPoint;
                    }
                }
                catch
                {
                    Logging.Write("Failed in currentComboPoint");
                    return 0;
                }
            }
        }

        #endregion

        #region Poison

        private static DateTime LastCastPoison;

        private static Composite Poison()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.PoisonLethal == 1 &&
                    SpellManager.HasSpell("Deadly Poison") &&
                    LastCastPoison < DateTime.Now &&
                    !MeMounted &&
                    !Me.Combat &&
                    !CurrentTargetAttackable(30) &&
                    MyAuraTimeLeft("Deadly Poison", Me) < 1800000 &&
                    //(!Me.Combat &&
                    // MyAuraTimeLeft("Deadly Poison", Me) < 1800000 ||
                    // MyAuraTimeLeft("Deadly Poison", Me) < 1000000) &&
                    SpellManager.CanCast("Deadly Poison"),
                    new Action(delegate
                        {
                            CastSpell("Deadly Poison", Me, "Wound Poison");
                            LastCastPoison = DateTime.Now + TimeSpan.FromMilliseconds(5000);
                        })
                    ),
                new Decorator(
                    ret =>
                    THSettings.Instance.PoisonLethal == 2 &&
                    LastCastPoison < DateTime.Now &&
                    SpellManager.HasSpell("Wound Poison") &&
                    !MeMounted &&
                    !Me.Combat &&
                    !CurrentTargetAttackable(30) &&
                    MyAuraTimeLeft("Wound Poison", Me) < 1800000 &&
                    //(!Me.Combat &&
                    // MyAuraTimeLeft("Wound Poison", Me) < 1800000 ||
                    // MyAuraTimeLeft("Wound Poison", Me) < 1000000) &&
                    SpellManager.CanCast("Wound Poison"),
                    new Action(delegate
                        {
                            CastSpell("Wound Poison", Me, "Wound Poison");
                            LastCastPoison = DateTime.Now + TimeSpan.FromMilliseconds(5000);
                        })
                    ),
                new Decorator(
                    ret =>
                    THSettings.Instance.PoisonLethal != 0 &&
                    SpellManager.HasSpell("Deadly Poison") &&
                    LastCastPoison < DateTime.Now &&
                    !MeMounted &&
                    !Me.Combat &&
                    !CurrentTargetAttackable(30) &&
                    !Me.HasAura("Deadly Poison") &&
                    !Me.HasAura("Wound Poison") &&
                    SpellManager.CanCast("Deadly Poison"),
                    new Action(delegate
                        {
                            CastSpell("Deadly Poison", Me, "Deadly Poison Default");
                            LastCastPoison = DateTime.Now + TimeSpan.FromMilliseconds(5000);
                        })
                    ),
                new Decorator(
                    ret =>
                    THSettings.Instance.PoisonNonLethal == 1 &&
                    SpellManager.HasSpell("Crippling Poison") &&
                    LastCastPoison < DateTime.Now &&
                    !MeMounted &&
                    !Me.Combat &&
                    !CurrentTargetAttackable(30) &&
                    MyAuraTimeLeft("Crippling Poison", Me) < 1800000 &&
                    //(!Me.Combat &&
                    // MyAuraTimeLeft("Crippling Poison", Me) < 1800000 ||
                    // MyAuraTimeLeft("Crippling Poison", Me) < 1000000) &&
                    SpellManager.CanCast("Crippling Poison"),
                    new Action(delegate
                        {
                            CastSpell("Crippling Poison", Me, "Crippling Poison");
                            LastCastPoison = DateTime.Now + TimeSpan.FromMilliseconds(5000);
                        })
                    ),
                new Decorator(
                    ret =>
                    THSettings.Instance.PoisonNonLethal == 2 &&
                    SpellManager.HasSpell("Mind-numbing Poison") &&
                    LastCastPoison < DateTime.Now &&
                    !MeMounted &&
                    !Me.Combat &&
                    !CurrentTargetAttackable(30) &&
                    MyAuraTimeLeft("Mind-numbing Poison", Me) < 1800000 &&
                    //(!Me.Combat &&
                    // MyAuraTimeLeft("Mind-numbing Poison", Me) < 1800000 ||
                    // MyAuraTimeLeft("Mind-numbing Poison", Me) < 1000000) &&
                    SpellManager.CanCast("Mind-numbing Poison"),
                    new Action(delegate
                        {
                            CastSpell("Mind-numbing Poison", Me, "Mind-numbing Poison");
                            LastCastPoison = DateTime.Now + TimeSpan.FromMilliseconds(5000);
                        })
                    ),
                new Decorator(
                    ret =>
                    THSettings.Instance.PoisonNonLethal == 3 &&
                    SpellManager.HasSpell("Leeching Poison") &&
                    LastCastPoison < DateTime.Now &&
                    !MeMounted &&
                    !Me.Combat &&
                    !CurrentTargetAttackable(30) &&
                    MyAuraTimeLeft("Leeching Poison", Me) < 1800000 &&
                    //(!Me.Combat &&
                    // MyAuraTimeLeft("Leeching Poison", Me) < 1800000 ||
                    // MyAuraTimeLeft("Leeching Poison", Me) < 1000000) &&
                    SpellManager.CanCast("Leeching Poison"),
                    new Action(delegate
                        {
                            CastSpell("Leeching Poison", Me, "Leeching Poison");
                            LastCastPoison = DateTime.Now + TimeSpan.FromMilliseconds(5000);
                        })
                    ),
                new Decorator(
                    ret =>
                    THSettings.Instance.PoisonNonLethal == 4 &&
                    SpellManager.HasSpell("Paralytic Poison") &&
                    LastCastPoison < DateTime.Now &&
                    !MeMounted &&
                    !Me.Combat &&
                    !CurrentTargetAttackable(30) &&
                    MyAuraTimeLeft("Paralytic Poison", Me) < 1800000 &&
                    //(!Me.Combat &&
                    // MyAuraTimeLeft("Paralytic Poison", Me) < 1800000 ||
                    // MyAuraTimeLeft("Paralytic Poison", Me) < 1000000) &&
                    SpellManager.CanCast("Paralytic Poison"),
                    new Action(delegate
                        {
                            CastSpell("Paralytic Poison", Me, "Paralytic Poison");
                            LastCastPoison = DateTime.Now + TimeSpan.FromMilliseconds(5000);
                        })
                    ),
                new Decorator(
                    ret =>
                    THSettings.Instance.PoisonLethal != 0 &&
                    SpellManager.HasSpell("Crippling Poison") &&
                    LastCastPoison < DateTime.Now &&
                    !MeMounted &&
                    !Me.Combat &&
                    !CurrentTargetAttackable(30) &&
                    !Me.HasAura("Crippling Poison") &&
                    !Me.HasAura("Mind-numbing Poison") &&
                    !Me.HasAura("Leeching Poison") &&
                    !Me.HasAura("Paralytic Poison") &&
                    SpellManager.CanCast("Crippling Poison"),
                    new Action(delegate
                        {
                            CastSpell("Crippling Poison", Me, "Crippling Poison Default");
                            LastCastPoison = DateTime.Now + TimeSpan.FromMilliseconds(5000);
                        })
                    )
                );
        }

        #endregion

        #region PoolEnergyCheck

        private static bool PoolEnergyPassCheck()
        {
            if (!THSettings.Instance.PoolEnergy)
            {
                return true;
            }


            if (Me.CurrentEnergy >= THSettings.Instance.PoolEnergyPC)
            {
                return true;
            }

            if (Me.HasAura("Shadow Blades"))
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Premeditation

        private static WoWPlayer UnitPremeditation;

        private static bool GetUnitPremeditation()
        {
            if (!InArena && !InBattleground)
            {
                return false;
            }

            UnitPremeditation = null;

            //using (StyxWoW.Memory.AcquireFrame())
            {
                UnitPremeditation = NearbyUnFriendlyPlayers.FirstOrDefault(
                    unit => unit != null && unit.IsValid &&
                            Attackable(unit, 30));


                //UnitPremeditation = (from unit in NearbyUnFriendlyPlayers
                //                     where unit != null &&
                //                           unit.IsValid &&
                //                           Attackable(unit, 30)
                //                     orderby unit.CurrentHealth ascending
                //                     select unit).
                //    FirstOrDefault();
            }

            return UnitPremeditation != null;
        }

        private static Composite Premeditation()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.Premeditation &&
                    (THSettings.Instance.AttackOOC || Me.Combat ||
                     THSettings.Instance.Recuperate &&
                     Me.HealthPercent <= THSettings.Instance.RecuperateHP &&
                     !Me.HasAura("Recuperate") &&
                     PlayerComboPoint < 1 ||
                     THSettings.Instance.SliceandDice &&
                     !Me.HasAura("Slice and Dice") &&
                     PlayerComboPoint < 1) &&
                    AddComboLast + TimeSpan.FromMilliseconds(2000) < DateTime.Now &&
                    SpellManager.HasSpell("Premeditation") &&
                    !MeMounted &&
                    CurrentTargetAttackable(30) &&
                    PlayerComboPoint < MaxComboPoint - 1 &&
                    IsStealthed(Me) &&
                    SpellManager.CanCast("Premeditation"),
                    new Action(delegate
                        {
                            AddComboLast = DateTime.Now;
                            CastSpell("Premeditation", Me.CurrentTarget, "Premeditation");
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Premeditation &&
                    THSettings.Instance.Recuperate &&
                    Me.HealthPercent <= THSettings.Instance.RecuperateHP &&
                    !Me.HasAura("Recuperate") &&
                    PlayerComboPoint < 1 &&
                    SpellManager.HasSpell("Premeditation") &&
                    IsStealthed(Me) &&
                    GetUnitPremeditation() &&
                    SpellManager.CanCast("Premeditation"),
                    new Action(delegate
                        {
                            AddComboLast = DateTime.Now;
                            CastSpell("Premeditation", UnitPremeditation, "Premeditation Recuperate");
                            return RunStatus.Failure;
                        }))
                );
        }

        #endregion

        #region Preparation

        private static bool ReadinessAllCD()
        {
            if (!THSettings.Instance.DismantlePre &&
                !THSettings.Instance.EvasionPre &&
                !THSettings.Instance.SprintPre &&
                !THSettings.Instance.VanishPre)
            {
                return false;
            }
            return true;
        }

        private static bool DismantlePre()
        {
            if (!THSettings.Instance.DismantlePre ||
                !SpellManager.HasSpell("Dismantle"))
            {
                return true;
            }

            if (SpellManager.HasSpell("Dismantle") &&
                SpellManager.Spells["Dismantle"].CooldownTimeLeft.TotalSeconds > 20)
            {
                return true;
            }
            return false;
        }

        private static bool EvasionPre()
        {
            if (!THSettings.Instance.EvasionPre ||
                !SpellManager.HasSpell("Evasion"))
            {
                return true;
            }

            if (SpellManager.HasSpell("Evasion") &&
                SpellManager.Spells["Evasion"].CooldownTimeLeft.TotalSeconds > 20)
            {
                return true;
            }
            return false;
        }

        private static bool SprintPre()
        {
            if (!THSettings.Instance.SprintPre ||
                !SpellManager.HasSpell("Sprint"))
            {
                return true;
            }

            if (SpellManager.HasSpell("Sprint") &&
                SpellManager.Spells["Sprint"].CooldownTimeLeft.TotalSeconds > 20)
            {
                return true;
            }
            return false;
        }

        private static bool VanishPre()
        {
            if (!THSettings.Instance.VanishPre ||
                !SpellManager.HasSpell("Vanish"))
            {
                return true;
            }

            if (SpellManager.HasSpell("Vanish") &&
                SpellManager.Spells["Vanish"].CooldownTimeLeft.TotalSeconds > 20)
            {
                return true;
            }
            return false;
        }

        private static Composite Preparation()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.Preparation &&
                SpellManager.HasSpell("Preparation") &&
                !MeMounted &&
                Me.Combat &&
                CurrentTargetAttackable(40) &&
                ReadinessAllCD() &&
                SpellManager.CanCast("Preparation") &&
                DismantlePre() &&
                EvasionPre() &&
                SprintPre() &&
                VanishPre(),
                new Action(
                    ret =>
                        {
                            CastSpell("Preparation", Me, "Preparation");
                            return RunStatus.Failure;
                        })
                );
        }

        #endregion

        #region Recuperate

        private static Composite Recuperate()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.Recuperate &&
                    SpellManager.HasSpell("Recuperate") &&
                    !MeMounted &&
                    !AmbushShadowDanceCheck() &&
                    !KidneyShotCheck() &&
                    (Me.RawComboPoints > 0 ||
                     PlayerComboPoint > 0) &&
                    Me.HealthPercent <= THSettings.Instance.RecuperateHP &&
                    //!Me.HasAura("Recuperate") &&
                    MyAuraTimeLeft("Recuperate", Me) < 2000 &&
                    !IsShadowDance(Me) &&
                    Me.Combat &&
                    SpellManager.CanCast("Recuperate"),
                    new Action(delegate
                        {
                            //KidneyShotCheckLog();
                            CastSpell("Recuperate", Me, "Recuperate");
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Recuperate &&
                    SpellManager.HasSpell("Recuperate") &&
                    !MeMounted &&
                    !Me.Combat &&
                    (Me.RawComboPoints > 0 ||
                     PlayerComboPoint > 0) &&
                    Me.HealthPercent <= THSettings.Instance.DoNotHealAbove &&
                    MyAuraTimeLeft("Recuperate", Me) < 2000 &&
                    !CurrentTargetAttackable(30) &&
                    SpellManager.CanCast("Recuperate"),
                    new Action(delegate
                        {
                            //KidneyShotCheckLog();
                            CastSpell("Recuperate", Me, "Recuperate Top Up");
                        }))
                );
        }

        #endregion

        #region Redirect

        private static Composite Redirect()
        {
            return new Decorator(
                ret =>
                //(THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.Redirect &&
                //AddComboLast + TimeSpan.FromMilliseconds(2000) < DateTime.Now &&
                SpellManager.HasSpell("Redirect") &&
                !MeMounted &&
                Me.RawComboPoints > 0 &&
                Math.Abs(Me.RawComboPoints - PlayerComboPoint) > 0 &&
                CurrentTargetAttackable(40) &&
                PlayerComboPoint + Me.RawComboPoints <= MaxComboPoint &&
                SpellManager.CanCast("Redirect"),
                new Action(delegate
                    {
                        //AddComboLast = DateTime.Now;
                        CastSpell("Redirect", Me.CurrentTarget, "Redirect");
                    })
                )
                ;
        }

        #endregion

        #region RestRotation

        private static Composite RestRotation()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.UseFood &&
                Me.HealthPercent <= THSettings.Instance.UseFoodHP &&
                !Me.Combat &&
                !Me.IsDead &&
                !Me.IsGhost &&
                !IsOverrideModeOn &&
                !Me.IsCasting &&
                !Me.IsSwimming &&
                !Me.HasAura("Food") &&
                Consumable.GetBestFood(true) != null,
                new Action(delegate
                    {
                        Logging.Write("Using Food");

                        if (Me.IsMoving)
                        {
                            WoWMovement.MoveStop();
                            //StyxWoW.SleepForLagDuration();
                        }

                        Styx.CommonBot.Rest.FeedImmediate();
                        Thread.Sleep(TimeSpan.FromMilliseconds(300));
                    })
                );
        }

        #endregion

        #region RevealingStrike

        private static Composite RevealingStrike()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.RevealingStrike &&
                BurstCooldownEnergyPool < DateTime.Now &&
                SpellManager.HasSpell("Revealing Strike") &&
                !MeMounted &&
                PlayerComboPoint < MaxComboPoint &&
                PoolEnergyPassCheck() &&
                CurrentTargetAttackable(5) &&
                WorthyTarget(Me.CurrentTarget, 0.5) &&
                FacingOverrideMeCurrentTarget &&
                //!AmbushCheck() &&
                MyAuraTimeLeft(84617, Me.CurrentTarget) < 2000 &&
                SpellManager.CanCast("Revealing Strike"),
                new Action(delegate
                    {
                        SafelyFacingTarget(Me.CurrentTarget);
                        CastSpell("Revealing Strike", Me.CurrentTarget, "RevealingStrike");
                    })
                );
        }

        #endregion

        #region Rupture

        private static Composite Rupture()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.Rupture &&
                SpellManager.HasSpell("Rupture") &&
                BurstCooldownEnergyPool < DateTime.Now &&
                !MeMounted &&
                CurrentTargetAttackable(5) &&
                !AmbushShadowDanceCheck() &&
                PlayerComboPoint >= THSettings.Instance.RuptureCP &&
                FacingOverrideMeCurrentTarget &&
                WorthyTarget(Me.CurrentTarget) &&
                MyAuraTimeLeft("Rupture", Me.CurrentTarget) < 2000 &&
                !Me.HasAura("Bloodlust") &&
                !Me.HasAura("Heroism") &&
                !Me.HasAura("Time Warp") &&
                !Me.HasAura("Blade Flurry") &&
                !IsStealthed(Me) &&
                !KidneyShotCheck() &&
                SpellManager.CanCast("Rupture"),
                new Action(delegate
                    {
                        //KidneyShotCheckLog();
                        CastSpell("Rupture", Me.CurrentTarget, "Rupture");
                    })
                );
        }

        #endregion

        #region Sap

        private static WoWUnit UnitSapNoCombat;

        private static bool GetUnitSapNoCombat()
        {
            UnitSapNoCombat = null;

            UnitSapNoCombat = NearbyUnFriendlyPlayers.FirstOrDefault(
                unit => unit != null && unit.IsValid &&
                        DistanceCheck(unit) <= 15 &&
                        FacingOverride(unit) &&
                        !unit.Combat &&
                        DisorientsLevelGet(unit) < THSettings.Instance.SapLevel &&
                        MyAuraTimeLeft(6770, unit) < 500 &&
                        !DebuffCCDuration(unit, 500) &&
                        !Invulnerable(unit) &&
                        unit.InLineOfSpellSight);

            //UnitSapNoCombat = (from unit in NearbyUnFriendlyPlayers
            //                   where unit != null &&
            //                         unit.IsValid &&
            //                         DistanceCheck(unit) <= 15 &&
            //                         FacingOverride(unit) &&
            //                         !unit.Combat &&
            //                         DisorientsLevelGet(unit) < THSettings.Instance.SapLevel &&
            //                         MyAuraTimeLeft(6770, unit) < 500 &&
            //                         !DebuffCCDuration(unit, 500) &&
            //                         !Invulnerable(unit) &&
            //                         unit.InLineOfSpellSight
            //                   orderby unit.Distance ascending
            //                   select unit).FirstOrDefault();

            //if (UnitSapNoCombat != null)
            //{
            //    Logging.Write("Found UnitSapNoCombat " + UnitSapNoCombat.Name);
            //}

            return UnitSapNoCombat != null;
        }

        private static WoWUnit UnitSapCC;

        private static bool GetUnitSapCC()
        {
            UnitSapCC = null;

            UnitSapCC = NearbyUnFriendlyPlayers.FirstOrDefault(
                unit => unit != null && unit.IsValid &&
                        DistanceCheck(unit) <= 10 &&
                        FacingOverride(unit) &&
                        !unit.Combat &&
                        DisorientsLevelGet(unit) < THSettings.Instance.SapLevel &&
                        MyAuraTimeLeft(6770, unit) < 500 &&
                        !DebuffCCDuration(unit, 500) &&
                        !Invulnerable(unit) &&
                        unit.InLineOfSpellSight);

            //UnitSapCC = (from unit in NearbyUnFriendlyPlayers
            //             where unit != null &&
            //                   unit.IsValid &&
            //                   DistanceCheck(unit) <= 10 &&
            //                   FacingOverride(unit) &&
            //                   !unit.Combat &&
            //                   DisorientsLevelGet(unit) < THSettings.Instance.SapLevel &&
            //                   MyAuraTimeLeft(6770, unit) < 500 &&
            //                   !DebuffCCDuration(unit, 500) &&
            //                   !Invulnerable(unit) &&
            //                   unit.InLineOfSpellSight
            //             orderby unit.Distance ascending
            //             select unit).FirstOrDefault();

            //if (UnitSapNoCombat != null)
            //{
            //    Logging.Write("Found UnitSapNoCombat " + UnitSapNoCombat.Name);
            //}

            return UnitSapCC != null;
        }

        private static WoWUnit MySappedUnit;

        private static bool GetMySappedUnit()
        {
            MySappedUnit = null;

            MySappedUnit = NearbyUnFriendlyPlayers.FirstOrDefault(
                unit => unit != null && unit.IsValid &&
                        MyAuraTimeLeft("Sap", unit) > 500);

            //MySappedUnit = (from unit in NearbyUnFriendlyPlayers
            //                where MyAuraTimeLeft("Sap", unit) > 500
            //                select unit).
            //    FirstOrDefault();

            //if (MySappedUnit != null)
            //{
            //    Logging.Write("Found MySappedUnit " + MySappedUnit.Name);
            //}
            return MySappedUnit != null;
        }

        private static Composite Sap()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.ShadowDanceSap &&
                    SpellManager.HasSpell("Sap") &&
                    SpellManager.HasSpell("Shadow Dance") &&
                    !MeMounted &&
                    !IsStealthed(Me) &&
                    Me.Combat &&
                    GetUnitSapCC() &&
                    UnitSapCC != null &&
                    UnitSapCC.IsValid &&
                    FacingOverride(UnitSapCC) &&
                    DistanceCheck(UnitSapCC) <= 10 &&
                    SpellManager.CanCast("Shadow Dance"),
                    new Action(delegate
                        {
                            //Logging.Write(LogLevel.Diagnostic, "Current DisorientsLevel DR Level {0}",
                            //              DisorientsLevelGet(UnitSapCC));

                            CastSpell("Shadow Dance", UnitSapCC, "ShadowDanceSap");
                            SafelyFacingTarget(UnitSapCC);
                            CastSpell("Sap", UnitSapCC, "UnitSapCC");
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.VanishSap &&
                    SpellManager.HasSpell("Sap") &&
                    SpellManager.HasSpell("Vanish") &&
                    !MeMounted &&
                    !IsStealthed(Me) &&
                    Me.Combat &&
                    GetUnitSapCC() &&
                    UnitSapCC != null &&
                    UnitSapCC.IsValid &&
                    FacingOverride(UnitSapCC) &&
                    DistanceCheck(UnitSapCC) <= 10 &&
                    SpellManager.CanCast("Vanish"),
                    new Action(delegate
                        {
                            //Logging.Write(LogLevel.Diagnostic, "Current DisorientsLevel DR Level {0}",
                            //              DisorientsLevelGet(UnitSapCC));

                            CastSpell("Vanish", UnitSapCC, "VanishSap");
                            SafelyFacingTarget(UnitSapCC);
                            CastSpell("Sap", UnitSapCC, "UnitSapCC");
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.SapRogueDruid &&
                    SpellManager.HasSpell("Sap") &&
                    !MeMounted &&
                    GetUnitUnitRogueDruidInStealth() &&
                    UnitRogueDruidInStealth != null &&
                    UnitRogueDruidInStealth.IsValid &&
                    FacingOverride(UnitRogueDruidInStealth) &&
                    DistanceCheck(UnitRogueDruidInStealth) <= 10 &&
                    SpellManager.CanCast("Sap"),
                    new Action(delegate
                        {
                            //Logging.Write(LogLevel.Diagnostic, "Current DisorientsLevel DR Level {0}",
                            //              DisorientsLevelGet(UnitRogueDruidInStealth));

                            SafelyFacingTarget(UnitRogueDruidInStealth);
                            CastSpell("Sap", UnitRogueDruidInStealth, "SapRogueDruid");
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.SapTarget &&
                    SpellManager.HasSpell("Sap") &&
                    !MeMounted &&
                    CurrentTargetAttackable(10) &&
                    !Me.CurrentTarget.Combat &&
                    FacingOverride(Me.CurrentTarget) &&
                    DisorientsLevelGet(Me.CurrentTarget) < THSettings.Instance.SapLevel &&
                    (Me.CurrentTarget.IsHumanoid ||
                     Me.CurrentTarget.IsBeast ||
                     Me.CurrentTarget.IsDemon ||
                     Me.CurrentTarget.IsDragon) &&
                    !Me.CurrentTarget.IsBoss &&
                    IsStealthed(Me) &&
                    !DebuffCCDuration(Me.CurrentTarget, 500) &&
                    //MyAuraTimeLeft(6770, Me.CurrentTarget) < 500 &&
                    //!GetMySappedUnit() &&
                    !DebuffCCDuration(Me.CurrentTarget, 500) &&
                    !Invulnerable(Me.CurrentTarget) &&
                    SpellManager.CanCast("Sap"),
                    new Action(delegate
                        {
                            //Logging.Write(LogLevel.Diagnostic, "Current DisorientsLevel DR Level {0}",
                            //              DisorientsLevelGet(Me.CurrentTarget));

                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Sap", Me.CurrentTarget, "SapTarget");
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.SapFocus &&
                    SpellManager.HasSpell("Sap") &&
                    !MeMounted &&
                    Me.FocusedUnit != null &&
                    Me.FocusedUnit.IsValid &&
                    !Me.FocusedUnit.Combat &&
                    FacingOverride(Me.FocusedUnit) &&
                    DistanceCheck(Me.FocusedUnit) <= 10 &&
                    DisorientsLevelGet(Me.FocusedUnit) < THSettings.Instance.SapLevel &&
                    Attackable(Me.FocusedUnit, 10) &&
                    (Me.FocusedUnit.IsHumanoid ||
                     Me.FocusedUnit.IsBeast ||
                     Me.FocusedUnit.IsDemon ||
                     Me.FocusedUnit.IsDragon) &&
                    !Me.FocusedUnit.IsBoss &&
                    IsStealthed(Me) &&
                    !DebuffCCDuration(Me.CurrentTarget, 500) &&
                    //MyAuraTimeLeft(6770, Me.FocusedUnit) < 500 &&
                    //!GetMySappedUnit() &&
                    !DebuffCCDuration(Me.FocusedUnit, 500) &&
                    !Invulnerable(Me.FocusedUnit) &&
                    SpellManager.CanCast("Sap"),
                    new Action(delegate
                        {
                            //Logging.Write(LogLevel.Diagnostic, "Current DisorientsLevel DR Level {0}",
                            //              DisorientsLevelGet(Me.FocusedUnit));

                            SafelyFacingTarget(Me.FocusedUnit);
                            CastSpell("Sap", Me.FocusedUnit, "SapFocus");
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.SapAny &&
                    SpellManager.HasSpell("Sap") &&
                    !MeMounted &&
                    !GetMySappedUnit() &&
                    GetUnitSapNoCombat() &&
                    UnitSapNoCombat != null &&
                    UnitSapNoCombat.IsValid &&
                    SpellManager.CanCast("Sap"),
                    new Action(delegate
                        {
                            //Logging.Write(LogLevel.Diagnostic, "Current DisorientsLevel DR Level {0}",
                            //              DisorientsLevelGet(UnitSapNoCombat));

                            SafelyFacingTarget(UnitSapNoCombat);
                            CastSpell("Sap", UnitSapNoCombat, "SapAny");
                        }))
                );
        }

        #endregion

        #region ShadowWalk

        private static Composite ShadowWalk()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.ShadowWalk &&
                SpellManager.HasSpell("Shadow Walk") &&
                !MeMounted &&
                IsStealthed(Me) &&
                GetUnitUnitRogueDruidInStealth() &&
                UnitRogueDruidInStealth != null &&
                UnitRogueDruidInStealth.IsValid &&
                UnitRogueDruidInStealth.Distance < 12 &&
                SpellManager.CanCast("Shadow Walk"),
                new Action(delegate { CastSpell("Shadow Walk", Me, "ShadowWalk"); })
                );
        }

        #endregion

        #region ShadowBlades

        private static Composite ShadowBlades()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.ShadowBlades &&
                SpellManager.HasSpell("Shadow Blades") &&
                (THSettings.Instance.ShadowBladesCD ||
                 THSettings.Instance.ShadowBladesBurst &&
                 Burst ||
                 THSettings.Instance.ShadowBladeSync &&
                 (Me.HasAura("Shadow Dance") ||
                  Me.HasAura("Killing Spree") ||
                  MyAura("Vendetta", Me.CurrentTarget))) &&
                !MeMounted &&
                Me.Combat &&
                Me.HasAura("Slice and Dice") &&
                CurrentTargetAttackable(5) &&
                WorthyTarget(Me.CurrentTarget) &&
                SafeUsingCooldown(Me.CurrentTarget) &&
                SpellManager.CanCast("Shadow Blades"),
                new Action(delegate
                    {
                        CastSpell("Shadow Blades", Me, "ShadowBlades");
                        return RunStatus.Failure;
                    })
                );
        }

        #endregion

        #region ShadowDance

        private static DateTime BurstCooldownTimer;
        private static DateTime BurstCooldownEnergyPool;

        private static Composite ShadowDance()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.ShadowDance &&
                SpellManager.HasSpell("Shadow Dance") &&
                (THSettings.Instance.ShadowDanceCD ||
                 THSettings.Instance.ShadowDanceBurst &&
                 Burst) &&
                !MeMounted &&
                Me.Combat &&
                !IsStealthed(Me) &&
                CurrentTargetAttackable(30) &&
                Me.IsBehind(Me.CurrentTarget) &&
                WorthyTarget(Me.CurrentTarget, 0.5) &&
                !DebuffCC(Me.CurrentTarget) &&
                SpellManager.CanCast("Shadow Dance"),
                new Action(delegate
                    {
                        if (PlayerEnergy < THSettings.Instance.ShadowDanceEnergy)
                        {
                            BurstCooldownEnergyPool = DateTime.Now + TimeSpan.FromMilliseconds(3000);
                        }
                        else
                        {
                            LastCastStealth = DateTime.Now;
                            BurstCooldownTimer = DateTime.Now;
                            CastSpell("Shadow Dance", Me, "ShadowDance");
                        }
                        return RunStatus.Failure;
                    })
                );
        }

        #endregion

        #region Shadowstep

        private static Composite Shadowstep()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.Shadowstep &&
                SpellManager.HasSpell("Shadowstep") &&
                !MeMounted &&
                CurrentTargetAttackable(25) &&
                Me.CurrentTarget.Distance - Me.CurrentTarget.CombatReach >=
                THSettings.Instance.ShadowstepDistance &&
                //FacingOverrideMeCurrentTarget &&
                SpellManager.CanCast("Shadowstep"),
                new Action(delegate
                    {
                        SafelyFacingTarget(Me.CurrentTarget);
                        CastSpell("Shadowstep", Me.CurrentTarget, "Shadowstep");
                        return RunStatus.Failure;
                    })
                );
        }

        #endregion

        #region ShurikenToss

        private static Composite ShurikenToss()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.AttackOOC || Me.Combat) &&
                THSettings.Instance.ShurikenToss &&
                SpellManager.HasSpell("Shuriken Toss") &&
                !MeMounted &&
                Me.Combat &&
                !IsStealthedNoSD(Me) &&
                PlayerComboPoint < MaxComboPoint &&
                CurrentTargetAttackable(30) &&
                FacingOverrideMeCurrentTarget &&
                !IsStealthed(Me) &&
                (Me.CurrentTarget.Distance >=
                 THSettings.Instance.ShurikenTossDistance ||
                 !Me.CurrentTarget.IsWithinMeleeRange) &&
                //!Me.CurrentTarget.IsWithinMeleeRange &&
                //(DebuffRoot(Me) ||
                // Me.IsMoving &&
                // IsOverrideModeOn)) &&
                SpellManager.CanCast("Shuriken Toss"),
                new Action(delegate
                    {
                        SafelyFacingTarget(Me.CurrentTarget);
                        CastSpell("Shuriken Toss", Me.CurrentTarget, "ShurikenToss");
                    })
                );
        }

        #endregion

        #region Shiv

        private static WoWUnit UnitShiv;

        private static bool GetUnitShiv()
        {
            UnitShiv = null;

            if (UnitShiv == null &&
                THSettings.Instance.ShivEnrage)
            {
                UnitShiv = NearbyUnFriendlyPlayers.FirstOrDefault(
                    unit => unit != null && unit.IsValid &&
                            unit.IsWithinMeleeRange &&
                            FacingOverride(unit) &&
                            BuffEnrage(unit));

                //UnitShiv = (from unit in NearbyUnFriendlyPlayers
                //            where unit != null &&
                //                  unit.IsValid &&
                //                  unit.IsWithinMeleeRange &&
                //                  FacingOverride(unit) &&
                //                  BuffEnrage(unit)
                //            orderby unit.HealthPercent ascending
                //            select unit).FirstOrDefault();
            }

            if (UnitShiv == null &&
                THSettings.Instance.ShivCripplingPoison &&
                Me.HasAura("Crippling Poison"))
            {
                UnitShiv = NearbyUnFriendlyPlayers.FirstOrDefault(
                    unit => unit != null && unit.IsValid &&
                            Me.CurrentTarget != null &&
                            unit != Me.CurrentTarget &&
                            unit.IsWithinMeleeRange &&
                            FacingOverride(unit) &&
                            !DebuffCC(unit) &&
                            !DebuffRootorSnare(unit));

                //UnitShiv = (from unit in NearbyUnFriendlyPlayers
                //            where unit != null &&
                //                  unit.IsValid &&
                //                  Me.CurrentTarget != null &&
                //                  unit != Me.CurrentTarget &&
                //                  unit.IsWithinMeleeRange &&
                //                  FacingOverride(unit) &&
                //                  !DebuffCC(unit) &&
                //                  !DebuffRootorSnare(unit)
                //            orderby unit.HealthPercent ascending
                //            select unit).FirstOrDefault();
            }

            if (UnitShiv == null &&
                THSettings.Instance.ShivLeechingPoison &&
                Me.HasAura("Leeching Poison") &&
                Me.HealthPercent <= THSettings.Instance.ShivLeechingPoisonHP &&
                CurrentTargetAttackable(5))
            {
                UnitShiv = Me.CurrentTarget;
            }

            if (UnitShiv == null &&
                THSettings.Instance.ShivMindNumbing &&
                Me.HasAura("Mind-numbing Poison"))
            {
                UnitShiv = NearbyUnFriendlyPlayers.FirstOrDefault(
                    unit => unit != null && unit.IsValid &&
                            unit.IsWithinMeleeRange &&
                            FacingOverride(unit) &&
                            !unit.HasAura("Mind Paralysis") &&
                            TalentSort(unit) >= 3 &&
                            !DebuffCC(unit) &&
                            !DebuffSilence(unit));

                //UnitShiv = (from unit in NearbyUnFriendlyPlayers
                //            where unit != null &&
                //                  unit.IsValid &&
                //                  unit.IsWithinMeleeRange &&
                //                  FacingOverride(unit) &&
                //                  !unit.HasAura("Mind Paralysis") &&
                //                  TalentSort(unit) >= 3 &&
                //                  !DebuffCC(unit) &&
                //                  !DebuffSilence(unit)
                //            orderby
                //                unit.HealthPercent ascending
                //            select unit).
                //    FirstOrDefault();
            }

            if (UnitShiv == null &&
                THSettings.Instance.ShivParalystic &&
                Me.HasAura("Paralytic Poison"))
            {
                UnitShiv = NearbyUnFriendlyPlayers.FirstOrDefault(
                    unit => unit != null && unit.IsValid &&
                            unit.IsWithinMeleeRange &&
                            unit.IsMoving &&
                            FacingOverride(unit) &&
                            !DebuffRoot(unit) &&
                            !InvulnerableRootandSnare(unit) &&
                            !DebuffCC(unit));

                //UnitShiv = (from unit in NearbyUnFriendlyPlayers
                //            where unit != null &&
                //                  unit.IsValid &&
                //                  unit.IsWithinMeleeRange &&
                //                  FacingOverride(unit) &&
                //                  !DebuffRoot(unit) &&
                //                  !InvulnerableRootandSnare(unit) &&
                //                  //(Me.CurrentTarget != null &&
                //                  // unit == Me.CurrentTarget ||
                //                  // TalentSort(unit) == 1 &&
                //                  // unit.CurrentTarget != null &&
                //                  // RaidPartyMembers.Contains(unit.CurrentTarget) &&
                //                  // unit.CurrentTarget.HealthPercent <= THSettings.Instance.ShivParalysticHP &&
                //                  // (unit.CurrentTarget != Me ||
                //                  //  unit.CurrentTarget == Me &&
                //                  //  unit.HealthPercent > Me.HealthPercent)) &&
                //                  !DebuffCC(unit)
                //            orderby unit.HealthPercent ascending
                //            select unit).FirstOrDefault();
            }
            return UnitShiv != null;
        }

        private static Composite Shiv()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.Shiv &&
                    SpellManager.HasSpell("Shiv") &&
                    !MeMounted &&
                    Me.Combat &&
                    !IsStealthed(Me) &&
                    GetUnitShiv() &&
                    UnitShiv != null &&
                    UnitShiv.IsValid &&
                    SpellManager.CanCast("Shiv"),
                    new Action(delegate
                        {
                            SafelyFacingTarget(UnitShiv);
                            CastSpell("Shiv", UnitShiv, "Shiv");
                        }))
                );
        }

        #endregion

        #region SinisterStrike

        private static Composite SinisterStrike()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.AttackOOC || Me.Combat) &&
                //THSettings.Instance.SinisterStrike &&
                BurstCooldownEnergyPool + TimeSpan.FromMilliseconds(5000) < DateTime.Now &&
                !SpellManager.HasSpell("Hemorrhage") &&
                !SpellManager.HasSpell("Dispatch") &&
                SpellManager.HasSpell("Sinister Strike") &&
                !MeMounted &&
                (PlayerComboPoint < MaxComboPoint ||
                 PlayerEnergy >= Me.MaxEnergy) &&
                CurrentTargetAttackable(5) &&
                PoolEnergyPassCheck() &&
                FacingOverrideMeCurrentTarget &&
                //!AmbushCheck() &&
                SpellManager.CanCast("Sinister Strike"),
                new Action(delegate
                    {
                        SafelyFacingTarget(Me.CurrentTarget);
                        CastSpell("Sinister Strike", Me.CurrentTarget, "SinisterStrike");
                    })
                );
        }

        private static Composite SinisterStrikeCombat()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.AttackOOC || Me.Combat) &&
                //THSettings.Instance.SinisterStrike &&
                BurstCooldownEnergyPool + TimeSpan.FromMilliseconds(5000) < DateTime.Now &&
                SpellManager.HasSpell("Sinister Strike") &&
                !MeMounted &&
                PlayerComboPoint < MaxComboPoint &&
                CurrentTargetAttackable(5) &&
                PoolEnergyPassCheck() &&
                FacingOverrideMeCurrentTarget &&
                //!AmbushCheck() &&
                SpellManager.CanCast("Sinister Strike"),
                new Action(delegate
                    {
                        SafelyFacingTarget(Me.CurrentTarget);
                        CastSpell("Sinister Strike", Me.CurrentTarget, "SinisterStrikeCombat");
                    })
                );
        }

        #endregion

        #region SliceandDice

        private static Composite SliceandDice()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.SliceandDice &&
                //(THSettings.Instance.AttackOOC || Me.Combat) &&
                SpellManager.HasSpell("Slice and Dice") &&
                !MeMounted &&
                !AmbushShadowDanceCheck() &&
                !KidneyShotCheck() &&
                (PlayerComboPoint >= THSettings.Instance.SliceandDiceCP ||
                 Me.RawComboPoints >= THSettings.Instance.SliceandDiceCP) &&
                (!THSettings.Instance.Recuperate ||
                 THSettings.Instance.Recuperate &&
                 Me.HasAura("Recuperate") ||
                 THSettings.Instance.Recuperate &&
                 Me.HealthPercent >= THSettings.Instance.RecuperateHP) &&
                MyAuraTimeLeft("Slice and Dice", Me) < 2000 &&
                (CurrentTargetAttackable(40) &&
                 Me.IsMoving || CurrentTargetAttackable(10)) &&
                FacingOverride(Me.CurrentTarget) &&
                SpellManager.CanCast("Slice and Dice"),
                new Action(delegate
                    {
                        //KidneyShotCheckLog();
                        CastSpell("Slice and Dice", Me, "SliceandDice");
                    })
                );
        }

        #endregion

        #region Sprint

        private static Composite Sprint()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.Sprint &&
                    SpellManager.HasSpell("Sprint") &&
                    !MeMounted &&
                    Me.Combat &&
                    CurrentTargetAttackable(40) &&
                    Me.CurrentTarget.Distance - Me.CurrentTarget.CombatReach >=
                    THSettings.Instance.SprintDistance &&
                    FacingOverrideMeCurrentTarget &&
                    !DebuffRoot(Me) &&
                    SpellManager.CanCast("Sprint"),
                    new Action(delegate
                        {
                            CastSpell("Sprint", Me, "Sprint");
                            return RunStatus.Failure;
                        }))
                );
        }

        #endregion

        #region Stealth

        private static Composite Stealth()
        {
            return new PrioritySelector(
                //new Action(delegate
                //    {
                //        Logging.Write("MeMounted " + MeMounted);
                //        Logging.Write("MeMounted " + Me.Combat);
                //        Logging.Write("Me.IsMoving " + Me.IsMoving);
                //        Logging.Write("Me.HasAura(Stealth) " + Me.HasAura("Stealth"));
                //        Logging.Write("DebuffDot(Me) " + DebuffDot(Me));
                //        Logging.Write("SpellManager.CanCast(Stealth) " + SpellManager.CanCast("Stealth"));
                //        return RunStatus.Failure;
                //    }),
                new Decorator(
                    ret =>
                    THSettings.Instance.Stealth &&
                    SpellManager.HasSpell("Stealth") &&
                    (THSettings.Instance.StealthForce ||
                     InInstance ||
                     InArena ||
                     InBattleground ||
                     InDungeon ||
                     InRaid ||
                     Me.IsMoving ||
                     Me.HasAura("Food") ||
                     CurrentTargetAttackable(15) &&
                     CurrentTargetCheckFacing) &&
                    !Me.Mounted &&
                    !Me.Combat &&
                    !Me.IsCasting &&
                    !Me.IsChanneling &&
                    !Me.HasAura("Stealth") &&
                    !Me.HasAura("Vanish") &&
                    !Me.HasAura("Alliance Flag") &&
                    !Me.HasAura("Horde Flag") &&
                    !Me.HasAura("Netherstorm Flag") &&
                    !Me.HasAura("Orb of Power") &&
                    (THSettings.Instance.StealthForce &&
                     !Me.HasAura("Flare") &&
                     !Me.HasAura("Faerie Fire") &&
                     !Me.HasAura("Faerie Swarm") ||
                     !DebuffDot(Me)) &&
                    //!IsStealthedNoSD(Me) &&
                    (SpellManager.CanCast("Stealth")),
                    new Action(delegate
                        {
                            CastSpell("Stealth", Me, "Stealth");
                            LastCastStealth = DateTime.Now;
                            return RunStatus.Failure;
                        }))
                );
        }

        private static void StealthVoid()
        {
            if (THSettings.Instance.Stealth &&
                SpellManager.HasSpell("Stealth") &&
                !MeMounted &&
                !Me.Combat &&
                !Me.IsCasting &&
                !Me.IsChanneling &&
                !IsStealthed(Me) &&
                (THSettings.Instance.StealthForce &&
                 !Me.HasAura("Faerie Fire") &&
                 !Me.HasAura("Faerie Swarm") ||
                 !DebuffDot(Me)) &&
                SpellManager.CanCast("Stealth"))

            {
                CastSpell("Stealth", Me, "Stealth");
                LastCastStealth = DateTime.Now;
            }
        }

        #endregion

        #region Throw

        /// <summary>
        /// determines if a target is off the ground far enough that you can
        /// reach it with melee spells if standing directly under.
        /// </summary>
        /// <param name="u">unit</param>
        /// <returns>true if above melee reach</returns>
        public static bool IsAboveTheGround(WoWUnit unit)
        {
            float height = HeightOffTheGround(unit);
            if (height == float.MaxValue)
                return false; // make this true if better to assume aerial 

            if (height >= unit.Distance2D)
                return true;

            return false;
        }

        /// <summary>
        /// calculate a unit's vertical distance (height) above ground level (mesh).  this is the units position
        /// relative to the ground and is independent of any other character.  
        /// </summary>
        /// <param name="u">unit</param>
        /// <returns>float.MinValue if can't determine, otherwise distance off ground</returns>
        public static float HeightOffTheGround(WoWUnit unit)
        {
            var unitLoc = new WoWPoint(unit.Location.X, unit.Location.Y, unit.Location.Z);
            var listMeshZ = Navigator.FindHeights(unitLoc.X, unitLoc.Y).Where(h => h <= unitLoc.Z);
            if (listMeshZ.Any())
                return unitLoc.Z - listMeshZ.Max();

            return float.MaxValue;
        }

        private static Composite Throw()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    (THSettings.Instance.AttackOOC || Me.Combat) &&
                    THSettings.Instance.Throw &&
                    SpellManager.HasSpell("Throw") &&
                    !MeMounted &&
                    CurrentTargetAttackable(30) &&
                    !Me.CurrentTarget.IsWithinMeleeRange &&
                    FacingOverrideMeCurrentTarget &&
                    !Me.IsInInstance &&
                    IsAboveTheGround(Me.CurrentTarget) &&
                    SpellManager.CanCast("Throw"),
                    new Action(delegate
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Throw", Me.CurrentTarget, "Throw");
                        }))
                );
        }

        #endregion

        #region TricksoftheTrade

        private static WoWUnit UnitTricksoftheTradeAny;

        private static bool GetTricksoftheTradeAny()
        {
            UnitTricksoftheTradeAny = null;

            UnitTricksoftheTradeAny = RaidPartyMembers.FirstOrDefault(
                unit => unit != null && unit.IsValid &&
                        unit != Me &&
                        //unit.IsPlayer &&
                        unit.Distance < 30 &&
                        !unit.HasAura("Tricks of the Trade") &&
                        TalentSort(unit) < 4 &&
                        unit.Combat &&
                        unit.CurrentTarget != null &&
                        IsEnemy(unit.CurrentTarget) &&
                        unit.Location.Distance(unit.CurrentTarget.Location) < 40 &&
                        unit.InLineOfSpellSight
                );

            //UnitTricksoftheTradeAny = (from unit in RaidPartyMembers
            //                           where unit != null &&
            //                                 unit.IsValid &&
            //                                 unit != Me &&
            //                                 unit.IsPlayer &&
            //                                 unit.Distance < 30 &&
            //                                 !unit.HasAura("Tricks of the Trade") &&
            //                                 TalentSort(unit) < 4 &&
            //                                 unit.Combat &&
            //                                 unit.CurrentTarget != null &&
            //                                 IsEnemy(unit.CurrentTarget) &&
            //                                 unit.Location.Distance(unit.CurrentTarget.Location) < 40 &&
            //                                 unit.InLineOfSpellSight
            //                           orderby unit.AttackPower descending
            //                           select unit).FirstOrDefault();

            return UnitTricksoftheTradeAny != null;
        }

        private static Composite TricksoftheTrade()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.TricksoftheTrade &&
                    THSettings.Instance.TricksoftheTradeFocus &&
                    SpellManager.HasSpell("Tricks of the Trade") &&
                    !MeMounted &&
                    CurrentTargetAttackable(40) &&
                    Me.FocusedUnit != null &&
                    Me.FocusedUnit.Distance < 100 &&
                    Me.FocusedUnit != Me &&
                    Me.FocusedUnit.IsPlayer &&
                    !Me.FocusedUnit.HasAura("Tricks of the Trade") &&
                    RaidPartyMembers.Contains(Me.FocusedUnit) &&
                    Me.FocusedUnit.Combat &&
                    IsEnemy(Me.FocusedUnit.CurrentTarget) &&
                    Me.FocusedUnit.Location.Distance(Me.FocusedUnit.CurrentTarget.Location) <= 40 &&
                    Me.FocusedUnit.InLineOfSpellSight &&
                    SpellManager.CanCast("Tricks of the Trade"),
                    new Action(delegate
                        {
                            CastSpell("Tricks of the Trade", Me.FocusedUnit, "TricksoftheTradeFocus");
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.TricksoftheTrade &&
                    THSettings.Instance.TricksoftheTradeAny &&
                    SpellManager.HasSpell("Tricks of the Trade") &&
                    !MeMounted &&
                    CurrentTargetAttackable(40) &&
                    GetTricksoftheTradeAny() &&
                    UnitTricksoftheTradeAny != null &&
                    UnitTricksoftheTradeAny.IsValid &&
                    SpellManager.CanCast("Tricks of the Trade"),
                    new Action(delegate
                        {
                            CastSpell("Tricks of the Trade", UnitTricksoftheTradeAny, "TricksoftheTradeAny");
                            return RunStatus.Failure;
                        }))
                )
                ;
        }

        #endregion

        #region UseRacial

        private static DateTime BreakCCLast;

        private static Composite UseRacial()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    Me.Combat &&
                    THSettings.Instance.AutoRacial &&
                    BreakCCLast + TimeSpan.FromMilliseconds(2000) < DateTime.Now &&
                    DebuffCCDuration(Me, 3000) &&
                    !Me.HasAura("Sap") &&
                    SpellManager.HasSpell("Every Man for Himself") &&
                    SpellManager.CanCast("Every Man for Himself"),
                    new Action(delegate
                        {
                            if (THSettings.Instance.AutoTarget && Me.CurrentTarget == null &&
                                ValidUnit(MyLastTarget))
                            {
                                MyLastTarget.Target();
                            }

                            if (DebuffCCBreakonDamage(Me) &&
                                DebuffDot(Me))
                            {
                                Logging.Write("Waiting for Dot / Hand of Sacrifice break it!");
                            }
                            else
                            {
                                BreakCCLast = DateTime.Now;
                                Logging.Write("Use: Every Man for Himself");
                                CastSpell("Every Man for Himself", Me);
                            }
                            return RunStatus.Failure;
                        })),
                //Stoneform
                new Decorator(
                    ret =>
                    Me.Combat &&
                    THSettings.Instance.AutoRacial && Me.HealthPercent < THSettings.Instance.UrgentHeal &&
                    SpellManager.HasSpell("Stoneform") &&
                    SpellManager.CanCast("Stoneform"),
                    new Action(delegate
                        {
                            {
                                Logging.Write("Stoneform");
                                CastSpell("Stoneform", Me);
                            }
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    Me.Combat &&
                    THSettings.Instance.AutoRacial && Me.HealthPercent < THSettings.Instance.UrgentHeal &&
                    SpellManager.HasSpell("Gift of the Naaru") &&
                    SpellManager.CanCast("Gift of the Naaru"),
                    new Action(delegate
                        {
                            {
                                Logging.Write("Gift of the Naaru");
                                CastSpell("Gift of the Naaru", Me);
                            }
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    Me.Combat &&
                    THSettings.Instance.AutoRacial && Me.ManaPercent < THSettings.Instance.PriorityHeal &&
                    SpellManager.HasSpell("Arcane Torrent") && SpellManager.HasSpell("Holy Insight") &&
                    (InDungeon || InRaid) &&
                    SpellManager.CanCast("Arcane Torrent"),
                    new Action(delegate
                        {
                            {
                                Logging.Write("Arcane Torrent");
                                CastSpell("Arcane Torrent", Me);
                            }
                            return RunStatus.Failure;
                        }))
                );
        }

        #endregion

        #region UseTrinket

        private static Composite UseTrinket()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket1 == 1 &&
                    Me.Combat &&
                    CurrentTargetAttackable(10) &&
                    FacingOverrideMeCurrentTarget &&
                    Me.Inventory.Equipped.Trinket1 != null &&
                    CanUseEquippedItem(Me.Inventory.Equipped.Trinket1),
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                          " on Cooldown");
                            StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket1 == 2 &&
                    Me.Combat &&
                    CurrentTargetAttackable(10) &&
                    Me.CurrentTarget.IsBoss &&
                    FacingOverrideMeCurrentTarget &&
                    Me.Inventory.Equipped.Trinket1 != null &&
                    CanUseEquippedItem(Me.Inventory.Equipped.Trinket1),
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                          " on Cooldown (Boss Only)");
                            StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket1 == 3 &&
                    Me.Combat && CurrentTargetAttackable(15) &&
                    Me.Inventory.Equipped.Trinket1 != null &&
                    CanUseEquippedItem(Me.Inventory.Equipped.Trinket1) &&
                    Burst,
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                          " on Burst Mode");
                            StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket1 == 4 &&
                    BreakCCLast + TimeSpan.FromMilliseconds(2000) < DateTime.Now &&
                    Me.Combat &&
                    Me.Inventory.Equipped.Trinket1 != null &&
                    CanUseEquippedItem(Me.Inventory.Equipped.Trinket1) &&
                    DebuffCCDuration(Me, 3000) && !Me.HasAura("Sap"),
                    new Action(delegate
                        {
                            BreakCCLast = DateTime.Now;
                            StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                          " on Lose Control");
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket1 == 5 &&
                    Me.Combat &&
                    Me.Inventory.Equipped.Trinket1 != null &&
                    CanUseEquippedItem(Me.Inventory.Equipped.Trinket1) &&
                    Me.HealthPercent < THSettings.Instance.Trinket1HP,
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                          " on Low HP");
                            StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket1 == 6 &&
                    Me.Combat &&
                    Me.Inventory.Equipped.Trinket1 != null &&
                    CanUseEquippedItem(Me.Inventory.Equipped.Trinket1) &&
                    Me.CurrentTarget != null && Me.CurrentTarget.IsValid &&
                    IsEnemy(Me.CurrentTarget) && !Me.CurrentTarget.IsPet &&
                    Me.CurrentTarget.Distance < 10 &&
                    Me.CurrentTarget.HealthPercent < THSettings.Instance.Trinket1HP,
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                          " on Enemy Unit Low HP");
                            StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                            return RunStatus.Failure;
                        })),
                //Trinket 2
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket2 == 1 &&
                    Me.Combat &&
                    CurrentTargetAttackable(10) &&
                    FacingOverrideMeCurrentTarget &&
                    Me.Inventory.Equipped.Trinket2 != null &&
                    CanUseEquippedItem(Me.Inventory.Equipped.Trinket2),
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                          " on Cooldown");
                            StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket2 == 2 &&
                    Me.Combat &&
                    CurrentTargetAttackable(10) &&
                    Me.CurrentTarget.IsBoss &&
                    FacingOverrideMeCurrentTarget &&
                    Me.Inventory.Equipped.Trinket2 != null &&
                    CanUseEquippedItem(Me.Inventory.Equipped.Trinket2),
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                          " on Cooldown (Boss Only)");
                            StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket2 == 3 &&
                    Me.Combat && CurrentTargetAttackable(15) &&
                    Me.Inventory.Equipped.Trinket2 != null &&
                    CanUseEquippedItem(Me.Inventory.Equipped.Trinket2) &&
                    Burst,
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                          " on Burst Mode");
                            StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket2 == 4 &&
                    BreakCCLast + TimeSpan.FromMilliseconds(2000) < DateTime.Now &&
                    Me.Combat &&
                    Me.Inventory.Equipped.Trinket2 != null &&
                    CanUseEquippedItem(Me.Inventory.Equipped.Trinket2) &&
                    DebuffCCDuration(Me, 3000) && !Me.HasAura("Sap"),
                    new Action(delegate
                        {
                            BreakCCLast = DateTime.Now;
                            StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                          " on Lose Control");
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket2 == 5 &&
                    Me.Combat &&
                    Me.Inventory.Equipped.Trinket2 != null &&
                    CanUseEquippedItem(Me.Inventory.Equipped.Trinket2) &&
                    Me.HealthPercent < THSettings.Instance.Trinket2HP,
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                          " on Low HP");
                            StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket2 == 6 &&
                    Me.Combat &&
                    Me.Inventory.Equipped.Trinket2 != null &&
                    CanUseEquippedItem(Me.Inventory.Equipped.Trinket2) &&
                    Me.CurrentTarget != null && Me.CurrentTarget.IsValid &&
                    IsEnemy(Me.CurrentTarget) && !Me.CurrentTarget.IsPet &&
                    Me.CurrentTarget.Distance < 10 &&
                    Me.CurrentTarget.HealthPercent < THSettings.Instance.Trinket2HP,
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                          " on Enemy Unit Low HP");
                            StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                            return RunStatus.Failure;
                        }))
                );
        }

        #endregion

        #region UseProfession

        private static Composite UseProfession()
        {
            return new PrioritySelector(
                //Engineering
                new Decorator(
                    ret =>
                    THSettings.Instance.ProfBuff == 1 &&
                    Me.Combat &&
                    CurrentTargetAttackable(10) &&
                    FacingOverrideMeCurrentTarget &&
                    (Me.GetAuraById(51271) != null || //Pillar of Frost
                     Me.HasAura("Unholy Frenzy")) &&
                    Me.Inventory.Equipped.Hands != null &&
                    CanUseEquippedItem(Me.Inventory.Equipped.Hands),
                    new Action(delegate
                        {
                            Logging.Write("Use: Gloves Buff Activated on Cooldown");
                            StyxWoW.Me.Inventory.Equipped.Hands.Use();
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.ProfBuff == 2 &&
                    Me.Combat &&
                    CurrentTargetAttackable(10) &&
                    Me.CurrentTarget.IsBoss &&
                    FacingOverrideMeCurrentTarget &&
                    (Me.GetAuraById(51271) != null || //Pillar of Frost
                     Me.HasAura("Unholy Frenzy")) &&
                    Me.Inventory.Equipped.Hands != null &&
                    CanUseEquippedItem(Me.Inventory.Equipped.Hands),
                    new Action(delegate
                        {
                            Logging.Write("Use: Gloves Buff Activated on Cooldown (Boss Only)");
                            StyxWoW.Me.Inventory.Equipped.Hands.Use();
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.ProfBuff == 3 &&
                    Burst &&
                    Me.Combat && CurrentTargetAttackable(15) &&
                    Me.Inventory.Equipped.Hands != null &&
                    CanUseEquippedItem(Me.Inventory.Equipped.Hands),
                    new Action(delegate
                        {
                            Logging.Write("Use: Gloves Buff Activated on Burst Mode");
                            StyxWoW.Me.Inventory.Equipped.Hands.Use();
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.ProfBuff == 4 &&
                    Me.Combat &&
                    CurrentTargetAttackable(10) &&
                    CanUseEquippedItem(Me.Inventory.Equipped.Hands) &&
                    DebuffCCDuration(Me, 3000),
                    new Action(delegate
                        {
                            Logging.Write("Use: Gloves Buff Activated on Lose Control");
                            StyxWoW.Me.Inventory.Equipped.Hands.Use();
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.ProfBuff == 5 &&
                    Me.Combat &&
                    Me.Inventory.Equipped.Hands != null &&
                    CurrentTargetAttackable(10) &&
                    Me.HealthPercent < THSettings.Instance.ProfBuffHP &&
                    CanUseEquippedItem(Me.Inventory.Equipped.Hands),
                    new Action(delegate
                        {
                            Logging.Write("Use: GLoves Buff Activated on Low HP");
                            StyxWoW.Me.Inventory.Equipped.Hands.Use();
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.ProfBuff == 6 &&
                    Me.Combat &&
                    Me.Inventory.Equipped.Hands != null &&
                    CurrentTargetAttackable(10) &&
                    !Me.CurrentTarget.IsPet &&
                    Me.CurrentTarget.HealthPercent < THSettings.Instance.ProfBuffHP &&
                    CanUseEquippedItem(Me.Inventory.Equipped.Hands),
                    new Action(delegate
                        {
                            Logging.Write("Use: Gloves Buff Activated on Enemy Unit Low HP");
                            StyxWoW.Me.Inventory.Equipped.Hands.Use();
                            return RunStatus.Failure;
                        })),
                //Herbalism
                new Decorator(
                    ret =>
                    THSettings.Instance.ProfBuff == 1 &&
                    Me.Combat &&
                    CurrentTargetAttackable(10) &&
                    (Me.GetAuraById(51271) != null || //Pillar of Frost
                     Me.HasAura("Unholy Frenzy")) &&
                    SpellManager.HasSpell("Lifeblood") &&
                    SpellManager.CanCast("Lifeblood"),
                    new Action(delegate
                        {
                            Logging.Write("Use: Lifeblood Activated on Cooldown");
                            CastSpell("Lifeblood", Me);
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.ProfBuff == 2 &&
                    Me.Combat &&
                    CurrentTargetAttackable(10) &&
                    Me.GetAuraById(51271) != null && //Pillar of Frost
                    SpellManager.HasSpell("Lifeblood") &&
                    SpellManager.CanCast("Lifeblood"),
                    new Action(delegate
                        {
                            Logging.Write("Use: Lifeblood Activated on Cooldown (Boss Only)");
                            CastSpell("Lifeblood", Me);
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.ProfBuff == 3 &&
                    Burst &&
                    Me.Combat && CurrentTargetAttackable(15) &&
                    SpellManager.HasSpell("Lifeblood") &&
                    SpellManager.CanCast("Lifeblood"),
                    new Action(delegate
                        {
                            Logging.Write("Use: Lifeblood Activated on Burst Mode");
                            CastSpell("Lifeblood", Me);
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.ProfBuff == 4 &&
                    Me.Combat &&
                    SpellManager.HasSpell("Lifeblood") &&
                    SpellManager.CanCast("Lifeblood") &&
                    DebuffCCDuration(Me, 3000),
                    new Action(delegate
                        {
                            Logging.Write("Use: Lifeblood Activated on Lose Control");
                            CastSpell("Lifeblood", Me);
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.ProfBuff == 5 &&
                    Me.Combat &&
                    SpellManager.HasSpell("Lifeblood") &&
                    SpellManager.CanCast("Lifeblood") &&
                    Me.HealthPercent < THSettings.Instance.ProfBuffHP,
                    new Action(delegate
                        {
                            Logging.Write("Use: Lifeblood Activated on Low HP");
                            CastSpell("Lifeblood", Me);
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.ProfBuff == 6 &&
                    Me.Combat &&
                    SpellManager.HasSpell("Lifeblood") &&
                    SpellManager.CanCast("Lifeblood") &&
                    Me.CurrentTarget != null && Me.CurrentTarget.IsValid &&
                    IsEnemy(Me.CurrentTarget) && !Me.CurrentTarget.IsPet &&
                    Me.CurrentTarget.IsWithinMeleeRange &&
                    Me.CurrentTarget.HealthPercent < THSettings.Instance.ProfBuffHP,
                    new Action(delegate
                        {
                            Logging.Write("Use: Lifeblood Activated on Enemy Unit Low HP");
                            CastSpell("Lifeblood", Me, "Use: Lifeblood Activated on Enemy Unit Low HP");
                            return RunStatus.Failure;
                        }))
                );
        }

        #endregion

        #region Vanish

        private static readonly HashSet<WoWPoint> SafeWoWPointMove = new HashSet<WoWPoint>();

        private static double GetClosestEnemyDistance(WoWPoint SafePoint)
        {
            var closestEnemy = NearbyUnFriendlyUnits.OrderBy(unit => unit.Location.Distance(SafePoint)).FirstOrDefault();

            //var ClosestEnemy = (from unit in NearbyUnFriendlyUnits
            //                    where unit != null &&
            //                          unit.IsValid
            //                    orderby unit.Location.Distance(SafePoint) ascending
            //                    select unit).FirstOrDefault();

            if (closestEnemy == null)
            {
                return 40;
            }
            return SafePoint.Distance(closestEnemy.Location);
        }

        private static void SafeWoWPointMoveGenerator()
        {
            SafeWoWPointMove.Clear();
            for (var i = 0; i <= 35; i++)
            {
                var WayPoint = WoWMathHelper.GetPointAt(Me.Location, 40, WoWMathHelper.DegreesToRadians(i*10), 0);

                if (WayPoint.Distance(Me.Location) < 45 && Navigator.CanNavigateFully(Me.Location, WayPoint))
                {
                    SafeWoWPointMove.Add(WayPoint);
                }
            }
        }

        private static WoWPoint SafeWoWPointMoveFind()
        {
            return (from WayPoint in SafeWoWPointMove
                    where Styx.WoWInternals.World.GameWorld.IsInLineOfSight(Me.Location, WayPoint)
                    orderby GetClosestEnemyDistance(WayPoint) descending
                    select WayPoint).FirstOrDefault();
        }

        private static WoWPoint SafeWoWPointMoveKite;

        private static Composite Vanish()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.Vanish &&
                    SpellManager.HasSpell("Vanish") &&
                    !MeMounted &&
                    Me.Combat &&
                    !Me.HasAura("Vanish") &&
                    !Me.HasAura("Alliance Flag") &&
                    !Me.HasAura("Horde Flag") &&
                    !Me.HasAura("Netherstorm Flag") &&
                    !Me.HasAura("Orb of Power") &&
                    Me.HealthPercent <= THSettings.Instance.VanishHP &&
                    !IsStealthed(Me) &&
                    CountEneyTargettingUnitInLoSMe(40) > 0 &&
                    SpellManager.CanCast("Vanish"),
                    new Action(delegate
                        {
                            LastVanishDefend = DateTime.Now;
                            CastSpell("Vanish", Me, "VanishHP");
                            LastVanishDefend = DateTime.Now;
                            Me.ClearTarget();

                            if (TreeRoot.Current.Name != "LazyRaider" &&
                                TreeRoot.Current.Name != "Raid Bot" &&
                                TreeRoot.Current.Name != "Combat Bot" &&
                                TreeRoot.Current.Name != "Tyrael")
                            {
                                while (CountEnemyNear(Me, 25) > 0)
                                {
                                    SafeWoWPointMoveGenerator();
                                    SafeWoWPointMoveKite = SafeWoWPointMoveFind();

                                    while (LastVanishDefend + TimeSpan.FromMilliseconds(5000) > DateTime.Now)
                                    {
                                        WoWMovement.ClickToMove(SafeWoWPointMoveKite);
                                        Logging.Write("Hold on Vanish VanishHP SafeWoWPointMoveKite");
                                    }
                                }
                            }

                            while (LastVanishDefend + TimeSpan.FromMilliseconds(2000) > DateTime.Now)
                            {
                                Logging.Write("Hold on Vanish VanishHP");
                            }
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    (THSettings.Instance.VanishCD ||
                     THSettings.Instance.VanishBurst &&
                     Burst) &&
                    SpellManager.HasSpell("Vanish") &&
                    !MeMounted &&
                    Me.Combat &&
                    PlayerEnergy > 50 &&
                    !Me.HasAura("Vanish") &&
                    !Me.HasAura("Shadow Blades") &&
                    !Me.HasAura("Alliance Flag") &&
                    !Me.HasAura("Horde Flag") &&
                    !Me.HasAura("Netherstorm Flag") &&
                    !Me.HasAura("Orb of Power") &&
                    !IsStealthed(Me) &&
                    CurrentTargetAttackable(5) &&
                    WorthyTarget(Me.CurrentTarget) &&
                    SpellManager.CanCast("Vanish"),
                    new Action(delegate
                        {
                            CastSpell("Vanish", Me, "VanishCD/VanishBurst");
                            return RunStatus.Failure;
                        }))
                );
        }

        #endregion

        #region Vendetta

        private static Composite Vendetta()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.ShadowDance &&
                SpellManager.HasSpell("Vendetta") &&
                (THSettings.Instance.VendettaCD ||
                 THSettings.Instance.VendettaBurst &&
                 Burst) &&
                !MeMounted &&
                Me.Combat &&
                !IsStealthed(Me) &&
                CurrentTargetAttackable(10) &&
                WorthyTarget(Me.CurrentTarget, 0.5) &&
                !DebuffCCBreakonDamage(Me.CurrentTarget) &&
                !DebuffRoot(Me) &&
                SpellManager.CanCast("Vendetta"),
                new Action(delegate
                    {
                        if (PlayerEnergy < THSettings.Instance.ShadowDanceEnergy)
                        {
                            BurstCooldownEnergyPool = DateTime.Now + TimeSpan.FromMilliseconds(3000);
                        }
                        else
                        {
                            LastCastStealth = DateTime.Now;
                            BurstCooldownTimer = DateTime.Now;
                            CastSpell("Vendetta", Me.CurrentTarget, "Vendetta");
                        }
                        //return RunStatus.Success;
                    })
                );
        }

        #endregion

        #region WorthyTarget

        private static bool WorthyTarget(WoWUnit target, double pvEPercent = 1, double pvPPercent = 0.3)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            if (IsDummy(target) ||
                !target.IsPet &&
                target.CurrentHealth > Me.MaxHealth*pvEPercent ||
                target.IsPlayer &&
                target.CurrentHealth > Me.MaxHealth*pvPPercent)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region WriteDebug

        private static Composite WriteDebug(string message)
        {
            return new Action(delegate
                {
                    if (Me.Combat)
                    {
                        Logging.Write(message);
                    }
                    return RunStatus.Failure;
                });
        }

        private static Composite WriteDebugSubterfuge()
        {
            return new Action(delegate
                {
                    if (Me.Combat)
                    {
                        Logging.Write(LogLevel.Diagnostic, "HasAura " + Me.HasAura(115192));
                        Logging.Write(LogLevel.Diagnostic, "Subterfuge " + MyAuraTimeLeft(115192, Me));
                        Logging.Write(LogLevel.Diagnostic, "IsStealthed " + IsStealthed(Me));
                        Logging.Write(LogLevel.Diagnostic, "IsStealthedNoSD " + IsStealthedNoSD(Me));
                        Logging.Write(LogLevel.Diagnostic,
                                      "DebuffCCDuration " + DebuffCCDuration(Me.CurrentTarget, 1500));
                        Logging.Write(LogLevel.Diagnostic,
                                      "FacingOverrideMeCurrentTarget " + FacingOverrideMeCurrentTarget);
                    }

                    return RunStatus.Failure;
                });
        }

        #endregion

        #region WriteTargetInfo

        private static DateTime LastWriteTargetInfo;

        private static Composite WriteTargetInfo()
        {
            return new Action(delegate
                {
                    if (Me.CurrentTarget != null &&
                        LastWriteTargetInfo + TimeSpan.FromSeconds(3) < DateTime.Now)
                    {
                        Logging.Write(Me.CurrentTarget.Name +
                                      " Entry: " + Me.CurrentTarget.Entry +
                                      " Dist: " + Math.Round(Me.CurrentTarget.Distance, 1) +
                                      " Dist2D: " + Math.Round(Me.CurrentTarget.Distance2D, 1) +
                                      " BoundingRadius: " +
                                      Math.Round(Me.CurrentTarget.BoundingRadius, 1) +
                                      " BoundingHeight: " +
                                      Math.Round(Me.CurrentTarget.BoundingHeight, 1) +
                                      " IsWithinMeleeRange: " + Me.CurrentTarget.IsWithinMeleeRange);
                        LastWriteTargetInfo = DateTime.Now;
                    }
                    return RunStatus.Failure;
                });
        }

        #endregion
    }
}