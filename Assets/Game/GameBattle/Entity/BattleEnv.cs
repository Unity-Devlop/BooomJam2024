using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;
using UnityToolkit;

namespace Game.GamePlay
{
    public class BattleEnv : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer background;
        [SerializeField] private SpriteRenderer background2;


        private SpriteRenderer currentBackground;

        private ICommand _unbind;

        private void Awake()
        {
            currentBackground = background;
        }

        public async UniTask Init(BattleEnvData battleEnvData)
        {
            _unbind?.Execute();
            _unbind = battleEnvData.bind.Listen(OnData);

            currentBackground = background;
            currentBackground.sprite = await Global.Get<ResourceSystem>().LoadBattleBG(battleEnvData.id);
        }

        private async UniTask OnData(BattleEnvData obj)
        {
            var prev = currentBackground;
            var cur = currentBackground == background ? background2 : background;

            cur.sprite = await Global.Get<ResourceSystem>().LoadBattleBG(obj.id);
            prev.sprite = currentBackground.sprite;

            prev.DOAlpha(0, 0.5f);
            cur.DOAlpha(1, 0.5f);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            currentBackground = cur;
        }
    }
}