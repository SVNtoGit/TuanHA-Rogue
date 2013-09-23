using System;
using Styx;
using Styx.Helpers;
using Styx.Pathing;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using Action = Styx.TreeSharp.Action;

namespace TuanHARogue
{
    public partial class Classname
    {
        #region Delegates

        public delegate float DynamicRangeRetriever(object context);

        public delegate WoWPoint LocationRetriever(object context);

        public delegate bool SimpleBooleanDelegate(object context);

        public delegate WoWUnit UnitSelectionDelegate(object context);

        #endregion

        public static bool IsMovingBehind = false;
        public static TimeSpan MovementDelay = TimeSpan.FromSeconds(0);
        public static DateTime LastMovementTime = new DateTime(1970, 1, 1);
        //public WoWPoint LastMovementWoWPoint = Me.Location;
        public static WoWPoint LastMovementWoWPoint;
        public static float DistanceToUpdateMovement = 3f;
        public static float DistanceToMelee;

        //Set Distance To Keep in Melee Range base on Unit type
        public static float Dtm(WoWUnit toUnit)
        {
            if (toUnit.IsPlayer)
            {
                return 3;
            }
            return 5;
        }

        private static Composite MovementMoveStop(UnitSelectionDelegate toUnit, double range)
        {
            return new Decorator(
                ret =>
                THSettings.Instance.AutoMove &&
                !IsOverrideModeOn &&
                toUnit != null &&
                toUnit(ret) != null &&
                toUnit(ret) != Me &&
                toUnit(ret).IsAlive &&
                Me.IsMoving &&
                !toUnit(ret).IsMoving &&
                IsEnemy(toUnit(ret)) &&
                toUnit(ret).Distance - toUnit(ret).CombatReach < 3 &&
                toUnit(ret).InLineOfSight,
                new Action(ret =>
                    {
                        //if (DateTime.Now < MovementDelay && !Me.IsBehind(toUnit(ret)))
                        //{
                        //    Logging.Write(LogLevel.Diagnostic,
                        //                  DateTime.Now.ToString("ss:fff ") + "IsMovingBehind");
                        //    return RunStatus.Failure;
                        //}
                        Navigator.PlayerMover.MoveStop();
                        //Logging.Write(LogLevel.Diagnostic,
                        //              DateTime.Now.ToString("ss:fff ") + "MovementMoveStop");
                        return RunStatus.Failure;
                    }));
        }

        private void MovementMoveStopVoid(WoWUnit toUnit)
        {
            if (THSettings.Instance.AutoMove &&
                !IsOverrideModeOn &&
                toUnit != null &&
                toUnit != Me &&
                toUnit.IsAlive &&
                Me.IsMoving &&
                !toUnit.IsMoving &&
                (toUnit.Location.Distance(Me.Location) - toUnit.BoundingRadius < Dtm(toUnit) ||
                 toUnit.Location.Distance(Me.Location) - toUnit.BoundingRadius < 1))
            {
                Navigator.PlayerMover.MoveStop();
            }
        }

        private Composite MovementMoveToLoS(UnitSelectionDelegate toUnit)
        {
            return new Decorator(
                ret =>
                THSettings.Instance.AutoMove &&
                DateTime.Now > DoNotMove &&
                !IsOverrideModeOn &&
                !Me.IsCasting &&
                //!Me.IsChanneling &&
                toUnit != null &&
                toUnit(ret) != null &&
                toUnit(ret) != Me &&
                !toUnit(ret).InLineOfSight,
                new Action(ret =>
                    {
                        LastMovementWoWPoint = toUnit(ret).Location;
                        Navigator.MoveTo(LastMovementWoWPoint);
                        //Logging.Write(LogLevel.Diagnostic,
                        //              DateTime.Now.ToString("ss:fff ") + "MovementMoveToLoS");
                        LastMovementTime = DateTime.Now;
                        return RunStatus.Failure;
                    }));
        }

