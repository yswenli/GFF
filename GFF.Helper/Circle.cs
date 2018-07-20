/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2017
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：90239232-36c7-4c7d-9692-83c36c96fc30
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.Helper
 * 类名称：Circle
 * 创建时间：2017/2/20 15:37:31
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
    /// Circle 圈结构。非线程安全。
    /// </summary>
    /// <typeparam name="T">圈的每个节点存储的对象的类型</typeparam>
    public class Circle<T>
    {
        private IList<T> list = new List<T>();
        private int currentPosition = 0;

        #region Ctor
        public Circle()
        {
        }

        public Circle(IList<T> _list)
        {
            if (_list != null)
            {
                this.list = _list;
            }
        }
        #endregion

        #region Header
        public T Header
        {
            get
            {
                if (list.Count == 0)
                {
                    return default(T);
                }

                return this.list[0];
            }
        }
        #endregion

        #region Tail
        public T Tail
        {
            get
            {
                if (list.Count == 0)
                {
                    return default(T);
                }

                return this.list[this.list.Count - 1];
            }
        }
        #endregion

        #region Count
        public int Count
        {
            get
            {
                return this.list.Count;
            }
        }
        #endregion

        #region Current
        public T Current
        {
            get
            {
                if (this.list.Count == 0)
                {
                    return default(T);
                }

                return this.list[this.currentPosition];
            }
        }
        #endregion

        #region MoveNext
        public void MoveNext()
        {
            if (this.list.Count == 0)
            {
                return;
            }

            this.currentPosition = (this.currentPosition + 1) % this.list.Count;
        }
        #endregion

        #region MoveBack
        public void MoveBack()
        {
            if (this.list.Count == 0)
            {
                return;
            }

            this.currentPosition = (this.currentPosition + this.list.Count - 1) % this.list.Count;
        }
        #endregion

        #region PeekNext
        public T PeekNext()
        {
            this.MoveNext();
            T next = this.Current;
            this.MoveBack();

            return next;
        }
        #endregion

        #region PeekBack
        public T PeekBack()
        {
            this.MoveBack();
            T previous = this.Current;
            this.MoveNext();

            return previous;
        }
        #endregion

        #region SetCurrent
        public void SetCurrent(T val)
        {
            if (this.Current.Equals(val))
            {
                return;
            }

            for (int i = 0; i < this.list.Count; i++)
            {
                if (this.list[i].Equals(val))
                {
                    this.currentPosition = i;
                    return;
                }
            }
        }
        #endregion

        #region Append
        public void Append(T obj)
        {
            this.list.Add(obj);
        }
        #endregion

        #region Insert
        public void InsertAt(T obj, int postionIndex)
        {
            if (this.list.Count == 0)
            {
                this.list.Add(obj);
                return;
            }

            int index = postionIndex % this.list.Count;
            this.list.Insert(index, obj);

            if (index <= this.currentPosition)
            {
                ++this.currentPosition;
            }
        }
        #endregion

        #region RemoveTail
        public void RemoveTail()
        {
            if (this.list.Count == 0)
            {
                return;
            }

            this.RemoveAt(this.list.Count - 1);
        }
        #endregion

        #region RemoveAt
        public void RemoveAt(int postionIndex)
        {
            if (this.list.Count == 0)
            {
                return;
            }

            int index = postionIndex % this.list.Count;
            this.list.RemoveAt(index);

            if (index < this.currentPosition)
            {
                --this.currentPosition;
            }
        }
        #endregion
    }
}
