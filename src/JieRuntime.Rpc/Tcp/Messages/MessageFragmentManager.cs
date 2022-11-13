using System;
using System.Collections.Generic;
using System.Threading;

using JieRuntime.Utils;

namespace JieRuntime.Rpc.Tcp.Messages
{
    /// <summary>
    /// 表示消息分片管理器的类
    /// </summary>
    internal class MessageFragmentManager
    {
        #region --字段--
        private readonly Semaphore semaphore;
        private readonly Dictionary<long, MessageFragmentCache> fragmentsDict;
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="MessageFragmentManager"/> 类的新实例
        /// </summary>
        public MessageFragmentManager ()
        {
            this.semaphore = new Semaphore (1, 1);
            this.fragmentsDict = new Dictionary<long, MessageFragmentCache> ();
        }
        #endregion

        #region --公开方法--
        /// <summary>
        /// 将消息分片推入管理器中
        /// </summary>
        /// <param name="fragment">消息分片</param>
        public void Push (MessageFragment fragment)
        {
            try
            {
                this.semaphore.WaitOne ();
                if (!this.fragmentsDict.ContainsKey (fragment.MessageTag))
                {
                    this.fragmentsDict.Add (fragment.MessageTag, new MessageFragmentCache (fragment.FragmentCount));
                }
                this.fragmentsDict[fragment.MessageTag].Set (fragment);
            }
            finally
            {
                this.semaphore.Release ();
            }
        }

        /// <summary>
        /// 从分片管理器中拉取已形成完整数据包的数据
        /// </summary>
        /// <param name="result">存放拉取的数据包</param>
        /// <returns>如果成功拉取到了数据, 则为 <see langword="true"/>; 否则为 <see langword="false"/></returns>
        public bool TryPull (out Message? result)
        {
            try
            {
                this.semaphore.WaitOne ();
                result = null;

                foreach (KeyValuePair<long, MessageFragmentCache> cache in this.fragmentsDict)
                {
                    if (cache.Value.IsComplete ())
                    {
                        result = cache.Value.Get ();
                        break;
                    }
                }
                if (result != null)
                {
                    this.fragmentsDict.Remove (result.Value.MessageTag);
                    return true;
                }
                return false;
            }
            finally
            {
                this.semaphore.Release ();
            }
        }
        #endregion

        #region --内部类--
        private class MessageFragmentCache
        {
            #region --字段--
            private int count;
            private readonly MessageFragment[] fragments;
            #endregion

            #region --构造函数--
            public MessageFragmentCache (int count)
            {
                this.count = 0;
                this.fragments = new MessageFragment[count];
            }
            #endregion

            #region --公开方法--
            public void Set (MessageFragment fragment)
            {
                this.fragments[fragment.FragmentIndex] = fragment;
                this.count += 1;
            }

            public Message Get ()
            {
                byte[][] temp = new byte[this.fragments.Length][];
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = this.fragments[i].FragmentData;
                }

                return new Message (this.fragments[0].MessageType, this.fragments[0].MessageTag, ArrayUtils.Concat (temp));
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
