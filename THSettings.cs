using System;
using System.IO;
using Styx;
using Styx.Helpers;

namespace TuanHARogue
{
    public class THSettings : Settings
    {
        public static readonly THSettings Instance = new THSettings();

        #region DataSource

        public bool UpdateStatus;

        public THSettings()
            : base(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                             string.Format(
                                 @"Routines/TuanHARogueOpenBeta/TuanHA-Rogue-Settings-20130401-{0}.xml",
                                 StyxWoW.Me.Name)))
        {
        }

        #endregion

        [Setting, DefaultValue(true)]
        public bool AdrenalineRush { get; set; }

        [Setting, DefaultValue(true)]
        public bool AdrenalineRushCD { get; set; }

        [Setting, DefaultValue(true)]
        public bool AdrenalineRushBurst { get; set; }

        [Setting, DefaultValue(true)]
        public bool AutoAoE { get; set; }

        [Setting, DefaultValue(true)]
        public bool AutoAttack { get; set; }

        [Setting, DefaultValue(true)]
        public bool AutoFace { get; set; }

        [Setting, DefaultValue(true)]
        public bool AutoMove { get; set; }

        [Setting, DefaultValue(true)]
        public bool AutoRacial { get; set; }

        [Setting, DefaultValue(true)]
        public bool AutoTarget { get; set; }

        [Setting, DefaultValue(true)]
        public bool AttackASAP { get; set; }

        [Setting, DefaultValue(false)]
        public bool AttackOOC { get; set; }

        [Setting, DefaultValue(true)]
        public bool Ambush { get; set; }

        [Setting, DefaultValue(true)]
        public bool Backstab { get; set; }

        [Setting, DefaultValue(19)] //S
        public int Backward { get; set; }

        [Setting, DefaultValue(true)]
        public bool BladeFlurry { get; set; }

        [Setting, DefaultValue(1)]
        public int BurstKey { get; set; }

        [Setting, DefaultValue(60)]
        public int BurstHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool BurstofSpeed { get; set; }

        [Setting, DefaultValue(8)]
        public int BurstofSpeedDistance { get; set; }

        [Setting, DefaultValue(true)]
        public bool BurstofSpeedEnergy { get; set; }

        [Setting, DefaultValue(80)]
        public int BurstofSpeedEnergyNumber { get; set; }

        [Setting, DefaultValue(500)]
        public int BurstofSpeedRenew { get; set; }

        [Setting, DefaultValue(2)]
        public int CheapShotStunLevel { get; set; }

        [Setting, DefaultValue(true)]
        public bool CheapShot { get; set; }

        [Setting, DefaultValue(true)]
        public bool CheapShotInterrupt { get; set; }

        [Setting, DefaultValue(1500)]
        public int CheapShotInterruptTimeLeft { get; set; }

        [Setting, DefaultValue(true)]
        public bool CloakofShadows { get; set; }

        [Setting, DefaultValue(60)]
        public int CloakofShadowsHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool CloakofShadowsTarget { get; set; }

        [Setting, DefaultValue(3)]
        public int CloakofShadowsTargetUnit { get; set; }

        [Setting, DefaultValue(true)]
        public bool CloakofShadowsRoot { get; set; }

        [Setting, DefaultValue(true)]
        public bool CombatReadiness { get; set; }

        [Setting, DefaultValue(60)]
        public int CombatReadinessHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool CombatReadinessTarget { get; set; }

        [Setting, DefaultValue(3)]
        public int CombatReadinessTargetUnit { get; set; }

        [Setting, DefaultValue(true)]
        public bool CrimsonTempest { get; set; }

        [Setting, DefaultValue(5)]
        public int CrimsonTempestCP { get; set; }

        [Setting, DefaultValue(5)]
        public int CrimsonTempestUnit { get; set; }

        [Setting, DefaultValue(true)]
        public bool DeadlyThrow { get; set; }

        [Setting, DefaultValue(true)]
        public bool DeadlyThrowInterrupt { get; set; }

        [Setting, DefaultValue(1500)]
        public int DeadlyThrowInterruptTimeLeft { get; set; }

