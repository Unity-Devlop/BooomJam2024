using System;
using System.IO;
using Newtonsoft.Json;
using UnityToolkit;

namespace Game
{
    [Serializable]
    public class PlayerData 
    {
        public string name;

        [JsonIgnore] public BindData<PlayerData> bind { get; private set; }

        public TrainerData trainerData;
        public bool isNewbie;

        public PlayerData(bool isNewbie = true)
        {
            this.isNewbie = isNewbie;
            bind = new BindData<PlayerData>(this);
            trainerData = new TrainerData();
        }

        [Sirenix.OdinInspector.Button]
        private void SaveToLocal()
        {
            string jsonStr = JsonConvert.SerializeObject(this);
            File.WriteAllText(Consts.LocalPlayerDataPath, jsonStr);
        }
    }
}