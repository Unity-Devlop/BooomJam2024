using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
        public List<Image> playerHuluImages = new List<Image>();
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
        private int limit = 3;

        public override void OnLoaded()
        {
            base.OnLoaded();
            playerHulus = Global.Get<DataSystem>().Get<GameData>().playerData.trainerData.datas;
            player = Global.Get<DataSystem>().Get<GameData>().playerData;
        }

        public override void OnOpened()
        {
            base.OnOpened();
            if (Global.Get<DataSystem>().Get<GameData>().ruleConfig.ruleList
                .Contains(GameRuleEnum.每局游戏上场的角色数量改为4)) limit = 4;
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
            if (m_time > 0)
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

            for (int i = 0; i < enemyHuluTexts.Count; ++i)
            {
                enemyHuluTexts[i].text = enemy.datas[i].id.ToString();
            }
        }

        public void ChooseHulu(int index)
        {
            if (playerChosenHulu.Count >= limit || playerChosenHulu.Contains(index)) return;
            playerChosenHulu.Add(index);
            playerHuluOrder[index].text = $"{playerChosenHulu.Count}";
            playerHuluOrder[index].gameObject.SetActive(true);
            EnemyChoose();
            if (playerChosenHulu.Count == limit && m_time > Consts.BattleChooseCountDown) m_time = Consts.BattleChooseCountDown;
        }

        private void EnterGame()
        {
            TrainerData tempPlayer = new TrainerData
            {
                name = player.trainerData.name
            };
            TrainerData tempEnemy = new TrainerData
            {
                name = enemy.name
            };

            tempPlayer.trainerSkills = DeepCopyUtil.DeepCopyByJson(player.trainerData.trainerSkills);
            foreach (var t in playerChosenHulu)
            {
                playerHulus[t].RecoverAllAbility();
                var copy = DeepCopyUtil.DeepCopyByJson(playerHulus[t]);
                tempPlayer.datas.Add(copy);
            }

            tempEnemy.trainerSkills = DeepCopyUtil.DeepCopyByJson(enemy.trainerSkills);
            foreach (var t in enemyChosenHulu)
            {
                enemy.datas[t].RecoverAllAbility();
                HuluData copy = DeepCopyUtil.DeepCopyByJson(enemy.datas[t]);
                tempEnemy.datas.Add(copy);
            }

            Global.Get<GameFlow>().ToGameBattle(tempPlayer, tempEnemy, env).Forget();
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
            if (enemyChosenHulu.Count >= limit) return;
            int r = UnityEngine.Random.Range(0, enemy.datas.Count);
            while (enemyChosenHulu.Contains(r))
            {
                r = UnityEngine.Random.Range(0, enemy.datas.Count);
            }

            enemyChosenHulu.Add(r);
            enemyHuluOrder[r].text = $"{enemyChosenHulu.Count}";
            enemyHuluOrder[r].gameObject.SetActive(true);
        }
    }
}