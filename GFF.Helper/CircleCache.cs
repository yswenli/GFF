/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2017
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：9fc3a6a9-c71e-4ae4-80c9-f14ffbc3f932
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.Helper
 * 类名称：CircleCache
 * 创建时间：2017/2/20 15:37:02
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
    /// CircleCache 循环缓存。循环使用缓存中的每个对象。线程安全。
    /// </summary>  
    public class CircleCache<T>
    {
        private object locker = new object();
        private Circle<T> circle = new Circle<T>();

        #region Ctor
        public CircleCache()
        {
        }
        public CircleCache(ICollection<T> collection)
        {
            if (collection != null && collection.Count > 0)
            {
                foreach (T t in collection)
                {
                    this.circle.Append(t);
                }

            }
        }
        #endregion

        #region Add
        public void Add(T t)
        {
            lock (this.locker)
            {
                this.circle.Append(t);
            }
        }
        #endregion

        #region Get
        public T Get()
        {
            lock (this.locker)
            {
                this.circle.MoveNext();
                return this.circle.Current;
            }
        }
        #endregion
    }
}
