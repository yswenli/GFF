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

using CCWin;
using CCWin.SkinControl;
using GFF.Component.Config;
using System;

namespace GFFClient
{
    public partial class InitForm : CCSkinMain
    {
        private readonly ClientConfig config = ClientConfig.Instance();

        public InitForm()
        {
            InitializeComponent();
        }

        private void InitForm_Load(object sender, EventArgs e)
        {
            skinWaterTextBox3.Text = config.IP;
        }

        private void skinButton1_Click(object sender, EventArgs e)
        {
            var name = skinWaterTextBox1.Text;
            var discription = skinWaterTextBox2.Text;
            if (!string.IsNullOrEmpty(discription) && (discription.Length > 20))
                discription = discription.Substring(0, 20);
            if (!string.IsNullOrEmpty(name))
            {
                name = name.Replace("|", "");

                if (name.Length > 10) name = name.Substring(0, 10);

                config.IP = skinWaterTextBox3.Text;
                ClientConfig.Save(config);

                new MainForm(new ChatListSubItem
                {
                    NicName = name,
                    DisplayName = name,
                    PersonalMsg = discription
                }).Show();
                Hide();
            }
        }
    }
}