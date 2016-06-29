﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using YatMing.Message.Contracts;

namespace YatMing.Message.Service
{
    public partial class FormPublish : Form
    {
        private ServiceHost _host = null;
        private int _current = 0;

        public FormPublish()
        {
            InitializeComponent();
        }

        private void FormPublish_Load(object sender, EventArgs e)
        {
            //开启服务
            _host = new ServiceHost(typeof(PushService));
            _host.Open();
            txtConsole.AppendText("服务器已经启动..." + "\r\n");
            //监控客户端
            ChannelManager.Instance.OnOparateChannel += Instance_OnOparateChannel;

            _current = ChannelManager.Instance.callbackChannelList.Count;
            lklblNum.Text = _current.ToString();
        }

        private void Instance_OnOparateChannel(object sender, ChannelEventArg e)
        {
            IPushCallback callback = sender as IPushCallback;
            switch (e.Type)
            {
                case OparateType.Add:                   
                    lstChannel.Items.Add(e.Id);
                    txtConsole.AppendText("订阅客户端:" + e.Id + "\r\n");
                    _current++;
                    lklblNum.Text = _current.ToString();
                    break;
                case OparateType.Remove:
                    lstChannel.Items.Remove(e.Id);
                    txtConsole.AppendText("退订客户端:" + e.Id + "\r\n");
                    _current--;
                    lklblNum.Text = _current.ToString();
                    break;
                case OparateType.Notify:
                    txtConsole.AppendText("发布给客户端:" + e.Id + "消息\r\n");
                    break;
                default:
                    break;
            }
        }

        private void btnSendText_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtText.Text) == false)
                ChannelManager.Instance.NotifyMessage(txtText.Text);
        }

        private void btnSendMedia_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(rtbText.Rtf) == false)
            {
                ChannelManager.Instance.NotifyMutiMedia(new MediaDTO { MediaContent = Encoding.Default.GetBytes(rtbText.Rtf) });
            }
        }

    }
}
