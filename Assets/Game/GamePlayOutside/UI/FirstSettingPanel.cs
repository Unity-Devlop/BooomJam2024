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
        }

        private void UnRegister()
        {
            leftBtn.onClick.RemoveListener(LastStyle);
            rightBtn.onClick.RemoveListener(NextStyle);
            confirmBtn.onClick.RemoveListener(Confirm);
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

        public void Confirm()
        {
            var playerData = Global.Get<DataSystem>().Get<GameData>().playerData;
            playerData.name = inputField.text;
            for (int i = 0; i < config.dataList[curStyle].skills.Count; ++i)
            {
                var temp = new ActiveSkillData();
                temp.id = config.dataList[curStyle].skills[i];
                playerData.trainerData.trainerSkills.Add(temp);
            }

            GamePlayOutsideMgr.Singleton.machine.Change<FirstChooseState>();
        }
    }
}