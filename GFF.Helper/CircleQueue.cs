/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2017
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：02f75cd7-871c-4e62-a604-1ac1cfa60c4d
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.Helper
 * 类名称：CircleQueue
 * 创建时间：2017/2/20 17:30:31
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GFF.Helper
{
    /// <summary>
    /// 内部使用Circle的固定大小的Queue，当装满后，再加入对象，则将最老的那个对象替换掉。该类的实现是线程安全的。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircleQueue<T>
    {
        private object locker = new object();
        private T[] array;
        private int headIndex = 0;
        private int tailIndex = 0;
        public event CbGeneric<T> ObjectBeDiscarded;

        public CircleQueue(int size)
        {
            this.array = new T[size];
        }

        #region MaxCount
        private int maxCount = 0;
        /// <summary>
        /// 历史中最大的元素个数。
        /// </summary>
        public int MaxCount
        {
            get
            {
                return maxCount;
            }
        }
        #endregion

        #region Size
        /// <summary>
        /// 最大容量。
        /// </summary>
        public int Size
        {
            get
            {
                return this.array.Length;
            }
        }
        #endregion

        #region Count
        private int count = 0;
        /// <summary>
        /// 队列中的元素个数。
        /// </summary>
        public int Count
        {
            get
            {
                return count;
            }
        }
        #endregion

        #region Full
        public bool Full
        {
            get
            {
                return this.count >= this.array.Length;
            }
        }
        #endregion

        #region Enqueue
        /// <summary>
        /// 当装满后，再加入对象，则将最老的那个对象覆盖掉。
        /// </summary>        
        public void Enqueue(T obj)
        {
            lock (this.locker)
            {
                if (this.count == 0)
                {
                    this.array[this.headIndex] = obj;
                    this.tailIndex = this.headIndex;
                    ++this.count;
                    return;
                }

                T head = this.array[this.headIndex];

                this.tailIndex = (this.tailIndex + 1) % this.array.Length;
                this.array[this.tailIndex] = obj;
                ++this.count;

                if (this.tailIndex == this.headIndex)
                {
                    this.headIndex = (this.headIndex + 1) % this.array.Length;
                    --this.count;

                    if (this.ObjectBeDiscarded != null)
                    {
                        this.ObjectBeDiscarded(head);
                    }
                }

                if (this.count > this.maxCount)
                {
                    this.maxCount = this.count;
                }
            }
        }
        #endregion

        #region Peek
        /// <summary>
        /// 查看队列首部的元素，但是不从队列中移除。
        /// </summary>       
        public T Peek()
        {
            lock (this.locker)
            {
                T obj = default(T);
                if (this.count == 0)
                {
                    return obj;
                }

                return this.array[this.headIndex];
            }
        }
        #endregion

        #region PeekAt
        /// <summary>
        /// 查看指定位置的元素，但是不从队列中移除。
        /// </summary>       
        public T PeekAt(int index)
        {
            lock (this.locker)
            {
                T obj = default(T);
                if (this.count < index + 1)
                {
                    return obj;
                }

                int pos = (this.headIndex + index) % this.array.Length;
                return this.array[pos];
            }
        }
        #endregion

        #region Dequeue
        public bool Dequeue(out T obj)
        {
            lock (this.locker)
            {
                obj = default(T);
                if (this.count == 0)
                {
                    return false;
                }

                obj = this.array[this.headIndex];
                this.array[this.headIndex] = default(T);
                this.headIndex = (this.headIndex + 1) % this.array.Length;
                --this.count;

                return true;
            }
        }

        public T Dequeue()
        {
            T obj;
            this.Dequeue(out obj);

            return obj;
        }
        #endregion

        #region Clear
        public void Clear()
        {
            lock (this.locker)
            {
                this.count = 0;
            }
        }
        #endregion

        #region ChangeSize
        /// <summary>
        /// 更改大小。如果当前队列中元素个数大于新的尺寸，则丢弃部分老的元素。
        /// </summary>        
        public void ChangeSize(int newSize)
        {
            if (newSize == this.Size)
            {
                return;
            }

            lock (this.locker)
            {
                T[] newArray = new T[newSize];
                int discardCount = this.count - newSize;
                if (discardCount < 0)
                {
                    discardCount = 0;
                }

                int newCount = this.count - discardCount;
                for (int i = 0; i < newCount; i++)
                {
                    int index = (this.headIndex + i) % this.array.Length;
                    newArray[i] = this.array[index];
                }

                this.array = newArray;
                this.count = newCount;
                this.headIndex = 0;
                this.tailIndex = this.headIndex + newCount - 1;
            }
        }
        #endregion

        public override string ToString()
        {
            return string.Format("Count:{0},Size:{1}", this.count, this.Size);
        }
    }
}
