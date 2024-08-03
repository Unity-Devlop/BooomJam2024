using System;
using Cysharp.Threading.Tasks;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityToolkit;

namespace Game.GamePlay
{
    public class HuluVisual : MonoBehaviour
    {
        private HuluData _data;
        [SerializeField] private SkeletonAnimation skeletonAnimation;
        [SerializeField] private TMP_Text hpText;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text elementText;
        // [SerializeField] private TextMeshPro statusText;

        private ICommand _unbindCmd;

        public void Bind(HuluData data)
        {
            _data = data;
            _unbindCmd = _data.bind.Listen(OnData);
            OnDataDirect(data);
        }

        private void OnDataDirect(HuluData obj)
        {
            nameText.text = obj.name;
            hpText.text = $"{obj.currentHp}/{obj.hp}";
            elementText.text = obj.elementEnum.ToString();
            // statusText.text = $"Atk:{obj.currentAtk}\nDef:{obj.currentDef}\nSpeed:{obj.currentSpeed}\nAdap:{obj.currentAdap}";
        }

        private async UniTask OnData(HuluData obj)
        {
            nameText.text = obj.name;

            elementText.text = obj.elementEnum.ToString();

            // statusText.text = $"Atk:{obj.currentAtk}\nDef:{obj.currentDef}\nSpeed:{obj.currentSpeed}\nAdap:{obj.currentAdap}";

            int origin = hpText.text == "" ? 0 : int.Parse(hpText.text.Split('/')[0]);
            int delta = obj.currentHp - origin;
            // 10滴血一帧
            int cnt = Mathf.Abs(delta) / 10;
            for (int i = 0; i < cnt; i++)
            {
                hpText.text = $"{origin + delta * i / 10}/{obj.hp}";
                await UniTask.Delay(TimeSpan.FromMilliseconds(1 / 60f * 1000));
            }

            OnDataDirect(obj);
        }

        public void UnBind()
        {
            if (_unbindCmd == null) return;
            _unbindCmd.Execute();
            _data = null;
        }

        public async UniTask ExecuteSkill(ActiveSkillData skill)
        {
            var trackEntry =
                skeletonAnimation.AnimationState.SetAnimation(0, Consts.Animation.BattlePokemonAttackAnim, false);
            // 等待动画播放完毕
            await UniTask.WaitUntil(() => trackEntry.IsComplete);
            skeletonAnimation.AnimationState.SetAnimation(0, Consts.Animation.BattlePokemonIdleAnim, true);
        }
    }
}