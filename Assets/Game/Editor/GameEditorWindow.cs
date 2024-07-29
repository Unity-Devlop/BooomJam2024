#if UNITY_EDITOR
using System.IO;
using Game.Game;
using Game.GamePlay;
using Newtonsoft.Json;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public class GameEditorWindow : OdinMenuEditorWindow
    {
        private OdinMenuTree _tree;

        protected override OdinMenuTree BuildMenuTree()
        {
            _tree = new OdinMenuTree();
            if (!File.Exists(Consts.LocalPlayerDataPath))
            {
                File.WriteAllText(Consts.LocalPlayerDataPath, JsonConvert.SerializeObject(new PlayerData()));
            }

            // 本地玩家数据
            PlayerData data = JsonConvert.DeserializeObject<PlayerData>(File.ReadAllText(Consts.LocalPlayerDataPath));
            _tree.Add("Player Data", data);

            // 编辑一个预设的TrainerData
            TrainerData trainerData = new TrainerData();
            _tree.Add("Trainer Data", trainerData);
            
            
            FMODEditor fmodEditor = new FMODEditor();
            _tree.Add("FMOD Editor", fmodEditor);

            return _tree;
        }

        [MenuItem("Tools/Game Editor")]
        private static void OpenWindow()
        {
            var window = GetWindow<GameEditorWindow>();
            window.titleContent = new UnityEngine.GUIContent("Game Editor");
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1200, 500);
        }
    }
}
#endif