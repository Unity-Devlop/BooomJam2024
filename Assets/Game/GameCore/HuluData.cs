using System;
using cfg;
using UnityToolkit;

namespace Game
{
    [Serializable]
    public class HuluData
    {
        public HuluEnum id;
        public BindData<HuluData> Bind { get; private set; }
        
        public HuluConfig Config => Global.Table.HuluTable.Get(id);
        
        public HuluData()
        {
            Bind = new BindData<HuluData>(this);
        }
    }
}