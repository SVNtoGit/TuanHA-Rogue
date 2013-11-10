using Styx.Common;
using Styx.CommonBot;
using Styx.TreeSharp;
using Action = Styx.TreeSharp.Action;

namespace TuanHARogue
{
    public partial class Classname
    {
        #region CombatRotation

        private static Composite CombatRotation()
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
                        BladeFlurryCancel(),
                        AttackASAP(),
                        PickPocket(),
                        AdrenalineRush(),
                        ShadowBlades(),
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
                        SliceandDice(),
                        RevealingStrike(),
                        Rupture(),
                        Eviscerate(),
                        KillingSpree(),
                        Shiv(),
                        GougeHelpFriend(),
                        Redirect(),
                        ExposeArmor(),
                        SinisterStrikeCombat(),
                        ShurikenToss(),
                        Throw(),
                        DeadlyThrow(),
                        DisarmTrap(),
                        Distract(),
                        Poison(),
                        SinisterStrike()
                        //WriteDebug("Finish Sub Single Target Rotation")
                        )
                    ),
                new Decorator(
                    ret => _aoEModeOn,
                    new PrioritySelector(
                        Vanish(),
                        PickPocket(),
                        AdrenalineRush(),
                        ShadowBlades(),
                        MarkedforDeath(),
                        BladeFlurry(),
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
                        KillingSpree(),
                        Shiv(),
                        CrimsonTempest(),
                        SliceandDice(),
                        //Rupture(),
                        Eviscerate(),
                        FanofKnives(),
                        Shiv(),
                        SinisterStrikeCombat(),
                        ShurikenToss(),
                        Throw(),
                        SinisterStrike()
                        )
                    ),
                RestRotation()
                );
        }

        #endregion
    }
}