        [Setting, DefaultValue(true)]
        public bool DeadlyThrowSlow { get; set; }

        [Setting, DefaultValue(15)]
        public int DeadlyThrowSlowDistance { get; set; }

        [Setting, DefaultValue(true)]
        public bool DeadlyThrowFinish { get; set; }

        [Setting, DefaultValue(5)]
        public int DeadlyThrowFinishCP { get; set; }

        [Setting, DefaultValue(15)]
        public int DeadlyThrowFinishDistance { get; set; }

        [Setting, DefaultValue(true)]
        public bool Dismantle { get; set; }

        [Setting, DefaultValue(true)]
        public bool DistmantleCooldown { get; set; }

        [Setting, DefaultValue(true)]
        public bool DismantlePre { get; set; }

        [Setting, DefaultValue(60)]
        public int DismantleHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool DismantleTarget { get; set; }

        [Setting, DefaultValue(3)]
        public int DismantleTargetUnit { get; set; }

        [Setting, DefaultValue(true)]
        public bool DisarmTrap { get; set; }

        [Setting, DefaultValue(true)]
        public bool Dispatch { get; set; }

        [Setting, DefaultValue(true)]
        public bool Distract { get; set; }

        [Setting, DefaultValue(95)]
        public int DoNotHealAbove { get; set; }

        [Setting, DefaultValue(true)]
        public bool Evasion { get; set; }

        [Setting, DefaultValue(true)]
        public bool EvasionPre { get; set; }

        [Setting, DefaultValue(60)]
        public int EvasionHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool EvasionTarget { get; set; }

        [Setting, DefaultValue(3)]
        public int EvasionTargetUnit { get; set; }

        [Setting, DefaultValue(true)]
        public bool Eviscerate { get; set; }

        [Setting, DefaultValue(5)]
        public int EviscerateCP { get; set; }

        [Setting, DefaultValue(true)]
        public bool Envenom { get; set; }

        [Setting, DefaultValue(5)]
        public int EnvenomCP { get; set; }

        [Setting, DefaultValue(true)]
        public bool ExposeArmor { get; set; }

        [Setting, DefaultValue(true)] //W
        public bool FanofKnives { get; set; }

        [Setting, DefaultValue(7)] //W
        public int FanofKnivesUnit { get; set; }

        [Setting, DefaultValue(23)] //W
        public int Forward { get; set; }

        [Setting, DefaultValue(true)]
        public bool Feint { get; set; }

        [Setting, DefaultValue(60)]
        public int FeintHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool FeintTarget { get; set; }

        [Setting, DefaultValue(3)]
        public int FeintTargetUnit { get; set; }

        [Setting, DefaultValue(true)]
        public bool Garrote { get; set; }

        [Setting, DefaultValue(true)]
        public bool Gouge { get; set; }

        [Setting, DefaultValue(1500)]
        public int GougeTimeLeft { get; set; }

        [Setting, DefaultValue(true)]
        public bool GougePet { get; set; }

        [Setting, DefaultValue(true)]
        public bool GougeHelpFriend { get; set; }

        [Setting, DefaultValue(40)]
        public int GougeHelpFriendHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool GougeOffensiveCooldown { get; set; }

        [Setting, DefaultValue(true)]
        public bool HealthStone { get; set; }

        [Setting, DefaultValue(40)]
        public int HealthStoneHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool Hemorrhage { get; set; }

        [Setting, DefaultValue(true)]
        public bool InterruptAll { get; set; }

        [Setting, DefaultValue(false)]
        public bool InterruptTarget { get; set; }

        [Setting, DefaultValue(false)]
        public bool InterruptFocus { get; set; }

        [Setting, DefaultValue(true)]
        public bool Kick { get; set; }

        [Setting, DefaultValue(1500)]
        public int KickTimeLeft { get; set; }

        [Setting, DefaultValue(true)]
        public bool KidneyShot { get; set; }

        [Setting, DefaultValue(2)]
        public int KidneyShotStunLevel { get; set; }

        [Setting, DefaultValue(5)]
        public int KidneyShotCP { get; set; }

