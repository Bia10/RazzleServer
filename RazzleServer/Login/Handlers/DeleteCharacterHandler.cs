﻿using RazzleServer.Common.Packet;
using RazzleServer.Common.Constants;
using RazzleServer.Game.Maple.Characters;

namespace RazzleServer.Login.Handlers
{
    [PacketHandler(ClientOperationCode.CharacterDelete)]
    public class DeleteCharacterHandler : LoginPacketHandler
    {
        public override void HandlePacket(PacketReader packet, LoginClient client)
        {
            var characterId = packet.ReadInt();
            var result = CharacterDeletionResult.Valid;

            Character.Delete(characterId);

            using (var oPacket = new PacketWriter(ServerOperationCode.DeleteCharacterResult))
            {
                oPacket.WriteInt(characterId);
                oPacket.WriteByte((byte)result);
                client.Send(oPacket);
            }
        }
    }
}
