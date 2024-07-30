using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.GamePlay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public class GameBattlePanel : UIPanel
    {
        [SerializeField] private CardHorizontalContainer selfCardContainer;
        [SerializeField] private Button endRoundButton;
        [SerializeField] private LeftTeamHuluView leftTeamHuluView;

        private IBattleTrainer _trainer;
        public TextMeshProUGUI tipText;

        private void Awake()
        {
            endRoundButton.onClick.AddListener(OnEndRoundButtonClick);
        }

        public override void OnOpened()
        {
            base.OnOpened();
            Global.Event.Listen<BattleTipEvent>(OnBattleTip);
        }

        private void OnBattleTip(BattleTipEvent obj)
        {
            tipText.text = obj.tip;
        }

        public override void OnClosed()
        {
            base.OnClosed();
            Global.Event.UnListen<BattleTipEvent>(OnBattleTip);
        }

        private void OnEndRoundButtonClick()
        {
            _trainer.PushOperation(default(EndRoundOperation));
        }

        public void Bind(IBattleTrainer battleTrainer)
        {
            _trainer = battleTrainer;
            _trainer.OnDrawCard += DrawCardToHand;
            _trainer.OnDiscardCard += DiscardCard;
            _trainer.OnDestroyCard += DestroyCard;
            _trainer.OnStartCalOperation += StartCalOperation;
            _trainer.OnEndCalOperation += EndCalOperation;
            _trainer.OnUseHandCard += UseHandHandCard;
            _trainer.OnDiscardToDraw += DiscardToDraw;

            selfCardContainer.Bind(battleTrainer);
            leftTeamHuluView.Bind(battleTrainer);
        }


        public void UnBind()
        {
            _trainer.OnDrawCard -= DrawCardToHand;
            _trainer.OnDiscardCard -= DiscardCard;
            _trainer.OnDestroyCard -= DestroyCard;
            _trainer.OnStartCalOperation -= StartCalOperation;
            _trainer.OnEndCalOperation -= EndCalOperation;
            _trainer.OnUseHandCard -= UseHandHandCard;
            _trainer.OnDiscardToDraw -= DiscardToDraw;


            selfCardContainer.UnBind();
            leftTeamHuluView.UnBind();
        }


        private async UniTask EndCalOperation()
        {
            await selfCardContainer.EndCalOperation();
            await leftTeamHuluView.EndCalOperation();
        }

        private async UniTask StartCalOperation()
        {
            var cardOper = selfCardContainer.StartCalOperation();
            var switchOper = leftTeamHuluView.StartCalOperation();
            await UniTask.WhenAny(cardOper, switchOper);
        }

        private async UniTask UseHandHandCard(ActiveSkillData arg)
        {
            await selfCardContainer.UseFromHand(arg);
        }

        private async UniTask DestroyCard(List<ActiveSkillData> arg)
        {
            await selfCardContainer.DestroyCard(arg);
        }

        private async UniTask DiscardCard(List<ActiveSkillData> arg)
        {
            // TODO 不能remove 而是要discard
            await selfCardContainer.Discard(arg);
        }

        private async UniTask DrawCardToHand(List<ActiveSkillData> obj)
        {
            await selfCardContainer.DrawCardToHand(obj);
        }

        private async UniTask DiscardToDraw(List<ActiveSkillData> discard, List<ActiveSkillData> darw)
        {
            await selfCardContainer.DiscardToDraw(discard, darw);
        }
    }
}