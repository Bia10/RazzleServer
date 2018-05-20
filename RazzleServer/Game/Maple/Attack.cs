﻿using System.Collections.Generic;
using RazzleServer.Common.Constants;
using RazzleServer.Common.Packet;

namespace RazzleServer.Game.Maple
{
    public sealed class Attack
    {
        public AttackType Type { get; private set; }
        public byte Portals { get; private set; }
        public int Targets { get; private set; }
        public int Hits { get; private set; }
        public int SkillId { get; private set; }

        public byte Display { get; private set; }
        public byte Animation { get; private set; }
        public byte WeaponClass { get; private set; }
        public byte WeaponSpeed { get; private set; }
        public int Ticks { get; private set; }

        public uint TotalDamage { get; private set; }
        public Dictionary<int, List<uint>> Damages { get; private set; }

        public Attack(PacketReader iPacket, AttackType type)
        {
            this.Type = type;
            this.Portals = iPacket.ReadByte();
            byte tByte = iPacket.ReadByte();
            this.Targets = tByte / 0x10;
            this.Hits = tByte % 0x10;
            this.SkillId = iPacket.ReadInt();

            if (this.SkillId > 0)
            {

            }

            iPacket.Skip(4); // NOTE: Unknown, probably CRC.
            iPacket.Skip(4); // NOTE: Unknown, probably CRC.
            iPacket.Skip(1); // NOTE: Unknown.
            this.Display = iPacket.ReadByte();
            this.Animation = iPacket.ReadByte();
            this.WeaponClass = iPacket.ReadByte();
            this.WeaponSpeed = iPacket.ReadByte();
            this.Ticks = iPacket.ReadInt();

            if (this.Type == AttackType.Range)
            {
                short starSlot = iPacket.ReadShort();
                short cashStarSlot = iPacket.ReadShort();
                iPacket.ReadByte(); // NOTE: Unknown.
            }

            this.Damages = new Dictionary<int, List<uint>>();

            for (int i = 0; i < this.Targets; i++)
            {
                int objectId = iPacket.ReadInt();
                iPacket.ReadInt(); // NOTE: Unknown.
                iPacket.ReadInt(); // NOTE: Mob position.
                iPacket.ReadInt(); // NOTE: Damage position.
                iPacket.ReadShort(); // NOTE: Distance.

                for (int j = 0; j < this.Hits; j++)
                {
                    uint damage = iPacket.ReadUInt();

                    if (!this.Damages.ContainsKey(objectId))
                    {
                        this.Damages.Add(objectId, new List<uint>());
                    }

                    this.Damages[objectId].Add(damage);

                    this.TotalDamage += damage;
                }

                if (this.Type != AttackType.Summon)
                {
                    iPacket.ReadInt(); // NOTE: Unknown, probably CRC.
                }
            }

            if (this.Type == AttackType.Range)
            {
                new Point(iPacket.ReadShort(), iPacket.ReadShort()); // NOTE: Projectile position.
            }

            new Point(iPacket.ReadShort(), iPacket.ReadShort()); // NOTE: Player position.
        }
    }
}
