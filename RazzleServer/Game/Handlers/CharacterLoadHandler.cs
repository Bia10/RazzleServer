﻿using RazzleServer.Center;
using RazzleServer.Common.Packet;
using RazzleServer.Game.Maple;
using RazzleServer.Game.Maple.Characters;

namespace RazzleServer.Game.Handlers
{
    [PacketHandler(ClientOperationCode.CharacterLoad)]
    public class CharacterLoadHandler : GamePacketHandler
    {
        public override void HandlePacket(PacketReader packet, GameClient client)
        {
            int characterID = packet.ReadInt();
            var accountID = client.Server.Manager.ValidateMigration(client.Host, characterID);

            if (accountID == 0)
            {
                client.Terminate("Invalid migraiton");
            }

            client.Account = new Account(accountID, client);
            client.Account.Load();

            client.Character = new Character(characterID, client);
            client.Character.Load();
            client.Character.Initialize();
        }
    }
}