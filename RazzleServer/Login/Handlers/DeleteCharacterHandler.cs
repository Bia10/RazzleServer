﻿using RazzleServer.Common.Constants;
using RazzleServer.Common.Packet;
using RazzleServer.Game.Maple.Characters;

namespace RazzleServer.Login.Handlers
{
    [PacketHandler(ClientOperationCode.DeleteCharacter)]
    public class DeleteCharacterHandler : LoginPacketHandler
    {
        public override void HandlePacket(PacketReader packet, LoginClient client)
        {
            var characterId = packet.ReadInt();
            Character.Delete(characterId);

            using (var pw = new PacketWriter(ServerOperationCode.DeleteCharacterResult))
            {
                pw.WriteInt(characterId);
                pw.WriteByte((byte)CharacterDeletionResult.Valid);
                client.Send(pw);
            }
        }
    }
}
