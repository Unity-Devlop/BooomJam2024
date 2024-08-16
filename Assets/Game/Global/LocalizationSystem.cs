using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityToolkit;

namespace Game
{
    /// <summary>
    /// 本地化系统 做翻译用的
    /// </summary>
    public class LocalizationSystem : MonoBehaviour, ISystem, IAsyncOnInit
    {
        public bool initialized { get; }
        public void OnInit()
        {
            
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }
        
        public void SetLanguage(string language)
        {
            // TODO set language here
        }
        
        public string Get(string key)
        {
            return key;
        }
        
        public UniTask<string> GetAsync(string key)
        {
            return UniTask.FromResult(key);
        }
    }
}