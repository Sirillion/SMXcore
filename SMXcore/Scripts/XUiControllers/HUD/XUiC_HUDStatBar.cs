using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//	Terms of Use: You can use this file as you want as long as this line and the credit lines are not removed or changed other than adding to them!
//	Credits: The Fun Pimps.
//	Tweaked: Laydor.

//	This is a modified vanilla controller. With the Active Item Entry related HUDStatTypes code removed and some new bindings.

namespace SMXcore
{
    public class XUiC_HUDStatBar : XUiController
    {
        private float lastValue;

        private HUDStatGroups statGroup;
        private HUDStatTypes statType;

        private string statImage = "";
        private string statIcon = "";
        private string statAtlas = "UIAtlas";
        private XUiV_Sprite barContent;
        private float deltaTime;
        private bool wasCrouching;
        private float oldValue;

        private readonly CachedStringFormatter<int> statcurrentFormatterInt = new CachedStringFormatter<int>((int _i) => _i.ToString());
        private readonly CachedStringFormatter<float> statcurrentFormatterFloat = new CachedStringFormatter<float>((float _i) => _i.ToCultureInvariantString());
        private readonly CachedStringFormatter<int, int> statcurrentWMaxFormatterAOfB = new CachedStringFormatter<int, int>((int _i, int _i1) => $"{_i}/{_i1}");
        private readonly CachedStringFormatter<int> statcurrentWMaxFormatterOf100 = new CachedStringFormatter<int>((int _i) => _i + "/100");
        private readonly CachedStringFormatter<int> statcurrentWMaxFormatterPercent = new CachedStringFormatter<int>((int _i) => _i + "%");
        private readonly CachedStringFormatter<float, float> statmodifiedmaxFormatter = new CachedStringFormatter<float, float>((float _f1, float _f2) => (_f1 / _f2).ToCultureInvariantString());
        private readonly CachedStringFormatter<float> statregenrateFormatter = new CachedStringFormatter<float>((float _f) => ((_f >= 0f) ? "+" : "") + _f.ToCultureInvariantString("0.00"));
        private readonly CachedStringFormatter<float> statfillFormatter = new CachedStringFormatter<float>((float _i) => _i.ToCultureInvariantString());

        private readonly CachedStringFormatterXuiRgbaColor staticoncolorFormatter = new CachedStringFormatterXuiRgbaColor();
        private readonly CachedStringFormatterXuiRgbaColor stealthColorFormatter = new CachedStringFormatterXuiRgbaColor();

        public HUDStatGroups StatGroup
        {
            get
            {
                return statGroup;
            }
            set
            {
                statGroup = value;
            }
        }

        public HUDStatTypes StatType
        {
            get
            {
                return statType;
            }
            set
            {
                statType = value;
                SetStatValues();
            }
        }

        public EntityPlayer LocalPlayer { get; internal set; }

        public EntityVehicle Vehicle { get; internal set; }

        public override void Init()
        {
            base.Init();
            IsDirty = true;
            XUiController childById = GetChildById("BarContent");
            if (childById != null)
            {
                barContent = (XUiV_Sprite)childById.ViewComponent;
            }

        }

        public override void Update(float _dt)
        {
            base.Update(_dt);
            deltaTime = _dt;
            if (LocalPlayer == null && XUi.IsGameRunning())
            {
                LocalPlayer = xui.playerUI.entityPlayer;
            }

            if (statGroup == HUDStatGroups.Vehicle && LocalPlayer != null)
            {
                if (Vehicle == null && LocalPlayer.AttachedToEntity != null && LocalPlayer.AttachedToEntity is EntityVehicle)
                {
                    Vehicle = (EntityVehicle)LocalPlayer.AttachedToEntity;
                    IsDirty = true;
                }
                else if (Vehicle != null && LocalPlayer.AttachedToEntity == null)
                {
                    Vehicle = null;
                    IsDirty = true;
                }
            }

            if (statType == HUDStatTypes.Stealth && LocalPlayer.IsCrouching != wasCrouching)
            {
                wasCrouching = LocalPlayer.IsCrouching;
                RefreshBindings(_forceAll: true);
                IsDirty = true;
            }

            RefreshFill();
            if (HasChanged() || IsDirty)
            {
                if (IsDirty)
                {
                    IsDirty = false;
                }

                RefreshBindings(_forceAll: true);
            }
        }

