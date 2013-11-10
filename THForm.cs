using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using Styx.Common;

namespace TuanHARogue
{
    public partial class THForm : Form
    {
        public THForm()
        {
            InitializeComponent();
        }

        private void THDKForm_Load(object sender, EventArgs e)
        {
            if (
                File.Exists(Utilities.AssemblyDirectory +
                            @"\Routines\TuanHARogueOpenBeta\TuanHA-Rogue-Picture.jpg"))
            {
                pictureBox1.ImageLocation = Utilities.AssemblyDirectory +
                                            @"\Routines\TuanHARogueOpenBeta\TuanHA-Rogue-Picture.jpg";
            }

            if (File.Exists(Utilities.AssemblyDirectory +
                            @"\Routines\TuanHARogueOpenBeta\SpecialThanks.rtf"))
            {
                richTextBox1.LoadFile(Utilities.AssemblyDirectory +
                                      @"\Routines\TuanHARogueOpenBeta\SpecialThanks.rtf");
            }

            AdrenalineRushCD.Checked = THSettings.Instance.AdrenalineRushCD;
            AdrenalineRushBurst.Checked = THSettings.Instance.AdrenalineRushBurst;
            Ambush.Checked = THSettings.Instance.Ambush;
            AutoAoE.Checked = THSettings.Instance.AutoAoE;
            AutoFace.Checked = THSettings.Instance.AutoFace;
            //AutoAttack.Checked = THSettings.Instance.AutoAttack;
            AutoMove.Checked = THSettings.Instance.AutoMove;
            AutoRacial.Checked = THSettings.Instance.AutoRacial;
            AutoTarget.Checked = THSettings.Instance.AutoTarget;
            AttackASAP.Checked = THSettings.Instance.AttackASAP;
            AttackOOC.Checked = THSettings.Instance.AttackOOC;
            Backstab.Checked = THSettings.Instance.Backstab;
            Backward.SelectedIndex = THSettings.Instance.Backward;
            BurstKey.SelectedIndex = THSettings.Instance.BurstKey;
            BurstHP.Value = THSettings.Instance.BurstHP;
            BurstofSpeed.Checked = THSettings.Instance.BurstofSpeed;
            BurstofSpeedDistance.Value = THSettings.Instance.BurstofSpeedDistance;
            BurstofSpeedEnergy.Checked = THSettings.Instance.BurstofSpeedEnergy;
            BurstofSpeedEnergyNumber.Value = THSettings.Instance.BurstofSpeedEnergyNumber;
            CheapShot.Checked = THSettings.Instance.CheapShot;
            CheapShotInterrupt.Checked = THSettings.Instance.CheapShotInterrupt;
            CheapShotInterruptTimeLeft.Value = THSettings.Instance.CheapShotInterruptTimeLeft;
            CloakofShadows.Checked = THSettings.Instance.CloakofShadows;
            CloakofShadowsHP.Value = THSettings.Instance.CloakofShadowsHP;
            CloakofShadowsTarget.Checked = THSettings.Instance.CloakofShadowsTarget;
            CloakofShadowsTargetUnit.Value = THSettings.Instance.CloakofShadowsTargetUnit;
            CloakofShadowsRoot.Checked = THSettings.Instance.CloakofShadowsRoot;
            CombatReadiness.Checked = THSettings.Instance.CombatReadiness;
            CombatReadinessHP.Value = THSettings.Instance.CombatReadinessHP;
            CombatReadinessTarget.Checked = THSettings.Instance.CombatReadinessTarget;
            CombatReadinessTargetUnit.Value = THSettings.Instance.CombatReadinessTargetUnit;
            CrimsonTempest.Checked = THSettings.Instance.CrimsonTempest;
            CrimsonTempestCP.Value = THSettings.Instance.CrimsonTempestCP;
            DeadlyThrowInterrupt.Checked = THSettings.Instance.DeadlyThrowInterrupt;
            DeadlyThrowInterruptTimeLeft.Value = THSettings.Instance.DeadlyThrowInterruptTimeLeft;
            DeadlyThrowSlow.Checked = THSettings.Instance.DeadlyThrowSlow;
            DeadlyThrowSlowDistance.Value = THSettings.Instance.DeadlyThrowSlowDistance;
            DeadlyThrowFinish.Checked = THSettings.Instance.DeadlyThrowFinish;
            DeadlyThrowFinishDistance.Value = THSettings.Instance.DeadlyThrowFinishDistance;
            Dismantle.Checked = THSettings.Instance.Dismantle;
            DistmantleCooldown.Checked = THSettings.Instance.Dismantle;
            DismantlePre.Checked = THSettings.Instance.DismantlePre;
            DismantleHP.Value = THSettings.Instance.DismantleHP;
            DismantleTarget.Checked = THSettings.Instance.DismantleTarget;
            DismantleTargetUnit.Value = THSettings.Instance.DismantleTargetUnit;
            DisarmTrap.Checked = THSettings.Instance.DisarmTrap;
            Distract.Checked = THSettings.Instance.Distract;
            Evasion.Checked = THSettings.Instance.Evasion;
            EvasionPre.Checked = THSettings.Instance.EvasionPre;
            EvasionHP.Value = THSettings.Instance.EvasionHP;
            EvasionTarget.Checked = THSettings.Instance.EvasionTarget;
            EvasionHPTargetUnit.Value = THSettings.Instance.EvasionTargetUnit;
            Eviscerate.Checked = THSettings.Instance.Eviscerate;
            EviscerateCP.Value = THSettings.Instance.EviscerateCP;
            ExposeArmor.Checked = THSettings.Instance.ExposeArmor;
            FanofKnives.Checked = THSettings.Instance.FanofKnives;
            FanofKnivesUnit.Value = THSettings.Instance.FanofKnivesUnit;
            Feint.Checked = THSettings.Instance.Feint;
            FeintHP.Value = THSettings.Instance.FeintHP;
            FeintTarget.Checked = THSettings.Instance.FeintTarget;
            FeintTargetUnit.Value = THSettings.Instance.FeintTargetUnit;
            Forward.SelectedIndex = THSettings.Instance.Forward;
            Garrote.Checked = THSettings.Instance.Garrote;
            Gouge.Checked = THSettings.Instance.Gouge;
            GougeTimeLeft.Value = THSettings.Instance.GougeTimeLeft;
            GougePet.Checked = THSettings.Instance.GougePet;
            GougeHelpFriend.Checked = THSettings.Instance.GougeHelpFriend;
            GougeHelpFriendHP.Value = THSettings.Instance.GougeHelpFriendHP;
            GougeOffensiveCooldown.Checked = THSettings.Instance.GougeOffensiveCooldown;
            HealthStone.Checked = THSettings.Instance.HealthStone;
            HealthStoneHP.Value = THSettings.Instance.HealthStoneHP;
            InterruptTarget.Checked = THSettings.Instance.InterruptTarget;
            InterruptFocus.Checked = THSettings.Instance.InterruptFocus;
            InterruptAll.Checked = THSettings.Instance.InterruptAll;
            Kick.Checked = THSettings.Instance.Kick;
            KickTimeLeft.Value = THSettings.Instance.KickTimeLeft;
            KidneyShot.Checked = THSettings.Instance.KidneyShot;
            KidneyShotCP.Value = THSettings.Instance.KidneyShotCP;
            KidneyShotInterrupt.Checked = THSettings.Instance.KidneyShotInterrupt;
            KidneyShotInterruptTimeLeft.Value = THSettings.Instance.KidneyShotInterruptTimeLeft;
            KillingSpreeCD.Checked = THSettings.Instance.KillingSpreeCD;
            KillingSpreeBurst.Checked = THSettings.Instance.KillingSpreeBurst;
            MarkedforDeathCD.Checked = THSettings.Instance.MarkedforDeathCD;
            MarkedforDeathBurst.Checked = THSettings.Instance.MarkedforDeathBurst;
            MarkedforDeathLow.Checked = THSettings.Instance.MarkedforDeathLow;
            MarkedforDeathLowHP.Value = THSettings.Instance.MarkedforDeathLowHP;
            PauseKey.SelectedIndex = THSettings.Instance.PauseKey;
            PickPocket.Checked = THSettings.Instance.PickPocket;
            PoisonLethal.SelectedIndex = THSettings.Instance.PoisonLethal;
            PoisonNonLethal.SelectedIndex = THSettings.Instance.PoisonNonLethal;
            PoolEnergy.Checked = THSettings.Instance.PoolEnergy;
            PoolEnergyPC.Value = THSettings.Instance.PoolEnergyPC;
            Preparation.Checked = THSettings.Instance.Preparation;
            ProfBuff.SelectedIndex = THSettings.Instance.ProfBuff;
            ProfBuffHP.Value = THSettings.Instance.ProfBuffHP;
            Recuperate.Checked = THSettings.Instance.Recuperate;
            RecuperateHP.Value = THSettings.Instance.RecuperateHP;
            Redirect.Checked = THSettings.Instance.Redirect;
            Rupture.Checked = THSettings.Instance.Rupture;
            RuptureCP.Value = THSettings.Instance.RuptureCP;
            SapTarget.Checked = THSettings.Instance.SapTarget;
            SapFocus.Checked = THSettings.Instance.SapFocus;
            SapRogueDruid.Checked = THSettings.Instance.SapRogueDruid;
            SapAny.Checked = THSettings.Instance.SapAny;
            SearchInterval.Value = THSettings.Instance.SearchInterval;
            ShadowDanceCD.Checked = THSettings.Instance.ShadowDanceCD;
            ShadowDance.Checked = THSettings.Instance.ShadowDance;
            ShadowDanceEnergy.Value = THSettings.Instance.ShadowDanceEnergy;
            ShadowDanceBurst.Checked = THSettings.Instance.ShadowDanceBurst;
            ShadowDanceSap.Checked = THSettings.Instance.ShadowDanceSap;
            ShadowBladesCD.Checked = THSettings.Instance.ShadowBladesCD;
            ShadowBladeSync.Checked = THSettings.Instance.ShadowBladeSync;
            ShadowBladesBurst.Checked = THSettings.Instance.ShadowBladesBurst;
            Shadowstep.Checked = THSettings.Instance.Shadowstep;
            ShadowstepDistance.Value = THSettings.Instance.ShadowstepDistance;
            ShadowWalk.Checked = THSettings.Instance.ShadowWalk;
            ShivEnrage.Checked = THSettings.Instance.ShivEnrage;
            ShivCripplingPoison.Checked = THSettings.Instance.ShivCripplingPoison;
            ShivMindNumbing.Checked = THSettings.Instance.ShivMindNumbing;
            ShivLeechingPoison.Checked = THSettings.Instance.ShivLeechingPoison;
            ShivLeechingPoisonHP.Value = THSettings.Instance.ShivLeechingPoisonHP;
            ShivParalystic.Checked = THSettings.Instance.ShivParalystic;
            ShroudofConcealment.Checked = THSettings.Instance.ShroudofConcealment;
            ShroudofConcealmentNumber.Value = THSettings.Instance.ShroudofConcealmentNumber;
            ShurikenToss.Checked = THSettings.Instance.ShurikenToss;
            ShurikenTossDistance.Value = THSettings.Instance.ShurikenTossDistance;
            SliceandDice.Checked = THSettings.Instance.SliceandDice;
            SliceandDiceCP.Value = THSettings.Instance.SliceandDiceCP;
            SmokeBomb.Checked = THSettings.Instance.SmokeBomb;
            SmokeBombHP.Value = THSettings.Instance.SmokeBombHP;
            SmokeBombEnemyUnit.Value = THSettings.Instance.SmokeBombEnemyUnit;
            Sprint.Checked = THSettings.Instance.Sprint;
            SprintDistance.Value = THSettings.Instance.SprintDistance;
            SprintPre.Checked = THSettings.Instance.SprintPre;
            Stealth.Checked = THSettings.Instance.Stealth;
            StealthForce.Checked = THSettings.Instance.StealthForce;
            StrafleLeft.SelectedIndex = THSettings.Instance.StrafleLeft;
            StrafleRight.SelectedIndex = THSettings.Instance.StrafleRight;
            TricksoftheTrade.Checked = THSettings.Instance.TricksoftheTrade;
            TricksoftheTradeFocus.Checked = THSettings.Instance.TricksoftheTradeFocus;
            TricksoftheTradeAny.Checked = THSettings.Instance.TricksoftheTradeAny;
            Trinket1.SelectedIndex = THSettings.Instance.Trinket1;
            Trinket1HP.Value = THSettings.Instance.Trinket1HP;
            Trinket2.SelectedIndex = THSettings.Instance.Trinket2;
            Trinket2HP.Value = THSettings.Instance.Trinket2HP;
            TurnRight.SelectedIndex = THSettings.Instance.TurnRight;
            TurnLeft.SelectedIndex = THSettings.Instance.TurnLeft;
            UnittoStartAoE.Value = THSettings.Instance.UnittoStartAoE;
            UseFood.Checked = THSettings.Instance.UseFood;
            UseFoodHP.Value = THSettings.Instance.UseFoodHP;
            Vanish.Checked = THSettings.Instance.Vanish;
            VanishHP.Value = THSettings.Instance.VanishHP;
            VanishPre.Checked = THSettings.Instance.VanishPre;
            VanishCD.Checked = THSettings.Instance.VanishCD;
            VanishBurst.Checked = THSettings.Instance.VanishBurst;
            VanishSap.Checked = THSettings.Instance.VanishSap;
            VendettaCD.Checked = THSettings.Instance.VendettaCD;
            VendettaBurst.Checked = THSettings.Instance.VendettaBurst;
        }

