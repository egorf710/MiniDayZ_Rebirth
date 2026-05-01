using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.GAME._scripts.Fic
{
    public static class EventBus
    {
        private static Dictionary<string, List<object>> _signalsCallbacks = new Dictionary<string, List<object>>();

        public static void Subscribe<T>(Action<T> callback)
        {
            string key = typeof(T).Name;

            if(_signalsCallbacks.ContainsKey(key))
            {
                _signalsCallbacks[key].Add(callback);
            }
            else
            {
                _signalsCallbacks.Add(key, new List<object>() { callback });
            }
        }
        public static void Unsubscribe<T>(Action<T> callback)
        {
            string key = typeof(T).Name;

            if (_signalsCallbacks.ContainsKey(key))
            {
                _signalsCallbacks[key].Remove(callback);
            }
        }
        public static void Invoke<T>(T signal)
        {
            string key = typeof(T).Name;

            if (_signalsCallbacks.ContainsKey(key))
            {
                foreach (var obj in _signalsCallbacks[key])
                {
                    var callback = obj as Action<T>;
                    callback?.Invoke(signal);
                }
            }
        }
    }
}
