using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityToolkit;

namespace Game
{
    public class GameFlow : MonoBehaviour, ISystem,IOnInit
    {
        [SerializeField] private AssetReference homeScene;
        [SerializeField] private AssetReference gameScene;


        public AsyncOperationHandle<SceneInstance> ToHomeScene()
        {
            return homeScene.LoadSceneAsync();
        }

        public AsyncOperationHandle<SceneInstance> ToGameScene()
        {
            return gameScene.LoadSceneAsync();
        }

        public void OnInit()
        {
        }

        public void Dispose()
        {
        }
    }
}