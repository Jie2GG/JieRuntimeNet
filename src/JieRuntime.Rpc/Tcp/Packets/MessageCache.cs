using System.Collections.Concurrent;
using System.Collections.Generic;

using JieRuntime.Utils;

namespace JieRuntime.Rpc.Tcp.Packets
{
    class MessageCache
    {
        #region --字段--
        private readonly ConcurrentDictionary<long, MessageArray> caches;
        #endregion

        #region --构造函数--
        public MessageCache ()
        {
            this.caches = new ConcurrentDictionary<long, MessageArray> ();
        }
        #endregion

        #region --公开方法--
        public void Push (Message message)
        {
            if (!this.caches.ContainsKey (message.Id))
            {
                this.caches.TryAdd (message.Id, new MessageArray (message.Count));
            }
            this.caches[message.Id].Set (message);

        }


        public bool TryPull (out Message result)
        {
            result = null;

            foreach (KeyValuePair<long, MessageArray> cache in this.caches)
            {
                if (cache.Value.IsComplete ())
                {
                    result = cache.Value.Get ();
                    break;
                }
            }
            if (result != null)
            {
                this.caches.TryRemove (result.Id, out _);
                return true;
            }
            return false;

        }
        #endregion

        #region --内部类--
        private class MessageArray
        {
            #region --字段--
            private int count;
            private readonly Message[] fragments;
            #endregion

            #region --构造函数--
            public MessageArray (int count)
            {
                this.count = 0;
                this.fragments = new Message[count];
            }
            #endregion

            #region --公开方法--
            public void Set (Message fragment)
            {
                this.fragments[fragment.Index] = fragment;
                this.count += 1;
            }

            public Message Get ()
            {
                byte[][] temp = new byte[this.fragments.Length][];
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = this.fragments[i].Data;
                }

                return new Message ()
                {
                    Id = this.fragments[0].Id,
                    Index = 0,
                    Count = 1,
                    Type = this.fragments[0].Type,
                    Data = ArrayUtils.Concat (temp)
                };
            }

            public bool IsComplete ()
            {
                return this.count == this.fragments.Length;
            }
            #endregion
        }
        #endregion
    }
}
