using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityToolkit;

namespace Game
{
    internal struct AddressablesUILoader : IUILoader
    {
        public GameObject Load<T>() where T : IUIPanel
        {
            string path = $"UI/{typeof(T).Name}/{typeof(T).Name}.prefab";
            return Addressables.LoadAssetAsync<GameObject>(path).WaitForCompletion();
        }

        public void LoadAsync<T>(Action<GameObject> callback) where T : IUIPanel
        {
            string path = $"UI/{typeof(T).Name}/{typeof(T).Name}.prefab";
            var handle = Addressables.LoadAssetAsync<GameObject>(path);
            handle.Completed += operation => { callback(handle.Result); };
        }

        public async Task<GameObject> LoadAsync<T>() where T : IUIPanel
        {
            string path = $"UI/{typeof(T).Name}/{typeof(T).Name}.prefab";
            return await Addressables.LoadAssetAsync<GameObject>(path);
        }

        public void Dispose(GameObject panel)
        {
            Addressables.Release(panel);
        }
    }
}