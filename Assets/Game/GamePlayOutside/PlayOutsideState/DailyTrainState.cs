using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolkit;

namespace Game
{
    public class DailyTrainState : PlayOutsideState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            UIRoot.Singleton.OpenPanel<DailyTrainPanel>();
        }

        public override void OnStay()
        {
            base.OnStay();
        }

        public override void OnExit()
        {
            base.OnExit();
            UIRoot.Singleton.ClosePanel<DailyTrainPanel>();
        }
    }
}
