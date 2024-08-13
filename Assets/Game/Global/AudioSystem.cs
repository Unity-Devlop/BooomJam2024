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
    public class AudioSystem : MonoBehaviour, ISystem, IAsyncOnInit
    {
        private Dictionary<string, EventInstance> _cache;
        [SerializeField] private AssetReference bank;

        [SerializeField] private AssetReference bankString;
        public bool initialized { get; private set; }

        public async void OnInit()
        {
            // initialized = false;
            _cache = new Dictionary<string, EventInstance>(32);

            // int cnt = 0;
            // RuntimeManager.LoadBank(bank, false, () => cnt++);
            // RuntimeManager.LoadBank(bankString, false, () => cnt++);

            // initTask = UniTask.WaitUntil(() => cnt == 2).ContinueWith(() => { initialized = true; });
            // initTask.Forget();

            // 等待加载完成 避免异常 虽然会导致启动的一点点卡顿
            TextAsset bankAsset = await Addressables.LoadAssetAsync<TextAsset>(bank);
            TextAsset bankStringAsset = await Addressables.LoadAssetAsync<TextAsset>(bankString);

            RuntimeManager.LoadBank(bankAsset, false);
            RuntimeManager.LoadBank(bankStringAsset, false);
            initialized = true;
        }


        public void Dispose()
        {
            foreach (var value in _cache.Values)
            {
                value.release();
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Create(string path, out EventInstance instance, bool singleton = true)
        {
            if (singleton) // 单例
            {
                if (_cache.TryGetValue(path, out instance))
                {
                    return;
                }

                instance = RuntimeManager.CreateInstance(path);
                _cache.Add(path, instance);
                return;
            }

            // 不缓存
            instance = RuntimeManager.CreateInstance(path);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EventInstance GetSingleton(string path)
        {
            if (!_cache.TryGetValue(path, out _))
            {
                Create(path, out _, true);
            }

            return _cache[path];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PlaySingleton(string path)
        {
            GetSingleton(path).start();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void StopSingleton(string path, FMOD.Studio.STOP_MODE mode)
        {
            if (_cache.TryGetValue(path, out var instance))
            {
                instance.stop(mode);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DestroySingleton(string path)
        {
            if (_cache.TryGetValue(path, out var instance))
            {
                instance.release();
                _cache.Remove(path);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PlayOneShot(string path)
        {
            RuntimeManager.PlayOneShot(path);
        }
    }
}