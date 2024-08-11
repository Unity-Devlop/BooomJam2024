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
                {1,"һ"},
                {2,"��"},
                {3,"��"},
                {4,"��"},
                {5,"��"},
                {6,"��"},
                {7, "��"},
                {8,"��"},
                {9,"��"},
                {10,"ʮ"},
            };
        }

        private void OnEnable()
        {
            var temp = Global.Get<DataSystem>().Get<GameData>();
            m_txtAdmire.text = temp.admireNum.ToString();
            m_txtDate.text = $"{temp.date.month}��{temp.date.day}�� ��{dateDic[temp.date.season]}����";
        }
    }
}
