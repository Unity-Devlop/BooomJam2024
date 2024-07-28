using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class RolePortraitUIItem : MonoBehaviour
    {
        public Image roleImage;
        public Text roleName;
        public Button btn;
        public int index;

        private void Awake()
        {
            btn.onClick.AddListener(ClickHulu);
        }

        public void ClickHulu()
        {
            ClickHuluEvent e = new ClickHuluEvent();
            e.index = index;
            Global.Event.Send<ClickHuluEvent>(e);
        }
    }
}
