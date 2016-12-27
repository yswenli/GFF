/*****************************************************************************************************
 * 本代码版权归Wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * 所属域：WENLI-PC
 * 登录用户：Administrator
 * CLR版本：4.0.30319.17929
 * 唯一标识：20da4241-0bdc-4a06-8793-6d0889c31f95
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 命名空间：MCITest


 * 创建年份：2015
 * 创建时间：2015-12-02 11:15:24
 * 创建人：Wenli
 * 创建说明：
 *****************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GFFClient
{
    /// <summary>
    ///     好友辅助类
    /// </summary>
    public static class FriendHelper
    {
        public delegate void OnListChangedHander(List<Friend> list);

        private static readonly List<Friend> _Friends = new List<Friend>();

        static FriendHelper()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    while (true)
                    {
                        try
                        {
                            var rlist = new List<Friend>();
                            foreach (var item in _Friends)
                                if (item.JoinTime.AddMinutes(1) < DateTime.Now)
                                    rlist.Add(item);
                            if (rlist.Count > 0)
                            {
                                rlist.ForEach(item => { _Friends.Remove(item); });
                                RaiseOnListChanged(_Friends);
                            }
                        }
                        catch
                        {
                            break;
                        }
                        Thread.Sleep(500);
                    }
                }
                catch
                {
                }
            });
        }

        public static event OnListChangedHander OnListChanged;

        private static void RaiseOnListChanged(List<Friend> list)
        {
            OnListChanged?.Invoke(list);
        }


        public static void Set(string userName)
        {
            var f = _Friends.Where(b => b.UserName == userName).FirstOrDefault();
            if (f != null)
            {
                _Friends.Remove(f);
                _Friends.Add(new Friend {UserName = userName, JoinTime = DateTime.Now});
            }
            else
            {
                _Friends.Add(new Friend {UserName = userName, JoinTime = DateTime.Now});
                RaiseOnListChanged(_Friends);
            }
        }
    }

    public class Friend
    {
        public string UserName { get; set; }

        public DateTime JoinTime { get; set; }
    }
}