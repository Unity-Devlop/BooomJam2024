using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cfg;
using Cysharp.Threading.Tasks;
using Spine.Unity;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityToolkit;

namespace Game
{
    public class ResourceSystem : MonoBehaviour, ISystem, IOnInit
    {
        public Dictionary<HuluEnum, SkeletonDataAsset>
            pokemonSpineCache = new Dictionary<HuluEnum, SkeletonDataAsset>();


        public void OnInit()
        {
        }

        public void Dispose()
        {
        }


        public async UniTask<SkeletonDataAsset> LoadPokemonSpine(HuluEnum @enum)
        {
            if (pokemonSpineCache.TryGetValue(@enum, out var spine))
            {
                return spine;
            }

            string address = $"Animation/{@enum}/skeleton_SkeletonData.asset";
            try
            {
                spine = await Addressables.LoadAssetAsync<SkeletonDataAsset>(address);
            }
            catch (InvalidKeyException e)
            {
                Global.LogWarning($"找不到{@enum}的Spine资源:{address}");
            }
            catch (Exception e)
            {
                Global.LogError($"加载{@enum}的Spine资源失败:{address}");
            }

            if (spine != null)
            {
                pokemonSpineCache.TryAdd(@enum, spine);
            }

            return spine;
        }


        // 中间的插画
        private Dictionary<ActiveSkillEnum, Sprite> skillCardImageCache =
            new Dictionary<ActiveSkillEnum, Sprite>();

        public async UniTask<Sprite> LoadSkillCardImage(ActiveSkillEnum id)
        {
            if (skillCardImageCache.TryGetValue(id, out var sprite))
            {
                return sprite;
            }

            string address = $"UI/CardAtlas/Image/card_{id}_icon.png";

            try
            {
                sprite = await Addressables.LoadAssetAsync<Sprite>(address);
            }
            catch (InvalidKeyException e)
            {
                Global.LogWarning($"找不到{id}的SkillCardIcon资源:{address}");
            }
            catch (Exception e)
            {
                Global.LogError($"加载{id}的SkillCardIcon资源失败:{address}");
            }

            if (sprite != null)
            {
                skillCardImageCache.TryAdd(id, sprite);
            }

            return sprite;
        }


        // 特殊技能的图标
        private Dictionary<ActiveSkillEnum, Sprite> specialSkillIconCache = new Dictionary<ActiveSkillEnum, Sprite>();

        public async UniTask<Sprite> LoadSpecialSkillIcon(ActiveSkillEnum id)
        {
            if (specialSkillIconCache.TryGetValue(id, out var sprite))
            {
                return sprite;
            }

            var config = Global.Table.ActiveSkillTable.Get(id);
            if (string.IsNullOrEmpty(config.SpecialIconPath) || string.IsNullOrWhiteSpace(config.SpecialIconPath))
            {
                Global.LogWarning($"找不到{id}的SpecialSkillIcon资源:{config.SpecialIconPath}");
                return null;
            }

            string address = config.SpecialIconPath;
            try
            {
                sprite = await Addressables.LoadAssetAsync<Sprite>(address);
            }
            catch (InvalidKeyException e)
            {
                Global.LogWarning($"找不到{id}的SpecialSkillIcon资源:{address}");
            }
            catch (Exception e)
            {
                Global.LogError($"加载{id}的SpecialSkillIcon资源失败:{address}");
            }

            if (sprite != null)
            {
                specialSkillIconCache.TryAdd(id, sprite);
            }

            return sprite;
        }

        // 描述文本背景
        private Dictionary<ElementEnum, Sprite> skillCardDescBgCache = new Dictionary<ElementEnum, Sprite>();

        public async UniTask<Sprite> LoadSkillCardDescBg(ElementEnum configElement)
        {
            if (skillCardDescBgCache.TryGetValue(configElement, out var sprite))
            {
                return sprite;
            }

            var config = Global.Table.ElementFitTable.Get(configElement);
            string address = $"UI/CardAtlas/skill/skillcard_{config.UiPathTranslate}_wordbox.png";
            try
            {
                sprite = await Addressables.LoadAssetAsync<Sprite>(address);
            }
            catch (InvalidKeyException e)
            {
                Global.LogWarning($"找不到{configElement}的SkillCardDescBg资源:{address}");
            }
            catch (Exception e)
            {
                Global.LogError($"加载{configElement}的SkillCardDescBg资源失败:{address}");
            }

            if (sprite != null)
            {
                skillCardDescBgCache.TryAdd(configElement, sprite);
            }

            return sprite;
        }

        // 小属性背景
        private readonly Dictionary<ElementEnum, Sprite> skillElementBgCache = new Dictionary<ElementEnum, Sprite>();

        public async UniTask<Sprite> LoadSkillCardElementBg(ElementEnum configElement)
        {
            // UI/Atlas/SkillCard/skillcard_electric_background.png,
            if (skillElementBgCache.TryGetValue(configElement, out var sprite))
            {
                return sprite;
            }

            var config = Global.Table.ElementFitTable.Get(configElement);
            string address = $"UI/CardAtlas/skill/skillcard_{config.UiPathTranslate}_value.png";

            try
            {
                sprite = await Addressables.LoadAssetAsync<Sprite>(address);
            }
            catch (InvalidKeyException e)
            {
                Global.LogWarning($"找不到{configElement}的ElementBg资源:{address}");
            }
            catch (Exception e)
            {
                Global.LogError($"加载{configElement}的ElementBg资源失败:{address}");
            }

            if (sprite != null)
            {
                skillElementBgCache.TryAdd(configElement, sprite);
            }

            return sprite;
        }


        private readonly Dictionary<ElementEnum, Sprite> skillBgCache = new Dictionary<ElementEnum, Sprite>();

        public async UniTask<Sprite> LoadSkillCardBg(ElementEnum elementEnum)
        {
            if (skillBgCache.TryGetValue(elementEnum, out var sprite))
            {
                return sprite;
            }

            var config = Global.Table.ElementFitTable.Get(elementEnum);
            string address = $"UI/CardAtlas/skill/skillcard_{config.UiPathTranslate}_background.png";

            try
            {
                sprite = await Addressables.LoadAssetAsync<Sprite>(address);
            }
            catch (InvalidKeyException e)
            {
                Global.LogWarning($"找不到{elementEnum}的SkillBg资源:{address}");
            }
            catch (Exception e)
            {
                Global.LogError($"加载{elementEnum}的SkillBg资源失败:{address}");
            }

            if (sprite != null)
            {
                skillBgCache.TryAdd(elementEnum, sprite);
            }

            return sprite;
        }
    }
}