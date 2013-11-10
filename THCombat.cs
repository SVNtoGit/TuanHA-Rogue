using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.CommonBot.Routines;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Action = Styx.TreeSharp.Action;

namespace TuanHARogue
{
    public partial class Classname : CombatRoutine
    {
        //[DllImport("user32.dll")]
        //private static extern IntPtr GetForegroundWindow();

        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        //private string GetActiveWindowTitle()
        //{
        //    const int nChars = 256;
        //    IntPtr handle = IntPtr.Zero;
        //    StringBuilder Buff = new StringBuilder(nChars);
        //    handle = GetForegroundWindow();

        //    if (GetWindowText(handle, Buff, nChars) > 0)
        //    {
        //        return Buff.ToString();
        //    }
        //    return null;
        //}

        #region Delegates

        public delegate WoWPoint LocationRetriverDelegate(object context);

        #endregion

        #region Basic Functions

        public override bool WantButton
        {
            get { return true; }
        }

        public override WoWClass Class
        {
            get { return WoWClass.Rogue; }
        }

        public override string Name
        {
            get { return "TuanHA Rogue [Open Beta]"; }
        }

        public override Composite RestBehavior
        {
            get { return RestRotation(); }
        }

        public override Composite PreCombatBuffBehavior
        {
            get { return MainRotation(); }
        }

        //public override Composite PullBuffBehavior
        //{
        //    get { return MainRotation(); }
        //}

        //public override Composite PullBehavior
        //{
        //    get { return MainRotation(); }
        //}

        //public override Composite CombatBuffBehavior
        //{
        //    get { return MainRotation(); }
        //}

        public override Composite CombatBehavior
        {
            get { return MainRotation(); }
        }

        //public override Composite HealBehavior
        //{
        //    get { return MainRotation(); }
        //}

        //public override Composite MoveToTargetBehavior
        //{
        //    get { return CreateMoveToLosBehavior(); }
        //}

        private static LocalPlayer Me
        {
            get { return StyxWoW.Me; }
        }

        public override void Initialize()
        {
            Logging.Write("");
            Logging.Write("Hello " + Me.Race + " " + Me.Class);
            Logging.Write("Thank you for using TuanHA Rogue");
            Logging.Write("");

            //Disable for speed
            Lua.Events.AttachEvent("GROUP_ROSTER_UPDATE", UpdateRaidPartyMembersEvent);
            Lua.Events.AttachEvent("ACTIVE_TALENT_GROUP_CHANGED", UpdateStatusEvent);
            Lua.Events.AttachEvent("PLAYER_TALENT_UPDATE", UpdateMyTalentEvent);
            Lua.Events.AttachEvent("GLYPH_ADDED", UpdateMyGlyphEvent);
            Lua.Events.AttachEvent("ZONE_CHANGED_NEW_AREA", UpdateRaidPartyMembersEvent);
            Lua.Events.AttachEvent("PLAYER_ENTERING_WORLD", UpdateRaidPartyMembersEvent);

            //BotEvents.Player.OnMapChanged += UpdateCurrentMapEvent;
            BotEvents.OnBotStarted += OnBotStartedEvent;

            //Set Camera angle so you never have problem with it
            //console cameraSmoothTrackingStyle 0 (credit by alxaw - member of CLU dev team) 
            //Lua.DoString("RunMacroText(\"/console cameraSmoothTrackingStyle 1\")");
            THSettings.Instance.UpdateStatus = true;
        }

        public override void OnButtonPress()
        {
            var gui = new THForm();
            gui.ShowDialog();
        }

        #endregion

        #region Function After this Region Order Alphabetically

        #endregion

        #region GetUnits

        private static IEnumerable<WoWUnit> GetAllUnits()
        {
            return ObjectManager.GetObjectsOfTypeFast<WoWUnit>()
                                .Where(u => ValidUnit(u) && u.Distance2DSqr < 40*40)
                                .ToList();
        }

