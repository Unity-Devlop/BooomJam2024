using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolkit;

namespace Game
{
    public class SelectOpponentState : IState<GamePlayOutsideMgr>
    {
        public void OnInit(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {
        }

        public void OnEnter(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {
            var opponentList = GamePlayOutsideMgr.Singleton.opponents;
            var metList = GamePlayOutsideMgr.Singleton.dateSystem.haveMet;
            opponentList.Shuffle();
            List<HuluData> hulus = new List<HuluData>();
            for (int i=0;i<opponentList.Count;++i)
            {
                if (!metList.Contains(opponentList[i].id))
                {
                    metList.Add(opponentList[i].id);
                    opponentList[i].hulus.Shuffle();
                    for (int j = 0; j < 3;++j)
                    {
                        var hulu = new HuluData();
                        hulu.id = opponentList[i].hulus[j];
                        hulu.Roll9Skills();
                        hulus.Add(hulu);
                    }
                    break;
                }
            }
            var player = Global.Get<DataSystem>().Get<GameData>().playerData.trainerData;
            var n_player = new TrainerData();
            n_player.trainerSkills = player.trainerSkills;
            n_player.datas.Add(player.datas[0]);
            n_player.datas.Add(player.datas[1]);
            n_player.datas.Add(player.datas[2]);
            var enemy = new TrainerData();
            enemy.RollTrainerSkill9();
            enemy.datas = hulus;
            var env = GameMath.RandomBattleEnvData();
            Global.Get<GameFlow>().ToGameBattle(n_player, enemy, env);
        }

        public void OnUpdate(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {
        }

        public void OnExit(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {

        }
    }
}
