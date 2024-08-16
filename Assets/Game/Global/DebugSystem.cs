using IngameDebugConsole;
using UnityEngine.InputSystem;
using UnityToolkit;

namespace Game
{
    public class DebugSystem : ISystem, IOnInit, IOnUpdate
    {
        public void OnInit()
        {
        }

        public void Dispose()
        {
        }

        private bool _updating;

        public async void OnUpdate()
        {
            if (_updating)
            {
                return;
            }

            _updating = true;
            if (Keyboard.current.backquoteKey.wasPressedThisFrame && UIRoot.Singleton.IsOpen<GameDebugPanel>())
            {
                UIRoot.Singleton.ClosePanel<GameDebugPanel>();
            }
            else if (Keyboard.current.backquoteKey.wasPressedThisFrame)
            {
                await UIRoot.Singleton.OpenPanelAsync<GameDebugPanel>();
                DebugLogManager.Instance.ShowLogWindow();
            }

            _updating = false;
        }
    }
}