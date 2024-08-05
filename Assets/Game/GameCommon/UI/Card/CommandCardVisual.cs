namespace Game
{
    public class CommandCardVisual : CardVisual
    {
        public async override void Initialize(Card card)
        {
            base.Initialize(card);
            // background.sprite = await Global.Get<ResourceSystem>().LoadSkillCardElementBg(card.data.config.Element);
        }
    }
}