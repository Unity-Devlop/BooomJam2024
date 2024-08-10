using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityToolkit;

namespace Game
{
    public class DataSystem : MonoBehaviour, ISystem, IOnInit
    {
        private ModelCenter _modelCenter;

        public void OnInit()
        {
            _modelCenter = new ModelCenter();
        }

        public void Add<T>(T data) where T : IModel
        {
            _modelCenter.Register<T>(data);
        }

        public T Get<T>() where T : IModel
        {
            return _modelCenter.Get<T>();
        }

        public void Dispose()
        {
        }

        public bool LoadPrevGameData(out GameData data)
        {
            // 本地玩家数据
            if (File.Exists(Consts.LocalGameDataPath))
            {
                data = JsonConvert.DeserializeObject<GameData>(File.ReadAllText(Consts.LocalGameDataPath));
                return true;
            }

            data = null;
            return false;
        }

        public void ClearGameData()
        {
            if (File.Exists(Consts.LocalGameDataPath))
            {
                File.Delete(Consts.LocalGameDataPath);
            }
        }

        public void WriteGameData(GameData data)
        {
            File.WriteAllText(Consts.LocalGameDataPath, JsonConvert.SerializeObject(data));
        }
    }
}