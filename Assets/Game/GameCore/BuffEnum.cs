namespace Game
{
    public enum BuffEnum
    {
        None,
        /// <summary>
        /// 技能优先度：5。在自己面前张开屏障保护自己，本回合不会受到伤害。
        /// </summary>
        守护,
        胆小鬼,
        胆小鬼归来,// 上场时 所有数值1.5倍
        起风,// 我方全体速度+10 敌方全体速度-10
    }
}