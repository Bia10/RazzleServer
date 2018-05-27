﻿using System.Collections.Generic;
using RazzleServer.Common.Constants;
using RazzleServer.Common.Packet;

namespace RazzleServer.Game.Maple
{
    public sealed class Attack
    {
        public AttackType Type { get; }
        public byte Portals { get; }
        public int Targets { get; }
        public int Hits { get; }
        public int SkillId { get; }

        public byte Display { get; }
        public byte Animation { get; }
        public byte WeaponClass { get; }
        public byte WeaponSpeed { get; }
        public int Ticks { get; }

        public uint TotalDamage { get; }
        public Dictionary<int, List<uint>> Damages { get; }

        public Attack(PacketReader iPacket, AttackType type)
        {
            Type = type;
            Portals = iPacket.ReadByte();
            var tByte = iPacket.ReadByte();
            Targets = tByte / 0x10;
            Hits = tByte % 0x10;
            SkillId = iPacket.ReadInt();

            if (SkillId > 0)
            {

            }

            iPacket.Skip(4); // NOTE: Unknown, probably CRC.
            iPacket.Skip(4); // NOTE: Unknown, probably CRC.
            iPacket.Skip(1); // NOTE: Unknown.
            Display = iPacket.ReadByte();
            Animation = iPacket.ReadByte();
            WeaponClass = iPacket.ReadByte();
            WeaponSpeed = iPacket.ReadByte();
            Ticks = iPacket.ReadInt();

            if (Type == AttackType.Range)
            {
                var starSlot = iPacket.ReadShort();
                var cashStarSlot = iPacket.ReadShort();
                iPacket.ReadByte(); // NOTE: Unknown.
            }

            Damages = new Dictionary<int, List<uint>>();

            for (var i = 0; i < Targets; i++)
            {
                var objectId = iPacket.ReadInt();
                iPacket.ReadInt(); // NOTE: Unknown.
                iPacket.ReadInt(); // NOTE: Mob position.
                iPacket.ReadInt(); // NOTE: Damage position.
                iPacket.ReadShort(); // NOTE: Distance.

                for (var j = 0; j < Hits; j++)
                {
                    var damage = iPacket.ReadUInt();

                    if (!Damages.ContainsKey(objectId))
                    {
                        Damages.Add(objectId, new List<uint>());
                    }

                    Damages[objectId].Add(damage);

                    TotalDamage += damage;
                }

                if (Type != AttackType.Summon)
                {
                    iPacket.ReadInt(); // NOTE: Unknown, probably CRC.
                }
            }

            if (Type == AttackType.Range)
            {
                new Point(iPacket.ReadShort(), iPacket.ReadShort()); // NOTE: Projectile position.
            }

            new Point(iPacket.ReadShort(), iPacket.ReadShort()); // NOTE: Player position.
        }
    }
}
