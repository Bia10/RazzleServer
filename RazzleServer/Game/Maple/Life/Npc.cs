﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RazzleServer.Common.Constants;
using RazzleServer.Common.Packet;
using RazzleServer.Common.Util;
using RazzleServer.Common.Wz;
using RazzleServer.Game.Maple.Characters;
using RazzleServer.Game.Maple.Scripts;
using RazzleServer.Game.Maple.Shops;
using RazzleServer.Game.Maple.Util;

namespace RazzleServer.Game.Maple.Life
{
    public class Npc : LifeObject, ISpawnable, IControllable
    {
        [JsonIgnore]
        public Character Controller { get; set; }

        public Shop Shop { get; set; }
        public int StorageCost { get; set; }

        private readonly ILogger _log = LogManager.Log;

        public Npc() { }

        public Npc(WzImageProperty img)
            : base(img, LifeObjectType.Npc)
        {
        }

        public void Move(PacketReader iPacket)
        {
            var action1 = iPacket.ReadByte();
            var action2 = iPacket.ReadByte();

            var movements = iPacket.Available > 0
                                   ? new Movements(iPacket)
                                   : null;

            using (var oPacket = new PacketWriter(ServerOperationCode.NpcMove))
            {
                oPacket.WriteInt(ObjectId);
                oPacket.WriteByte(action1);
                oPacket.WriteByte(action2);

                if (movements != null)
                {
                    oPacket.WriteBytes(movements.ToByteArray());
                }

                Map.Send(oPacket);
            }
        }

        public void Converse(Character talker)
        {
            if (Shop != null)
            {
                talker.CurrentNpcShop = Shop;
                Shop.Show(talker);
            }
            else if (StorageCost > 0)
            {
                talker.Storage.Show(this);
            }
            else
            {
                ScriptProvider.Npcs.Execute(this, talker);
            }
        }

        public void Handle(Character talker, PacketReader iPacket)
        {
            if (talker.NpcScript == null)
            {
                return;
            }

            var lastMessageType = (NpcMessageType)iPacket.ReadByte();
            var action = iPacket.ReadByte();

            var selection = -1;

            byte endTalkByte;

            switch (lastMessageType)
            {
                case NpcMessageType.RequestText:
                case NpcMessageType.RequestNumber:
                case NpcMessageType.RequestStyle:
                case NpcMessageType.Choice:
                    endTalkByte = 0;
                    break;

                default:
                    endTalkByte = byte.MaxValue;
                    break;
            }

            if (action == endTalkByte)
            {
                talker.NpcScript = null;

            }
            else
            {

                if (iPacket.Available >= 4)
                {
                    selection = iPacket.ReadInt();
                }
                else if (iPacket.Available > 0)
                {
                    selection = iPacket.ReadByte();
                }

                if (lastMessageType == NpcMessageType.RequestStyle)
                {
                    //selection = this.StyleSelectionHelpers[talker][selection];
                }

                talker.NpcScript.SetResult(selection != -1 ? selection : action);
            }
        }

        public void AssignController()
        {
            if (Controller == null)
            {
                var leastControlled = int.MaxValue;
                Character newController = null;

                lock (Map.Characters)
                {
                    foreach (var character in Map.Characters.Values.Where(x => x.Client.Connected))
                    {
                        if (character.ControlledNpcs.Count < leastControlled)
                        {
                            leastControlled = character.ControlledNpcs.Count;
                            newController = character;
                        }
                    }
                }

                newController?.ControlledNpcs.Add(this);
            }
        }

        public PacketWriter GetCreatePacket() => GetSpawnPacket();

        public PacketWriter GetSpawnPacket() => GetInternalPacket(false);

        public PacketWriter GetControlRequestPacket() => GetInternalPacket(true);

        private PacketWriter GetInternalPacket(bool requestControl)
        {
            var oPacket = new PacketWriter(requestControl ? ServerOperationCode.NpcChangeController : ServerOperationCode.NpcEnterField);

            if (requestControl)
            {
                oPacket.WriteBool(true);
            }

            //  [C2 00] [00 00 00 00] [BE 65 8C 00] [3A 13] [BE 01] [01] [C9 00] [08 13] [6C 13] [01]

            oPacket.WriteInt(ObjectId);
            oPacket.WriteInt(MapleId);
            oPacket.WritePoint(Position);
            oPacket.WriteBool(!FacesLeft);
            oPacket.WriteShort(Foothold);
            oPacket.WriteShort(MinimumClickX);
            oPacket.WriteShort(MaximumClickX);
            oPacket.WriteBool(!Hide);

            return oPacket;
        }

        public PacketWriter GetControlCancelPacket()
        {
            var oPacket = new PacketWriter(ServerOperationCode.NpcChangeController);

            oPacket.WriteBool(false);
            oPacket.WriteInt(ObjectId);

            return oPacket;
        }

        public PacketWriter GetDialogPacket(string text, NpcMessageType messageType, params byte[] footer)
        {
            var oPacket = new PacketWriter(ServerOperationCode.ScriptMessage);


            oPacket.WriteByte(4); // NOTE: Unknown.
            oPacket.WriteInt(MapleId);
            oPacket.WriteByte((byte)messageType);
            oPacket.WriteByte(0); // NOTE: Speaker.
            oPacket.WriteString(text);
            oPacket.WriteBytes(footer);

            return oPacket;
        }

        public PacketWriter GetDestroyPacket()
        {
            var oPacket = new PacketWriter(ServerOperationCode.NpcLeaveField);

            oPacket.WriteInt(ObjectId);

            return oPacket;
        }
    }
}
