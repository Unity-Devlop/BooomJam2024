using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.GamePlay;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class LeftTeamHuluView : MonoBehaviour
    {
        [SerializeField] private HuluIcon[] huluIcons;

        private IBattleTrainer _trainer;

        private void Awake()
        {
            huluIcons = transform.GetComponentsInChildren<HuluIcon>();
            foreach (var icon in huluIcons)
            {
                icon.OnClick += OnHuluIconClick;
            }
        }

        private void OnHuluIconClick(int idx)
        {
            if (_calCts is { IsCancellationRequested: false } && _trainer.trainerData.datas[idx] !=
                _trainer.currentBattleData && !_trainer.trainerData.datas[idx].HealthIsZero())
            {
                _trainer.PushOperation(new ChangeHuluOperation()
                {
                    next = idx
                });
                return;
            }
        }

        public void Bind(IBattleTrainer battleTrainer)
        {
            _trainer = battleTrainer;
            for (var i = 0; i < battleTrainer.trainerData.datas.Count; i++)
            {
                HuluData data = battleTrainer.trainerData.datas[i];
                huluIcons[i].Bind(data, i);
            }
        }

        public void UnBind()
        {
            foreach (var icon in huluIcons)
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