namespace Game.GamePlay
{
    public struct ActiveSkillBattleOperation : IBattleOperation
    {
        public ActiveSkillData data;

        public override string ToString()
        {
            return $"{data.config.Id}";
        }
    }

    public struct ChangeHuluOperation : IBattleOperation
    {
        public int next;
    }

    public struct EndRoundOperation : IBattleOperation
    {
    }
}