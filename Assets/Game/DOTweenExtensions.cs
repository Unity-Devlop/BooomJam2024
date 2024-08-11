using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Game
{
    public static class DOTweenExtension
    {
        public static TweenerCore<float, float, FloatOptions> DOAlpha(this SpriteRenderer renderer, float to,
            float duration)
        {
            return DOTween.To(() => renderer.color.a,
                x => renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, x), to, duration);
        }

        public static TweenerCore<float, float, FloatOptions> DOAlpha(this CanvasGroup group, float to, float duration)
        {
            return DOTween.To(() => group.alpha, x => group.alpha = x, to, duration);
        }
    }
}