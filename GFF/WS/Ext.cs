/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2017
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：868da569-d081-457e-8987-2833a5861e71
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.WS
 * 类名称：Extentions
 * 创建时间：2017/6/26 11:19:07
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Web;



namespace GFF.WS
{
    public static class Ext
    {
        #region Private Fields

        private static readonly byte[] _last = new byte[] { 0x00 };

        private static readonly int _retry = 5;

        private const string _tspecials = "()<>@,;:\\\"/[]?={} \t";

        #endregion

        #region Private Methods
        private static void times(this ulong n, Action action)
        {
            for (ulong i = 0; i < n; i++)
                action();
        }

        #endregion

        #region Internal Methods

        internal static byte[] EmptyBytes = new byte[0];

        internal static readonly RandomNumberGenerator RandomNumber = new RNGCryptoServiceProvider();
        
     
        internal static byte[] InternalToByteArray(this ushort value, ByteOrder order)
        {
            var bytes = BitConverter.GetBytes(value);
            if (!order.IsHostOrder())
                Array.Reverse(bytes);

            return bytes;
        }

        internal static byte[] InternalToByteArray(this ulong value, ByteOrder order)
        {
            var bytes = BitConverter.GetBytes(value);
            if (!order.IsHostOrder())
                Array.Reverse(bytes);

            return bytes;
        }    
        

        #endregion

        #region Public Methods  

        /// <summary>
        /// Determines whether the specified <see cref="ByteOrder"/> is host (this computer
        /// architecture) byte order.
        /// </summary>
        /// <returns>
        /// <c>true</c> if <paramref name="order"/> is host byte order; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="order">
        /// One of the <see cref="ByteOrder"/> enum values, to test.
        /// </param>
        public static bool IsHostOrder(this ByteOrder order)
        {
            // true: !(true ^ true) or !(false ^ false)
            // false: !(true ^ false) or !(false ^ true)
            return !(BitConverter.IsLittleEndian ^ (order == ByteOrder.Little));
        }
        #endregion
    }
}