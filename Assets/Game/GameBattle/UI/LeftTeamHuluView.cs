using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.GamePlay;
using UnityEngine;

namespace Game
{
    public class LeftTeamHuluView : MonoBehaviour
    {
        [SerializeField] private HuluIcon[] huluIcons;

        private void Awake()
        {
            huluIcons = transform.GetComponentsInChildren<HuluIcon>();
            foreach (var icon in huluIcons)
            {
                icon.OnClick += OnHuluIconClick;
            }
        }

        private void OnHuluIconClick(int obj)
        {
            Debug.Log($"点击了{obj}");
        }

        public void Bind(IBattleTrainer battleTrainer)
        {
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
        }

        public async UniTask EndCalOperation()
        {
        }

        public UniTask StartCalOperation()
        {
            return UniTask.CompletedTask;
        }
    }
}