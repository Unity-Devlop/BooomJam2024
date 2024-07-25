using System;

namespace UnityToolkit
{
    public sealed class BindData<T> : IDisposable
    {
        private event Action<T> Listeners = delegate { };
        private readonly T _data;

        public BindData(T data)
        {
            _data = data;
        }

        public void Invoke()
        {
            Listeners(_data);
        }

        public ICommand Listen(Action<T> action)
        {
            Listeners += action;
            return new CommonCommand(() => UnListen(action));
        }

        public void UnListen(Action<T> action)
        {
            Listeners -= action;
        }

        public void Dispose()
        {
            Listeners = null;
        }
    }
}