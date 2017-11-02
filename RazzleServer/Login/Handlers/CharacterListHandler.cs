﻿using RazzleServer.Common.Packet;

namespace RazzleServer.Login.Handlers
{
    [PacketHandler(ClientOperationCode.WorldSelect)]
    public class CharacterListHandler : LoginPacketHandler
    {
        public override void HandlePacket(PacketReader packet, LoginClient client)
        {
            packet.ReadByte(); // NOTE: Connection kind (GameLaunching, WebStart, etc.).
            client.World = packet.ReadByte();
            client.Channel = packet.ReadByte();
            var ip = packet.ReadBytes(4);

            var characters = client.Server.CenterConnection.GetCharacters(client.World, client.Account.ID);

            using (var oPacket = new PacketWriter(ServerOperationCode.SelectWorldResult))
            {
                oPacket.WriteBool(false);
                oPacket.WriteByte((byte)characters.Count);
                characters.ForEach(x => oPacket.WriteBytes(x));
                oPacket.WriteInt(client.Account.MaxCharacters);
                client.Send(oPacket);
            }
        }
    }
}