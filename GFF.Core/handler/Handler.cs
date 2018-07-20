/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2017
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：8a7ce760-7726-484d-9185-507a6ccf2606
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.Core.handler
 * 类名称：Handler
 * 创建时间：2017/6/20 14:43:29
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/

using System;
using GFF.Core.Interface;
using GFF.Core.Tcp.Model;
using GFF.Model.Entity;

namespace GFF.Core.Handler
{
    #region client

    public delegate void OnConnectedHandler(object sender);

    public delegate void OnCleintMessageReceivedHandler(object sender, Message msg);

    public delegate void OnCleintDataReceivedHandler(object sender, byte[] data);

    public delegate void OnClientErrorHandler(Exception ex, string msg);


    #endregion


    #region server

    public delegate void OnAcceptedHandler(int num, IUserToken userToken);

    public delegate void OnUnAcceptedHandler(int num, IUserToken userToken);

    public delegate void OnSocketReceivedMessageHandler(IUserToken userToken, byte[] args);

    public delegate void OnServerReceivedMessageHandler(IUserToken userToken, Message msg);

    public delegate void OnServerErrorHandler(Exception ex, params object[] args);

    public delegate void OnWSConnectedHandler(int num, IUserToken userToken);


    #endregion




}
