using System;
using cfg;
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

        private void Awake()
        {
            skeletonAnimation.gameObject.SetActive(false);
        }

        public async void Bind(HuluData data, Direction direction)
        {
            _data = data;
            _unbindCmd = _data.bind.Listen(OnData);

            switch (direction)
            {
                case Direction.Left:
                    if (data.id == HuluEnum.怒潮龙)
                    {
                        skeletonAnimation.transform.localScale = new Vector3(-1, 1, 1);
                    }
                    else
                    {
                        skeletonAnimation.transform.localScale = new Vector3(1, 1, 1);
                    }

                    break;
                case Direction.Right:
                    if (data.id == HuluEnum.怒潮龙)
                    {
                        skeletonAnimation.transform.localScale = new Vector3(1, 1, 1);
                    }
                    else
                    {
                        skeletonAnimation.transform.localScale = new Vector3(-1, 1, 1);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }


            SkeletonDataAsset dataAsset = await Global.Get<ResourceSystem>().LoadPokemonSpine(data.id);
            if (dataAsset != null)
            {
                skeletonAnimation.skeletonDataAsset = dataAsset;
                skeletonAnimation.Initialize(true);
            }
            else
            {
                Global.LogWarning($"加载PokemonSpine:{data.name}失败,使用默认Spine资源");
            }

            skeletonAnimation.gameObject.SetActive(true);

            ToIdle();
            OnDataDirect(data);
        }

        public void UnBind()
        {
            skeletonAnimation.gameObject.SetActive(false);
            if (_unbindCmd == null) return;
            _unbindCmd.Execute();
            _data = null;
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


        public async UniTask ExecuteSkill(ActiveSkillData skill)
        {
            var trackEntry =
                skeletonAnimation.AnimationState.SetAnimation(0, Consts.Animation.BattlePokemonAttackAnim, false);
            // 等待动画播放完毕
            await UniTask.WaitUntil(() => trackEntry.IsComplete);
            ToIdle();
        }

        private void ToIdle()
        {
            skeletonAnimation.AnimationState.SetAnimation(0, Consts.Animation.BattlePokemonIdleAnim, true);
        }
    }
}