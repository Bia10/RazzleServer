﻿namespace RazzleServer.Game.Maple.Scripting.Scripts.Npcs
{
    [NpcScript("levelUP2")]
    public class LevelUp2 : ANpcScript
    {
        public override void Execute()
        {
            SendOk("Welcome to RazzleServer.");
            System.Console.WriteLine("SCRIPT - Levelup2!");
        }
    }
}