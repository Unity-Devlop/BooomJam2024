using UnityEngine;
using UnityToolkit;

namespace Game
{
    public class DataSystem : MonoBehaviour, ISystem, IOnInit
    {
        private ModelCenter _modelCenter;

        public void OnInit()
        {
            _modelCenter = new ModelCenter();
        }

        public void Add<T>(T data) where T : IModel
        {
            _modelCenter.Register<T>(data);
        }

        public T Get<T>() where T : IModel
        {
            return _modelCenter.Get<T>();
        }

        public void Dispose()
        {
        }
    }
}