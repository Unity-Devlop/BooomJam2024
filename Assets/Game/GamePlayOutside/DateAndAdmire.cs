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
        private Dictionary<int, string> dateDic;

        private void Awake()
        {
            dateDic = new Dictionary<int, string>()
            {
                {1,"一"},
                {2,"二"},
                {3,"三"},
                {4,"四"},
                {5,"五"},
                {6,"六"},
                {7, "七"},
                {8,"八"},
                {9,"九"},
                {10,"十"},
            };
        }

        private void OnEnable()
        {
            var temp = Global.Get<DataSystem>().Get<GameData>();
            m_txtAdmire.text = temp.admireNum.ToString();
            m_txtDate.text = $"{temp.date.month}月{temp.date.day}日 第{dateDic[temp.date.season]}赛季";
        }
    }
}
