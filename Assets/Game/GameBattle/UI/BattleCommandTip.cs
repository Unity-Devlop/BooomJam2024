using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    [RequireComponent(typeof(CanvasGroup))]
    public class BattleCommandTip : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI content;
        private CanvasGroup _group;
        // [SerializeField] private AnimationCurve fadeOutCurve;

        protected void Awake()
        {
            _group = GetComponent<CanvasGroup>();
            _group.alpha = 0;
        }

        protected void OnEnable()
        {
            Global.Event.Listen<OnExecuteSkill, UniTask>(OnUsingCommandSkill);
        }

        protected void OnDisable()
        {
            Global.Event.UnListen<OnExecuteSkill, UniTask>(OnUsingCommandSkill);
        }

        private async UniTask OnUsingCommandSkill(OnExecuteSkill arg)
        {
            _group.alpha = 1;
            transform.localScale = Vector3.zero;
            content.text = arg.data.id.ToString();
            transform.DOScale(Vector3.one * 1.5f, 0.5f);
            _group.DOAlpha(0, 1);
            await UniTask.Delay(TimeSpan.FromSeconds(1.5));
        }
    }
}