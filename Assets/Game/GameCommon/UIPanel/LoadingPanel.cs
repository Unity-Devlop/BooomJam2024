using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityToolkit;

namespace Game
{
    public class LoadingPanel : UIPanel
    {
        private CancellationTokenSource _cancellationTokenSource;
        [SerializeField] private TextMeshProUGUI loadingText;

        public override void OnOpened()
        {
            base.OnOpened();
            loadingText.text = "Loading";
            Assert.IsNull(_cancellationTokenSource);
            _cancellationTokenSource = new CancellationTokenSource();
            // 开始打字机效果
            TypewriterEffectAsync(_cancellationTokenSource.Token).Forget();
        }

        private async UniTask TypewriterEffectAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                loadingText.text = "Loading";
                await UniTask.Delay(500, cancellationToken: token);
                if (token.IsCancellationRequested) break;
                loadingText.text = "Loading.";
                await UniTask.Delay(500, cancellationToken: token);
                if (token.IsCancellationRequested) break;
                loadingText.text = "Loading..";
                await UniTask.Delay(500, cancellationToken: token);
                if (token.IsCancellationRequested) break;
                loadingText.text = "Loading...";
                await UniTask.Delay(500, cancellationToken: token);
                if (token.IsCancellationRequested) break;
            }
        }

        public override void OnClosed()
        {
            base.OnClosed();
            Assert.IsNotNull(_cancellationTokenSource);
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }
}