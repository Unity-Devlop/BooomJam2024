using System;
using System.Collections.Generic;

namespace UnityToolkit
{
    public interface IModel
    {
    }

    /// <summary>
    /// 含有事件监听的数据层
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Model<T> : IModel where T : Model<T>
    {
        private event Action<T> OnEvent;

        public ICommand Register(Action<T> onEvent)
        {
            this.OnEvent += onEvent;
            return new CommonCommand(() => { Unregister(onEvent); });
        }

        public void Unregister(Action<T> onEvent)
        {
            OnEvent -= onEvent;
        }

        public void Trigger()
        {
            OnEvent?.Invoke(this as T);
        }
    }

    public class ModelCenter
    {
        private readonly Dictionary<Type, IModel> _models = new Dictionary<Type, IModel>();

        public T Register<T>() where T : IModel, new()
        {
            T model = new T();
            return Register(model);
        }

        public T Register<T>(T model) where T : IModel
        {
            var type = typeof(T);
            if (_models.ContainsKey(type))
            {
                throw new ArgumentException($"ModelCenter.Register<{type}>() failed, model already registered");
            }

            _models[type] = model;
            return Get<T>();
        }

        public TModel Get<TModel>() where TModel : IModel
        {
            var type = typeof(TModel);
            if (_models.TryGetValue(type, out var model))
            {
                return (TModel)model;
            }

            Log.Error($"please register model<{typeof(TModel)}> first");
            return default;
        }

        public void UnRegister<TModel>() where TModel : IModel
        {
            var type = typeof(TModel);
            if (_models.ContainsKey(type))
            {
                _models.Remove(type);
            }
        }
    }
}