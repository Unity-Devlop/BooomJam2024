using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.GamePlay;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public class GameBattlePanel : UIPanel
    {
        [SerializeField] private BattleCardContainer selfBattleCardContainer;
        [SerializeField] private Button endRoundButton;
        [SerializeField] private LeftTeamHuluView leftTeamHuluView;
        private IBattleTrainer _trainer;

        private void Awake()
        {
            endRoundButton.onClick.AddListener(OnEndRoundButtonClick);
        }

        public override void OnOpened()
        {
            base.OnOpened();
        }
        

        public override void OnClosed()
        {
            base.OnClosed();
        }

        private void OnEndRoundButtonClick()
        {
            _trainer.PushOperation(default(EndRoundOperation));
        }

        public void Bind(IBattleTrainer battleTrainer)
        {
            _trainer = battleTrainer;
            _trainer.OnDrawCard += DrawCardToHand;
            _trainer.OnDiscardCardFromHand += DiscardCard;
            _trainer.OnConsumedCard += OnConsumedCard;
            _trainer.OnDestroyCard += DestroyCard;
            _trainer.OnStartCalOperation += StartCalOperation;
            _trainer.OnEndCalOperation += EndCalOperation;
            _trainer.OnUseCardFromHand += UseHandHandCard;
            _trainer.OnDiscardToDraw += DiscardToDraw;

            selfBattleCardContainer.Bind(battleTrainer);
            leftTeamHuluView.Bind(battleTrainer);
        }




        public void UnBind()
        {
            _trainer.OnDrawCard -= DrawCardToHand;
            _trainer.OnDiscardCardFromHand -= DiscardCard;
            _trainer.OnConsumedCard -= OnConsumedCard;
            _trainer.OnDestroyCard -= DestroyCard;
            _trainer.OnStartCalOperation -= StartCalOperation;
            _trainer.OnEndCalOperation -= EndCalOperation;
            _trainer.OnUseCardFromHand -= UseHandHandCard;
            _trainer.OnDiscardToDraw -= DiscardToDraw;


            selfBattleCardContainer.UnBind();
            leftTeamHuluView.UnBind();
        }


        private async UniTask EndCalOperation()
        {
            await selfBattleCardContainer.EndCalOperation();
            await leftTeamHuluView.EndCalOperation();
        }

        private async UniTask StartCalOperation()
        {
            var cardOper = selfBattleCardContainer.StartCalOperation();
            var switchOper = leftTeamHuluView.StartCalOperation();
            await UniTask.WhenAny(cardOper, switchOper);
        }

        private async UniTask UseHandHandCard(ActiveSkillData arg)
        {
            await selfBattleCardContainer.UseFromHand(arg);
        }

        private async UniTask DestroyCard(List<ActiveSkillData> arg)
        {
            await selfBattleCardContainer.DestroyCard(arg);
        }

        private async UniTask DiscardCard(List<ActiveSkillData> arg, IBattleTrainer trainer)
        {
            await selfBattleCardContainer.Discard(arg);
        }
        
        private async UniTask OnConsumedCard(List<ActiveSkillData> arg)
        {
            await selfBattleCardContainer.ConsumedCard(arg);
        }

        private async UniTask DrawCardToHand(List<ActiveSkillData> obj)
        {
            await selfBattleCardContainer.DrawCardToHand(obj);
        }

        private async UniTask DiscardToDraw(List<ActiveSkillData> discard, List<ActiveSkillData> darw)
        {
            await selfBattleCardContainer.DiscardToDraw(discard, darw);
        }
    }
}