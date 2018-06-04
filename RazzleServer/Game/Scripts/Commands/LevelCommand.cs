﻿using RazzleServer.Game.Maple.Characters;
using RazzleServer.Game.Maple.Scripting;

namespace RazzleServer.Game.Scripts.Commands
{
    public sealed class LevelCommand : ACommandScript
    {
        public override string Name => "level";

        public override string Parameters => "level";

        public override bool IsRestricted => true;

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length != 1)
            {
                ShowSyntax(caller);
            }
            else
            {
                caller.Level = byte.Parse(args[0]);
            }
        }
    }
}