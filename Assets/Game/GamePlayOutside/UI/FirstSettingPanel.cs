using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public class FirstSettingPanel : UIPanel
    {
        public InputField inputField;
        public Text styleText;
        public Button leftBtn;
        public Button rightBtn;
        public Button confirmBtn;
        public List<StyleToSkill> styleToSkills = new List<StyleToSkill>();

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
            styleText.text = styleToSkills[curStyle].styleName;
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
            curStyle %= styleToSkills.Count;
            styleText.text = styleToSkills[curStyle].styleName;
        }

        public void LastStyle()
        {
            --curStyle;
            curStyle += styleToSkills.Count;
            curStyle %= styleToSkills.Count;
            styleText.text = styleToSkills[curStyle].styleName;
        }

        public void Confirm()
        {
            var playerData = Global.Get<DataSystem>().Get<PlayerData>();
            playerData.name = inputField.text;
            for(int i = 0; i < styleToSkills[curStyle].skills.Count;++i)
            {
                var temp = new ActiveSkillData();
                temp.id = styleToSkills[curStyle].skills[i];
                 playerData.trainerData.trainerSkills.Add(temp);
            }
            var e = new ChangeStateEvent();
            e.poState = POState.FirstChooseState;
            TypeEventSystem.Global.Send<ChangeStateEvent>(e);
        }
    }
}
