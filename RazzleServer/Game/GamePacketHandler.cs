﻿using System;
using RazzleServer.Common.Packet;

namespace RazzleServer.Game
{
    public abstract class GamePacketHandler : APacketHandler<GameClient>
    {
        public GamePacketHandler()
        {
        }
    }
}