        private static readonly List<WoWPlayer> NearbyUnFriendlyPlayers = new List<WoWPlayer>();
        private static readonly List<WoWUnit> NearbyUnFriendlyUnits = new List<WoWUnit>();
        //private static DateTime LastGetUnits;

        private static Composite GetUnits()
        {
            return new Action(delegate
                {
                    //if (LastGetUnits > DateTime.Now)
                    //{
                    //    return RunStatus.Failure;
                    //}

                    //LastGetUnits = DateTime.Now + TimeSpan.FromMilliseconds(THSettings.Instance.SearchInterval);

                    NearbyUnFriendlyPlayers.Clear();
                    NearbyUnFriendlyUnits.Clear();

                    foreach (WoWUnit unit in GetAllUnits())
                    {
                        if (!unit.IsValid)
                        {
                            continue;
                        }
                        if (IsEnemy(unit))
                        {
                            //Logging.Write(DateTime.Now.ToString("ss:fff") + " NearbyUnFriendlyUnits Add: " + unit.Name);
                            NearbyUnFriendlyUnits.Add(unit);

                            var player = unit as WoWPlayer;
                            if ((InArena || InBattleground) && player != null)
                            {
                                NearbyUnFriendlyPlayers.Add(player);
                            }
                        }
                    }
                    return RunStatus.Failure;
                });
        }

        #endregion

        #region Hold

        private static Composite Hold()
        {
            return new Decorator(
                ret =>
                !Me.IsValid ||
                InArena &&
                !Me.Name.Contains("Nofe") &&
                !Me.Name.Contains("Nire") &&
                !Me.Name.Contains("Мунжи") &&
                !Me.Name.Contains("Sodo") &&
                !Me.Name.Contains("Isbe") &&
                !Me.Name.Contains("Dustb") &&
                !Me.Name.Contains("Elán") &&
                !Me.Name.Contains("Derpi") &&
                !Me.Name.Contains("Gg") &&
                !Me.Name.Contains("Darks") ||
                !StyxWoW.IsInWorld ||
                !Me.IsAlive ||
                //!THSettings.Instance.AttackOOC &&
                //!Me.Combat ||
                Me.HasAura("Food") && !Me.Combat &&
                Me.HealthPercent < THSettings.Instance.DoNotHealAbove ||
                Me.HasAura("Resurrection Sickness"),
                new Action(delegate
                    {
                        if (Me.IsAutoAttacking)
                        {
                            Lua.DoString("RunMacroText('/stopattack');");
                        }

                        Logging.Write("Hold");
                        return RunStatus.Success;
                    })
                );
        }

        #endregion

        #region Pulse

        private static DateTime LastSwitch;
        private static DateTime BurstLast;
        private static bool Burst = false;

