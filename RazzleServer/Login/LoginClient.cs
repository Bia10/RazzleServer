﻿using System;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using RazzleServer.Center;
using RazzleServer.Common.Constants;
using RazzleServer.Common.Exceptions;
using RazzleServer.Common.Network;
using RazzleServer.Common.Packet;
using RazzleServer.Common.Util;
using RazzleServer.Login.Maple;

namespace RazzleServer.Login
{
    public sealed class LoginClient : AClient
    {
        public byte World { get; internal set; }
        public byte Channel { get; internal set; }
        public Account Account { get; internal set; }
        public string LastUsername { get; internal set; }
        public string LastPassword { get; internal set; }
        public string[] MacAddresses { get; internal set; }
        public LoginServer Server { get; internal set; }

        public LoginClient(Socket socket, LoginServer server)
            : base(socket)
        {
            Server = server;
        }

        public override void Receive(PacketReader packet)
        {
            var header = ClientOperationCode.Unknown;
            try
            {
                if (packet.Available >= 2)
                {
                    header = (ClientOperationCode)packet.ReadUShort();

                    if (LoginServer.PacketHandlers.ContainsKey(header))
                    {
                        Log.LogInformation($"Received [{header.ToString()}] {packet.ToPacketString()}");

                        foreach (var handler in LoginServer.PacketHandlers[header])
                        {
                            handler.HandlePacket(packet, this);
                        }
                    }
                    else
                    {
                        Log.LogWarning($"Unhandled Packet [{header.ToString()}] {packet.ToPacketString()}");
                    }

                }
            }
            catch (Exception e)
            {
                Log.LogError(e, $"Packet Processing Error [{header.ToString()}] - {e.Message} - {e.StackTrace}");
            }
        }
    }
}
