using System;
using System.Runtime.CompilerServices;
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
        public IState<GameFlow> currentState => _stateMachine.currentState;

        public AsyncOperationHandle<SceneInstance> ToGameHomeScene()
        {
            return Addressables.LoadSceneAsync(homeScene);
        }

        public AsyncOperationHandle<SceneInstance> ToGameBattleScene()
        {
            return Addressables.LoadSceneAsync(gameBattleScene);
        }

        public AsyncOperationHandle<SceneInstance> ToGameOutsideScene()
        {
            return Addressables.LoadSceneAsync(gameOutsideScene);
        }


        public void OnInit()
        {
            _stateMachine = new StateMachine<GameFlow>(this);
            _stateMachine.Add<GameBattleState>(new GameBattleState());
            _stateMachine.Add<GameHomeState>(new GameHomeState());
            _stateMachine.Add<GameOutsideState>(new GameOutsideState());
            enabled = true;
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
            enabled = false;
        }

        public async UniTask ToGameHome()
        {
            _stateMachine.Change<GameHomeState>();
            await UniTask.CompletedTask;
        }

        public async UniTask ToGameBattle(TrainerData self, TrainerData enemy, BattleEnvData battleEnvData)
        {
            _stateMachine.SetParam(Consts.GameBattleData, battleEnvData);
            _stateMachine.SetParam(Consts.EnemyTrainerData, enemy);
            _stateMachine.SetParam(Consts.LocalPlayerTrainerData, self);
            _stateMachine.Change<GameBattleState>();
            await UniTask.CompletedTask;
        }

        public async UniTask ToGameOutside<TOutsideState>() where TOutsideState : IState<GamePlayOutsideMgr>
        {
            Type outsideStateType = typeof(TOutsideState);
            _stateMachine.SetParam(Consts.GamePlayOutsideStateType, outsideStateType);
            _stateMachine.Change<GameOutsideState>();
            await UniTask.CompletedTask;
        }

        public async UniTask ToGameOutside(Type outsideStateType)
        {
            _stateMachine.SetParam(Consts.GamePlayOutsideStateType, outsideStateType);
            _stateMachine.Change<GameOutsideState>();
            await UniTask.CompletedTask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetParam<T>(string key)
        {
            return _stateMachine.GetParam<T>(key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetParam<T>(string key, T data)
        {
            _stateMachine.SetParam(key, data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveParam(string key)
        {
            _stateMachine.RemoveParam(key);
        }
    }
}