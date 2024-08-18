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
        public PokemonHUD playerHud;
        public PokemonUIShow playerShow;
        public PokemonHUD enemyHud;
        public PokemonUIShow enemyShow;
        public TextMeshProUGUI playerName;
        public TextMeshProUGUI enemyName;
        public TextMeshProUGUI raceName;
        public Image enemyTeamIcon;

        private List<HuluData> playerHulus => Global.Get<DataSystem>().Get<GameData>().playerData.trainerData.datas;
        private List<int> playerChosenHulu = new List<int>();
        private List<int> enemyChosenHulu = new List<int>();
        private PlayerData player => Global.Get<DataSystem>().Get<GameData>().playerData;
        private TrainerData enemy;
        private BattleEnvData env;
        private float M_Time = 90f;
        private float m_time = 0;
        private int limit = 3;

        public override void OnLoaded()
        {
            base.OnLoaded();
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
            /*playerHud.UnBind();
              playerShow.UnBind();
              enemyHud.UnBind();
              enemyShow.UnBind();*/
            playerHud.gameObject.SetActive(false);
            playerShow.gameObject.SetActive(false);
            enemyHud.gameObject.SetActive(false);
            enemyShow.gameObject.SetActive(false);
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
                if (m_time <= 0)
                {
                    if (playerChosenHulu.Count < limit) AutoChoose();
                    EnterGame();
                }
            }
        }

        private void ShowUI()
        {
            var c = Global.Get<DataSystem>().Get<GameData>().date.count;
            if (c == 0) raceName.text = "64强争夺赛";
            else if (c == 1) raceName.text = "32强争夺赛";
            else if (c == 2) raceName.text = "16强争夺赛";
            else if(c==3) raceName.text = "8强争夺赛";
            else if(c==4) raceName.text = "4强争夺赛";
            else raceName.text = "决赛";

            playerName.text = player.trainerData.name;
            enemyName.text = enemy.name;
            LoadTeamIcon();
            for (int i = 0; i < chooseBtns.Count; ++i)
            {
                if (i < player.trainerData.datas.Count)
                {
                    chooseBtns[i].gameObject.SetActive(true);
                    playerHuluTexts[i].text = player.trainerData.datas[i].id.ToString();
                    LoadElementSprite(playerHuluImages[i], player.trainerData.datas[i]);
                }
                else
                {
                    chooseBtns[i].gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < enemyHuluTexts.Count; ++i)
            {
                enemyHuluTexts[i].text = enemy.datas[i].id.ToString();
                LoadElementSprite(enemyHuluImages[i], enemy.datas[i]);
            }
        }

        private async void LoadElementSprite(Image image, HuluData data)
        {
            image.sprite = await Global.Get<ResourceSystem>().LoadElementPortraitBox(data.elementEnum);
        }

        private async void LoadEnvSprite(BattleEnvData battleEnvData)
        {
            envImg.sprite = await Global.Get<ResourceSystem>().LoadImage(battleEnvData.config.BackgroundPath);
        }

        private async void LoadTeamIcon()
        {
            enemyTeamIcon.sprite= await Global.Get<ResourceSystem>().LoadImage(enemy.path);
        }
        public void ChooseHulu(int index)
        {
            playerHud.gameObject.SetActive(true);
            playerShow.gameObject.SetActive(true);
            enemyHud.gameObject.SetActive(true);
            enemyShow.gameObject.SetActive(true);
            if (playerChosenHulu.Count >= limit || playerChosenHulu.Contains(index)) return;
            playerChosenHulu.Add(index);
            playerHuluOrder[index].text = $"{playerChosenHulu.Count}";
            playerHuluOrder[index].gameObject.SetActive(true);
            playerHud.UnBind();
            playerHud.Bind(player.trainerData.datas[index]);
            playerShow.UnBind();
            playerShow.Bind(player.trainerData.datas[index]);
            EnemyChoose();
            if (playerChosenHulu.Count == limit && m_time > Consts.BattleChooseCountDown)
                m_time = Consts.BattleChooseCountDown;
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
                copy.guid = playerHulus[t].guid;
                tempPlayer.datas.Add(copy);
            }

            tempEnemy.trainerSkills = DeepCopyUtil.DeepCopyByJson(enemy.trainerSkills);
            foreach (var t in enemyChosenHulu)
            {
                enemy.datas[t].RecoverAllAbility();
                HuluData copy = DeepCopyUtil.DeepCopyByJson(enemy.datas[t]);
                copy.guid = enemy.datas[t].guid;
                tempEnemy.datas.Add(copy);
            }

            Global.Get<GameFlow>().ToGameBattle(tempPlayer, tempEnemy, env).Forget();
            CloseSelf();
        }

        private void AutoChoose()
        {
            for (int i = 0; i < player.trainerData.datas.Count; ++i)
            {
                if (playerChosenHulu.Count >= limit) break;
                if (!playerChosenHulu.Contains(i))
                {
                    playerChosenHulu.Add(i);
                    EnemyChoose();
                }
            }
        }

        private void LoadEnv()
        {
            env = GameMath.RandomBattleEnvData();
            envName.text = env.id.ToString();
            LoadEnvSprite(env);
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
            enemyHud.UnBind();
            enemyHud.Bind(enemy.datas[r]);
            enemyShow.UnBind();
            enemyShow.Bind(enemy.datas[r]);
        }
    }
}