        [Setting, DefaultValue(true)]
        public bool KidneyShotInterrupt { get; set; }

        [Setting, DefaultValue(1500)]
        public int KidneyShotInterruptTimeLeft { get; set; }

        [Setting, DefaultValue(500)]
        public int KidneyShotStunLeft { get; set; }

        [Setting, DefaultValue(true)]
        public bool KillingSpree { get; set; }

        [Setting, DefaultValue(true)]
        public bool KillingSpreeCD { get; set; }

        [Setting, DefaultValue(true)]
        public bool KillingSpreeBurst { get; set; }

        [Setting, DefaultValue("")]
        public string LastSavedSpec { get; set; }

        [Setting, DefaultValue("")]
        public string LastSavedMode { get; set; }

        [Setting, DefaultValue(true)]
        public bool LagTolerance { get; set; }

        [Setting, DefaultValue(true)]
        public bool MarkedforDeath { get; set; }

        [Setting, DefaultValue(true)]
        public bool MarkedforDeathCD { get; set; }

        [Setting, DefaultValue(true)]
        public bool MarkedforDeathBurst { get; set; }

        [Setting, DefaultValue(true)]
        public bool MarkedforDeathLow { get; set; }

        [Setting, DefaultValue(20)]
        public int MarkedforDeathLowHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool Mutilate { get; set; }

        [Setting, DefaultValue(33)]
        public int PauseKey { get; set; }

        [Setting, DefaultValue(false)]
        public bool Pause { get; set; }

        [Setting, DefaultValue(false)]
        public bool PickPocket { get; set; }

        [Setting, DefaultValue(1)]
        public int PoisonLethal { get; set; }

        [Setting, DefaultValue(1)]
        public int PoisonNonLethal { get; set; }

        [Setting, DefaultValue(true)]
        public bool PoolEnergy { get; set; }

        [Setting, DefaultValue(70)]
        public int PoolEnergyPC { get; set; }

        [Setting, DefaultValue(true)]
        public bool Premeditation { get; set; }

        [Setting, DefaultValue(true)]
        public bool Preparation { get; set; }

        [Setting, DefaultValue(75)]
        public int PriorityHeal { get; set; }

        [Setting, DefaultValue(1)]
        public int ProfBuff { get; set; }

        [Setting, DefaultValue(60)]
        public int ProfBuffHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool Recuperate { get; set; }

        [Setting, DefaultValue(70)]
        public int RecuperateHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool Redirect { get; set; }

        [Setting, DefaultValue(true)]
        public bool RevealingStrike { get; set; }

        [Setting, DefaultValue(true)]
        public bool Rupture { get; set; }

        [Setting, DefaultValue(5)]
        public int RuptureCP { get; set; }

        [Setting, DefaultValue(3)]
        public int SapLevel { get; set; }

        [Setting, DefaultValue(true)]
        public bool SapTarget { get; set; }

        [Setting, DefaultValue(true)]
        public bool SapFocus { get; set; }

        [Setting, DefaultValue(true)]
        public bool SapRogueDruid { get; set; }

        [Setting, DefaultValue(true)]
        public bool SapAny { get; set; }

        [Setting, DefaultValue(500)]
        public int SearchInterval { get; set; }

        [Setting, DefaultValue(true)]
        public bool ShadowBlades { get; set; }

        [Setting, DefaultValue(true)]
        public bool ShadowBladesCD { get; set; }

        [Setting, DefaultValue(true)]
        public bool ShadowBladesBurst { get; set; }

        [Setting, DefaultValue(true)]
        public bool ShadowBladeSync { get; set; }

        [Setting, DefaultValue(true)]
        public bool ShadowDance { get; set; }

        [Setting, DefaultValue(90)]
        public int ShadowDanceEnergy { get; set; }

        [Setting, DefaultValue(true)]
        public bool ShadowDanceCD { get; set; }

        [Setting, DefaultValue(true)]
        public bool ShadowDanceBurst { get; set; }

        [Setting, DefaultValue(true)]
        public bool ShadowDanceSap { get; set; }

        [Setting, DefaultValue(true)]
        public bool Shadowstep { get; set; }

