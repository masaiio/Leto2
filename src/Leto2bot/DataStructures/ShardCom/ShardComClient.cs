﻿using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Leto2bot.DataStructures.ShardCom
{
    public class ShardComClient
    {
        private int port;

        public ShardComClient(int port)
        {
            this.port = port;
        }

        public async Task Send(ShardComMessage data)
        {
            var msg = JsonConvert.SerializeObject(data);
            using (var client = new UdpClient())
            {
                var bytes = Encoding.UTF8.GetBytes(msg);
                await client.SendAsync(bytes, bytes.Length, IPAddress.Loopback.ToString(), port).ConfigureAwait(false);
            }
        }
    }
}
