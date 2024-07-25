using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityToolkit;

namespace Game
{
    public class AudioSystem : MonoBehaviour, ISystem,IOnInit
    {
        private Dictionary<string, EventInstance> _cache;
        [SerializeField] private AssetReference bank;
        [SerializeField] private AssetReference bankString;
        // public bool initialized { get; private set; }
        // public UniTask initTask { get; private set; }

        public void OnInit()
        {
            // initialized = false;
            _cache = new Dictionary<string, EventInstance>(32);

            // int cnt = 0;
            // RuntimeManager.LoadBank(bank, false, () => cnt++);
            // RuntimeManager.LoadBank(bankString, false, () => cnt++);

            // initTask = UniTask.WaitUntil(() => cnt == 2).ContinueWith(() => { initialized = true; });
            // initTask.Forget();
            
            // 等待加载完成 避免异常 虽然会导致启动的一点点卡顿
            TextAsset bankAsset = Addressables.LoadAssetAsync<TextAsset>(bank).WaitForCompletion();
            TextAsset bankStringAsset = Addressables.LoadAssetAsync<TextAsset>(bankString).WaitForCompletion();
            
            RuntimeManager.LoadBank(bankAsset, false);
            RuntimeManager.LoadBank(bankStringAsset, false);
            
        }


        public void Dispose()
        {
            foreach (var value in _cache.Values)
            {
                value.release();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Create(string path, out EventInstance instance)
        {
            if (_cache.TryGetValue(path, out instance))
            {
                return;
            }

            instance = RuntimeManager.CreateInstance(path);
            _cache.Add(path, instance);
        }
    }
}