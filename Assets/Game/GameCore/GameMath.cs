using System.Collections.Generic;
using cfg;

namespace Game
{
    public static class GameMath
    {
        /// <summary>
        /// 计算属性伤害适应度
        /// </summary>
        /// <param name="atk"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static float CalEleFit(ElementEnum atk, ElementEnum def)
        {
            var fit = Global.Table.ElementFitTable.Get(atk).Fit;
            return fit.GetValueOrDefault(def, 1);
        }
    }
}