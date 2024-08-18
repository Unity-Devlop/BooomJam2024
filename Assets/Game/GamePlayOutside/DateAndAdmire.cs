using NodeCanvas.DialogueTrees;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game
{
    public class DateAndAdmire : MonoBehaviour
    {
        public TextMeshProUGUI m_txtAdmire;
        public TextMeshProUGUI m_txtDate;
        private string[] chineseDigits= { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
        private string[] chineseUnits={ "", "十", "百", "千" };

        private void OnEnable()
        {
            try
            {
                var temp = Global.Get<DataSystem>().Get<GameData>();
                string chineseNumber = "";
                string numberStr = temp.date.season.ToString();
                int length = numberStr.Length;
                for (int i = 0; i < length; i++)
                {
                    int digit = int.Parse(numberStr[i].ToString());
                    int unitIndex = length - i - 1;

                    // 处理零的情况
                    if (digit == 0)
                    {
                        if (i == length - 1 || numberStr[i + 1] == '0')
                        {
                            continue;
                        }
                        else
                        {
                            chineseNumber += chineseDigits[digit];
                        }
                    }
                    else
                    {
                        chineseNumber += chineseDigits[digit] + chineseUnits[unitIndex];
                    }
                }
                m_txtAdmire.text = temp.admireNum.ToString();
                m_txtDate.text = $"{temp.date.month}月{temp.date.day}日 第{chineseNumber}赛季";
            }
            catch (Exception e)
            {
                Global.LogError(e.ToString());
            }
        }
    }
}