using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Game
{
    public class BattleStateTip : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI content;

        private void Awake()
        {
            Global.Event.Listen<BattleStateTipEvent>(OnBattleStateTipEvent);
        }

        private void OnDestroy()
        {
            Global.Event.UnListen<BattleStateTipEvent>(OnBattleStateTipEvent);
            _printCts?.Cancel();
            _printCts = null;
        }

        private CancellationTokenSource _printCts;

        private void OnBattleStateTipEvent(BattleStateTipEvent obj)
        {
            content.text = obj.tip;
            // 开启打字机 在末尾循环 . .. ... ....
            _printCts?.Cancel();
            _printCts = null;
            if (!string.IsNullOrEmpty(obj.tip) && !string.IsNullOrWhiteSpace(obj.tip))
            {
                _printCts = new CancellationTokenSource();
                Print(obj.tip, _printCts.Token).Forget();
            }
        }

        private async UniTask Print(string objTip, CancellationToken printCtsToken)
        {
            while (!printCtsToken.IsCancellationRequested)
            {
                content.text = objTip;
                await UniTask.Delay(500, cancellationToken: printCtsToken);
                if (printCtsToken.IsCancellationRequested) break;
                content.text = objTip + ".";
                await UniTask.Delay(500, cancellationToken: printCtsToken);
                if (printCtsToken.IsCancellationRequested) break;
                content.text = objTip + "..";
                await UniTask.Delay(500, cancellationToken: printCtsToken);
                if (printCtsToken.IsCancellationRequested) break;
                content.text = objTip + "...";
                await UniTask.Delay(500, cancellationToken: printCtsToken);
                if (printCtsToken.IsCancellationRequested) break;
                content.text = objTip + "....";
                await UniTask.Delay(500, cancellationToken: printCtsToken);
                if (printCtsToken.IsCancellationRequested) break;
            }
        }
    }
}