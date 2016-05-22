using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Identity.OAuth.WeChat
{
    internal partial class WeChatAuthenticationHandler
    {
        private class StateKeeper
        {
            private static readonly SortedList<string, string> _states = new SortedList<string, string>();
            private static object _locker = new object();
            private StateKeeper() { }

            public static string Put(string state)
            {
                lock (_locker)
                {
                    if (string.IsNullOrEmpty(state))
                    {
                        return null;
                    }

                    var key = Guid.NewGuid().ToString();
                    _states.Add(key, state);
                    return key;
                }
            }

            public static string Pop(string key)
            {
                lock (_locker)
                {
                    if (string.IsNullOrEmpty(key) || !_states.ContainsKey(key))
                    {
                        return null;
                    }

                    var state = _states[key];
                    _states.Remove(state);
                    return state;
                }
            }
        }
    }
}