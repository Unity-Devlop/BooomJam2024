using cfg;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    struct ValueUp
    {
        public int index;
        public float target;
        public int num;

        public ValueUp(int index,float target,int num)
        {
            this.index = index;
            this.target = target;
            this.num = num;
        }
    }

    public class SpecialTrainPanel : UIPanel
    {
        public GameObject roleList;
        public Image roleShowImg;
        public Text roleShowName;
        public Text roleShowPassiveSkill;
        public GameObject skillList;
        public GameObject ValueList;
        public Button confirmBtn;
        public GameObject rolePortraitUIItem;
        public SpecialTrainData trainData;
        public LoopHorizontalScrollRect skillScroll;
        public EasyGameObjectPool skillItemPool;
        private PlayerData playerData;

        private RolePortraitUIItem[] rolePortraitUIItems;
        private ValueUIItem[] valueUIItems;
        private int curHulu = 0;
        private WaitForFixedUpdate wait=new WaitForFixedUpdate();
        private bool haveTrained = false;
        

        public override void OnLoaded()
        {
            base.OnLoaded();
            valueUIItems = ValueList.GetComponentsInChildren<ValueUIItem>();
            playerData = Global.Get<DataSystem>().Get<GameData>().playerData;
            var list = playerData.trainerData.datas;
            rolePortraitUIItems = new RolePortraitUIItem[list.Count];
            for (int i = 0; i < list.Count; ++i)
            {
                var go = Instantiate(rolePortraitUIItem, roleList.transform);
                rolePortraitUIItems[i] = go.GetComponent<RolePortraitUIItem>();
                rolePortraitUIItems[i].roleName.text = list[i].id.ToString();
                rolePortraitUIItems[i].index = i;
            }
            skillScroll.itemRenderer = ItemRenderer;
            skillScroll.ItemProvider = ItemProvider;
            skillScroll.ItemReturn = ItemReturn;
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
            haveTrained = false;
            ShowUI();
        }

        public override void OnClosed()
        {
            base.OnClosed();
            haveTrained = false;
        }

        private void Register()
        {
            Global.Event.Listen<ClickHuluEvent>(ClickHulu);
            confirmBtn.onClick.AddListener(Confirm);
            valueUIItems[0].addBtn.onClick.AddListener(AddHealth);
            valueUIItems[1].addBtn.onClick.AddListener(AddAttack);
            valueUIItems[2].addBtn.onClick.AddListener(AddDefence);
            valueUIItems[3].addBtn.onClick.AddListener(AddSpeed);
            valueUIItems[4].addBtn.onClick.AddListener(AddAdaptability);
            Global.Event.Listen<OperateSkillEvent>(HaveOperateSkill);
        }

        private void UnRegister()
        {
            Global.Event.UnListen<ClickHuluEvent>(ClickHulu);
            confirmBtn.onClick.RemoveListener(Confirm);
            valueUIItems[0].addBtn.onClick.RemoveListener(AddHealth);
            valueUIItems[1].addBtn.onClick.RemoveListener(AddAttack);
            valueUIItems[2].addBtn.onClick.RemoveListener(AddDefence);
            valueUIItems[3].addBtn.onClick.RemoveListener(AddSpeed);
            valueUIItems[4].addBtn.onClick.RemoveListener(AddAdaptability);
            Global.Event.UnListen<OperateSkillEvent>(HaveOperateSkill);
        }

        private void ItemReturn(Transform transform1)
        {
            skillItemPool.Release(transform1.gameObject);
        }

        private GameObject ItemProvider(int idx)
        {
            GameObject obj = skillItemPool.Get();
            obj.name = idx.ToString();
            return obj;
        }

        private void ItemRenderer(Transform transform1, int idx)
        {
            var huluData = playerData.trainerData.datas[curHulu];
            SkillUIItem item = transform1.GetComponentInChildren<SkillUIItem>();
            var index=transform1.GetSiblingIndex();
            if (haveTrained)
            {
                ActiveSkillData skillData = huluData.ownedSkills[index];
                item.Init(huluData, skillData.id, SkillOperation.Delete);
            }
            else
            {
                if (index >= 3)
                {
                    ActiveSkillData skillData = huluData.ownedSkills[index - 3];
                    item.Init(huluData, skillData.id, SkillOperation.Delete);
                }
                else
                {
                    var list = huluData.config.SkillPool.Shuffle();
                    item.Init(huluData, list[0], SkillOperation.Select);
                }
            }
        }

        public void UnBind()
        {
            skillScroll.totalCount = 0;
            skillScroll.RefillCells();
            skillScroll.RefreshCells();
        }


        public void Bind()
        {
            var huluData = playerData.trainerData.datas[curHulu];
            if (!haveTrained) skillScroll.totalCount = huluData.ownedSkills.Count + 3;
            else skillScroll.totalCount = huluData.ownedSkills.Count;
            skillScroll.RefillCells();
            skillScroll.RefreshCells();
        }

        private void ShowUI()
        {
            var list = playerData.trainerData.datas;
            var huluData = list[curHulu];
            roleShowName.text = huluData.id.ToString();
            roleShowPassiveSkill.text = Global.Table.PassiveSkillTable.Get(huluData.passiveSkillConfig.Id).Desc;
            //UnBind();
            Bind();
            valueUIItems[0].valueNum.text = huluData.hp.ToString();
            valueUIItems[0].slider.value = (float)huluData.hp / huluData.config.MaxHp;
            valueUIItems[0].addText.text = $"+{trainData.minHealth}~{trainData.maxHealth}";
            valueUIItems[1].valueNum.text = huluData.atk.ToString();
            valueUIItems[1].slider.value = (float)huluData.atk / huluData.config.MaxAtk;
            valueUIItems[1].addText.text = $"+{trainData.minAttack}~{trainData.maxAttack}";
            valueUIItems[2].valueNum.text = huluData.def.ToString();
            valueUIItems[2].slider.value = (float)huluData.def / huluData.config.MaxDef;
            valueUIItems[2].addText.text = $"+{trainData.minDefence}~{trainData.maxDefence}";
            valueUIItems[3].valueNum.text = huluData.speed.ToString();
            valueUIItems[3].slider.value = (float)huluData.speed / huluData.config.MaxSpeed;
            valueUIItems[3].addText.text = $"+{trainData.minSpeed}~{trainData.maxSpeed}";
            valueUIItems[4].valueNum.text = huluData.adap.ToString();
            valueUIItems[4].slider.value = (float)huluData.adap / huluData.config.MaxAdap;
            valueUIItems[4].addText.text = $"+{trainData.minAdaptability}~{trainData.maxAdaptability}";
        }

        public void ClickHulu(ClickHuluEvent e)
        {
            curHulu = e.index;
            ShowUI();
        }

        public void Choose()
        {
        }

        private void AddHealth()
        {
            int value=trainData.Train(Ability.Health);
            playerData.trainerData.datas[curHulu].hp += value;
            playerData.trainerData.datas[curHulu].hp = Mathf.Min(playerData.trainerData.datas[curHulu].hp, playerData.trainerData.datas[curHulu].config.MaxHp);
            HideAddBtnAndTxt();
            StartCoroutine("ValueUp",new ValueUp(0, (float)playerData.trainerData.datas[curHulu].hp/ playerData.trainerData.datas[curHulu].config.MaxHp, playerData.trainerData.datas[curHulu].hp));
        }

        private void AddAttack()
        {
            int value = trainData.Train(Ability.Attack);
            playerData.trainerData.datas[curHulu].atk += value;
            playerData.trainerData.datas[curHulu].atk = Mathf.Min(playerData.trainerData.datas[curHulu].atk, playerData.trainerData.datas[curHulu].config.MaxAtk);
            HideAddBtnAndTxt();
            StartCoroutine("ValueUp", new ValueUp(1, (float)playerData.trainerData.datas[curHulu].atk / playerData.trainerData.datas[curHulu].config.MaxAtk, playerData.trainerData.datas[curHulu].atk));
        }

        private void AddDefence()
        {
            int value = trainData.Train(Ability.Defence);
            playerData.trainerData.datas[curHulu].def += value;
            playerData.trainerData.datas[curHulu].def = Mathf.Min(playerData.trainerData.datas[curHulu].def, playerData.trainerData.datas[curHulu].config.MaxDef);
            HideAddBtnAndTxt();
            StartCoroutine("ValueUp", new ValueUp(2, (float)playerData.trainerData.datas[curHulu].def / playerData.trainerData.datas[curHulu].config.MaxDef, playerData.trainerData.datas[curHulu].def));
        }

        private void AddSpeed()
        {
            int value = trainData.Train(Ability.Speed);
            playerData.trainerData.datas[curHulu].speed += value;
            playerData.trainerData.datas[curHulu].speed = Mathf.Min(playerData.trainerData.datas[curHulu].speed, playerData.trainerData.datas[curHulu].config.MaxSpeed);
            HideAddBtnAndTxt();
            StartCoroutine("ValueUp", new ValueUp(3, (float)playerData.trainerData.datas[curHulu].speed / playerData.trainerData.datas[curHulu].config.MaxSpeed, playerData.trainerData.datas[curHulu].speed));
        }

        private void AddAdaptability()
        {
            int value = trainData.Train(Ability.Adaptability);
            playerData.trainerData.datas[curHulu].adap += value;
            playerData.trainerData.datas[curHulu].adap = Mathf.Min(playerData.trainerData.datas[curHulu].adap, playerData.trainerData.datas[curHulu].config.MaxAdap);
            HideAddBtnAndTxt();
            StartCoroutine("ValueUp", new ValueUp(4, (float)playerData.trainerData.datas[curHulu].adap / playerData.trainerData.datas[curHulu].config.MaxAdap, playerData.trainerData.datas[curHulu].adap));
        }


        private void HideAddBtnAndTxt()
        {
            for(int i=0;i<valueUIItems.Length;++i)
            {
                valueUIItems[i].addBtn.gameObject.SetActive(false);
            }

/*            for(int i=0;i<rolePortraitUIItems.Length;++i)
            {
                rolePortraitUIItems[i].btn.enabled = false;
            }*/
        }

        public void HaveOperateSkill(OperateSkillEvent e)
        {
            haveTrained = true;
            HideAddBtnAndTxt();
            ShowUI();
        }

        public void Confirm()
        {
            CloseSelf();
        }

        IEnumerator ValueUp(ValueUp v)
        {
            valueUIItems[v.index].valueNum.text = v.num.ToString();
            while (valueUIItems[v.index].slider.value < v.target)
            {
                valueUIItems[v.index].slider.value = Mathf.Lerp(valueUIItems[v.index].slider.value, v.target, Time.deltaTime);
                yield return wait;
            }
        }
    }
}
