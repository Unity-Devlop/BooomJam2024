using UnityEngine.EventSystems;
using UnityToolkit;

namespace Game.GameHome
{
    public class DeveloperPanel : UIPanel,IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            CloseSelf();
        }
    }
}