        public override void OnOpen()
        {
            base.OnOpen();
            IsDirty = true;
            RefreshBindings(_forceAll: true);
        }

        public override void OnClose()
        {
            base.OnClose();
        }

        public override bool GetBindingValue(ref string value, string bindingName)
        {
            switch (bindingName)
            {
                case "statcurrent":
                    value = GetCurrentStat();
                    return true;
                case "statmax":
                    value = GetMaxStat();
                    return true;
                case "statcurrentwithmax":
                    value = GetCurrentStatWithMax();
                    return true;
                case "statmodifiedmax":
                    if (LocalPlayer == null || (statGroup == HUDStatGroups.Vehicle && Vehicle == null))
                    {
                        value = "0";
                        return true;
                    }

                    switch (statType)
                    {
                        case HUDStatTypes.Health:
                            value = statmodifiedmaxFormatter.Format(LocalPlayer.Stats.Health.ModifiedMax, LocalPlayer.Stats.Health.Max);
                            break;
                        case HUDStatTypes.Stamina:
                            value = statmodifiedmaxFormatter.Format(LocalPlayer.Stats.Stamina.ModifiedMax, LocalPlayer.Stats.Stamina.Max);
                            break;
                        case HUDStatTypes.Water:
                            value = statmodifiedmaxFormatter.Format(LocalPlayer.Stats.Water.ModifiedMax, LocalPlayer.Stats.Water.Max);
                            break;
                        case HUDStatTypes.Food:
                            value = statmodifiedmaxFormatter.Format(LocalPlayer.Stats.Food.ModifiedMax, LocalPlayer.Stats.Food.Max);
                            break;
                    }

                    return true;
                case "statregenrate":
                    if (LocalPlayer == null || (statGroup == HUDStatGroups.Vehicle && Vehicle == null))
                    {
                        value = "0";
                        return true;
                    }

                    switch (statType)
                    {
                        case HUDStatTypes.Health:
                            value = statregenrateFormatter.Format(LocalPlayer.Stats.Health.RegenerationAmountUI);
                            break;
                        case HUDStatTypes.Stamina:
                            value = statregenrateFormatter.Format(LocalPlayer.Stats.Stamina.RegenerationAmountUI);
                            break;
                        case HUDStatTypes.Water:
                            value = statregenrateFormatter.Format(LocalPlayer.Stats.Water.RegenerationAmountUI);
                            break;
                        case HUDStatTypes.Food:
                            value = statregenrateFormatter.Format(LocalPlayer.Stats.Food.RegenerationAmountUI);
                            break;
                    }

                    return true;
                case "statfill":
                    {
                        if (LocalPlayer == null || (statGroup == HUDStatGroups.Vehicle && Vehicle == null))
                        {
                            value = "0";
                            return true;
                        }

                        float t = deltaTime * 3f;
                        float b = 0f;
                        switch (statType)
                        {
                            case HUDStatTypes.Health:
                                b = LocalPlayer.Stats.Health.ValuePercentUI;
                                break;
                            case HUDStatTypes.Stamina:
                                b = LocalPlayer.Stats.Stamina.ValuePercentUI;
                                break;
                            case HUDStatTypes.Water:
                                b = LocalPlayer.Stats.Water.ValuePercentUI;
                                break;
                            case HUDStatTypes.Food:
                                b = LocalPlayer.Stats.Food.ValuePercentUI;
                                break;
                            case HUDStatTypes.Stealth:
                                b = LocalPlayer.Stealth.ValuePercentUI;
                                break;
                            case HUDStatTypes.VehicleHealth:
                                b = Vehicle.GetVehicle().GetHealthPercent();
                                break;
                            case HUDStatTypes.VehicleFuel:
                                b = Vehicle.GetVehicle().GetFuelPercent();
                                break;
                            case HUDStatTypes.VehicleBattery:
                                b = Vehicle.GetVehicle().GetBatteryLevel();
                                break;
                        }

                        float v = Math.Max(lastValue, 0f) * 1.01f;
                        value = statfillFormatter.Format(v);
                        lastValue = Mathf.Lerp(lastValue, b, t);
                        return true;
                    }
                case "staticon":
                    if (statType == HUDStatTypes.VehicleHealth)
                    {
                        value = ((Vehicle != null) ? Vehicle.GetMapIcon() : "");
                    }
                    else
                    {
                        value = statIcon;
                    }

                    return true;
                case "staticonatlas":
                    value = statAtlas;
                    return true;
                case "staticoncolor":
                    {
                        value = staticoncolorFormatter.Format(Color.white);
                        return true;
                    }
                case "statimage":
                    value = statImage;
                    return true;
                case "stealthcolor":
                    {
                        Color32 v3 = Color32.Lerp(new Color32(72, 82, 0, byte.MaxValue), new Color32(187, 199, 0, byte.MaxValue), lastValue);
                        value = stealthColorFormatter.Format(v3);
                        return true;
                    }
                case "statvisible":
                    value = IsStatVisible().ToString();
                    return true;
                default:
                    return base.GetBindingValue(ref value, bindingName);
            }
        }

