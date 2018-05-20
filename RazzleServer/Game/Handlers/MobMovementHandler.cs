﻿using RazzleServer.Common.Packet;

namespace RazzleServer.Game.Handlers
{
    [PacketHandler(ClientOperationCode.MobMovement)]
    public class MobMovementHandler : GamePacketHandler
    {
        public override void HandlePacket(PacketReader packet, GameClient client)
        {
            int objectId = packet.ReadInt();
            var mob = client.Character.ControlledMobs[objectId];
            mob?.Move(packet);
        }
    }
}