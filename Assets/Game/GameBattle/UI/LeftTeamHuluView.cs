using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.GamePlay;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Game
{
    public class LeftTeamHuluView : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private Vector2 size;
        private PokemonPortraitIcon[] _pokemonPortraitIcons;

        private IBattleTrainer _trainer;

        private void Awake()
        {
            _pokemonPortraitIcons = new PokemonPortraitIcon[6];
            for (int i = 0; i < 6; i++)
            {
                GameObject obj = GameObject.Instantiate(prefab, transform);
                RectTransform rectTransform = obj.transform as RectTransform;
                rectTransform.sizeDelta = size;
                _pokemonPortraitIcons[i] = obj.GetComponent<PokemonPortraitIcon>();
                _pokemonPortraitIcons[i].OnClick += OnHuluIconClick;
                _pokemonPortraitIcons[i].gameObject.SetActive(false);
            }
        }

        private void OnHuluIconClick(int idx)
        {
            if (_calCts is { IsCancellationRequested: false } && _trainer.trainerData.datas[idx] !=
                _trainer.currentBattleData && _trainer.trainerData.datas[idx].CanFight())
            {
                _trainer.PushOperation(new ChangeHuluOperation()
                {
                    next = idx
                });
            }
        }

        public void Bind(IBattleTrainer battleTrainer)
        {
            foreach (var icon in _pokemonPortraitIcons)
            {
                icon.gameObject.SetActive(false);
            }

            _trainer = battleTrainer;
            for (var i = 0; i < battleTrainer.trainerData.datas.Count; i++)
            {
                HuluData data = battleTrainer.trainerData.datas[i];
                _pokemonPortraitIcons[i].Bind(data, i);
                _pokemonPortraitIcons[i].gameObject.SetActive(true);
            }
        }

        public void UnBind()
        {
            foreach (var icon in _pokemonPortraitIcons)
            {
                icon.Unbind();
            }

            _trainer = null;
        }

        private CancellationTokenSource _calCts;

        public UniTask EndCalOperation()
        {
            Assert.IsNotNull(_calCts);
            _calCts.Cancel();
            _calCts = null;
            return UniTask.CompletedTask;
        }

        public UniTask StartCalOperation()
        {
            Assert.IsNull(_calCts);
            _calCts = new CancellationTokenSource();
            return UniTask.CompletedTask;
        }
    }
}