        public override bool ParseAttribute(string name, string value, XUiController _parent)
        {
            bool flag = base.ParseAttribute(name, value, _parent);
            if (!flag)
            {
                if (name != null && name == "stat_type")
                {
                    StatType = EnumUtils.Parse<HUDStatTypes>(value, _ignoreCase: true);
                    return true;
                }

                return false;
            }

            return flag;
        }

        public bool HasChanged()
        {
            bool result = false;
            switch (statType)
            {
                case HUDStatTypes.Health:
                    result = true;
                    break;
                case HUDStatTypes.Stamina:
                    result = true;
                    break;
                case HUDStatTypes.Water:
                    result = oldValue != LocalPlayer.Stats.Water.ValuePercentUI;
                    oldValue = LocalPlayer.Stats.Water.ValuePercentUI;
                    break;
                case HUDStatTypes.Food:
                    result = oldValue != LocalPlayer.Stats.Food.ValuePercentUI;
                    oldValue = LocalPlayer.Stats.Food.ValuePercentUI;
                    break;
                case HUDStatTypes.Stealth:
                    result = oldValue != lastValue;
                    oldValue = lastValue;
                    break;
                case HUDStatTypes.VehicleHealth:
                    {
                        if (Vehicle == null)
                        {
                            return false;
                        }

                        int health = Vehicle.GetVehicle().GetHealth();
                        result = oldValue != (float)health;
                        oldValue = health;
                        break;
                    }
                case HUDStatTypes.VehicleFuel:
                    if (Vehicle == null)
                    {
                        return false;
                    }

                    result = oldValue != Vehicle.GetVehicle().GetFuelLevel();
                    oldValue = Vehicle.GetVehicle().GetFuelLevel();
                    break;
                case HUDStatTypes.VehicleBattery:
                    if (Vehicle == null)
                    {
                        return false;
                    }

                    result = oldValue != Vehicle.GetVehicle().GetBatteryLevel();
                    oldValue = Vehicle.GetVehicle().GetBatteryLevel();
                    break;
            }

            return result;
        }

