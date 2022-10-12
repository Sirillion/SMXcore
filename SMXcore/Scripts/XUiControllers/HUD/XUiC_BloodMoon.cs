using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XMLData.Parsers;

namespace SMXcore
{
    public class XUiC_BloodMoon : XUiController
    {
        private XUiV_Sprite sprite;

        private ulong fullMoonTime;
        private ulong bloodMoonTime;

        private Color bloodMoonColorTint;
        private Color moonColorTint;

        private EnumGameStats bloodMoonWarningEnum;
        private EnumGameStats bloodMoonDayEnum;
        private EnumGameStats dayLightLengthEnum;

        private bool isInit = false;

        public override void Init()
        {
            base.Init();
            sprite = viewComponent as XUiV_Sprite;

            Color color = sprite.Color;
            color.a = 0;
            sprite.Color = color;

            bloodMoonWarningEnum = EnumParser.Parse<EnumGameStats>("BloodMoonWarning");
            bloodMoonDayEnum = EnumParser.Parse<EnumGameStats>("BloodMoonDay");
            dayLightLengthEnum = EnumParser.Parse<EnumGameStats>("DayLightLength");

            //ValueTuple<int, int, int> valueTuple2 = GameUtils.WorldTimeToElements(GameManager.Instance.World.worldTime);
            //int item3 = valueTuple2.Item2;
            //int item4 = valueTuple2.Item3;
            //value = this.timeFormatter.Format(item3, item4);

            //int v = GameUtils.WorldTimeToDays(GameManager.Instance.World.worldTime);
        }

        public override void Update(float _dt)
        {
            base.Update(_dt);

            if (XUi.IsGameRunning())
            {
                int bloodMoonWarning = GameStats.GetInt(bloodMoonWarningEnum);

                //Checks to see if the game is set to show the BloodMoon Warning
                //Returns if not to show the BloodMoon Warning
                if (bloodMoonWarning == -1)
                {
                    return;
                }

                var (day, hour, minute) = GameUtils.WorldTimeToElements(GameManager.Instance.World.worldTime);
                var (duskHour, dawnHour) = GameUtils.CalcDuskDawnHours(GameStats.GetInt(dayLightLengthEnum));

                if(!isInit)
                {
                    ulong duskTime = HourMinuteTimeToLong(duskHour, 0);
                    bloodMoonTime = duskTime - bloodMoonTime;
                    fullMoonTime = duskTime - fullMoonTime;

                    isInit = true;
                }

                int bloodMoonDay = GameStats.GetInt(bloodMoonDayEnum);

                if (day != bloodMoonDay && day != (bloodMoonDay + 1))
                {
                    if(sprite.Color.a != 0)
                    {
                        Color color = sprite.Color;
                        color.a = 0;
                        sprite.Color = color;
                    }
                    return;
                }

                if (day == bloodMoonDay)
                {
                    ulong warningTime = HourMinuteTimeToLong(bloodMoonWarning, 0);

                    if(warningTime > fullMoonTime)
                    {
                        fullMoonTime = warningTime;
                    }

                    ulong currentTime = WorldTimeToHourMinute(GameManager.Instance.World.worldTime);

                    //Changes the alpha of the white moon
                    if (warningTime <= currentTime && currentTime < fullMoonTime)
                    {
                        Color color = moonColorTint;
                        float alphaProgress = GetProgressBetweenTime(currentTime, warningTime, fullMoonTime);
                        color.a = Mathf.Lerp(0, 1, alphaProgress);
                        sprite.Color = color;
                    }

                    //Changes the color from the normal moon color to the blood moon color
                    if (fullMoonTime <= currentTime && currentTime <= bloodMoonTime)
                    {
                        float progress = GetProgressBetweenTime(currentTime, fullMoonTime, bloodMoonTime);
                        Color color = Color.Lerp(moonColorTint, bloodMoonColorTint, progress);
                        sprite.Color = color;
                    }

                    if(bloodMoonTime < currentTime && sprite.Color != bloodMoonColorTint)
                    {
                        sprite.Color = bloodMoonColorTint;
                    }
                }

                if(day == (bloodMoonDay + 1))
                {
                    ulong currentTime = WorldTimeToHourMinute(GameManager.Instance.World.worldTime);
                    ulong endBloodMoonTime = HourMinuteTimeToLong(dawnHour - 1, 60 - 10);
                    ulong noMoonTime = HourMinuteTimeToLong(dawnHour, 0);

                    if(currentTime < endBloodMoonTime && sprite.Color != bloodMoonColorTint)
                    {
                        sprite.Color = bloodMoonColorTint;
                    }

                    if(endBloodMoonTime <= currentTime && currentTime <= noMoonTime)
                    {
                        Color noMoonColor = moonColorTint;
                        noMoonColor.a = 0;
                        float progress = GetProgressBetweenTime(currentTime, endBloodMoonTime, noMoonTime);
                        Color color = Color.Lerp(bloodMoonColorTint, noMoonColor, progress);
                        sprite.Color = color;
                    }

                    if(noMoonTime < currentTime && sprite.Color.a != 0)
                    {
                        Color color = sprite.Color;
                        color.a = 0;
                        sprite.Color = color;
                    }
                }
            }
        }

        public override bool ParseAttribute(string name, string value, XUiController parent)
        {
            switch (name)
            {
                case "fullmoontime":
                    fullMoonTime = ParseTime(value);
                    return true;
                case "bloodmoontime":
                    bloodMoonTime = ParseTime(value);
                    return true;
                case "bloodmooncolortint":
                    bloodMoonColorTint = StringParsers.ParseColor32(value);
                    return true;
                case "mooncolortint":
                    moonColorTint = StringParsers.ParseColor32(value);
                    return true;
                default:
                    return base.ParseAttribute(name, value, parent);
            }
        }

        private float GetProgressBetweenTime(ulong currentTime, ulong minTime, ulong maxTime)
        {
            currentTime = ClampUlong(currentTime, minTime, maxTime);

            return ((float)currentTime - minTime) / (maxTime - minTime);
        }

        private ulong ClampUlong(ulong value, ulong min, ulong max)
        {
            if (!(value < min))
            {
                if (!(value > max))
                {
                    return value;
                }

                return max;
            }

            return min;
        }

        private ulong HourMinuteTimeToLong(long hours, long minutes)
        {
            return (ulong)((hours * 1000) + (minutes * 1000 / 60));
        }

        private ulong WorldTimeToHourMinute(ulong worldTime)
        {
            return worldTime % 24000UL;
        }

        private ulong ParseTime(string time)
        {
            string[] splits = time.Split(':');
            if(splits.Length == 2)
            {
                return HourMinuteTimeToLong(long.Parse(splits[0]), long.Parse(splits[1]));
            }

            return 0;
        }
    }
}
