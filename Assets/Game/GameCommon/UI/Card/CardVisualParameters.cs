using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "CardVisualParameters", menuName = "Game/CardVisualParameters")]
    public class CardVisualParameters : ScriptableObject
    {
        public AnimationCurve positioning;
        public float positioningInfluence = .1f;
        public AnimationCurve rotation;
        public float rotationInfluence = 10f;
    }
}