        public void RefreshFill()
        {
            if (barContent != null && !(LocalPlayer == null) && (statGroup != HUDStatGroups.Vehicle || !(Vehicle == null)))
            {
                float t = Time.deltaTime * 3f;
                float b = 0f;
                switch (statType)
                {
                    case HUDStatTypes.Health:
                        b = Mathf.Clamp01(LocalPlayer.Stats.Health.ValuePercentUI);
                        break;
                    case HUDStatTypes.Stamina:
                        b = Mathf.Clamp01(LocalPlayer.Stats.Stamina.ValuePercentUI);
                        break;
                    case HUDStatTypes.Water:
                        b = LocalPlayer.Stats.Water.ValuePercentUI;
                        break;
                    case HUDStatTypes.Food:
                        b = LocalPlayer.Stats.Food.ValuePercentUI;
                        break;
                    case HUDStatTypes.Stealth:
                        b = LocalPlayer.Stealth.ValuePercentUI;
                        break;
                    case HUDStatTypes.VehicleHealth:
                        b = Vehicle.GetVehicle().GetHealthPercent();
                        break;
                    case HUDStatTypes.VehicleFuel:
                        b = Vehicle.GetVehicle().GetFuelPercent();
                        break;
                    case HUDStatTypes.VehicleBattery:
                        b = Vehicle.GetVehicle().GetBatteryLevel();
                        break;
                }

                float fill = Math.Max(lastValue, 0f);
                lastValue = Mathf.Lerp(lastValue, b, t);
                barContent.Fill = fill;
            }
        }

        public void SetStatValues()
        {
            switch (statType)
            {
                case HUDStatTypes.Health:
                    statImage = "ui_game_stat_bar_health";
                    statIcon = "ui_game_symbol_add";
                    statGroup = HUDStatGroups.Player;
                    break;
                case HUDStatTypes.Stamina:
                    statImage = "ui_game_stat_bar_stamina";
                    statIcon = "ui_game_symbol_run";
                    statGroup = HUDStatGroups.Player;
                    break;
                case HUDStatTypes.Water:
                    statImage = "ui_game_stat_bar_stamina";
                    statIcon = "ui_game_symbol_water";
                    statGroup = HUDStatGroups.Player;
                    break;
                case HUDStatTypes.Food:
                    statImage = "ui_game_stat_bar_health";
                    statIcon = "ui_game_symbol_hunger";
                    statGroup = HUDStatGroups.Player;
                    break;
                case HUDStatTypes.Stealth:
                    statImage = "ui_game_stat_bar_health";
                    statIcon = "ui_game_symbol_stealth";
                    statGroup = HUDStatGroups.Player;
                    break;
                case HUDStatTypes.VehicleHealth:
                    statImage = "ui_game_stat_bar_health";
                    statIcon = "ui_game_symbol_minibike";
                    statGroup = HUDStatGroups.Vehicle;
                    break;
                case HUDStatTypes.VehicleFuel:
                    statImage = "ui_game_stat_bar_stamina";
                    statIcon = "ui_game_symbol_gas";
                    statGroup = HUDStatGroups.Vehicle;
                    break;
                case HUDStatTypes.VehicleBattery:
                    statImage = "ui_game_popup";
                    statIcon = "ui_game_symbol_battery";
                    statGroup = HUDStatGroups.Vehicle;
                    break;
            }
        }

        private string GetCurrentStat()
        {
            string value = "";
            if (LocalPlayer == null || (statGroup == HUDStatGroups.Vehicle && Vehicle == null))
            {
                return value;
            }

            switch (statType)
            {
                case HUDStatTypes.Health:
                    value = statcurrentFormatterInt.Format(LocalPlayer.Health);
                    break;
                case HUDStatTypes.Stamina:
                    value = statcurrentFormatterFloat.Format(LocalPlayer.Stamina);
                    break;
                case HUDStatTypes.Water:
                    value = statcurrentFormatterInt.Format((int)(LocalPlayer.Stats.Water.ValuePercentUI * 100f));
                    break;
                case HUDStatTypes.Food:
                    value = statcurrentFormatterInt.Format((int)(LocalPlayer.Stats.Food.ValuePercentUI * 100f));
                    break;
                case HUDStatTypes.Stealth:
                    value = statcurrentFormatterFloat.Format((int)(LocalPlayer.Stealth.ValuePercentUI * 100f));
                    break;
                case HUDStatTypes.VehicleHealth:
                    value = statcurrentFormatterInt.Format(Vehicle.GetVehicle().GetHealth());
                    break;
                case HUDStatTypes.VehicleFuel:
                    value = statcurrentFormatterFloat.Format(Vehicle.GetVehicle().GetFuelLevel());
                    break;
                case HUDStatTypes.VehicleBattery:
                    value = statcurrentFormatterFloat.Format(Vehicle.GetVehicle().GetBatteryLevel());
                    break;
            }

            return value;

        }

