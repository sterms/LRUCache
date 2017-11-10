using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sterms
{
    public class LRUCache<TKey, TValue>
    {
        private Dictionary<TKey, LRUNode> m_CacheMap = new Dictionary<TKey, LRUNode>();
        private Queue<KeyValuePair<TKey, long>> m_TimeQueue = new Queue<KeyValuePair<TKey, long>>();
        private int m_Capacity;
        private long m_SystemClock = 0;

        public LRUCache(int capacity)
        {
            m_Capacity = capacity;
        }

        public TValue Get(TKey key)
        {
            if (m_CacheMap.ContainsKey(key))
            {
                m_CacheMap[key].timeStamp = m_SystemClock++;
                m_TimeQueue.Enqueue(new KeyValuePair<TKey, long>(key, m_CacheMap[key].timeStamp));                
                return m_CacheMap[key].value;
            }
            return default(TValue);
        }

        public void Put(TKey key, TValue value)
        {
            if (m_CacheMap.ContainsKey(key)) m_CacheMap[key].value = value;
            else if (m_CacheMap.Count >= m_Capacity)
            {
                while(m_TimeQueue.Count > 0)
                {
                    KeyValuePair<TKey, long> lastEntry = m_TimeQueue.Dequeue();
                    if (!m_CacheMap.ContainsKey(lastEntry.Key)) continue;
                    if(lastEntry.Value == m_CacheMap[lastEntry.Key].timeStamp)
                    {
                        m_CacheMap.Remove(lastEntry.Key);
                        m_CacheMap.Add(key, new LRUNode(value));
                        break;
                    }
                }
            }
            else
            {
                m_CacheMap.Add(key, new LRUNode(value));
            }
            m_CacheMap[key].timeStamp = m_SystemClock++;
            m_TimeQueue.Enqueue(new KeyValuePair<TKey, long>(key, m_CacheMap[key].timeStamp));
        }

        public IEnumerable<TKey> Keys
        {
            get { return m_CacheMap.Keys; }
        }


        public IEnumerable<TValue> Values
        {
            get
            {
                IEnumerable<LRUNode> valueList = m_CacheMap.Values;
                foreach (LRUNode node in valueList)
                {
                    yield return node.value;
                }
            }
        }


        private class LRUNode
        {
            public TValue value;
            public long timeStamp;

            public LRUNode(TValue value) { this.value = value; }
        }

    }

}
