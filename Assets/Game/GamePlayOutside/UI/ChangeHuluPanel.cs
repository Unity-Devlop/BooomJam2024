using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public class ChangeHuluPanel : UIPanel
    {
        public Button chooseBtn;
        public TextMeshProUGUI chooseBtnText;
        public Button fireBtn;
        public TextMeshProUGUI fireBtnText;
        public Button nextBtn;
        public Sprite _lock;
        public Sprite _default;
        public RectTransform m_rectOwnedShow;
        public RectTransform m_rectOwnedHud;
        public RectTransform m_rectRule;
        public TextMeshProUGUI m_txtRule;
        public Button m_btnRule;

        [SerializeField] private RectTransform selectContainer;
        private PokemonSelectItem[] _selectItems;
        [SerializeField] private RectTransform selectOwnedContainer;
        private PokemonSelectItem[] _selectOwnedItems;


        private List<HuluData> _generatedPokemons = new List<HuluData>();
        private List<HuluData> _chooseHulus = new List<HuluData>();
        private List<HuluData> _chooseOwnedHulus = new List<HuluData>();

        private int _curSelectedHulu = 0;
        private int _curSelectedOwnedHulu = -1;

        private int limit = 3;


        [SerializeField] private PokemonUIShow show;
        [SerializeField] private PokemonHUD hud;


        [SerializeField] private PokemonUIShow ownedShow;
        [SerializeField] private PokemonHUD ownedHud;

        public override void OnLoaded()
        {
            base.OnLoaded();
            Register();
            _selectItems = selectContainer.GetComponentsInChildren<PokemonSelectItem>();
            _selectOwnedItems=selectOwnedContainer.GetComponentsInChildren<PokemonSelectItem>();

            foreach (var selectItem in _selectItems)
            {
                selectItem.OnClickEvent += OnSelectItemClick;
            }
        }

        private void OnSelectItemClick(int index)
        {
            _curSelectedHulu = index;
            ShowUI(index);
        }

        private void OnSelectOwnedItemClick(int index)
        {
            _curSelectedOwnedHulu = index;
            ShowOwnedUI(index);
        }

        public override void OnClosed()
        {
            base.OnClosed();
            foreach (var selectItem in _selectOwnedItems)
            {
                selectItem.OnClickEvent -= OnSelectOwnedItemClick;
            }
            _generatedPokemons.Clear();
            _chooseHulus.Clear();
            _chooseOwnedHulus.Clear();
            m_rectOwnedHud.gameObject.SetActive(false);
            m_rectOwnedShow.gameObject.SetActive(false);
            fireBtn.gameObject.SetActive(false);
        }

        public override void OnDispose()
        {
            UnRegister();
            foreach (var selectItem in _selectItems)
            {
                selectItem.OnClickEvent -= OnSelectItemClick;
            }
            base.OnDispose();
        }

        public override void OnOpened()
        {
            base.OnOpened();
            if (Global.Get<DataSystem>().Get<GameData>().ruleConfig.ruleList.Contains(GameRuleEnum.每局游戏上场的角色数量改为4)) limit = 4;
            _chooseHulus.Clear();
            _generatedPokemons = GameMath.RandomGeneratedFirstPokemon(_selectItems.Length);
            _curSelectedHulu = 0;
            _curSelectedOwnedHulu = -1;
            for (int i = 0; i < _selectItems.Length; i++)
            {
                _selectItems[i].UnBind();
                _selectItems[i].Bind(_generatedPokemons[i], i);
                LoadElementSprite(_selectOwnedItems[i].bg, _generatedPokemons[i], 1);
            }
            var owendHulus = Global.Get<DataSystem>().Get<GameData>().playerData.trainerData.datas;
            for (int i = 0; i < _selectOwnedItems.Length; i++)
            {
                if(i<owendHulus.Count)
                {
                    _selectOwnedItems[i].OnClickEvent += OnSelectOwnedItemClick;
                    _selectOwnedItems[i].UnBind();
                    _selectOwnedItems[i].Bind(owendHulus[i], i);
                    LoadElementSprite(_selectOwnedItems[i].bg, owendHulus[i],0);
                }
                else
                {
                    _selectOwnedItems[i].UnBind();
                    if (i>= Global.Get<DataSystem>().Get<GameData>().huluCapacity)
                    {
                        _selectOwnedItems[i].bg.sprite = _lock;
                    }
                    else
                    {
                        _selectOwnedItems[i].bg.sprite = _default;
                    }
                }
            }
            //charmNum.text = Global.Get<DataSystem>().Get<GameData>().admireNum.ToString();
            ShowUI(0);
        }

        private async void LoadElementSprite(Image image,HuluData data,int kind)
        {
            if(kind==0) image.sprite = await Global.Get<ResourceSystem>().LoadElementPortraitBox(data.elementEnum);
            else image.sprite = await Global.Get<ResourceSystem>().LoadElementTag(data.elementEnum);
        }

        private void Register()
        {
            chooseBtn.onClick.AddListener(OnChooseBtnClick);
            fireBtn.onClick.AddListener(OnFireBtnClick);
            nextBtn.onClick.AddListener(OnContinueBtnClick);
            m_btnRule.onClick.AddListener(OnUnderstandNewRule);
        }

        private void UnRegister()
        {
            chooseBtn.onClick.RemoveListener(OnChooseBtnClick);
            fireBtn.onClick.RemoveListener(OnFireBtnClick);
            nextBtn.onClick.RemoveListener(OnContinueBtnClick);
            m_btnRule.onClick.RemoveListener(OnUnderstandNewRule);
        }

        private void ShowUI(int target)
        {
            HuluData data = _generatedPokemons[target];
            if (_chooseHulus.Count+ Global.Get<DataSystem>().Get<GameData>().playerData.trainerData.datas.Count-_chooseOwnedHulus.Count >= Global.Get<DataSystem>().Get<GameData>().huluCapacity && Global.Get<DataSystem>().Get<GameData>().admireNum >= data.cost)
            {
                chooseBtn.GetComponent<Image>().color = Color.gray;
                chooseBtn.enabled = false;
            }
            else
            {
                chooseBtn.GetComponent<Image>().color = Color.white;
                chooseBtn.enabled = true;
            }
            show.UnBind();
            show.Bind(data);

            hud.UnBind();
            hud.Bind(data);


            if (_chooseHulus.Contains(data))
            {
                chooseBtnText.text = "取消招募";
            }
            else
            {
                chooseBtnText.text = "招募";
            }
        }

        private void ShowOwnedUI(int target)
        {
            if (_curSelectedOwnedHulu < 0) return;
            m_rectOwnedHud.gameObject.SetActive(true);
            m_rectOwnedShow.gameObject.SetActive(true);
            fireBtn.gameObject.SetActive(true);
            HuluData data = Global.Get<DataSystem>().Get<GameData>().playerData.trainerData.datas[target];
            if (Global.Get<DataSystem>().Get<GameData>().playerData.trainerData.datas.Count-_chooseOwnedHulus.Count+_chooseHulus.Count<=limit)
            {
                fireBtn.GetComponent<Image>().color = Color.gray;
                fireBtn.enabled = false;
            }
            else
            {
                fireBtn.GetComponent<Image>().color = Color.white;
                fireBtn.enabled = true;
            }
            ownedShow.UnBind();
            ownedShow.Bind(data);

            ownedHud.UnBind();
            ownedHud.Bind(data);

            if (_chooseOwnedHulus.Contains(data))
            {
                fireBtnText.text = "取消解雇";
            }
            else
            {
                fireBtnText.text = "解雇";
            }
        }

        public void OnChooseBtnClick()
        {
            var chooseTar = _generatedPokemons[_curSelectedHulu];

            PokemonSelectItem selectItem = _selectItems[_curSelectedHulu];

            if (_chooseHulus.Contains(chooseTar))
            {
                _chooseHulus.Remove(chooseTar);
                chooseBtnText.text = "招募";
                selectItem.SetState(Color.white);
            }
            else
            {
                if (_chooseHulus.Count + Global.Get<DataSystem>().Get<GameData>().playerData.trainerData.datas.Count - _chooseOwnedHulus.Count < Global.Get<DataSystem>().Get<GameData>().huluCapacity&& Global.Get<DataSystem>().Get<GameData>().admireNum>=chooseTar.cost)
                {
                    _chooseHulus.Add(chooseTar);
                    selectItem.SetState(Color.green);
                    chooseBtnText.text = "取消招募";
                }
            }
            ShowOwnedUI(_curSelectedOwnedHulu);
        }

        public void OnFireBtnClick()
        {
            var chooseTar = Global.Get<DataSystem>().Get<GameData>().playerData.trainerData.datas[_curSelectedOwnedHulu]; ;

            PokemonSelectItem selectItem = _selectOwnedItems[_curSelectedOwnedHulu];

            if (_chooseOwnedHulus.Contains(chooseTar))
            {
                _chooseOwnedHulus.Remove(chooseTar);
                fireBtnText.text = "解雇";
                selectItem.SetState(Color.white);
            }
            else
            {
                if (Global.Get<DataSystem>().Get<GameData>().playerData.trainerData.datas.Count - _chooseOwnedHulus.Count + _chooseHulus.Count > limit)
                {
                    _chooseOwnedHulus.Add(chooseTar);
                    selectItem.SetState(Color.green);
                    fireBtnText.text = "取消解雇";
                }
            }
            ShowUI(_curSelectedHulu);
        }

        public void ShowNewRule()
        {
            m_rectRule.gameObject.SetActive(true);
            if (Global.Get<DataSystem>().Get<GameData>().date.season == 2)
            {
                Global.Get<DataSystem>().Get<GameData>().huluCapacity = 5;
                m_txtRule.text = "队伍上限变更为5";
            }
            else if (Global.Get<DataSystem>().Get<GameData>().date.season == 4)
            {
                Global.Get<DataSystem>().Get<GameData>().huluCapacity = 6;
                m_txtRule.text = "队伍上限变更为6";
            }
            else
            {
                var gameData = Global.Get<DataSystem>().Get<GameData>();
                if (gameData.ruleConfig.prevCnt < gameData.rulePool.Count)
                {
                    gameData.ruleConfig.ruleList.Add(gameData.rulePool[gameData.ruleConfig.prevCnt++]);
                    m_txtRule.text = gameData.rulePool[gameData.ruleConfig.prevCnt - 1].ToString();
                }
                else m_txtRule.text = "无";
            }
        }

        public void OnUnderstandNewRule()
        {
            m_rectRule.gameObject.SetActive(false);
            GamePlayOutsideMgr.Singleton.machine.Change<DailyTrainState>();
        }

        public void OnContinueBtnClick()
        {
            var playerData = Global.Get<DataSystem>().Get<GameData>().playerData;
            foreach (var chooseHulu in _chooseOwnedHulus)
            {
                playerData.trainerData.datas.Remove(chooseHulu);
            }
            foreach (var chooseHulu in _chooseHulus)
            {
                playerData.trainerData.datas.Add(chooseHulu);
            }
            ShowNewRule();
        }
    }
}