        private string GetMaxStat()
        {
            string value = "";
            if (LocalPlayer == null || (statGroup == HUDStatGroups.Vehicle && Vehicle == null))
            {
                return value;
            }
            switch (statType)
            {
                case HUDStatTypes.Health:
                    value = statcurrentFormatterInt.Format((int)LocalPlayer.Stats.Health.Max);
                    break;
                case HUDStatTypes.Stamina:
                    value = statcurrentFormatterInt.Format((int)LocalPlayer.Stats.Stamina.Max);
                    break;
                case HUDStatTypes.Water:
                    value = statcurrentFormatterInt.Format((int)LocalPlayer.Stats.Water.Max);
                    break;
                case HUDStatTypes.Food:
                    value = statcurrentFormatterInt.Format((int)LocalPlayer.Stats.Food.Max);
                    break;
                case HUDStatTypes.VehicleHealth:
                    value = statcurrentFormatterInt.Format(Vehicle.GetVehicle().GetMaxHealth());
                    break;
                case HUDStatTypes.VehicleFuel:
                    value = statcurrentFormatterInt.Format((int)Vehicle.GetVehicle().GetMaxFuelLevel());
                    break;
            }

            return value;
        }

        private string GetCurrentStatWithMax()
        {
            string value = "";

            if (LocalPlayer == null || (statGroup == HUDStatGroups.Vehicle && Vehicle == null))
            {
                return value;
            }

            switch (statType)
            {
                case HUDStatTypes.Health:
                    value = statcurrentWMaxFormatterAOfB.Format((int)LocalPlayer.Stats.Health.Value, (int)LocalPlayer.Stats.Health.Max);
                    break;
                case HUDStatTypes.Stamina:
                    value = statcurrentWMaxFormatterAOfB.Format((int)XUiM_Player.GetStamina(LocalPlayer), (int)LocalPlayer.Stats.Stamina.Max);
                    break;
                case HUDStatTypes.Water:
                    value = statcurrentWMaxFormatterOf100.Format((int)(LocalPlayer.Stats.Water.ValuePercentUI * 100f));
                    break;
                case HUDStatTypes.Food:
                    value = statcurrentWMaxFormatterOf100.Format((int)(LocalPlayer.Stats.Food.ValuePercentUI * 100f));
                    break;
                case HUDStatTypes.Stealth:
                    value = statcurrentWMaxFormatterOf100.Format((int)(LocalPlayer.Stealth.ValuePercentUI * 100f));
                    break;
                case HUDStatTypes.VehicleHealth:
                    value = statcurrentWMaxFormatterPercent.Format((int)(Vehicle.GetVehicle().GetHealthPercent() * 100f));
                    break;
                case HUDStatTypes.VehicleFuel:
                    value = statcurrentWMaxFormatterPercent.Format((int)(Vehicle.GetVehicle().GetFuelPercent() * 100f));
                    break;
                case HUDStatTypes.VehicleBattery:
                    value = statcurrentWMaxFormatterPercent.Format((int)(Vehicle.GetVehicle().GetBatteryLevel() * 100f));
                    break;
            }

            return value;
        }

        private bool IsStatVisible()
        {
            if (LocalPlayer == null)
            {
                return true;
            }

            if (LocalPlayer.IsDead())
            {
                return false;
            }

            if (statGroup == HUDStatGroups.Vehicle)
            {
                if (statType == HUDStatTypes.VehicleFuel)
                {
                    return Vehicle != null && Vehicle.GetVehicle().HasEnginePart();
                }

                return Vehicle != null;
            }

            if (statType == HUDStatTypes.Stealth)
            {
                xui.BuffPopoutList.SetYOffset(LocalPlayer.Crouching ? 52 : 0);
                return LocalPlayer.Crouching;
            }

            return true;
        }
    }
}
