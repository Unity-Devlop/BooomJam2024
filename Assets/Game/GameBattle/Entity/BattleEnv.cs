using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityToolkit;

namespace Game.GamePlay
{
    public class BattleEnv : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer background;
        private ICommand _unbind;

        public async UniTask Init(BattleEnvData battleEnvData)
        {
            _unbind?.Execute();
            _unbind = battleEnvData.bind.Listen(OnData);
            await OnData(battleEnvData);
        }

        private async UniTask OnData(BattleEnvData obj)
        {
            background.sprite = await Global.Get<ResourceSystem>().LoadBattleBG(obj.id);
        }
    }
}