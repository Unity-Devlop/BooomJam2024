using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.GameEntry
{
    public class GameEntry : MonoBehaviour
    {
        public AssetReference homeScene;

        private async void Start()
        {
            Global.Get<GameFlow>().Run<GameHomeState>();
        }
    }
}