        [Setting, DefaultValue(8)]
        public int ShadowstepDistance { get; set; }

        [Setting, DefaultValue(true)]
        public bool ShadowWalk { get; set; }

        [Setting, DefaultValue(true)]
        public bool Shiv { get; set; }

        [Setting, DefaultValue(true)]
        public bool ShivEnrage { get; set; }

        [Setting, DefaultValue(true)]
        public bool ShivCripplingPoison { get; set; }

        [Setting, DefaultValue(true)]
        public bool ShivMindNumbing { get; set; }

        [Setting, DefaultValue(true)]
        public bool ShivLeechingPoison { get; set; }

        [Setting, DefaultValue(70)]
        public int ShivLeechingPoisonHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool ShivParalystic { get; set; }

        [Setting, DefaultValue(70)]
        public int ShivParalysticHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool ShroudofConcealment { get; set; }

        [Setting, DefaultValue(3)]
        public int ShroudofConcealmentNumber { get; set; }

        [Setting, DefaultValue(true)]
        public bool ShurikenToss { get; set; }

        [Setting, DefaultValue(11)]
        public int ShurikenTossDistance { get; set; }

        [Setting, DefaultValue(true)]
        public bool SliceandDice { get; set; }

        [Setting, DefaultValue(1)]
        public int SliceandDiceCP { get; set; }

        [Setting, DefaultValue(true)]
        public bool SmokeBomb { get; set; }

        [Setting, DefaultValue(35)]
        public int SmokeBombHP { get; set; }

        [Setting, DefaultValue(2)]
        public int SmokeBombEnemyUnit { get; set; }

        [Setting, DefaultValue(true)]
        public bool Sprint { get; set; }

        [Setting, DefaultValue(10)]
        public int SprintDistance { get; set; }

        [Setting, DefaultValue(true)]
        public bool SprintPre { get; set; }

        [Setting, DefaultValue(true)]
        public bool Stealth { get; set; }

        [Setting, DefaultValue(true)]
        public bool StealthForce { get; set; }

        [Setting, DefaultValue(17)] //Q
        public int StrafleLeft { get; set; }

        [Setting, DefaultValue(5)] //E
        public int StrafleRight { get; set; }

        [Setting, DefaultValue(true)]
        public bool Throw { get; set; }

        [Setting, DefaultValue(true)]
        public bool TricksoftheTrade { get; set; }

        [Setting, DefaultValue(true)]
        public bool TricksoftheTradeFocus { get; set; }

        [Setting, DefaultValue(false)]
        public bool TricksoftheTradeAny { get; set; }

        [Setting, DefaultValue(1)]
        public int Trinket1 { get; set; }

        [Setting, DefaultValue(60)]
        public int Trinket1HP { get; set; }

        [Setting, DefaultValue(1)]
        public int Trinket2 { get; set; }

        [Setting, DefaultValue(60)]
        public int Trinket2HP { get; set; }

        [Setting, DefaultValue(4)] //A
        public int TurnRight { get; set; }

        [Setting, DefaultValue(1)] //D
        public int TurnLeft { get; set; }

        [Setting, DefaultValue(4)]
        public int UnittoStartAoE { get; set; }

        [Setting, DefaultValue(40)]
        public int UrgentHeal { get; set; }

        [Setting, DefaultValue(true)]
        public bool UseFood { get; set; }

        [Setting, DefaultValue(50)]
        public int UseFoodHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool UsePauseKey { get; set; }

        [Setting, DefaultValue(true)]
        public bool Vanish { get; set; }

        [Setting, DefaultValue(true)]
        public bool VanishPre { get; set; }

        [Setting, DefaultValue(25)]
        public int VanishHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool VanishCD { get; set; }

        [Setting, DefaultValue(true)]
        public bool VanishBurst { get; set; }

        [Setting, DefaultValue(true)]
        public bool VanishSap { get; set; }

        [Setting, DefaultValue(true)]
        public bool Vendetta { get; set; }

        [Setting, DefaultValue(true)]
        public bool VendettaCD { get; set; }

        [Setting, DefaultValue(true)]
        public bool VendettaBurst { get; set; }
    }
}