﻿using RazzleServer.Common.Constants;
using RazzleServer.Net.Packet;

namespace RazzleServer.Game.Handlers
{
    [PacketHandler(ClientOperationCode.UseSkill)]
    public class UseSkillHandler : GamePacketHandler
    {
        public override void HandlePacket(PacketReader packet, GameClient client)
        {
            var skillId = packet.ReadInt();
            var skillLevel = packet.ReadByte();
            var skill = client.Character.Skills[skillId];

            if (skill.CurrentLevel != skillLevel)
            {
                client.Character.LogCheatWarning(CheatType.InvalidSkillChange);
                return;
            }

            if (!client.Character.IsAlive)
            {
                client.Character.LogCheatWarning(CheatType.InvalidSkillChange);
                return;
            }

            //MapPacket.SendPlayerSkillAnim(chr, SkillID, SkillLevel);

            client.Character.Buffs.Add(skill, 0);

            switch (skillId)
            {
                case (int)SkillNames.Spearman.HyperBody:
                {
                    var healRate = (ushort)skill.Hp;
                    if (healRate > 100)
                    {
                        healRate = 100;
                    }

                    var healAmount =
                        (short)(healRate * client.Character.PrimaryStats.MaxHealth / 100); // Party: / (amount players)
                    if (!client.Character.Buffs.Contains(skillId))
                    {
                        client.Character.PrimaryStats.MaxHealth += healAmount;
                    }

                    break;
                }

                case (int)SkillNames.Cleric.Heal:
                {
                    var healRate = (ushort)skill.Hp;
                    if (healRate > 100)
                    {
                        healRate = 100;
                    }

                    var healAmount =
                        (short)(healRate * client.Character.PrimaryStats.MaxHealth / 100); // Party: / (amount players)

                    client.Character.PrimaryStats.Health += healAmount;
                    break;
                }

                case (int)SkillNames.Gm.Hide:
                {
                    client.Character.Hide(true);
                    break;
                }

                case (int)SkillNames.Priest.MysticDoor:
                {
                    //var door = new Door(chr, client.Character.Map, DataProvider.Maps[client.Character.Map].ReturnMap, client.Character.Position.X, client.Character.Position.Y);
                    //client.Character.Door = door;
                    //client.Character.Map.Doors.Add(door);
                    //MapPacket.SpawnDoor(chr, true, client.Character.Position.X, client.Character.Position.Y);
                    //MapPacket.SpawnPortal(chr, client.Character.Position);
                    client.Character.Release();
                    break;
                }

                case (int)SkillNames.Gm.Haste:
                case (int)SkillNames.Gm.HolySymbol:
                case (int)SkillNames.Gm.Bless:
                {
                    var players = packet.ReadByte();
                    for (byte i = 0; i < players; i++)
                    {
                        var playerid = packet.ReadInt();
                        var victim = client.Server.GetCharacterById(playerid);
                        if (victim != null && victim.Id != client.Character.Id)
                        {
                            //MapPacket.SendPlayerSkillAnimThirdParty(victim, SkillID, SkillLevel, true, true);
                            //MapPacket.SendPlayerSkillAnimThirdParty(victim, SkillID, SkillLevel, true, false);
                            //victim.Buffs.AddBuff(SkillID, SkillLevel);
                        }
                    }

                    break;
                }

                case (int)SkillNames.Gm.HealPlusDispell:
                {
                    var players = packet.ReadByte();
                    for (byte i = 0; i < players; i++)
                    {
                        var playerid = packet.ReadInt();
                        var victim = client.Server.GetCharacterById(playerid);
                        if (victim != null)
                        {
                            //MapPacket.SendPlayerSkillAnimThirdParty(victim, SkillID, SkillLevel, true, true);
                            //MapPacket.SendPlayerSkillAnimThirdParty(victim, SkillID, SkillLevel, true, false);
                            victim.PrimaryStats.Health = victim.PrimaryStats.MaxHealth;
                            victim.PrimaryStats.Mana = victim.PrimaryStats.MaxMana;
                        }
                    }

                    client.Character.PrimaryStats.Health = client.Character.PrimaryStats.MaxHealth;
                    client.Character.PrimaryStats.Mana = client.Character.PrimaryStats.MaxMana;
                    break;
                }

                case (int)SkillNames.Gm.Resurrection:
                {
                    var players = packet.ReadByte();
                    for (byte i = 0; i < players; i++)
                    {
                        var playerid = packet.ReadInt();
                        var victim = client.Server.GetCharacterById(playerid);
                        if (victim != null && !victim.IsAlive)
                        {
                            //MapPacket.SendPlayerSkillAnimThirdParty(victim, SkillID, SkillLevel, true, true);
                            //MapPacket.SendPlayerSkillAnimThirdParty(victim, SkillID, SkillLevel, true, false);
                            victim.PrimaryStats.Health = victim.PrimaryStats.MaxHealth;
                        }
                    }

                    break;
                }
            }

            client.Character.Release();
            skill.Cast();
//            if (Constants.isSummon(SkillID))
//            {
//                client.Character.Summons.NewSummon(SkillID, SkillLevel);
//            }  
        }
    }
}
