using System;
using System.Collections.Generic;
using System.IO;
using Game.Game;
using Newtonsoft.Json;
using UnityToolkit;

namespace Game
{
    [Serializable]
    public class PlayerData : IModel
    {
        public string name;
        [JsonIgnore] public BindData<PlayerData> bind { get; private set; }

        public TrainerData trainerData;

        public PlayerData()
        {
            bind = new BindData<PlayerData>(this);
        }

        [Sirenix.OdinInspector.Button]
        private void SaveToLocal()
        {
            string jsonStr = JsonConvert.SerializeObject(this);
            File.WriteAllText(Consts.LocalPlayerDataPath, jsonStr);
        }
    }
}