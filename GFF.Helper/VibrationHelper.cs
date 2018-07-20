/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：e678aad3-469f-4f9b-a5da-0824f90d56fc
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.Helper
 * 类名称：VibrationHelper
 * 创建时间：2016/12/27 14:17:02
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace GFF.Helper
{
    /// <summary>
    /// 控件震动效果
    /// </summary>
    public class VibrationHelper
    {
        public static void Vibration(Control control)
        {
            Point pOld = control.Location;//原来的位置
            int radius = 3;//半径
            for (int n = 0; n < 3; n++) //旋转圈数
            {
                //右半圆逆时针
                for (int i = -radius; i <= radius; i++)
                {
                    int x = Convert.ToInt32(Math.Sqrt(radius * radius - i * i));
                    int y = -i;

                    control.Location = new Point(pOld.X + x, pOld.Y + y);
                    Thread.Sleep(10);
                }
                //左半圆逆时针
                for (int j = radius; j >= -radius; j--)
                {
                    int x = -Convert.ToInt32(Math.Sqrt(radius * radius - j * j));
                    int y = -j;

                    control.Location = new Point(pOld.X + x, pOld.Y + y);
                    Thread.Sleep(10);
                }
            }
            //抖动完成，恢复原来位置
            control.Location = pOld;
        }
    }
}