        public override void Pulse()
        {
            if (!Me.IsValid ||
                !Me.IsAlive ||
                !StyxWoW.IsInWorld)
            {
                return;
            }

            if (THSettings.Instance.UpdateStatus)
            {
                UpdateStatus();
            }
            else
            {
                ObjectManager.Update();
                KickVoid();
                CheapShotInterruptVoid();
                GougeVoid();
                KidneyShotInterruptVoid();
                DeadlyThrowVoid();
                PendingSpellRemove();
                //StealthVoid();

                //Pause
                if (THSettings.Instance.UsePauseKey && THSettings.Instance.PauseKey != 0)
                {
                    //AoE Mode
                    if (GetAsyncKeyState(Keys.LControlKey) < 0 &&
                        GetAsyncKeyState(IndexToKeys(THSettings.Instance.PauseKey)) < 0 &&
                        LastSwitch.AddSeconds(1) < DateTime.Now) // && GetActiveWindowTitle() == "World of Warcraft")
                    {
                        if (THSettings.Instance.Pause)
                        {
                            THSettings.Instance.Pause = false;
                            LastSwitch = DateTime.Now;
                            Logging.Write("Pause Mode is OFF, Hold 1 second Ctrl + " +
                                          IndexToKeys(THSettings.Instance.PauseKey) +
                                          " to Override bot action.");
                            //if (THSettings.Instance.PrintChat)
                            //{
                            //    Lua.DoString("RunMacroText(\"/script msg='Pause Mode OFF' print(msg)\")");
                            //}
                        }
                        else
                        {
                            THSettings.Instance.Pause = true;
                            THSettings.Instance.UpdateStatus = true;
                            LastSwitch = DateTime.Now;
                            Logging.Write("Pause Mode is ON, Hold 1 second Ctrl + " +
                                          IndexToKeys(THSettings.Instance.PauseKey) +
                                          " to resume bot action.");
                            //if (THSettings.Instance.PrintChat)
                            //{
                            //    Lua.DoString("RunMacroText(\"/script msg='Pause Mode ON' print(msg)\")");
                            //}
                        }
                    }
                }

                //Auto Disactivate Burst after a Timer or Me get CC
                if (Burst && BurstLast < DateTime.Now)
                {
                    Burst = false;
                    //BurstLast = DateTime.Now;
                    Logging.Write("Burst Mode is OFF");
                    //if (THSettings.Instance.PrintChat)
                    //{
                    //    Lua.DoString("RunMacroText(\"/script msg='Burst Mode OFF' print(msg)\")");
                    //}
                }

                //Burst on Cooldown
                if (THSettings.Instance.BurstKey == 1 &&
                    Burst == false &&
                    Me.Combat &&
                    !DebuffCC(Me))
                {
                    BurstLast = DateTime.Now.AddSeconds(15);
                    Logging.Write("Burst Mode Activated On Cooldown");
                    Burst = true;
                }

                if (THSettings.Instance.BurstKey == 2 &&
                    Burst == false &&
                    Me.Combat &&
                    Attackable(Me.CurrentTarget, 15) &&
                    Me.CurrentTarget.IsBoss &&
                    !DebuffCC(Me))
                {
                    BurstLast = DateTime.Now.AddSeconds(15);
                    Logging.Write("Burst Mode Activated On Cooldown (Boss Only)");
                    Burst = true;
                }

                //Burst on Bloodlust
                if (THSettings.Instance.BurstKey == 3 && Burst == false &&
                    (Me.HasAura("Bloodlust") || Me.HasAura("Heroism") || Me.HasAura("Time Warp") ||
                     Me.HasAura("Ancient Hysteria")) && !DebuffCC(Me))
                {
                    BurstLast = DateTime.Now.AddSeconds(15);
                    Logging.Write("Burst Mode Activated On Bloodlust/Heroism/Time Warp/Ancient Hysteria");
                    Burst = true;
                }

                //Burst On Lose Control
                if (THSettings.Instance.BurstKey == 4 && Burst == false &&
                    DebuffCCDuration(Me, 3000))
                {
                    BurstLast = DateTime.Now.AddSeconds(15);
                    Logging.Write("Burst Mode Activated on Lose Control");
                    Burst = true;
                }

                //Burst On My Health Below
                if (THSettings.Instance.BurstKey == 5 && Burst == false &&
                    Me.HealthPercent < THSettings.Instance.BurstHP && !DebuffCC(Me))
                {
                    BurstLast = DateTime.Now.AddSeconds(15);
                    Logging.Write("Burst Mode Activated on My Health Low");
                    Burst = true;
                }

                //Burst On Enemy Health Below
                if (THSettings.Instance.BurstKey == 6 && Burst == false &&
                    CurrentTargetAttackable(10) &&
                    !Me.CurrentTarget.IsPet &&
                    Me.HealthPercent > THSettings.Instance.UrgentHeal &&
                    Me.CurrentTarget.HealthPercent < THSettings.Instance.BurstHP && !DebuffCC(Me))
                {
                    BurstLast = DateTime.Now.AddSeconds(15);
                    Logging.Write("Burst Mode Activated on Enemy Health Low");
                    Burst = true;
                }

                //Burst On Using Shadow Dance / Killing Spree / Vendetta
                if (THSettings.Instance.BurstKey == 7 && Burst == false &&
                    CurrentTargetAttackable(10) &&
                    !Me.CurrentTarget.IsPet &&
                    (Me.HasAura("Shadow Dance") ||
                     Me.HasAura("Killing Spree") ||
                     MyAura("Vendetta", Me.CurrentTarget)) &&
                    !DebuffCC(Me))
                {
                    BurstLast = DateTime.Now.AddSeconds(15);
                    Logging.Write("Burst Mode Activated on using Cooldown");
                    Burst = true;
                }

                //Burst by key press
                if (THSettings.Instance.BurstKey > 7)
                {
                    //Burst Mode
                    if (GetAsyncKeyState(Keys.LControlKey) < 0 &&
                        GetAsyncKeyState(IndexToKeys(THSettings.Instance.BurstKey - 7)) < 0 &&
                        LastSwitch.AddSeconds(1) < DateTime.Now) // && GetActiveWindowTitle() == "World of Warcraft")
                    {
                        if (Burst)
                        {
                            Burst = false;
                            LastSwitch = DateTime.Now;
                            Logging.Write("Burst Mode is OFF, Hold 1 second Ctrl + " +
                                          IndexToKeys(THSettings.Instance.BurstKey - 6) +
                                          " to Turn Burst Mode ON.");
                            //if (THSettings.Instance.PrintChat)
                            //{
                            //    Lua.DoString("RunMacroText(\"/script msg='Burst Mode OFF' print(msg)\")");
                            //}
                        }
                        else if (!DebuffCC(Me))
                        {
                            Burst = true;
                            LastSwitch = DateTime.Now;
                            BurstLast = DateTime.Now.AddSeconds(15);
                            Logging.Write("Burst Mode is ON for 15 seconds, Hold 1 second Ctrl + " +
                                          IndexToKeys(THSettings.Instance.BurstKey - 6) +
                                          " to Turn Burst Mode OFF.");
                            //if (THSettings.Instance.PrintChat)
                            //{
                            //    Lua.DoString("RunMacroText(\"/script msg='Burst Mode ON' print(msg)\")");
                            //}
                        }
                    }
                }
            }
        }

