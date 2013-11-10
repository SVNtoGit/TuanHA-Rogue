using Styx.Common;
using Styx.CommonBot;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Action = Styx.TreeSharp.Action;

namespace TuanHARogue
{
    public partial class Classname
    {
        #region AssassinationRotation

        //
        private static Composite AssassinationRotation()
        {
            return new PrioritySelector(
                new Action(delegate
                    {
                        //Switch to BestTarget
                        if (THSettings.Instance.AutoTarget &&
                            Me.CurrentTarget != null &&
                            Me.CurrentTarget.IsValid &&
                            Me.CurrentTarget.Distance > 40 &&
                            GetBestTarget() &&
                            BestTarget.IsValid &&
                            Me.CurrentTarget != BestTarget)
                        {
                            BestTarget.Target();
                            Logging.Write(LogLevel.Diagnostic, "Switch to BestTarget");
                        }

                        //Target BestTarget
                        if (THSettings.Instance.AutoTarget &&
                            (Me.CurrentTarget == null ||
                             DistanceCheck(Me.CurrentTarget) > 40) &&
                            GetBestTarget() &&
                            BestTarget.IsValid)
                        {
                            BestTarget.Target();
                            Logging.Write(LogLevel.Diagnostic, "Target BestTarget");
                        }

                        //Hold dps on Dungeon or Raid in No Combat
                        if (TreeRoot.Current.Name != "DungeonBuddy" &&
                            (InDungeon ||
                             InRaid) &&
                            !Me.Combat &&
                            Me.CurrentTarget != null &&
                            IsEnemy(Me.CurrentTarget) &&
                            !Me.CurrentTarget.IsTargetingMyPartyMember &&
                            !Me.CurrentTarget.IsTargetingMyRaidMember &&
                            !Me.CurrentTarget.IsTargetingMeOrPet)
                        {
                            return RunStatus.Success;
                        }

                        if (THSettings.Instance.AutoAoE &&
                            THSettings.Instance.UnittoStartAoE > 0 &&
                            CountEnemyNear(Me, 10) >= THSettings.Instance.UnittoStartAoE)
                        {
                            _aoEModeOn = true;
                        }
                        else
                        {
                            _aoEModeOn = false;
                        }

                        return RunStatus.Failure;
                    }),
                //But first, facing target
                FacingTarget(),
                //Starting the movement right here
                //MovementMoveBehind(ret => Me.CurrentTarget),
                //MovementMoveToLoS(ret => Me.CurrentTarget),
                MovementMoveStop(ret => Me.CurrentTarget, 3),
                MovementMoveToMelee(ret => Me.CurrentTarget),
                //DPS Rotation Here
                UseHealthstone(),
                UseBattleStandard(),
                BloodFury(),
                new Decorator(
                    ret => !_aoEModeOn,
                    new PrioritySelector(
                        //WriteDebug("Enter Sub Single Target Rotation"),
                        Vanish(),
                        ShadowWalk(),
                        AttackASAP(),
                        PickPocket(),
                        Vendetta(),
                        ShadowBlades(),
                        Premeditation(),
                        MarkedforDeath(),
                        Shadowstep(),
                        Sprint(),
                        BurstofSpeed(),
                        CloakofShadows(),
                        Evasion(),
                        CombatReadiness(),
                        Preparation(),
                        TricksoftheTrade(),
                        Sap(),
                        CheapShot(),
                        CheapShotCaD(),
                        Garrote(),
                        GarroteCaD(),
                        Ambush(),
                        AmbushCaD(),
                        OpenerStopCheck(),
                        KidneyShot(),
                        Dismantle(),
                        Feint(),
                        Recuperate(),
                        EnvenomRefreshSnD(),
                        SliceandDice(),
                        Rupture(),
                        Envenom(),
                        Shiv(),
                        GougeHelpFriend(),
                        Redirect(),
                        ExposeArmor(),
                        Dispatch(),
                        Mutilate(),
                        ShurikenToss(),
                        Throw(),
                        DeadlyThrow(),
                        DisarmTrap(),
                        Distract(),
                        Poison()
                        //WriteDebug("Finish Sub Single Target Rotation")
                        )
                    ),
                new Decorator(
                    ret => _aoEModeOn,
                    new PrioritySelector(
                        Vanish(),
                        PickPocket(),
                        Vendetta(),
                        ShadowBlades(),
                        Premeditation(),
                        MarkedforDeath(),
                        Shadowstep(),
                        BurstofSpeed(),
                        CloakofShadows(),
                        Evasion(),
                        CombatReadiness(),
                        Preparation(),
                        TricksoftheTrade(),
                        CheapShot(),
                        CheapShotCaD(),
                        Garrote(),
                        GarroteCaD(),
                        Ambush(),
                        AmbushCaD(),
                        OpenerStopCheck(),
                        Feint(),
                        Recuperate(),
                        Redirect(),
                        Shiv(),
                        CrimsonTempest(),
                        EnvenomRefreshSnD(),
                        SliceandDice(),
                        //Rupture(),
                        Envenom(),
                        FanofKnives(),
                        Shiv(),
                        Dispatch(),
                        Mutilate(),
                        ShurikenToss(),
                        Throw()
                        )
                    ),
                RestRotation()
                );
        }

        #endregion
    }
}