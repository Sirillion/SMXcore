using System;
using System.Globalization;
using UnityEngine;

//	Terms of Use: You can use this file as you want as long as this line and the credit lines are not removed or changed other than adding to them!
//	Credits: The Fun Pimps.
//	Tweaked: Sirillion, Laydor.

//	This is the complete vanilla controller. We disabled the visible component that removes the entire toolbelt when entering a vehicle so that we can display our XP bar all the time.
//	Difference: Disables the visible component, no other change. Can be used as a replacement for the vanilla XUiC_ToolbeltWindow controller if you want to show the toolbelt while using vehicles.


namespace SMXcore
{
	public class XUiC_ToolbeltXP : XUiController
	{
        private EntityPlayer localPlayer;

        private DateTime updateTime;

        private float lastValue;
        private float currentValue;
        private float lastDeficitValue;

        private string standardXPColor = "";
		private string updatingXPColor = "";
        private string expDeficitColor = "";

        private float xpFillSpeed = 2.5f;

        private CachedStringFormatter<float> bindingXp = new CachedStringFormatter<float>((float _f) => _f.ToCultureInvariantString());

        public override void Update(float _dt)
		{
			base.Update(_dt);
			if ((DateTime.Now - updateTime).TotalSeconds > 0.5)
			{
				updateTime = DateTime.Now;
			}
			RefreshBindings(false);
			// base.ViewComponent.IsVisible = ((!(localPlayer.AttachedToEntity != null) || !(localPlayer.AttachedToEntity is EntityVehicle)) && !localPlayer.IsDead());
			if (CustomAttributes.ContainsKey("standard_xp_color"))
			{
				standardXPColor = CustomAttributes["standard_xp_color"];
			}
			else
			{
				standardXPColor = "128,4,128";
			}
			if (CustomAttributes.ContainsKey("updating_xp_color"))
			{
				updatingXPColor = CustomAttributes["updating_xp_color"];
			}
			else
			{
				updatingXPColor = "128,4,128";
			}
			if (CustomAttributes.ContainsKey("deficit_xp_color"))
			{
				expDeficitColor = CustomAttributes["deficit_xp_color"];
			}
			else
			{
				expDeficitColor = "222,20,20";
			}
			if (CustomAttributes.ContainsKey("xp_fill_speed"))
			{
				xpFillSpeed = StringParsers.ParseFloat(CustomAttributes["xp_fill_speed"], 0, -1, NumberStyles.Any);
			}
		}

		public override void OnOpen()
		{
			base.OnOpen();
			if (localPlayer == null)
			{
				localPlayer = xui.playerUI.entityPlayer;
			}
			currentValue = (lastValue = XUiM_Player.GetLevelPercent(localPlayer));
		}

		public override bool GetBindingValue(ref string value, string bindingName)
		{
			if (bindingName != null)
			{
				if (bindingName == "xp")
				{
					if (localPlayer != null)
					{
						if (localPlayer.Progression.ExpDeficit > 0)
						{
							float v = Math.Max(lastDeficitValue, 0f) * 1.01f;
							value = bindingXp.Format(v);
							currentValue = (float)localPlayer.Progression.ExpDeficit / localPlayer.Progression.GetExpForNextLevel();
							if (currentValue != lastDeficitValue)
							{
								lastDeficitValue = Mathf.Lerp(lastDeficitValue, currentValue, Time.deltaTime * xpFillSpeed);
								if (Mathf.Abs(currentValue - lastDeficitValue) < 0.005f)
								{
									lastDeficitValue = currentValue;
								}
							}
						}
						else
						{
							float v2 = Math.Max(lastValue, 0f) * 1.01f;
							value = bindingXp.Format(v2);
							currentValue = XUiM_Player.GetLevelPercent(localPlayer);
							if (currentValue != lastValue)
							{
								lastValue = Mathf.Lerp(lastValue, currentValue, Time.deltaTime * xpFillSpeed);
								if (Mathf.Abs(currentValue - lastValue) < 0.005f)
								{
									lastValue = currentValue;
								}
							}
						}
					}
					return true;
				}
				if (bindingName == "xpcolor")
				{
					if (localPlayer != null)
					{
						if (localPlayer.Progression.ExpDeficit > 0)
						{
							value = expDeficitColor;
						}
						else
						{
							value = ((currentValue == lastValue) ? standardXPColor : updatingXPColor);
						}
					}
					else
					{
						value = "";
					}
					return true;
				}
			}
			return false;
		}
	}
}
