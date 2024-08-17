using Game.GamePlay;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public class FirstChoosePanel : UIPanel
    {
        public Button chooseBtn;
        public TextMeshProUGUI chooseBtnText;
        public Button nextBtn;
        public PresetHuluConfig config;

        [SerializeField] private RectTransform selectContainer;
        private PokemonSelectItem[] _selectItems;


        private List<HuluData> _firstGeneratedPokemons = new List<HuluData>();
        private List<HuluData> _chooseHulus = new List<HuluData>();

        private int _curSelectedHulu = 0;


        [SerializeField] private PokemonUIShow show;
        [SerializeField] private PokemonHUD hud;

        public override void OnLoaded()
        {
            base.OnLoaded();
            Register();
            _selectItems = selectContainer.GetComponentsInChildren<PokemonSelectItem>();

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
            _chooseHulus.Clear();
            _firstGeneratedPokemons = RollHulus();
            _curSelectedHulu = 0;
            for (int i = 0; i < _selectItems.Length; i++)
            {
                _selectItems[i].UnBind();
                _selectItems[i].Bind(_firstGeneratedPokemons[i], i);
                LoadElementSprite(_selectItems[i].bg, _firstGeneratedPokemons[i]);
            }
            //charmNum.text = Global.Get<DataSystem>().Get<GameData>().admireNum.ToString();
            ShowUI(0);
        }

        private async void LoadElementSprite(Image image, HuluData data)
        {
            image.sprite = await Global.Get<ResourceSystem>().LoadElementTag(data.elementEnum);
        }

        private void Register()
        {
            chooseBtn.onClick.AddListener(OnChooseBtnClick);
            nextBtn.onClick.AddListener(OnContinueBtnClick);
        }

        private void UnRegister()
        {
            chooseBtn.onClick.RemoveListener(OnChooseBtnClick);
            nextBtn.onClick.RemoveListener(OnContinueBtnClick);
        }

        private void ShowUI(int target)
        {
            HuluData data = _firstGeneratedPokemons[target];
            show.UnBind();
            show.Bind(data);

            hud.UnBind();
            hud.Bind(data);


            if (_chooseHulus.Contains(data))
            {
                chooseBtnText.text = "取消选择";
            }
            else
            {
                chooseBtnText.text = "选择";
            }
        }

        public void OnChooseBtnClick()
        {
            var chooseTar = _firstGeneratedPokemons[_curSelectedHulu];
            
            PokemonSelectItem selectItem = _selectItems[_curSelectedHulu];
            
            if (_chooseHulus.Contains(chooseTar))
            {
                _chooseHulus.Remove(chooseTar);
                chooseBtnText.text = "选择";
                selectItem.SetState(Color.white);
            }
            else
            {
                if (_chooseHulus.Count < 4)
                {
                    _chooseHulus.Add(chooseTar);
                    selectItem.SetState(Color.green);
                    chooseBtnText.text = "取消选择";
                }
            }

            if (_chooseHulus.Count >= 4)
            {
                nextBtn.gameObject.SetActive(true);
            }
            else
            {
                nextBtn.gameObject.SetActive(false);
            }
        }

        private List<HuluData> RollHulus()
        {
            List<HuluData> result = new List<HuluData>();
            List<int> indexs = new List<int>();
            for(int i=0;i<6;++i)
            {
                int r = Random.Range(0, config.hulus.Count);
                if (!indexs.Contains(r))
                {
                    indexs.Add(r);
                    HuluData hulu = new HuluData(config.hulus[r].id);
                    hulu.RollAbility();
                    for (int j = 0; j < config.hulus[r].ownedSkills.Count; ++j) hulu.ownedSkills.Add(config.hulus[r].ownedSkills[j]);
                    int t = Random.Range(6, 10) - config.hulus[r].ownedSkills.Count;
                    if (t > 0) hulu.RollTargetSkills(t);
                    result.Add(hulu);
                }
                else --i;
            }
            return result;
        }

        public void OnContinueBtnClick()
        {
            var playerData = Global.Get<DataSystem>().Get<GameData>().playerData;
            foreach (var chooseHulu in _chooseHulus)
            {
                playerData.trainerData.datas.Add(chooseHulu);
            }

            GamePlayOutsideMgr.Singleton.machine.Change<DailyTrainState>();
        }
    }


}