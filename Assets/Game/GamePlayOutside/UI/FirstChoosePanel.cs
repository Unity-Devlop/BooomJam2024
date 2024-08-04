using cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            _firstGeneratedPokemons = GameMath.RandomGeneratedFirstPokemon(_selectItems.Length);
            _curSelectedHulu = 0;
            for (int i = 0; i < _selectItems.Length; i++)
            {
                _selectItems[i].UnBind();
                _selectItems[i].Bind(_firstGeneratedPokemons[i], i);
            }

            ShowUI(0);
        }

        private void Register()
        {
            chooseBtn.onClick.AddListener(Choose);
            nextBtn.onClick.AddListener(Continue);
        }

        private void UnRegister()
        {
            chooseBtn.onClick.RemoveListener(Choose);
            nextBtn.onClick.RemoveListener(Continue);
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

        public void Choose()
        {
            if (_chooseHulus.Contains(_firstGeneratedPokemons[_curSelectedHulu]))
            {
                _chooseHulus.Remove(_firstGeneratedPokemons[_curSelectedHulu]);
                chooseBtnText.text = "选择";
            }
            else
            {
                if (_chooseHulus.Count < 4)
                {
                    _chooseHulus.Add(_firstGeneratedPokemons[_curSelectedHulu]);
                    chooseBtnText.text = "取消选择";
                }
            }

            if (_chooseHulus.Count >= 4) nextBtn.gameObject.SetActive(true);
            else nextBtn.gameObject.SetActive(false);
        }

        public void Continue()
        {
            var playerData = Global.Get<DataSystem>().Get<PlayerData>();
            foreach (var chooseHulu in _chooseHulus)
            {
                playerData.trainerData.datas.Add(chooseHulu);
            }

            GamePlayOutsideMgr.Singleton.machine.Change<DailyTrainState>();
        }
    }

    public class ClickHuluEvent
    {
        public int index;
    }
}