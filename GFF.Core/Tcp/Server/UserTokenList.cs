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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GFF.Core.Interface;
using GFF.Core.Tcp.Model;
using GFF.Helper;
using GFF.Model.Entity;

namespace GFF.Core.Tcp.Server
{
    /// <summary>
    ///     订阅用户列表
    /// </summary>
    public static class UserTokenList
    {
        /// <summary>
        ///     在线用户集合
        /// </summary>
        private static readonly ConcurrentDictionary<string, IUserToken> _OnlineUsers =
            new ConcurrentDictionary<string, IUserToken>();

        /// <summary>
        ///     频道映射集合
        /// </summary>
        private static ConcurrentDictionary<string, Mapping> _AsyncUserTokenList =
            new ConcurrentDictionary<string, Mapping>();

        /// <summary>
        ///     多线程类
        /// </summary>
        private static MutiThreadHelper mutiThreadHelper = new MutiThreadHelper();

        private static readonly string privateMsg = "privateMsg";

        /// <summary>
        ///     订阅用户列表
        /// </summary>
        public static ConcurrentDictionary<string, Mapping> AsyncUserTokenList
        {
            get
            {
                return _AsyncUserTokenList;
            }
            set
            {
                _AsyncUserTokenList = value;
            }
        }

        public static bool IsEmpty
        {
            get
            {
                return (_AsyncUserTokenList == null) || (_AsyncUserTokenList.Count == 0);
            }
        }

        static UserTokenList()
        {
            TaskHelper.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        if (_OnlineUsers != null && !_OnlineUsers.IsEmpty)
                        {
                            var list = _OnlineUsers.Values.ToList();
                            if (list != null)
                            {
                                foreach (var item in list)
                                {
                                    if (item.ActiveDateTime.AddMinutes(1) < DateTime.Now)
                                    {
                                        try
                                        {
                                            item.ConnectSocket.Close(1000);
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }
                                list.Clear();
                            }
                        }
                    }
                    catch { }

                    Thread.Sleep(1);
                }
            });
        }


        #region 发布订阅

        /// <summary>
        ///     添加
        /// </summary>
        /// <param name="asyncUserToken"></param>
        /// <returns></returns>
        public static void SetByChannelID(string channelID, IUserToken asyncUserToken)
        {
            if (!string.IsNullOrWhiteSpace(asyncUserToken.UID))
            {
                var mapping = new Mapping
                {
                    ChannelID = channelID,
                    UID = asyncUserToken.UID,
                    UserToken = asyncUserToken
                };
                _OnlineUsers.AddOrUpdate(mapping.UID, mapping.UserToken, (x, y) =>
                {
                    return mapping.UserToken;
                });
                _AsyncUserTokenList.AddOrUpdate(mapping.ChannelID + "|" + mapping.UID, mapping, (x, y) =>
                {
                    return mapping;
                });
            }
        }

        /// <summary>
        ///     删除
        /// </summary>
        /// <param name="channelID"></param>
        /// <returns></returns>
        public static bool DelByChannelID(string channelID, IUserToken asyncUserToken)
        {

            IUserToken u;
            _OnlineUsers.TryRemove(asyncUserToken.UID, out u);

            Mapping m;
            return _AsyncUserTokenList.TryRemove(channelID + "|" + asyncUserToken.UID, out m);
        }

        public static List<IUserToken> GetListByChannelID(string channelID)
        {
            if (_AsyncUserTokenList != null && _AsyncUserTokenList.Values != null)
            {
                var list = _AsyncUserTokenList.Values.ToList();
                if (list != null)
                {
                    return list.Where(b => (b.ChannelID == channelID) && (b.UserToken != null) && (b.UserToken.ConnectSocket != null) && b.UserToken.ConnectSocket.Connected)
                               .Select(b => b.UserToken)
                               .Distinct()
                               .ToList();
                }
            }
            return null;
        }

        #endregion

        #region 私信模式        

        public static void SetByUID(IUserToken asyncUserToken)
        {
            SetByChannelID(privateMsg, asyncUserToken);
        }

        public static bool DelByUID(IUserToken asyncUserToken)
        {
            return DelByChannelID(privateMsg, asyncUserToken);
        }

        public static List<IUserToken> GetList()
        {
            return GetListByChannelID(privateMsg);
        }

        public static IUserToken GetUserTokenByUID(string uid)
        {
            if (!_OnlineUsers.IsEmpty)
            {
                IUserToken u;
                if (_OnlineUsers.TryGetValue(uid, out u))
                    return u;
            }
            return null;
        }

        #endregion
    }
}