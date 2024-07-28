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
        public int season;//Èü¼¾
    }

    public class DateSystem
    {
        public Date curDate;

        public DateSystem(int year,int month,int day,int season)
        {
            curDate.year = year;
            curDate.month = month;
            curDate.day = day;
            curDate.season = season;
        }

        public void YearElapse(int value)
        {
            curDate.year += value;
        }

        public void MonthElapse(int value)
        {
            curDate.month += value;
        }

        public void DayElapse(int value)
        {
            curDate.day += value;
        }

        public void SeasonElapse(int value)
        {
            curDate.season += value;
        }
    }
}
