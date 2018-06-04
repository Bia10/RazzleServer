﻿using RazzleServer.Game.Maple.Life;

namespace RazzleServer.Game.Maple.Maps
{
    public class MapSummons : MapObjects<Summon>
    {
        public MapSummons(Map map) : base(map) { }

        public override void Add(Summon item)
        {
            lock (this)
            {
                base.Add(item);
                Map.Send(item.GetCreatePacket());
            }
        }

        public override void Remove(Summon item)
        {
            lock (this)
            {
                Map.Send(item.GetDestroyPacket());
                base.Remove(item);
            }
        }
    }
}
