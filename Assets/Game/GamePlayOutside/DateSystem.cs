using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Game
{
    public struct Date
    {
        public int year;
        public int month;
        public int day;
        public int season;//赛季
        public int count;//比赛场数

        public Date(int year,int month,int day,int season,int count)
        {
            this.year = year;
            this.month = month;
            this.day = day;
            this.season = season;
            this.count = count;
        }
    }

    public class DateSystem
    {
        public List<string> hasMeet = new List<string>();

        public DateSystem()
        {

        }

        public void YearElapse(int value)
        {
            Global.Get<DataSystem>().Get<GameData>().date.year += value;
        }

        public void MonthElapse(int value)
        {
            Global.Get<DataSystem>().Get<GameData>().date.month += value;
            if(Global.Get<DataSystem>().Get<GameData>().date.month>12)
            {
                Global.Get<DataSystem>().Get<GameData>().date.year += Global.Get<DataSystem>().Get<GameData>().date.month/13;
                Global.Get<DataSystem>().Get<GameData>().date.month = Global.Get<DataSystem>().Get<GameData>().date.month%13+1;
            }
        }

        public void DayElapse(int value)
        {
            Global.Get<DataSystem>().Get<GameData>().date.day += value;
            if(Global.Get<DataSystem>().Get<GameData>().date.day>30)
            {
                MonthElapse(Global.Get<DataSystem>().Get<GameData>().date.day / 31);
                Global.Get<DataSystem>().Get<GameData>().date.day= Global.Get<DataSystem>().Get<GameData>().date.day%31+1;
            }
        }

        public void SeasonElapse(int value)
        {
            Global.Get<DataSystem>().Get<GameData>().date.season += value;
            if (Global.Get<DataSystem>().Get<GameData>().date.season >= 10) Global.Get<DataSystem>().Get<GameData>().date.season = 1;
            if (Global.Get<DataSystem>().Get<GameData>().date.season == 2) Global.Get<DataSystem>().Get<GameData>().huluCapacity=5;
            if (Global.Get<DataSystem>().Get<GameData>().date.season == 4) Global.Get<DataSystem>().Get<GameData>().huluCapacity=6;
        }
    }
}
