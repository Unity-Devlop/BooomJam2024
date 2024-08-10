using UnityEngine;

namespace Game.GamePlay
{
    public class BattleEnv : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer background;
        public async void Init(BattleEnvData battleEnvData)
        {
            background.sprite = await Global.Get<ResourceSystem>().LoadBattleBG(battleEnvData.id);
        }
    }
}