using System;
using System.Collections.Generic;
using System.Linq;


namespace Assets.GAME._scripts.Fic
{
    public static class ServiceLocator
    {
        private static Dictionary<string, object> _services = new Dictionary<string, object>();

        public static T Get<T>()
        {
            string key = typeof(T).Name;

            if(_services.ContainsKey(key))
            {
                return (T) _services[key];
            }

            return default(T);
        }
        public static void Register<T>(T service)
        {
            string key = typeof(T).Name;

            if (!_services.ContainsKey(key))
            {
                _services.Add(key, service);
            }
        }
        public static void Unregister<T>(T service)
        {
            string key = typeof(T).Name;

            if (_services.ContainsKey(key))
            {
                _services.Remove(key);
            }
        }

        public static void Reset()
        {
            _services.Clear();
        }
    }
}
