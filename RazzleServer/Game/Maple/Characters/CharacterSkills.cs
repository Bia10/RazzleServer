﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using RazzleServer.Common.Constants;
using RazzleServer.Common.Data;
using RazzleServer.Common.Packet;

namespace RazzleServer.Game.Maple.Characters
{
    public sealed class CharacterSkills : KeyedCollection<int, Skill>
    {
        public Character Parent { get; private set; }

        public CharacterSkills(Character parent)
             : base()
        {
            Parent = parent;
        }

        public void Load()
        {
            foreach (Datum datum in new Datums("skills").Populate("CharacterID = {0}", Parent.ID))
            {
                Add(new Skill(datum));
            }
        }

        public void Save()
        {
            foreach (Skill skill in this)
            {
                skill.Save();
            }
        }

        public void Delete()
        {
            foreach (Skill skill in this)
            {
                skill.Delete();
            }
        }

        public void Cast(PacketReader iPacket)
        {
            iPacket.ReadInt(); // NOTE: Ticks.
            int mapleID = iPacket.ReadInt();
            byte level = iPacket.ReadByte();

            Skill skill = this[mapleID];

            if (level != skill.CurrentLevel)
            {
                return;
            }

            skill.Recalculate();
            skill.Cast();

            switch (skill.MapleID)
            {
                case (int)SkillNames.SuperGM.Resurrection:
                    {
                        byte targets = iPacket.ReadByte();

                        while (targets-- > 0)
                        {
                            int targetID = iPacket.ReadInt();

                            Character target = Parent.Map.Characters[targetID];

                            if (!target.IsAlive)
                            {
                                target.Health = target.MaxHealth;
                            }
                        }
                    }
                    break;
            }
        }

        public byte[] ToByteArray()
        {
            using (var oPacket = new PacketWriter())
            {
                oPacket.WriteShort((short)Count);

                List<Skill> cooldownSkills = new List<Skill>();

                foreach (Skill loopSkill in this)
                {
                    oPacket.WriteBytes(loopSkill.ToByteArray());

                    if (loopSkill.IsCoolingDown)
                    {
                        cooldownSkills.Add(loopSkill);
                    }
                }

                oPacket.WriteShort((short)cooldownSkills.Count);

                foreach (Skill loopCooldown in cooldownSkills)
                {

                    oPacket.WriteInt(loopCooldown.MapleID);
                    oPacket.WriteShort((short)loopCooldown.RemainingCooldownSeconds);
                }

                return oPacket.ToArray();
            }
        }

        protected override void InsertItem(int index, Skill item)
        {
            item.Parent = this;

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            Skill item = base.Items[index];

            item.Parent = null;

            base.RemoveItem(index);
        }

        protected override int GetKeyForItem(Skill item)
        {
            return item.MapleID;
        }
    }
}