        public string LastSavedMode = "";

        private void OK_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("TuanHA Paladin Settings have been saved", "Save");
            THSettings.Instance.LastSavedMode = LastSavedMode;
            THSettings.Instance.LastSavedSpec = Classname.GetCurrentSpec();
            THSettings.Instance.Save();
            Logging.Write("----------------------------------");
            Logging.Write("TuanHA Rogue Settings have been saved");

            Logging.Write(LogLevel.Diagnostic, "Your Setting for Debug Purpose Only");
            foreach (var var in THSettings.Instance.GetSettings())
            {
                Logging.Write(LogLevel.Diagnostic, var.ToString());
            }

            Close();

            THSettings.Instance.UpdateStatus = true;
        }

        private void Apply_Click(object sender, EventArgs e)
        {
            THSettings.Instance.LastSavedMode = LastSavedMode;
            THSettings.Instance.LastSavedSpec = Classname.GetCurrentSpec();
            THSettings.Instance.Save();
            Logging.Write("TuanHA Rogue Settings have been saved");
            THSettings.Instance.UpdateStatus = true;
        }


        public void Dungeon_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                Classname.GetCurrentSpec() + " Rogue " +
                "Dungeon Mode: Work best with Tyrael/LazyRaider in Dungeon.",
                //\r\n\r\nMake sure ONLY 3 Options Enabled in LazyRaider:\r\n - Run Without a Tank\r\n - Disable Plug-ins\r\n - Frame Lock",
                "Important Note",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            Logging.Write("----------------------------------");
            Logging.Write("Dungeon Mode On");
            Logging.Write("----------------------------------");

