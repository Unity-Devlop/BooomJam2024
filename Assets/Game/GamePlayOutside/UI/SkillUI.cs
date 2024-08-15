using cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public enum SkillOperation
    {
        Select,
        Delete,
    }

    public class SkillUIItem : Selectable
    {
        public Image skillImg;
        public Text skillName;
        public Button confirmBtn;
        public Text btnText;
        private ActiveSkillEnum skillId;
        private HuluData hulu;
        private SkillOperation operation;


        protected override void Awake()
        {
            confirmBtn.onClick.AddListener(OperateSkill);
            Global.Event.Listen<OperateSkillEvent>(HaveOperateSkill);
        }

        public void Init(HuluData hulu, ActiveSkillEnum skillId,SkillOperation operation)
        {
            this.hulu = hulu;
            this.skillId = skillId;
            var config = Global.Table.ActiveSkillTable.Get(skillId);
            skillName.text = config.Id.ToString();
            this.operation = operation;
            if (operation == SkillOperation.Select) btnText.text = "选择";
            else btnText.text = "删除";
        }

        private void OperateSkill()
        {
            if(operation==SkillOperation.Select)
            {
                ActiveSkillData data = new ActiveSkillData();
                data.id = skillId;
                hulu.ownedSkills.Add(data);
            }
            else
            {
                hulu.RemoveOwnedSkill(skillId);
            }
            hulu.ownedSkills.Sort((a, b) => a.id.CompareTo(b.id));
            Global.Event.Send<OperateSkillEvent>();
        }

        private void HaveOperateSkill(OperateSkillEvent e)
        {
            confirmBtn.gameObject.SetActive(false);
        }

        protected override void OnDestroy()
        {
            confirmBtn.onClick.RemoveAllListeners();
            Global.Event.UnListen<OperateSkillEvent>(HaveOperateSkill);
        }
    }

    public class OperateSkillEvent
    {

    }
}
