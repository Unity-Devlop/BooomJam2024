using System;
using System.Collections.Generic;
using UnityToolkit;

namespace Game
{
    [Serializable]
    public class GlobalData : IModel
    {
        public bool newbieGuide = true;
        public HashSet<Type> newbieGuideSet = new HashSet<Type>();
    }
}