        #endregion

        #region MainRotation

        private static bool IsOverrideModeOn;
        private static WoWUnit MyLastTarget;
        private static DateTime TickMilisecondTotal;
        private static DateTime DoNotMove;
        private static bool _aoEModeOn;

        private static Composite MainRotation()
        {
            return new PrioritySelector(
                new Action(delegate
                    {
                        CurrentTargetCheck();
                        GlobalCheck();
                        return RunStatus.Failure;
                    }),
                Stealth(),
                StopAttackOOC(),
                Hold(),
                GetUnits(),
                new Decorator(
                    ret => THSettings.Instance.Pause,
                    new Action(delegate { return RunStatus.Success; })),
                new Decorator(
                    ret =>
                    THSettings.Instance.UpdateStatus,
                    new Action(delegate
                        {
                            //Logging.Write("Need Update Status first");
                            //UpdateStatus();
                            return RunStatus.Success;
                        })),
                //Trying to stop bot movement while casting
                new Decorator(
                    ret =>
                    TreeRoot.Current.Name != "Tyrael" &&
                    TreeRoot.Current.Name != "LazyRaider" &&
                    TreeRoot.Current.Name != "Raid Bot" &&
                    TreeRoot.Current.Name != "Combat Bot" &&
                    (Me.IsCasting || DoNotMove > DateTime.Now),
                    new Action(delegate { return RunStatus.Success; })
                    ),
                new Action(delegate
                    {
                        if (!IsOverrideModeOn &&
                            //Do not stop movement in LazyRaider if SMoveToTarget enabled
                            //TreeRoot.Current.Name != "LazyRaider" &&
                            (GetAsyncKeyState(Keys.LButton) < 0 &&
                             GetAsyncKeyState(Keys.RButton) < 0) ||
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.StrafleLeft)) < 0 &&
                            IndexToKeys(THSettings.Instance.StrafleLeft) != Keys.None ||
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.Forward)) < 0 &&
                            IndexToKeys(THSettings.Instance.Forward) != Keys.None ||
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.StrafleRight)) < 0 &&
                            IndexToKeys(THSettings.Instance.StrafleRight) != Keys.None ||
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.TurnLeft)) < 0 &&
                            IndexToKeys(THSettings.Instance.TurnLeft) != Keys.None ||
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.Backward)) < 0 &&
                            IndexToKeys(THSettings.Instance.Backward) != Keys.None ||
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.TurnRight)) < 0 &&
                            IndexToKeys(THSettings.Instance.TurnRight) != Keys.None)
                        {
                            //Logging.Write(LogLevel.Diagnostic, "Override mode on. Stop all bot movement");
                            IsOverrideModeOn = true;
                        }

                        if (IsOverrideModeOn &&
                            //Do not stop movement in LazyRaider if SMoveToTarget enabled
                            //TreeRoot.Current.Name != "LazyRaider" &&
                            (GetAsyncKeyState(Keys.LButton) >= 0 ||
                             GetAsyncKeyState(Keys.RButton) >= 0) &&
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.StrafleLeft)) >= 0 &&
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.Forward)) >= 0 &&
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.StrafleRight)) >= 0 &&
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.TurnLeft)) >= 0 &&
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.Backward)) >= 0 &&
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.TurnRight)) >= 0)
                        {
                            //Logging.Write(LogLevel.Diagnostic, "Override mode off. Bot movement resume");
                            IsOverrideModeOn = false;
                        }

                        if (GetAsyncKeyState(Keys.F9) != 0)
                        {
                            Logging.Write("MainRotation take " +
                                          Math.Round((DateTime.Now -
                                                      TickMilisecondTotal).TotalMilliseconds) +
                                          " ms");
                            TickMilisecondTotal = DateTime.Now;
                        }


                        //Get target back after a fear
                        if (Me.CurrentTarget != null && Me.CurrentTarget.IsValid && MyLastTarget != Me.CurrentTarget)
                        {
                            MyLastTarget = Me.CurrentTarget;
                        }

                        //Clear Target if dead and still in combat
                        if (Me.CurrentTarget != null && !Me.CurrentTarget.IsAlive && Me.Combat)
                        {
                            Lua.DoString("RunMacroText(\"/cleartarget\")");
                        }

                        //Clear Backlist Target
                        if (Me.CurrentTarget != null &&
                            Blacklist.Contains(Me.CurrentTarget.Guid, BlacklistFlags.All))
                        {
                            Lua.DoString("RunMacroText(\"/cleartarget\")");
                        }

                        if (Me.HasAura("Arena Preparation") || Me.HasAura("Preparation"))
                        {
                            UpdateRaidPartyMembers(false);
                        }

                        ControlStunRemove();
                        DisorientsRemove();
                        return RunStatus.Failure;
                    }
                    ),
                SetAutoAttack(),
                UseRacial(),
                UseTrinket(),
                UseProfession(),
                //UpdateRaidPartyMembersComp(),
                new Decorator(
                    ret =>
                    UseRotation == "RogueCombat",
                    CombatRotation()
                    ),
                new Decorator(
                    ret =>
                    UseRotation == "RogueAssassination",
                    AssassinationRotation()
                    ),
                new Decorator(
                    ret =>
                    UseRotation == "RogueSubtlety",
                    SubtletyRotation()
                    ),
                RestRotation(),
                UpdateEventHandlerComp()
                );
        }

        #endregion
    }
}