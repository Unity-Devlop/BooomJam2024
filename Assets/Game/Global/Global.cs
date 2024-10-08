﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using cfg;
using Cysharp.Threading.Tasks;
using SimpleJSON;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.Profiling;
using UnityToolkit;
using Object = UnityEngine.Object;

namespace Game
{
    public class Global : MonoSingleton<Global>, IOnlyPlayingModelSingleton
    {
        private static Tables _table;

        public static Tables Table
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_table == null)
                {
                    _table = new Tables(Load);
                }

                return _table;
            }
        }

        private static TypeEventSystem _event;

        public static TypeEventSystem Event
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_event == null)
                {
                    _event = new TypeEventSystem();
                }

                return _event;
            }
        }

        protected override bool DontDestroyOnLoad() => true;

        private SystemLocator _systemLocator;

        [field: SerializeField] public Camera mainCamera { get; private set; }

        public bool initialized { get; private set; }

#if UNITY_WEBGL
        private static readonly Dictionary<string, JSONNode> _jsonCache = new Dictionary<string, JSONNode>();

        private static JSONNode Load2(string name)
        {
            var path = $"{nameof(Tables)}/{name}.json";
            return _jsonCache[path];
        }

        private static async UniTask InitLoad2()
        {
            var path1 = $"{nameof(Tables)}/hulutable.json";
            var path2 = $"{nameof(Tables)}/elementtable.json";
            var path3 = $"{nameof(Tables)}/activeskilltable.json";
            var path4 = $"{nameof(Tables)}/passiveskilltable.json";
            var path5 = $"{nameof(Tables)}/battleenvironmenttable.json";
            var path6 = $"{nameof(Tables)}/battlebufftable.json";

            _jsonCache[path1] = JSON.Parse((await Addressables.LoadAssetAsync<TextAsset>(path1)).text);
            _jsonCache[path2] = JSON.Parse((await Addressables.LoadAssetAsync<TextAsset>(path2)).text);
            _jsonCache[path3] = JSON.Parse((await Addressables.LoadAssetAsync<TextAsset>(path3)).text);
            _jsonCache[path4] = JSON.Parse((await Addressables.LoadAssetAsync<TextAsset>(path4)).text);
            _jsonCache[path5] = JSON.Parse((await Addressables.LoadAssetAsync<TextAsset>(path5)).text);
            _jsonCache[path6] = JSON.Parse((await Addressables.LoadAssetAsync<TextAsset>(path6)).text);
        }
#endif

        protected override async void OnInit()
        {
            Application.runInBackground = true;
#if UNITY_WEBGL
            await InitLoad2();
            _table = new Tables(Load2);

#else
            _table = new Tables(Load);
#endif

            _event = new TypeEventSystem();

            _systemLocator = new SystemLocator();
            _systemLocator.Register<GameFlow>(GetComponent<GameFlow>());
            _systemLocator.Register<AudioSystem>(GetComponent<AudioSystem>());
            _systemLocator.Register<DataSystem>(GetComponent<DataSystem>());
            _systemLocator.Register<ResourceSystem>(GetComponent<ResourceSystem>());
            _systemLocator.Register<DebugSystem>(new DebugSystem());
            // 初始化UI资源加载器
            UIRoot.Singleton.UIDatabase.Loader = new AddressablesUILoader();

            List<UniTask> systemInitTasks = ListPool<UniTask>.Get();
            foreach (var system in _systemLocator.systems)
            {
                if (system is IAsyncOnInit asyncOnInit)
                {
                    systemInitTasks.Add(UniTask.WaitUntil(() => asyncOnInit.initialized));
                }
            }

            await UniTask.WhenAll(systemInitTasks);
            // 质量变成Ultra
            QualitySettings.SetQualityLevel(5);
            initialized = true;
        }

        private void Update()
        {
            if (!initialized) return;
            Profiler.BeginSample("Global.Update");
            foreach (var system in _systemLocator.systems)
            {
                if (system is IOnUpdate update)
                {
                    update.OnUpdate();
                }
            }

            Profiler.EndSample();
        }

        protected override void OnDispose()
        {
            _systemLocator.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSystem Get<TSystem>() where TSystem : ISystem
        {
            return Singleton._systemLocator.Get<TSystem>();
        }

        private static JSONNode Load(string name)
        {
            var path = $"{nameof(Tables)}/{name}.json";
            TextAsset asset = Addressables.LoadAssetAsync<TextAsset>(path).WaitForCompletion();
            return JSON.Parse(asset.text);
        }


        #region Logger

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogColor(string message, Color color)
        {
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogInfo(string message)
        {
            Debug.Log($"[Info] {message}".Color(Color.white));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogInfo(string message, Object context)
        {
            Debug.Log($"[Info] {message}", context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarning(string message)
        {
            Debug.LogWarning($"[Warning] {message}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarning(string message, Object context)
        {
            Debug.LogWarning($"[Warning] {message}", context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogError(string message)
        {
            Debug.LogError($"[Error] {message}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogError(string message, Object context)
        {
            Debug.LogError($"[Error] {message}", context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogInfo(params string[] message)
        {
            Debug.Log($"[Info] {string.Join(" ", message)}".Color(Color.white));
        }

        #endregion
    }
}