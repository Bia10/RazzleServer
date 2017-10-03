using MapleLib.PacketLib;
using RazzleServer.Player;

namespace RazzleServer.Packet
{
    public abstract class APacketHandler
    {
        public abstract void HandlePacket(PacketReader packet, MapleClient client);
    }
}