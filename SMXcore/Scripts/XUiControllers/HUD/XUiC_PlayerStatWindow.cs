using UnityEngine;
using XMLData;

//	Terms of Use: You can use this file as you want as long as this line and the credit lines are not removed or changed other than adding to them!
//	Credits: sphereii.
//	Tweaked: Sirillion, TormentedEmu, Laydor.

//	Adds a custom controller with bindings lost with the removal of the PlayerStatWindow controller. Additional compatible bindings added from other controllers.
//	Difference: Vanilla removed the PlayerStatWindow controller with A20. As such we lost access to a lot of bindings needed to put things onto the HUD, this restores that.
namespace SMXcore
{
    public class XUiC_PlayerStatWindow : XUiController
    {

        private EntityPlayer player;
        private EntityNPC NPC;
        private EntityVehicle vehicle;

        private readonly CachedStringFormatter<int> playerDeathsFormatter = new CachedStringFormatter<int>((int _i) => _i.ToString());
        private readonly CachedStringFormatter<float> playerFoodFormatter = new CachedStringFormatter<float>((float _i) => _i.ToCultureInvariantString("0"));
        private readonly CachedStringFormatter<float> playerFoodFillFormatter = new CachedStringFormatter<float>((float _i) => _i.ToCultureInvariantString());
        private readonly CachedStringFormatter<int> playerItemsCraftedFormatter = new CachedStringFormatter<int>((int _i) => _i.ToString());
        private readonly CachedStringFormatter<int> playerLevelFormatter = new CachedStringFormatter<int>((int _i) => _i.ToString());
        private readonly CachedStringFormatter<float> playerLevelFillFormatter = new CachedStringFormatter<float>((float _i) => _i.ToCultureInvariantString());
        private readonly CachedStringFormatter<int> playerPvpKillsFormatter = new CachedStringFormatter<int>((int _i) => _i.ToString());
        private readonly CachedStringFormatter<float> playerWaterFormatter = new CachedStringFormatter<float>((float _i) => _i.ToCultureInvariantString("0"));
        private readonly CachedStringFormatter<float> playerWaterFillFormatter = new CachedStringFormatter<float>((float _i) => _i.ToCultureInvariantString());
        private readonly CachedStringFormatter<int> playerZombieKillsFormatter = new CachedStringFormatter<int>((int _i) => _i.ToString());

        // From CharacterWindow
        private readonly CachedStringFormatter<int> playerWaterMaxFormatter = new CachedStringFormatter<int>((int _i) => _i.ToString());
        private readonly CachedStringFormatter<int> playerFoodMaxFormatter = new CachedStringFormatter<int>((int _i) => _i.ToString());
        private readonly CachedStringFormatter<int> playerHealthFormatter = new CachedStringFormatter<int>((int _i) => _i.ToString());
        private readonly CachedStringFormatter<int> playerMaxHealthFormatter = new CachedStringFormatter<int>((int _i) => _i.ToString());
        private readonly CachedStringFormatter<int> playerStaminaFormatter = new CachedStringFormatter<int>((int _i) => _i.ToString());
        private readonly CachedStringFormatter<int> playerMaxStaminaFormatter = new CachedStringFormatter<int>((int _i) => _i.ToString());
        private readonly CachedStringFormatter<int> playerXpToNextLevelFormatter = new CachedStringFormatter<int>((int _i) => _i.ToString());
        private readonly CachedStringFormatter<int> playerCarryCapacityFormatter = new CachedStringFormatter<int>((int _i) => _i.ToString());

        // From PassiveEffects
        private readonly CachedStringFormatter<int> playerBagSizeFormatter = new CachedStringFormatter<int>((int _i) => _i.ToString());

        // From MapWindow
        private readonly CachedStringFormatter<ulong> dayFormatter = new CachedStringFormatter<ulong>((ulong _worldTime) => ValueDisplayFormatters.WorldTime(_worldTime, "{0}"));
        private readonly CachedStringFormatter<ulong> timeFormatter = new CachedStringFormatter<ulong>((ulong _worldTime) => ValueDisplayFormatters.WorldTime(_worldTime, "{1:00}:{2:00}"));