            if (File.Exists(Utilities.AssemblyDirectory +
                            @"\Routines\TuanHARogueOpenBeta\Preset\TuanHA Rogue Defaut Dungeon.xml"))
            {
                Logging.Write("----------------------------------");
                THSettings.Instance.LoadFromXML(XElement.Load(Utilities.AssemblyDirectory +
                                                              @"\Routines\TuanHARogueOpenBeta\Preset\TuanHA Rogue Defaut Dungeon.xml"));
                Logging.Write("Load TuanHA Rogue Defaut Dungeon Settings from a file complete");
                Logging.Write("----------------------------------");
                THDKForm_Load(null, null);
            }
            else
            {
                Logging.Write("----------------------------------");
                Logging.Write("File not exists: " + Utilities.AssemblyDirectory +
                              @"\Routines\TuanHARogueOpenBeta\Preset\TuanHA Rogue Defaut Dungeon.xml");
                Logging.Write("Load TuanHA Rogue Defaut AFK Dungeon from a file fail.");
                Logging.Write("----------------------------------");
            }
        }


        private void Raid_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                Classname.GetCurrentSpec() + " Rogue " +
                "Raid Mode: Work best with Tyrael/LazyRaider in Raid.",
                //\r\n\r\nMake sure ONLY 3 Options Enabled in LazyRaider:\r\n - Run Without a Tank\r\n - Disable Plug-ins\r\n - Frame Lock",
                "Important Note",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            Logging.Write("----------------------------------");
            Logging.Write("Raid Mode On");
            Logging.Write("----------------------------------");

            if (File.Exists(Utilities.AssemblyDirectory +
                            @"\Routines\TuanHARogueOpenBeta\Preset\TuanHA Rogue Defaut Raid.xml"))
            {
                Logging.Write("----------------------------------");
                THSettings.Instance.LoadFromXML(XElement.Load(Utilities.AssemblyDirectory +
                                                              @"\Routines\TuanHARogueOpenBeta\Preset\TuanHA Rogue Defaut Raid.xml"));
                Logging.Write("Load TuanHA Rogue Defaut Raid Settings from a file complete");
                Logging.Write("----------------------------------");
                THDKForm_Load(null, null);
            }
            else
            {
                Logging.Write("----------------------------------");
                Logging.Write("File not exists: " + Utilities.AssemblyDirectory +
                              @"\Routines\TuanHARogueOpenBeta\Preset\TuanHA Rogue Defaut Raid.xml");
                Logging.Write("Load TuanHA Rogue Defaut AFK Raid from a file fail.");
                Logging.Write("----------------------------------");
            }
        }


        private void PvP_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                Classname.GetCurrentSpec() + " Rogue " +
                "PvP Mode: Work best with Tyrael/LazyRaider in PvP.",
                //\r\n\r\nMake sure ONLY 3 Options Enabled in LazyRaider:\r\n - Run Without a Tank\r\n - Disable Plug-ins\r\n - Frame Lock",
                "Important Note",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            Logging.Write("----------------------------------");
            Logging.Write("PvP Mode On");
            Logging.Write("----------------------------------");

            if (File.Exists(Utilities.AssemblyDirectory +
                            @"\Routines\TuanHARogueOpenBeta\Preset\TuanHA Rogue Defaut PvP.xml"))
            {
                Logging.Write("----------------------------------");
                THSettings.Instance.LoadFromXML(XElement.Load(Utilities.AssemblyDirectory +
                                                              @"\Routines\TuanHARogueOpenBeta\Preset\TuanHA Rogue Defaut PvP.xml"));
                Logging.Write("Load TuanHA Rogue Defaut PvP Settings from a file complete");
                Logging.Write("----------------------------------");
                THDKForm_Load(null, null);
            }
            else
            {
                Logging.Write("----------------------------------");
                Logging.Write("File not exists: " + Utilities.AssemblyDirectory +
                              @"\Routines\TuanHARogueOpenBeta\Preset\TuanHA Rogue Defaut PvP.xml");
                Logging.Write("Load TuanHA Rogue Defaut AFK PvP from a file fail.");
                Logging.Write("----------------------------------");
            }
        }

        private void Arena_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                Classname.GetCurrentSpec() + " Rogue " +
                "Arena Mode: Work best with Tyrael/LazyRaider in Arena.",
                //\r\n\r\nMake sure ONLY 3 Options Enabled in LazyRaider:\r\n - Run Without a Tank\r\n - Disable Plug-ins\r\n - Frame Lock",
                "Important Note",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            Logging.Write("----------------------------------");
            Logging.Write("Arena Mode On");
            Logging.Write("----------------------------------");

            if (File.Exists(Utilities.AssemblyDirectory +
                            @"\Routines\TuanHARogueOpenBeta\Preset\TuanHA Rogue Defaut Arena.xml"))
            {
                Logging.Write("----------------------------------");
                THSettings.Instance.LoadFromXML(XElement.Load(Utilities.AssemblyDirectory +
                                                              @"\Routines\TuanHARogueOpenBeta\Preset\TuanHA Rogue Defaut Arena.xml"));
                Logging.Write("Load TuanHA Rogue Defaut Arena Settings from a file complete");
                Logging.Write("----------------------------------");
                THDKForm_Load(null, null);
            }
            else
            {
                Logging.Write("----------------------------------");
                Logging.Write("File not exists: " + Utilities.AssemblyDirectory +
                              @"\Routines\TuanHARogueOpenBeta\Preset\TuanHA Rogue Defaut Arena.xml");
                Logging.Write("Load TuanHA Rogue Defaut AFK Arena from a file fail.");
                Logging.Write("----------------------------------");
            }
        }

        private void AFK_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                Classname.GetCurrentSpec() + " Rogue " +
                "Full AFK Mode: Work best with BGBuddy, ArcheologyBuddy, DungeonBuddy, Grind Bot, Questing...",
                "Important Note", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Logging.Write("----------------------------------");
            Logging.Write("Full AFK Mode On");
            Logging.Write("----------------------------------");

            if (File.Exists(Utilities.AssemblyDirectory +
                            @"\Routines\TuanHARogueOpenBeta\Preset\TuanHA Rogue Defaut AFK.xml"))
            {
                Logging.Write("----------------------------------");
                THSettings.Instance.LoadFromXML(XElement.Load(Utilities.AssemblyDirectory +
                                                              @"\Routines\TuanHARogueOpenBeta\Preset\TuanHA Rogue Defaut AFK.xml"));
                Logging.Write("Load TuanHA Rogue Defaut AFK Settings from a file complete");
                Logging.Write("----------------------------------");
                THDKForm_Load(null, null);
            }
            else
            {
                Logging.Write("----------------------------------");
                Logging.Write("File not exists: " + Utilities.AssemblyDirectory +
                              @"\Routines\TuanHARogueOpenBeta\Preset\TuanHA Rogue Defaut AFK.xml");
                Logging.Write("Load TuanHA Rogue Defaut AFK Settings from a file fail.");
                Logging.Write("----------------------------------");
            }
        }

        private void SaveSetting_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Setting File|*.xml";
            saveFileDialog.Title = "Save Setting to a File";
            saveFileDialog.InitialDirectory = Utilities.AssemblyDirectory +
                                              @"\Routines\TuanHARogueOpenBeta\User Settings\";
            saveFileDialog.DefaultExt = "xml";
            saveFileDialog.FileName = "TuanHA Rogue " + Classname.GetCurrentSpec();

            saveFileDialog.ShowDialog();

            //if (DialogResult != DialogResult.Abort &&
            //    DialogResult != DialogResult.Ignore &&
            //    DialogResult != DialogResult.No &&
            //    DialogResult != DialogResult.None &&
            //    DialogResult != DialogResult.Retry &&
            //    DialogResult != DialogResult.Cancel)
            //{
            //    Logging.Write(DialogResult.ToString());
            //    return;
            //}

            if (saveFileDialog.FileName.Contains(".xml"))
            {
                //Logging.Write(DialogResult.ToString());
                THSettings.Instance.SaveToFile(saveFileDialog.FileName);
                Logging.Write("----------------------------------");
                Logging.Write("Save Setting to: " + saveFileDialog.FileName);
                Logging.Write("----------------------------------");
            }
        }

        private void LoadSetting_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
                {
                    Filter = "Setting File|*.xml",
                    Title = "Load Setting from a File",
                    InitialDirectory = Utilities.AssemblyDirectory + @"\Routines\TuanHARogueOpenBeta\User Settings\"
                };
            openFileDialog.ShowDialog();

            //if (DialogResult != DialogResult.OK)
            //{
            //    Logging.Write(DialogResult.ToString());
            //    return;
            //}

            if (openFileDialog.FileName.Contains(".xml"))
            {
                //Logging.Write(DialogResult.ToString());
                THSettings.Instance.LoadFromXML(XElement.Load(openFileDialog.FileName));
                Logging.Write("----------------------------------");
                Logging.Write("Load Setting from: " + openFileDialog.FileName);
                Logging.Write("----------------------------------");
                THDKForm_Load(null, null);
            }
        }


        private void Trinket1_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 7; i++)
            {
                if (Trinket1.SelectedIndex == i)
                {
                    THSettings.Instance.Trinket1 = i;
                }
            }
        }

        private void Trinket1HP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Trinket1HP = (int) Trinket1HP.Value;
        }

        private void Trinket2_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 7; i++)
            {
                if (Trinket2.SelectedIndex == i)
                {
                    THSettings.Instance.Trinket2 = i;
                }
            }
        }

        private void Trinket2HP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Trinket2HP = (int) Trinket2HP.Value;
        }

        private void ProfBuff_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 7; i++)
            {
                if (ProfBuff.SelectedIndex == i)
                {
                    THSettings.Instance.ProfBuff = i;
                }
            }
        }

        private void ProfBuffHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ProfBuffHP = (int) ProfBuffHP.Value;
        }

        private void BurstKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 46; i++)
            {
                if (BurstKey.SelectedIndex == i)
                {
                    THSettings.Instance.BurstKey = i;
                }
            }
        }

        private void BurstHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.BurstHP = (int) BurstHP.Value;
        }

        private void HealthStone_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealthStoneHP = (int) HealthStoneHP.Value;
        }


        private void UnittoStartAoE_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.UnittoStartAoE = (int) UnittoStartAoE.Value;
        }


        private void Face_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AutoFace = AutoFace.Checked;
        }

        private void AutoTarget_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AutoTarget = AutoTarget.Checked;
        }

        private void AttackOOC_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AttackOOC = AttackOOC.Checked;
        }

        private void StartEating_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.UseFoodHP = (int) UseFoodHP.Value;
        }

        private void PauseKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (PauseKey.SelectedIndex == i)
                {
                    THSettings.Instance.PauseKey = i;
                }
            }
        }

        private void StrafleLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (StrafleLeft.SelectedIndex == i)
                {
                    THSettings.Instance.StrafleLeft = i;
                }
            }
        }

        private void Forward_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (Forward.SelectedIndex == i)
                {
                    THSettings.Instance.Forward = i;
                }
            }
        }

        private void StrafleRight_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (StrafleRight.SelectedIndex == i)
                {
                    THSettings.Instance.StrafleRight = i;
                }
            }
        }

        private void TurnLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (TurnLeft.SelectedIndex == i)
                {
                    THSettings.Instance.TurnLeft = i;
                }
            }
        }

        private void Backward_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (Backward.SelectedIndex == i)
                {
                    THSettings.Instance.Backward = i;
                }
            }
        }

        private void TurnRight_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (TurnRight.SelectedIndex == i)
                {
                    THSettings.Instance.TurnRight = i;
                }
            }
        }


        private void HealthStone_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealthStone = HealthStone.Checked;
        }


        private void UseFood_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.UseFood = UseFood.Checked;
        }

        private void AutoMove_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AutoMove = AutoMove.Checked;
        }


        private void AutoRacial_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AutoRacial = AutoRacial.Checked;
        }


        private void AutoAoE_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AutoAoE = AutoAoE.Checked;
        }

        private void SearchInterval_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.SearchInterval = (int) SearchInterval.Value;
        }


        private void InterruptTarget_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.InterruptTarget = InterruptTarget.Checked;
        }

        private void InterruptFocus_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.InterruptFocus = InterruptFocus.Checked;
        }

        private void AttackASAP_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AttackASAP = AttackASAP.Checked;
        }

        private void Backstab_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Backstab = Backstab.Checked;
        }

        private void PoisonLethal_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 2; i++)
            {
                if (PoisonLethal.SelectedIndex == i)
                {
                    THSettings.Instance.PoisonLethal = i;
                }
            }
        }

        private void PoisonNonLethal_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 4; i++)
            {
                if (PoisonNonLethal.SelectedIndex == i)
                {
                    THSettings.Instance.PoisonNonLethal = i;
                }
            }
        }

        private void PoolEnergy_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.PoolEnergy = PoolEnergy.Checked;
        }

        private void PoolEnergyPC_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.PoolEnergyPC = (int) PoolEnergyPC.Value;
        }

        private void Dismantle_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Dismantle = Dismantle.Checked;
        }

        private void DismantleHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.DismantleHP = (int) DismantleHP.Value;
        }

        private void DismantleTarget_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.DismantleTarget = DismantleTarget.Checked;
        }

        private void DismantleTargetUnit_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.DismantleTargetUnit = (int) DismantleTargetUnit.Value;
        }

        private void Evasion_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Evasion = Evasion.Checked;
        }

        private void EvasionHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.EvasionHP = (int) EvasionHP.Value;
        }

        private void EvasionTarget_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.EvasionTarget = EvasionTarget.Checked;
        }

        private void EvasionHPTargetUnit_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.EvasionTargetUnit = (int) EvasionHPTargetUnit.Value;
        }

        private void CombatReadiness_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CombatReadiness = CombatReadiness.Checked;
        }

        private void CombatReadinessHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CombatReadinessHP = (int) CombatReadinessHP.Value;
        }

        private void CombatReadinessTarget_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CombatReadinessTarget = CombatReadinessTarget.Checked;
        }

        private void CombatReadinessTargetUnit_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CombatReadinessTargetUnit = (int) CombatReadinessTargetUnit.Value;
        }

        private void Kick_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Kick = Kick.Checked;
        }

        private void KickTimeLeft_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.KickTimeLeft = (int) KickTimeLeft.Value;
        }

        private void Gouge_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Gouge = Gouge.Checked;
        }

        private void GougeTimeLeft_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.GougeTimeLeft = (int) GougeTimeLeft.Value;
        }

        private void CloakofShadowsTarget_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CloakofShadowsTarget = CloakofShadowsTarget.Checked;
        }

        private void CloakofShadowsTargetUnit_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CloakofShadowsTargetUnit = (int) CloakofShadowsTargetUnit.Value;
        }

        private void CloakofShadows_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CloakofShadows = CloakofShadows.Checked;
        }

        private void CloakofShadowsHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CloakofShadowsHP = (int) CloakofShadowsHP.Value;
        }

        private void FanofKnives_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.FanofKnives = FanofKnives.Checked;
        }

        private void FanofKnivesUnit_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.FanofKnivesUnit = (int) FanofKnivesUnit.Value;
        }

        private void Ambush_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Ambush = Ambush.Checked;
        }

        private void CheapShot_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CheapShot = CheapShot.Checked;
        }

        private void Garrote_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Garrote = Garrote.Checked;
        }

        private void CrimsonTempest_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CrimsonTempest = CrimsonTempest.Checked;
        }

        private void Eviscerate_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Eviscerate = Eviscerate.Checked;
        }

        private void KidneyShot_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.KidneyShot = KidneyShot.Checked;
        }

        private void Rupture_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Rupture = Rupture.Checked;
        }

        private void SliceandDice_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.SliceandDice = SliceandDice.Checked;
        }

        private void Recuperate_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Recuperate = Recuperate.Checked;
        }

        private void RecuperateHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.RecuperateHP = (int) RecuperateHP.Value;
        }

        private void CrimsonTempestCP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CrimsonTempestCP = (int) CrimsonTempestCP.Value;
        }

        private void EviscerateCP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.EviscerateCP = (int) EviscerateCP.Value;
        }

        private void KidneyShotCP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.KidneyShotCP = (int) KidneyShotCP.Value;
        }

        private void RuptureCP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.RuptureCP = (int) RuptureCP.Value;
        }

        private void SliceandDiceCP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.SliceandDiceCP = (int) SliceandDiceCP.Value;
        }

        private void KidneyShotInterrupt_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.KidneyShotInterrupt = KidneyShotInterrupt.Checked;
        }

        private void KidneyShotInterruptTimeLeft_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.KidneyShotInterruptTimeLeft = (int) KidneyShotInterruptTimeLeft.Value;
        }

        private void Feint_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Feint = Feint.Checked;
        }

        private void FeintHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.FeintHP = (int) FeintHP.Value;
        }

        private void FeintTarget_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.FeintTarget = FeintTarget.Checked;
        }

        private void FeintTargetUnit_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.FeintTargetUnit = (int) FeintTargetUnit.Value;
        }

        private void KillingSpreeCD_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.KillingSpreeCD = KillingSpreeCD.Checked;
        }

        private void KillingSpreeBurst_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.KillingSpreeBurst = KillingSpreeBurst.Checked;
        }

        private void MarkedforDeathCD_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.MarkedforDeathCD = MarkedforDeathCD.Checked;
        }

        private void MarkedforDeathBurst_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.MarkedforDeathBurst = MarkedforDeathBurst.Checked;
        }

        private void VendettaCD_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.VendettaCD = VendettaCD.Checked;
        }

        private void VendettaBurst_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.VendettaBurst = VendettaBurst.Checked;
        }

        private void ShadowDanceCD_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShadowDanceCD = ShadowDanceCD.Checked;
        }

        private void ShadowBladesCD_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShadowBladesCD = ShadowBladesCD.Checked;
        }

        private void ShadowBladesBurst_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShadowBladesBurst = ShadowBladesBurst.Checked;
        }

        private void VanishCD_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.VanishCD = VanishCD.Checked;
        }

        private void VanishBurst_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.VanishBurst = VanishBurst.Checked;
        }

        private void Preparation_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Preparation = Preparation.Checked;
        }

        private void DismantlePre_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.DismantlePre = DismantlePre.Checked;
        }

        private void EvasionPre_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.EvasionPre = EvasionPre.Checked;
        }

        private void SprintPre_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.SprintPre = SprintPre.Checked;
        }

        private void VanishPre_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.VanishPre = VanishPre.Checked;
        }

        private void ShadowDanceBurst_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShadowDanceBurst = ShadowDanceBurst.Checked;
        }

        private void Vanish_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Vanish = Vanish.Checked;
        }

        private void VanishHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.VanishHP = (int) VanishHP.Value;
        }

        private void ShivEnrage_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShivEnrage = ShivEnrage.Checked;
        }

        private void DeadlyThrowInterrupt_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.DeadlyThrowInterrupt = DeadlyThrowInterrupt.Checked;
        }

        private void DeadlyThrowInterruptTimeLeft_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.DeadlyThrowInterruptTimeLeft = (int) DeadlyThrowInterruptTimeLeft.Value;
        }

        private void CheapShotInterrupt_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CheapShotInterrupt = CheapShotInterrupt.Checked;
        }

        private void CheapShotInterruptTimeLeft_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CheapShotInterruptTimeLeft = (int) CheapShotInterruptTimeLeft.Value;
        }

        private void DeadlyThrowSlow_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.DeadlyThrowSlow = DeadlyThrowSlow.Checked;
        }

        private void DeadlyThrowSlowDistance_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.DeadlyThrowSlowDistance = (int) DeadlyThrowSlowDistance.Value;
        }

        private void DeadlyThrowFinish_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.DeadlyThrowFinish = DeadlyThrowFinish.Checked;
        }

        private void DeadlyThrowFinishDistance_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.DeadlyThrowFinishDistance = (int) DeadlyThrowFinishDistance.Value;
        }

        private void ShurikenToss_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShurikenToss = ShurikenToss.Checked;
        }

        private void ShurikenTossDistance_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShurikenTossDistance = (int) ShurikenTossDistance.Value;
        }

        private void BurstofSpeed_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.BurstofSpeed = BurstofSpeed.Checked;
        }

        private void BurstofSpeedDistance_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.BurstofSpeedDistance = (int) BurstofSpeedDistance.Value;
        }

        private void Shadowstep_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Shadowstep = Shadowstep.Checked;
        }

        private void ShadowstepDistance_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShadowstepDistance = (int) ShadowstepDistance.Value;
        }

        private void DistmantleCooldown_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.DistmantleCooldown = DistmantleCooldown.Checked;
        }

        private void BurstofSpeedEnergy_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.BurstofSpeedEnergy = BurstofSpeedEnergy.Checked;
        }

        private void BurstofSpeedEnergyNumber_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.BurstofSpeedEnergyNumber = (int) BurstofSpeedEnergyNumber.Value;
        }

        private void DisarmTrap_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.DisarmTrap = DisarmTrap.Checked;
        }

        private void Distract_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Distract = Distract.Checked;
        }

        private void ExposeArmor_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ExposeArmor = ExposeArmor.Checked;
        }

        private void GougePet_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.GougePet = GougePet.Checked;
        }

        private void GougeHelpFriend_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.GougeHelpFriend = GougeHelpFriend.Checked;
        }

        private void GougeHelpFriendHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.GougeHelpFriendHP = (int) GougeHelpFriendHP.Value;
        }

        private void PickPocket_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.PickPocket = PickPocket.Checked;
        }

        private void Redirect_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Redirect = Redirect.Checked;
        }

        private void SapTarget_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.SapTarget = SapTarget.Checked;
        }

        private void SapFocus_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.SapFocus = SapFocus.Checked;
        }

        private void SapRogueDruid_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.SapRogueDruid = SapRogueDruid.Checked;
        }

        private void SapAny_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.SapAny = SapAny.Checked;
        }

        private void ShadowWalk_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShadowWalk = ShadowWalk.Checked;
        }

        private void ShivCripplingPoison_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShivCripplingPoison = ShivCripplingPoison.Checked;
        }

        private void ShivMindNumbing_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShivMindNumbing = ShivMindNumbing.Checked;
        }

        private void ShivLeechingPoison_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShivLeechingPoison = ShivLeechingPoison.Checked;
        }

        private void ShivLeechingPoisonHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShivLeechingPoisonHP = (int) ShivLeechingPoisonHP.Value;
        }

        private void ShivParalystic_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShivParalystic = ShivParalystic.Checked;
        }

        private void ShroudofConcealment_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShroudofConcealment = ShroudofConcealment.Checked;
        }

        private void ShroudofConcealmentNumber_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShroudofConcealmentNumber = (int) ShroudofConcealmentNumber.Value;
        }

        private void SmokeBomb_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.SmokeBomb = SmokeBomb.Checked;
        }

        private void SmokeBombHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.SmokeBombHP = (int) SmokeBombHP.Value;
        }

        private void SmokeBombEnemyUnit_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.SmokeBombEnemyUnit = (int) SmokeBombEnemyUnit.Value;
        }

        private void Sprint_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Sprint = Sprint.Checked;
        }

        private void SprintDistance_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.SprintDistance = (int) SprintDistance.Value;
        }

        private void TricksoftheTradeFocus_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.TricksoftheTradeFocus = TricksoftheTradeFocus.Checked;
        }

        private void TricksoftheTradeAny_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.TricksoftheTradeAny = TricksoftheTradeAny.Checked;
        }

        private void GougeOffensiveCooldown_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.GougeOffensiveCooldown = GougeOffensiveCooldown.Checked;
        }

        private void TricksoftheTrade_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.TricksoftheTrade = TricksoftheTrade.Checked;
        }

        private void ShadowDance_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShadowDance = ShadowDance.Checked;
        }

        private void ShadowDanceEnergy_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShadowDanceEnergy = (int) ShadowDanceEnergy.Value;
        }


        private void ShadowBladeSync_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShadowBladeSync = ShadowBladeSync.Checked;
        }

        private void Stealth_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Stealth = Stealth.Checked;
        }

        private void StealthForce_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.StealthForce = StealthForce.Checked;
        }

        private void CloakofShadowsRoot_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CloakofShadowsRoot = CloakofShadowsRoot.Checked;
        }

        private void AdrenalineRushCD_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AdrenalineRushCD = AdrenalineRushCD.Checked;
        }

        private void AdrenalineRushBurst_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AdrenalineRushBurst = AdrenalineRushBurst.Checked;
        }

        private void InterruptAll_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.InterruptAll = InterruptAll.Checked;
        }

        private void ShadowDanceSap_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShadowDanceSap = ShadowDanceSap.Checked;
        }

        private void VanishSap_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.VanishSap = VanishSap.Checked;
        }
    }
}