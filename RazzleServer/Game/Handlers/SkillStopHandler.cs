using RazzleServer.Common.Constants;
using RazzleServer.Net.Packet;

namespace RazzleServer.Game.Handlers
{
    [PacketHandler(ClientOperationCode.SkillStop)]
    public class SkillStopHandler : GamePacketHandler
    {
        public override void HandlePacket(PacketReader packet, GameClient client)
        {
            var skillId = packet.ReadInt();
            client.Character.Buffs.Remove(skillId);

            switch (skillId)
            {
                case (int)SkillNames.Gm.Hide:
                {
                    //DataProvider.Maps[chr.Map].ShowPlayer(chr);
                    //AdminPacket.Hide(chr, false);
                    break;
                }
            }
        }
    }
}
