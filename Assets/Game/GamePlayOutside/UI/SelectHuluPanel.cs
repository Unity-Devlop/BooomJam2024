using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public class SelectHuluPanel : UIPanel
    {
        public TextMeshProUGUI envName;
        public Image envImg;
        public List<Button> chooseBtns = new List<Button>();
        public List<Image> playerHuluImages=new List<Image>();
        public List<Image> enemyHuluImages = new List<Image>();
        public List<TextMeshProUGUI> playerHuluTexts = new List<TextMeshProUGUI>();
        public List<TextMeshProUGUI> enemyHuluTexts = new List<TextMeshProUGUI>();
        public List<TextMeshProUGUI> playerHuluOrder = new List<TextMeshProUGUI>();
        public List<TextMeshProUGUI> enemyHuluOrder = new List<TextMeshProUGUI>();
        public TextMeshProUGUI countDown;

        private List<HuluData> playerHulus;
        private List<int> playerChosenHulu = new List<int>();
        private List<int> enemyChosenHulu = new List<int>();
        private PlayerData player;
        private TrainerData enemy;
        private BattleEnvData env;
        private float M_Time = 90f;
        private float m_time = 0;

        public override void OnLoaded()
        {
            base.OnLoaded();
            playerHulus = Global.Get<DataSystem>().Get<GameData>().playerData.trainerData.datas;
            player = Global.Get<DataSystem>().Get<GameData>().playerData;
        }

        public override void OnOpened()
        {
            base.OnOpened();
            m_time = M_Time;
            LoadEnv();
            LoadOpponent();
            ShowUI();
        }

        public override void OnClosed()
        {
            base.OnClosed();
            playerChosenHulu.Clear();
            enemyChosenHulu.Clear();
            for (int i = 0; i < playerHuluOrder.Count; ++i) playerHuluOrder[i].gameObject.SetActive(false);
            for (int i = 0; i < enemyHuluOrder.Count; ++i) enemyHuluOrder[i].gameObject.SetActive(false);
        }

        private void Update()
        {
            if(m_time>0)
            {
                m_time -= Time.deltaTime;
                countDown.text = $"{(int)Mathf.Ceil(m_time)}";
                if (m_time <= 0) EnterGame();
            }
        }

        private void ShowUI()
        {
            for (int i = 0; i < chooseBtns.Count; ++i)
            {
                if (i < player.trainerData.datas.Count)
                {
                    chooseBtns[i].gameObject.SetActive(true);
                    playerHuluTexts[i].text = player.trainerData.datas[i].id.ToString();
                }
                else
                {
                    chooseBtns[i].gameObject.SetActive(false);
                }
            }

            for(int i=0;i<enemyHuluTexts.Count;++i)
            {
                enemyHuluTexts[i].text = enemy.datas[i].id.ToString();
            }
        }

        public void ChooseHulu(int index)
        {
            if (playerChosenHulu.Count >= 3||playerChosenHulu.Contains(index)) return;
            playerChosenHulu.Add(index);
            playerHuluOrder[index].text = $"{playerChosenHulu.Count}";
            playerHuluOrder[index].gameObject.SetActive(true);
            EnemyChoose();
            if (playerChosenHulu.Count == 3 && m_time > 10f) m_time = 10f; 
        }

        private void EnterGame()
        {
            TrainerData temp_player = new();
            TrainerData temp_enemy = new();
            temp_player.trainerSkills = player.trainerData.trainerSkills;
            for(int i=0;i< playerChosenHulu.Count;++i)
            {
                playerHulus[playerChosenHulu[i]].RecoverAllAbility();
                temp_player.datas.Add(playerHulus[playerChosenHulu[i]]);
            }
            temp_enemy.trainerSkills = enemy.trainerSkills;
            for (int i = 0; i < enemyChosenHulu.Count; ++i)
            {
                temp_enemy.datas.Add(enemy.datas[enemyChosenHulu[i]]);
            }
            Global.Get<GameFlow>().ToGameBattle(temp_player, temp_enemy, env);
            CloseSelf();
        }

        private void LoadEnv()
        {
            env = GameMath.RandomBattleEnvData();
            envName.text = env.id.ToString();
        }

        private void LoadOpponent()
        {
            var opponentList = GamePlayOutsideMgr.Singleton.opponents.opponents;
            var metList = GamePlayOutsideMgr.Singleton.dateSystem.hasMeet;
            opponentList.Shuffle();
            var target = opponentList.Find(x => !metList.Contains(x.name));
            enemy = target;
        }

        private void EnemyChoose()
        {
            if (enemyChosenHulu.Count >= 3) return;
            int r = UnityEngine.Random.Range(0, enemy.datas.Count);
            while(enemyChosenHulu.Contains(r))
            {
                r = UnityEngine.Random.Range(0, enemy.datas.Count);
            }
            enemyChosenHulu.Add(r);
            enemyHuluOrder[r].text = $"{enemyChosenHulu.Count}";
            enemyHuluOrder[r].gameObject.SetActive(true);
        }
    }
}
