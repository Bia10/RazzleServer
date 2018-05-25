﻿using System.Collections.Generic;
using System.Linq;
using RazzleServer.Common.Data;
using RazzleServer.Common.Packet;
using RazzleServer.Data;
using RazzleServer.Game.Maple.Life;

namespace RazzleServer.Game.Maple.Characters
{
    public sealed class CharacterStorage
    {
        public Character Parent { get; private set; }
        public Npc Npc { get; private set; }
        public byte Slots { get; private set; }
        public int Meso { get; private set; }
        public List<Item> Items { get; set; }

        public bool IsFull => Items.Count == Slots;

        public CharacterStorage(Character parent) => Parent = parent;

        public void Load()
        {
            using (var dbContext = new MapleDbContext())
            {
                var entity = dbContext.CharacterStorages.FirstOrDefault(x => x.AccountId == Parent.AccountId);

                if (entity == null)
                {
                    entity = GenerateDefault();
                    dbContext.CharacterStorages.Add(entity);
                    dbContext.SaveChanges();
                }

                Slots = entity.Slots;
                Meso = entity.Meso;

                var itemEntities = dbContext.Items
                                            .Where(x => x.AccountId == Parent.AccountId)
                                            .Where(x => x.IsStored)
                                            .ToList();

                itemEntities.ForEach(x => Items.Add(new Item(x)));
            }
        }

        private CharacterStorageEntity GenerateDefault() => 
        new CharacterStorageEntity
        {
            AccountId = Parent.AccountId,
            Slots = 4,
            Meso = 0
        };

        public void Save()
        {
            //Datum datum = new Datum("storages");

            //datum["Slots"] = Slots;
            //datum["Meso"] = Meso;

            //datum.Update("AccountId = {0}", Parent.AccountId);

            Items.ForEach(item => item.Save());
        }

        public void Show(Npc npc)
        {
            Npc = npc;

            Load();

            using (var oPacket = new PacketWriter(ServerOperationCode.Storage))
            {
                oPacket.WriteByte(22);
                oPacket.WriteInt(npc.MapleId);
                oPacket.WriteByte(Slots);
                oPacket.WriteShort(126);
                oPacket.WriteShort(0);
                oPacket.WriteInt(0);
                oPacket.WriteInt(Meso);
                oPacket.WriteShort(0);
                oPacket.WriteByte((byte)Items.Count);

                Items.ForEach(item => item.ToByteArray(true, true));

                oPacket.WriteShort(0);
                oPacket.WriteByte(0);

                Parent.Client.Send(oPacket);
            }
        }
    }
}
