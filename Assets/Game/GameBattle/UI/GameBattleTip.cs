using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public class GameBattleTip : MonoBehaviour
    {
        [SerializeField] private LoopVerticalScrollRect list;
        [SerializeField] private EasyGameObjectPool listItemPool;
        private List<string> _dataSource;

        private void Awake()
        {
            Global.Event.Listen<BattleInfoRecordEvent>(OnBattleTipEvent);
            Global.Event.Listen<BattlePokemonBuffActionEvent>(OnBattlePokemonBuffActionEvent);
            _dataSource = new List<string>();
            list.itemRenderer = RenderItem;
            list.ItemProvider = GetItem;
            list.ItemReturn = ReturnItem;
        }

        private void OnDestroy()
        {
            Global.Event.UnListen<BattlePokemonBuffActionEvent>(OnBattlePokemonBuffActionEvent);
            Global.Event.UnListen<BattleInfoRecordEvent>(OnBattleTipEvent);
        }

        private void ReturnItem(Transform transform1)
        {
            listItemPool.Release(transform1.gameObject);
        }

        private GameObject GetItem(int idx)
        {
            GameObject obj = listItemPool.Get();
            return obj;
        }

        private void OnDisable()
        {
            _dataSource.Clear();
            list.totalCount = 0;
            list.RefreshCells();
        }

        private async void RenderItem(Transform transform1, int idx)
        {
            TextMeshProUGUI tip = transform1.GetComponentInChildren<TextMeshProUGUI>();
            tip.text = _dataSource[idx];
            await UniTask.DelayFrame(1);
            var layoutElement = transform1.GetComponent<LayoutElement>();
            layoutElement.preferredHeight = tip.rectTransform.sizeDelta.y;
        }

        private void OnBattleTipEvent(BattleInfoRecordEvent obj)
        {
            _dataSource.Add(obj.tip);
            list.totalCount = _dataSource.Count;
            list.RefreshCells();
            list.SrollToCellWithinTime(list.totalCount - 1, .2f);
        }

        private void OnBattlePokemonBuffActionEvent(BattlePokemonBuffActionEvent obj)
        {
            _dataSource.Add($"{obj.pokemon} {obj.buff} 生效");
            list.totalCount = _dataSource.Count;
            list.RefreshCells();
            list.SrollToCellWithinTime(list.totalCount - 1, .2f);
        }
    }
}