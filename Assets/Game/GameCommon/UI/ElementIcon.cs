using cfg;
using UnityEngine.UI;

namespace Game
{
    public class ElementIcon : Image
    {
        public async void Bind(ElementEnum id)
        {
            var config = Global.Table.ElementTable.Get(id);
            sprite = await Global.Get<ResourceSystem>().LoadImage(config.UiIconPath);
        }
    }
}