        private static Composite MovementMoveToMelee(UnitSelectionDelegate toUnit)
        {
            return new Decorator(
                ret =>
                THSettings.Instance.AutoMove &&
                !Me.Mounted &&
                DateTime.Now > DoNotMove &&
                !IsOverrideModeOn &&
                !Me.IsCasting &&
                !Me.IsChanneling &&
                toUnit != null &&
                toUnit(ret) != null &&
                toUnit(ret) != Me &&
                toUnit(ret).IsAlive &&
                //(Me.IsFacing(toUnit(ret)) ||
                // Me.Combat) &&
                toUnit(ret).Distance - toUnit(ret).CombatReach > 2 &&
                IsEnemy(toUnit(ret)),
                new Action(ret =>
                    {
                        //WoWMovement.ConstantFace(toUnit(ret).Guid);
                        LastMovementWoWPoint = toUnit(ret).Location;
                        Navigator.MoveTo(LastMovementWoWPoint);
                        //Logging.Write(LogLevel.Diagnostic,
                        //              DateTime.Now.ToString("ss:fff ") + "MovementMoveToMelee");

                        //I'm just a stupid
                        //Logging.Write(LogLevel.Diagnostic,
                        //              (LastMovementTime + MovementDelay).ToString("yyyy-mm-dd hh:mm:ss:fff ") +
                        //              "LastMovementTime + MovementDelay");
                        //Logging.Write(LogLevel.Diagnostic,
                        //              DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss:fff ") + "DateTime.Now");
                        //if (DateTime.Now > LastMovementTime + MovementDelay)
                        //{
                        //    Logging.Write("DateTime.Now > LastMovementTime + MovementDelay " +
                        //                  DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss:fff > ") +
                        //                  (LastMovementTime + MovementDelay).ToString(
                        //                      "yyyy-mm-dd hh:mm:ss:fff "));
                        //}

                        //if (DateTime.Now < LastMovementTime + MovementDelay)
                        //{
                        //    Logging.Write("DateTime.Now < LastMovementTime + MovementDelay " +
                        //                  DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss:fff < ") +
                        //                  (LastMovementTime + MovementDelay).ToString(
                        //                      "yyyy-mm-dd hh:mm:ss:fff "));
                        //}

                        LastMovementTime = DateTime.Now;
                        return RunStatus.Failure;
                    }));
        }

        private void MovementMoveToMeleeVoid(WoWUnit toUnit)
        {
            if (THSettings.Instance.AutoMove &&
                DateTime.Now > DoNotMove &&
                !IsOverrideModeOn &&
                !Me.IsCasting &&
                !Me.IsChanneling &&
                toUnit != null &&
                toUnit != Me &&
                toUnit.IsAlive &&
                //Only Move again After a certain delay or target move 3 yard from original posision
                (DateTime.Now > LastMovementTime + MovementDelay ||
                 LastMovementWoWPoint.Distance(toUnit.Location) >
                 DistanceToUpdateMovement) &&
                (toUnit.Distance - toUnit.BoundingRadius > Dtm(toUnit) ||
                 !toUnit.IsWithinMeleeRange))
            {
                LastMovementTime = DateTime.Now;
                LastMovementWoWPoint = toUnit.Location;
                Navigator.MoveTo(LastMovementWoWPoint);
            }
        }


        private Composite MovementMoveBehind(UnitSelectionDelegate toUnit)
        {
            return new Decorator(
                ret =>
                THSettings.Instance.AutoMove &&
                DateTime.Now > DoNotMove &&
                !IsOverrideModeOn &&
                !Me.IsCasting &&
                //!Me.IsChanneling &&
                toUnit != null &&
                toUnit(ret) != null &&
                toUnit(ret) != Me &&
                toUnit(ret).IsAlive &&
                //only MovementMoveBehind if IsWithinMeleeRange
                !toUnit(ret).IsMoving &&
                toUnit(ret).Distance < Dtm(toUnit(ret)) &&
                !Me.IsBehind(toUnit(ret)) &&
                !MeIsTank &&
                //Only Move again After a certain delay or target move 3 yard from original posision
                (DateTime.Now > LastMovementTime + MovementDelay ||
                 LastMovementWoWPoint.Distance(toUnit(ret).Location) >
                 DistanceToUpdateMovement) &&
                (toUnit(ret).IsPlayer ||
                 !toUnit(ret).IsPlayer && toUnit(ret).CurrentTarget != Me && toUnit(ret).Combat),
                new Action(ret =>
                    {
                        WoWPoint pointBehind =
                            toUnit(ret).Location.RayCast(
                                toUnit(ret).Rotation + WoWMathHelper.DegreesToRadians(150), 3f);
                        LastMovementWoWPoint = pointBehind;

                        Navigator.MoveTo(pointBehind);
                        //Logging.Write(LogLevel.Diagnostic,
                        //              DateTime.Now.ToString("ss:fff ") + "MovementMoveBehind");
                        //Logging.Write(LogLevel.Diagnostic, "IsMovingBehind = true");
                        LastMovementTime = DateTime.Now;
                        return RunStatus.Failure;
                    }));
        }
    }
}