        private float updateTime;
        private string pointsAvailable;

        private readonly CachedStringFormatter<string, int> skillPointsAvailableFormatter = new CachedStringFormatter<string, int>((string _s, int _i) => string.Format("{1}", _s, _i));

        public override void Init()
        {
            base.Init();
        }

        public override bool GetBindingValue(ref string value, string bindingName)
        {
            switch (bindingName)
            {
                case "playerleveltitle":
                    value = Localization.Get("xuiLevel");
                    return true;
                case "playerlevel":
                    value = ((this.player != null) ? this.playerLevelFormatter.Format(XUiM_Player.GetLevel(this.player)) : "");
                    return true;
                case "playerlevelfill":
                    value = ((this.player != null) ? this.playerLevelFillFormatter.Format(XUiM_Player.GetLevelPercent(this.player)) : "");
                    return true;

                // Player DEATH bindings.
                case "playerdeathstitle":
                    value = Localization.Get("xuiDeaths");
                    return true;
                case "playerdeaths":
                    value = (player != null) ? playerDeathsFormatter.Format(XUiM_Player.GetDeaths(player)) : "";
                    return true;

                // Player ZOMBIE KILLS bindings.
                case "playerzombiekillstitle":
                    value = Localization.Get("xuiZombieKills");
                    return true;
                case "playerzombiekills":
                    value = ((this.player != null) ? this.playerZombieKillsFormatter.Format(XUiM_Player.GetZombieKills(this.player)) : "");
                    return true;

                // Player PVP KILLS bindings.
                case "playerpvpkillstitle":
                    value = Localization.Get("xuiPlayerKills");
                    return true;
                case "playerpvpkills":
                    value = ((this.player != null) ? this.playerPvpKillsFormatter.Format(XUiM_Player.GetPlayerKills(this.player)) : "");
                    return true;

                // Player LONGEST LIFE bindings.
                case "playerlongestlifetitle":
                    value = Localization.Get("xuiLongestLife");
                    return true;
                case "playerlongestlife":
                    value = ((this.player != null) ? XUiM_Player.GetLongestLife(this.player) : "");
                    return true;

                // Player TRAVELLED bindings.
                case "playertravelledtitle":
                    value = Localization.Get("xuiKMTravelled");
                    return true;
                case "playertravelled":
                    value = ((this.player != null) ? XUiM_Player.GetKMTraveled(this.player) : "");
                    return true;

                // Player ITEMS CRAFTED bindings.
                case "playeritemscraftedtitle":
                    value = Localization.Get("xuiItemsCrafted");
                    return true;
                case "playeritemscrafted":
                    value = ((this.player != null) ? this.playerItemsCraftedFormatter.Format(XUiM_Player.GetItemsCrafted(this.player)) : "");
                    return true;

                // Player WELLNESS bindings.
                case "playerwellnesstitle":
                    value = Localization.Get("xuiWellness");
                    return true;

                // CORETEMP
                case "playercoretemptitle":
                    value = Localization.Get("xuiFeelsLike");
                    return true;
                case "playercoretemp":
                    value = ((this.player != null) ? XUiM_Player.GetCoreTemp(this.player) : "");
                    return true;

                // FOOD
                case "playerfoodtitle":
                    value = Localization.Get("xuiFood");
                    return true;
                case "playerfood":
                    value = ((this.player != null) ? this.playerFoodFormatter.Format(XUiM_Player.GetFood(this.player)) : "");
                    return true;
                case "playerfoodfill":
                    value = ((this.player != null) ? this.playerFoodFillFormatter.Format(XUiM_Player.GetFoodPercent(this.player)) : "");
                    return true;

                // WATER
                case "playerwatertitle":
                    value = Localization.Get("xuiWater");
                    return true;
                case "playerwater":
                    value = ((this.player != null) ? this.playerWaterFormatter.Format(XUiM_Player.GetWater(this.player)) : "");
                    return true;
                case "playerwaterfill":
                    value = ((this.player != null) ? this.playerWaterFillFormatter.Format(XUiM_Player.GetWaterPercent(this.player)) : "");
                    return true;

                // From XUiC_CharacterFrameWindow
                // Player LOOTSTAGE bindings.
                case "playerlootstagetitle":
                    value = Localization.Get("xuiLootstage");
                    return true;
                case "playerlootstage":
                    value = ((this.player != null) ? this.player.GetHighestPartyLootStage(0f, 0f).ToString() : "");
                    return true;

                // Player WELLNESS bindings.
                case "playerwatermax":
                    value = ((this.player != null) ? this.playerWaterMaxFormatter.Format(XUiM_Player.GetWaterMax(this.player)) : "");
                    return true;
                case "playerfoodmax":
                    value = ((this.player != null) ? this.playerFoodMaxFormatter.Format(XUiM_Player.GetFoodMax(this.player)) : "");
                    return true;
                case "playerhealth":
                    value = ((this.player != null) ? this.playerHealthFormatter.Format((int)XUiM_Player.GetHealth(this.player)) : "");
                    return true;
                case "playermaxhealth":
                    value = ((this.player != null) ? this.playerMaxHealthFormatter.Format((int)XUiM_Player.GetMaxHealth(this.player)) : "");
                    return true;
                case "playerstamina":
                    value = ((this.player != null) ? this.playerStaminaFormatter.Format((int)XUiM_Player.GetStamina(this.player)) : "");
                    return true;
                case "playermaxstamina":
                    value = ((this.player != null) ? this.playerMaxStaminaFormatter.Format((int)XUiM_Player.GetMaxStamina(this.player)) : "");
                    return true;
                case "playerxptonextlevel":
                    value = ((this.player != null) ? this.playerXpToNextLevelFormatter.Format(XUiM_Player.GetXPToNextLevel(this.player) + this.player.Progression.ExpDeficit) : "");
                    return true;

                // From PassiveEffects
                case "playerbagsize":
                    value = ((this.player != null) ? this.player.bag.SlotCount.ToString() : "");
                    return true;

                // From XUiC_MapStats
                case "day":
                    value = "";
                    if (XUi.IsGameRunning() && base.xui.playerUI.entityPlayer != null)
                    {
                        value = this.dayFormatter.Format(GameManager.Instance.World.worldTime);
                    }
                    return true;
                case "time":
                    value = "";
                    if (XUi.IsGameRunning() && base.xui.playerUI.entityPlayer != null)
                    {
                        value = this.timeFormatter.Format(GameManager.Instance.World.worldTime);
                    }
                    return true;


                // Team SMX custom code below.

                // From sphereii - Custom Code - Counts Encumbrance up to equal bagsize for display.
                // Edited by Landor for fix for 20.6
                case "playercarrycapacity":
                    if (XUi.IsGameRunning() && this.player != null)
                    {
                        float carryCapacity = MathUtils.Min(this.player.bag.MaxItemCount, this.player.bag.SlotCount);
                        value = carryCapacity.ToString();
                    }

                    return true;

                // From TormentedEmu - Custom Code - Player Bag Used Slots
                case "playerbagusedslots":
                    if (XUi.IsGameRunning() && this.player != null)
                    {
                        value = this.player.bag.GetUsedSlotCount().ToString();
                    }
                    return true;

                // From Sirillion - Custom Code - Player Bag Free Slots
                // Edited by Landor for fix for 20.6
                case "playerbagfreeslots":
                    if (XUi.IsGameRunning() && this.player != null)
                    {
                        var total = this.player.bag.SlotCount;
                        var used = this.player.bag.GetUsedSlotCount();

                        var freeslots = total - used;

                        value = freeslots.ToString();
                    }
                    return true;

                // From TormentedEmu - Custom Code - Player Bag Fill Bar
                case "playerbagfill":
                    if (XUi.IsGameRunning() && this.player != null)
                    {
                        value = Mathf.Clamp01((float)this.player.bag.GetUsedSlotCount() / (float)this.player.bag.SlotCount).ToCultureInvariantString();
                    }
                    return true;

                // From sphereii - Custom Code - Player Bag Fill Bar Coloring
                // Edited by Landor for fix for 20.6
                case "playerbagfillcolor":
                    if (XUi.IsGameRunning() && this.player != null)
                    {
                        //MaxItemCount is how many items can be in the bag before enumberance kicks in.
                        float enumberance = MathUtils.Min(this.player.bag.MaxItemCount, this.player.bag.SlotCount);

                        float percent = this.player.bag.GetUsedSlotCount() / enumberance;

                        value = "43,124,18,255"; // Green
                        if (percent > 0.75)
                            value = "255,255,0,255"; // Yellow
                        if (percent > 0.90)
                            value = "255,144,24,255"; // Orange
                        if (percent > 1)
                            value = "175,30,25,255"; // Red
                    }

                    return true;

                // From Sirillion - Custom Code - Player Hunger & Thirst Deficiency
                case "playerhungerdeficiency":
                    if (XUi.IsGameRunning() && this.player != null)
                    {
                        var total = StringParsers.ParseSInt32(this.playerFoodMaxFormatter.Format(XUiM_Player.GetFoodMax(this.player)));
                        var missing = StringParsers.ParseSInt32(this.playerFoodFormatter.Format(XUiM_Player.GetFood(this.player)));

                        var deficit = total - missing;

                        value = deficit.ToString();
                    }
                    return true;
                case "playerthirstdeficiency":
                    if (XUi.IsGameRunning() && this.player != null)
                    {
                        var total = StringParsers.ParseSInt32(this.playerWaterMaxFormatter.Format(XUiM_Player.GetWaterMax(this.player)));
                        var missing = StringParsers.ParseSInt32(this.playerWaterFormatter.Format(XUiM_Player.GetWater(this.player)));

                        var deficit = total - missing;

                        value = deficit.ToString();
                    }
                    return true;

                // From sphereii - Custom Code - "Free's" NPC Portrait for use.
                case "npcportrait":
                    if (base.xui.Dialog.Respondent != null)
                    {
                        value = this.NPC.NPCInfo.Portrait;
                    }
                    return true;

                // From XUiC_SkillListWindow
                case "skillpointsavailable":
                    string v = this.pointsAvailable;
                    EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
                    if (XUi.IsGameRunning() && entityPlayer != null)
                    {
                        value = this.skillPointsAvailableFormatter.Format(v, entityPlayer.Progression.SkillPoints);
                    }
                    return true;

                // From TormentedEmu - Custom Code - Hides entries when no skillpoints are available.
                case "hasskillpoint":
                    if (XUi.IsGameRunning() && this.player != null)
                    {
                        if (player.Progression.SkillPoints > 0)
                            value = "true";
                        else
                            value = "false";
                    }
                    return true;

                // From Sirillion - Custom Code - Displays weapon damage on HUD.
                //case "damageonhud":
                //    value = (!this.itemStack.IsEmpty() ? XUiM_ItemStack.GetStatItemValueTextWithModInfo(this.itemStack, base.xui.playerUI.entityPlayer(0) : "");
                //    return true;

                default:
                    return base.GetBindingValue(ref value, bindingName);
            }
        }


        public override void Update(float _dt)
        {
            if (viewComponent.IsVisible && Time.time > updateTime)
            {
                updateTime = Time.time + 0.25f;
                RefreshBindings(IsDirty);
                if (IsDirty)
                {
                    IsDirty = false;
                }
            }
            base.Update(_dt);
        }

        public override void OnOpen()
        {
            base.OnOpen();
            IsDirty = true;
            player = xui.playerUI.entityPlayer;
            NPC = xui.Dialog.Respondent;
        }
    }
}



