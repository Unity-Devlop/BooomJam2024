using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public class DailyTrainPanel : UIPanel
    {
        public Button confirmBtn;
        [SerializeField] private RectTransform huluNameRoot;
        private TextMeshProUGUI[] _nameTextList;

        [field: SerializeField] public DailyTrainTable table { get; private set; }
        private PlayerData _playerData;

        private void Awake()
        {
            _nameTextList = huluNameRoot.GetComponentsInChildren<TextMeshProUGUI>();
        }

        public override void OnLoaded()
        {
            base.OnLoaded();
            Register();
        }

        public override void OnDispose()
        {
            UnRegister();
            base.OnDispose();
        }

        public override void OnOpened()
        {
            base.OnOpened();
        }

        private void Register()
        {
            confirmBtn.onClick.AddListener(Confirm);
        }

        private void UnRegister()
        {
            confirmBtn.onClick.RemoveListener(Confirm);
        }

        public void Bind(PlayerData playerData)
        {
            Assert.IsNull(_playerData);
            _playerData = playerData;
            _playerData.bind.Listen(OnData);
            OnData(playerData);
        }

        private void OnData(PlayerData data)
        {
            for (int i = 0; i < data.trainerData.datas.Count; ++i)
            {
                _nameTextList[i].transform.parent.gameObject.SetActive(true);
                _nameTextList[i].text = Global.Table.HuluTable.Get(data.trainerData.datas[i].id).Id.ToString();
            }

            for (int i = data.trainerData.datas.Count; i < _nameTextList.Length; ++i)
            {
                _nameTextList[i].transform.parent.gameObject.SetActive(false);
            }
        }

        public void UnBind()
        {
            if (_playerData != null)
            {
                _playerData.bind.UnListen(OnData);
                _playerData = null;
            }
        }


        private void Confirm()
        {
            table.Train(_playerData.trainerData.datas);
            GamePlayOutsideMgr.Singleton.machine.Change<SpecialTrainState>();
        }
    }
}