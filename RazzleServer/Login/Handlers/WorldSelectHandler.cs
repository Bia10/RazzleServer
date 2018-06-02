﻿using RazzleServer.Common;
using RazzleServer.Common.Packet;

namespace RazzleServer.Login.Handlers
{
    [PacketHandler(ClientOperationCode.WorldSelect)]
    public class WorldSelectHandler : LoginPacketHandler
    {
        public override void HandlePacket(PacketReader packet, LoginClient client)
        {
            client.World = packet.ReadByte();
            client.Channel = packet.ReadByte();

            var channelExists = client.Server.Manager.Worlds[client.World].Contains(client.Channel);
            var characters = client.Server.GetCharacters(client.World, client.Account.Id);

            using (var pw = new PacketWriter(ServerOperationCode.SelectWorldResult))
            {
                if (!channelExists)
                {
                    pw.WriteByte(0);
                    pw.WriteByte((byte)characters.Count);
                    characters.ForEach(x => pw.WriteBytes(x.ToByteArray()));
                    pw.WriteInt(client.Account.MaxCharacters);
                }
                else
                {
                    pw.WriteByte(8); // Channel offline
                }

                client.Send(pw);
            }
        }
    }
}