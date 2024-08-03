using System;
using System.Collections.Generic;
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

            if (spine != null)
            {
                pokemonSpineCache.Add(@enum, spine);
            }

            return spine;
        }
    }
}