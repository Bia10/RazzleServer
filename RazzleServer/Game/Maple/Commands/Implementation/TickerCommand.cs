﻿using RazzleServer.Common.Constants;
using RazzleServer.Game.Maple.Characters;

namespace RazzleServer.Game.Maple.Commands.Implementation
{
    public sealed class TickerCommand : Command
    {
        public override string Name => "ticker";

        public override string Parameters => "[ message ]";

        public override bool IsRestricted => true;

        public override void Execute(Character caller, string[] args)
        {
            var message = args.Length > 0 ? args[0] : string.Empty;
            caller.Client.Server.World.TickerMessage = message;
            //caller.Client.World.Notify(message, NoticeType.Ticker);
        }
    }
}
