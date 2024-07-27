using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.Serialization;
using UnityToolkit;

namespace Game
{
    public class GameFlow : MonoBehaviour, ISystem, IOnInit
    {
        [SerializeField] private AssetReference homeScene;
        [SerializeField] private AssetReference gameBattleScene;
        [SerializeField] private AssetReference gameOutsideScene;

        private StateMachine<GameFlow> _stateMachine;

        public AsyncOperationHandle<SceneInstance> ToGameHomeScene()
        {
            return homeScene.LoadSceneAsync();
        }

        public AsyncOperationHandle<SceneInstance> ToGameBattleScene()
        {
            return gameBattleScene.LoadSceneAsync();
        }
        
        public AsyncOperationHandle<SceneInstance> ToGameOutsideScene()
        {
            return gameOutsideScene.LoadSceneAsync();
        }
        

        public void OnInit()
        {
            _stateMachine = new StateMachine<GameFlow>(this);
            _stateMachine.Add<GameBattleState>(new GameBattleState());
            _stateMachine.Add<GameHomeState>(new GameHomeState());
            _stateMachine.Add<GameOutsideState>(new GameOutsideState());
        }

        public void Run<T>() where T : IState<GameFlow>
        {
            _stateMachine.Run<T>();
        }

        private void Update()
        {
            _stateMachine.OnUpdate();
        }

        public void Dispose()
        {
        }

        public async UniTask ToGameHome()
        {
            _stateMachine.Change<GameHomeState>();
            await UniTask.CompletedTask;
        }

        public async UniTask ToGameBattle()
        {
            _stateMachine.Change<GameBattleState>();
            await UniTask.CompletedTask;
        }

        public async UniTask ToGameOutside()
        {
            _stateMachine.Change<GameOutsideState>();
            await UniTask.CompletedTask;
        }

        public T GetParam<T>(string battleDataName)
        {
            return _stateMachine.GetParam<T>(battleDataName);
        }

        public void SetParam<T>(string battleDataName, T data)
        {
            _stateMachine.SetParm(battleDataName, data);
        }
    }
}