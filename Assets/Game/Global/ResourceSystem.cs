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
        private Dictionary<HuluEnum, SkeletonDataAsset>
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

            var config = Global.Table.ElementTable.Get(configElement);
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

            var config = Global.Table.ElementTable.Get(configElement);
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

        public async UniTask<Sprite> LoadCardElementBg(ElementEnum elementEnum)
        {
            if (skillBgCache.TryGetValue(elementEnum, out var sprite))
            {
                return sprite;
            }

            var config = Global.Table.ElementTable.Get(elementEnum);
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

        private Dictionary<ActiveSkillEnum, Sprite> cardBgCache = new Dictionary<ActiveSkillEnum, Sprite>();

        public async UniTask<Sprite> LoadCardBg(ActiveSkillEnum activeSkillEnum)
        {
            var config = Global.Table.ActiveSkillTable.Get(activeSkillEnum);

            string address = "";

            if (config.Id == ActiveSkillEnum.保时捷的赞助)
            {
                address = $"UI/CardAtlas/bg/Command_Porsche_background.png";
            }
            else
            {
                if ((config.Type & ActiveSkillTypeEnum.指挥) != 0)
                {
                    address = $"UI/CardAtlas/bg/Command_background.png";
                }
                else if ((config.Type & ActiveSkillTypeEnum.伤害技能) != 0 ||
                         (config.Type & ActiveSkillTypeEnum.变化技能) != 0)
                {
                    address = $"UI/CardAtlas/bg/skillcard_background.png";
                }
            }

            if (cardBgCache.TryGetValue(activeSkillEnum, out var sprite))
            {
                return sprite;
            }

            if (string.IsNullOrEmpty(address) || string.IsNullOrWhiteSpace(address))
            {
                Global.LogWarning($"找不到{activeSkillEnum}的CardBg资源:{address}");
                return null;
            }

            try
            {
                sprite = await Addressables.LoadAssetAsync<Sprite>(address);
            }
            catch (InvalidKeyException e)
            {
                Global.LogWarning($"找不到{activeSkillEnum}的CardBg资源:{address}");
            }
            catch (Exception e)
            {
                Global.LogError($"加载{activeSkillEnum}的CardBg资源失败:{address}");
            }

            if (sprite != null)
            {
                cardBgCache.TryAdd(activeSkillEnum, sprite);
            }

            return null;
        }

        private Dictionary<string, Sprite> imageCache = new Dictionary<string, Sprite>();

        public async UniTask<Sprite> LoadImage(string address)
        {
            if (imageCache.TryGetValue(address, out var sprite))
            {
                return sprite;
            }

            try
            {
                sprite = await Addressables.LoadAssetAsync<Sprite>(address);
            }
            catch (InvalidKeyException e)
            {
                Global.LogWarning($"找不到{address}的资源:{address}");
            }
            catch (Exception e)
            {
                Global.LogError($"加载{address}的资源失败:{address}");
            }

            if (sprite != null)
            {
                imageCache.TryAdd(address, sprite);
            }

            return sprite;
        }


        #region 一些属性的图

        private readonly Dictionary<ElementEnum, Sprite> _uiElementPortraitboxCache =
            new Dictionary<ElementEnum, Sprite>();

        public async UniTask<Sprite> LoadElementIcon(ElementEnum id)
        {
            var cof = Global.Table.ElementTable.Get(id);
            string address = $"UI/Atlas/Element/ui_{cof.UiPathTranslate}_icon.png";
            return await LoadImage(address);
        }

        public async UniTask<Sprite> LoadElementPortraitBox(ElementEnum id)
        {
            string key = Global.Table.ElementTable.Get(id).UiPathTranslate;
            string address = $"UI/Atlas/Element/ui_{key}_portraitbox.png";

            if (_uiElementPortraitboxCache.TryGetValue(id, out var sprite))
            {
                return sprite;
            }

            try
            {
                sprite = await Addressables.LoadAssetAsync<Sprite>(address);
            }
            catch (InvalidKeyException e)
            {
                Global.LogWarning($"找不到{id}的PortraitBox资源:{address}");
            }
            catch (Exception e)
            {
                Global.LogError($"加载{id}的PortraitBox资源失败:{address}");
            }

            if (sprite != null)
            {
                _uiElementPortraitboxCache.TryAdd(id, sprite);
            }

            return sprite;
        }

        private readonly Dictionary<ElementEnum, Sprite> _uiElementComparisonCache =
            new Dictionary<ElementEnum, Sprite>();

        public async UniTask<Sprite> LoadElementComparison(ElementEnum id)
        {
            string key = Global.Table.ElementTable.Get(id).UiPathTranslate;
            string address = $"UI/Atlas/Element/ui_{key}_comparison.png";

            if (_uiElementComparisonCache.TryGetValue(id, out var sprite))
            {
                return sprite;
            }

            try
            {
                sprite = await Addressables.LoadAssetAsync<Sprite>(address);
            }
            catch (InvalidKeyException e)
            {
                Global.LogWarning($"找不到{id}的Comparison资源:{address}");
            }
            catch (Exception e)
            {
                Global.LogError($"加载{id}的Comparison资源失败:{address}");
            }

            if (sprite != null)
            {
                _uiElementComparisonCache.TryAdd(id, sprite);
            }

            return sprite;
        }

        private readonly Dictionary<ElementEnum, Sprite> _uiElementIdelBoxCache = new Dictionary<ElementEnum, Sprite>();

        public async UniTask<Sprite> LoadElementIdelBox(ElementEnum id)
        {
            string key = Global.Table.ElementTable.Get(id).UiPathTranslate;
            string address = $"UI/Atlas/Element/ui_{key}_idelbox.png";

            if (_uiElementIdelBoxCache.TryGetValue(id, out var sprite))
            {
                return sprite;
            }

            try
            {
                sprite = await Addressables.LoadAssetAsync<Sprite>(address);
            }
            catch (InvalidKeyException e)
            {
                Global.LogWarning($"找不到{id}的IdleBox资源:{address}");
            }
            catch (Exception e)
            {
                Global.LogError($"加载{id}的IdleBox资源失败:{address}");
            }

            if (sprite != null)
            {
                _uiElementIdelBoxCache.TryAdd(id, sprite);
            }

            return sprite;
        }

        private readonly Dictionary<ElementEnum, Sprite> _uiElementTagCache = new Dictionary<ElementEnum, Sprite>();

        public async UniTask<Sprite> LoadElementTag(ElementEnum id)
        {
            string key = Global.Table.ElementTable.Get(id).UiPathTranslate;
            string address = $"UI/Atlas/Element/ui_{key}_tag.png";

            if (_uiElementTagCache.TryGetValue(id, out var sprite))
            {
                return sprite;
            }

            try
            {
                sprite = await Addressables.LoadAssetAsync<Sprite>(address);
            }
            catch (InvalidKeyException e)
            {
                Global.LogWarning($"找不到{id}的Tag资源:{address}");
            }
            catch (Exception e)
            {
                Global.LogError($"加载{id}的Tag资源失败:{address}");
            }

            if (sprite != null)
            {
                _uiElementTagCache.TryAdd(id, sprite);
            }

            return sprite;
        }

        private readonly Dictionary<ElementEnum, Sprite> _uiElementTirleCache = new Dictionary<ElementEnum, Sprite>();

        public async UniTask<Sprite> LoadElementTitle(ElementEnum id)
        {
            string key = Global.Table.ElementTable.Get(id).UiPathTranslate;
            string address = $"UI/Atlas/Element/ui_{key}_title.png";

            if (_uiElementTirleCache.TryGetValue(id, out var sprite))
            {
                return sprite;
            }

            try
            {
                sprite = await Addressables.LoadAssetAsync<Sprite>(address);
            }
            catch (InvalidKeyException e)
            {
                Global.LogWarning($"找不到{id}的Title资源:{address}");
            }
            catch (Exception e)
            {
                Global.LogError($"加载{id}的Title资源失败:{address}");
            }

            if (sprite != null)
            {
                _uiElementTirleCache.TryAdd(id, sprite);
            }

            return sprite;
        }

        #endregion
    }
}