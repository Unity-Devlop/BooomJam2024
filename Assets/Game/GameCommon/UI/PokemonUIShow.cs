using System;
using cfg;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Game
{
    public class PokemonUIShow : MonoBehaviour
    {
        public Image BG;
        public Image element;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI descText;

        // public TextMeshProUGUI descriptionText;
        public TextMeshProUGUI passiveSkillDescriptionText;
        public SkeletonGraphic skeletonGraphic;

        public void UnBind()
        {
            skeletonGraphic.gameObject.SetActive(false);
            nameText.text = "";
            // descriptionText.text = "";
            passiveSkillDescriptionText.text = "";
        }

        public async void Bind(HuluData data)
        {
            if (data.id == HuluEnum.怒潮龙)
            {
                skeletonGraphic.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                skeletonGraphic.transform.localScale = new Vector3(1, 1, 1);
            }
            
            nameText.text = data.name;
            // descriptionText.text = data.config.Desc;
            passiveSkillDescriptionText.text = data.passiveSkillConfig.Desc;
            descText.text = data.config.Desc;
            BG.sprite = await Global.Get<ResourceSystem>().LoadElementIdelBox(data.elementEnum);
            element.sprite = await Global.Get<ResourceSystem>().LoadElementIcon(data.elementEnum);
            element.SetNativeSize();
            try
            {
                skeletonGraphic.skeletonDataAsset = await Global.Get<ResourceSystem>().LoadPokemonSpine(data.id);
            }
            catch (InvalidKeyException)
            {
                Global.LogWarning($"没有找到精灵{data.id}的Spine资源");
            }

            skeletonGraphic.gameObject.SetActive(true);
            skeletonGraphic.Initialize(true);
            skeletonGraphic.AnimationState.SetAnimation(0, Consts.Animation.BattlePokemonIdleAnim, true);
        }
    }
}