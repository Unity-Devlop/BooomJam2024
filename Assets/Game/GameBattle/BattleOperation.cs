namespace Game.GamePlay
{
    public struct ActiveSkillBattleOperation : IBattleOperation
    {
        public ActiveSkillData data;
    }

    public struct ChangeHuluOperation : IBattleOperation
    {
        public int next;
    }

    public struct EndRoundOperation : IBattleOperation
    {
    }
}