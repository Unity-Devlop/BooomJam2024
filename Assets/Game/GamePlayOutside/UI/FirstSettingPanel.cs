using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public class FirstSettingPanel : UIPanel
    {
        public TMP_InputField inputField;
        public TextMeshProUGUI styleText;
        public Button leftBtn;
        public Button rightBtn;
        public Button confirmBtn;
        public Button goBtn;
        [SerializeField] private StyleToSkillConfig config;

        private int curStyle = 0;

        public override void OnLoaded()
        {
            base.OnLoaded();
            Register();
        }

        public override void OnDispose()
        {
            UnRegister();
            base.OnDispose();
        }

        public override void OnOpened()
        {
            base.OnOpened();
            styleText.text = config.dataList[0].styleName;
        }

        private void Register()
        {
            leftBtn.onClick.AddListener(LastStyle);
            rightBtn.onClick.AddListener(NextStyle);
            confirmBtn.onClick.AddListener(Confirm);
            goBtn.onClick.AddListener(Go);
        }

        private void UnRegister()
        {
            leftBtn.onClick.RemoveListener(LastStyle);
            rightBtn.onClick.RemoveListener(NextStyle);
            confirmBtn.onClick.RemoveListener(Confirm);
            goBtn.onClick.RemoveListener(Go);
        }

        public void NextStyle()
        {
            ++curStyle;
            curStyle %= config.dataList.Count;
            styleText.text = config.dataList[curStyle].styleName;
        }

        public void LastStyle()
        {
            --curStyle;
            curStyle += config.dataList.Count;
            curStyle %= config.dataList.Count;
            styleText.text = config.dataList[curStyle].styleName;
        }

        public async void Confirm()
        {
            var playerData = Global.Get<DataSystem>().Get<GameData>().playerData;
            playerData.name = inputField.text;
            playerData.trainerData.name = inputField.text;
            for (int i = 0; i < config.dataList[curStyle].skills.Count; ++i)
            {
                var temp = new ActiveSkillData();
                temp.id = config.dataList[curStyle].skills[i];
                playerData.trainerData.trainerSkills.Add(temp);
            }
            goBtn.gameObject.SetActive(true);
            confirmBtn.gameObject.SetActive(false);
            leftBtn.gameObject.SetActive(false);
            rightBtn.gameObject.SetActive(false);
            var p = await UIRoot.Singleton.OpenPanelAsync<ManageCardsPanel>();
            p.GetPorsche();
            //GamePlayOutsideMgr.Singleton.machine.Change<FirstChooseState>();
        }

        public void Go()
        {
            GamePlayOutsideMgr.Singleton.machine.Change<FirstChooseState>();
        }
    }
}