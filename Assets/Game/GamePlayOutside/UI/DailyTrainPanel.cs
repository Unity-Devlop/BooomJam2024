using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public class DailyTrainPanel : UIPanel
    {
        public Button confirmBtn;
        public List<GameObject> huluNames = new List<GameObject>();
        private List<Text> names = new List<Text>();

        private PlayerData playerData;
        

        public override void OnLoaded()
        {
            base.OnLoaded();
            Register();
            playerData = Global.Get<DataSystem>().Get<PlayerData>();
            for (int i = 0; i < huluNames.Count; ++i) names.Add(huluNames[i].GetComponentInChildren<Text>());
        }

        public override void OnDispose()
        {
            UnRegister();
            base.OnDispose();
        }

        public override void OnOpened()
        {
            base.OnOpened();
            ShowUI();
        }

        private void Register()
        {
            confirmBtn.onClick.AddListener(Confirm);
        }

        private void UnRegister()
        {
            confirmBtn.onClick.RemoveListener(Confirm);
        }

        private void ShowUI()
        {
            var list = playerData.trainerData.datas;
            for(int i=0;i<list.Count;++i)
            {
                huluNames[i].gameObject.SetActive(true);
                names[i].text = Global.Table.HuluTable.Get(list[i].id).Id.ToString();
            }
        }

        private void Confirm()
        {
            var list = playerData.trainerData.datas;
            DailyTrainTable.Instance.Train(list);
            GamePlayOutsideMgr.Singleton.machine.Change<SpecialTrainState>();
        }
    }
}
