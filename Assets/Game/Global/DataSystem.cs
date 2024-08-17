﻿using System;
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

            // 全局数据
// #if UNITY_EDITOR
//             Add(new GlobalData
//             {
//                 newbieGuide = true
//             });
// #else
            if (File.Exists(Consts.LocalGlobalDataPath))
            {
                Add(JsonConvert.DeserializeObject<GlobalData>(File.ReadAllText(Consts.LocalGlobalDataPath)));
            }
            else
            {
                Add(new GlobalData
                {
                    newbieGuide = true
                });
            }
// #endif
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
            if (Get<GameData>() == null)
            {
                return;
            }

            Global.LogInfo($"Save Game Data");
            WriteGameData(Get<GameData>());
            WriteGlobalData();
        }


        public bool LoadPrevGameData(out GameData data)
        {
            // 本地玩家数据
            if (File.Exists(Consts.LocalGameDataPath))
            {
                data = JsonConvert.DeserializeObject<GameData>(File.ReadAllText(Consts.LocalGameDataPath));
                if (data != null)
                {
                    if (data.battleSettlementData != null)
                    {
                        Global.LogInfo($"恢复战斗结算数据");
                        data.battleSettlementData.localPlayerTrainerData = data.playerData.trainerData;
                    }
                }

                return data != null;
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

        private void WriteGlobalData()
        {
            Global.LogInfo($"Save Global Data:{Get<GlobalData>()},path:{Consts.LocalGlobalDataPath}");
            File.WriteAllText(Consts.LocalGlobalDataPath, JsonConvert.SerializeObject(Get<GlobalData>()));
        }

        private void WriteGameData(GameData data)
        {
            Global.LogInfo($"Save Game Data:{data},path:{Consts.LocalGameDataPath}");
            File.WriteAllText(Consts.LocalGameDataPath, JsonConvert.SerializeObject(data));
        }
    }
}