﻿using System;
using System.Collections.Generic;
using RazzleServer.Common.Constants;
using RazzleServer.Common.Packet;
using RazzleServer.Game.Maple.Characters;
using RazzleServer.Game.Maple.Data;
using RazzleServer.Game.Maple.Life;

namespace RazzleServer.Game.Maple.Maps
{
    public sealed class MapMobs : MapObjects<Mob>
    {
        public MapMobs(Map map) : base(map) { }

        protected override void InsertItem(int index, Mob item)
        {
            base.InsertItem(index, item);

            if (DataProvider.IsInitialized)
            {
                this.Map.Broadcast(item.GetCreatePacket());
                item.AssignController();
            }
        }

        protected override void RemoveItem(int index) // NOTE: Equivalent of mob death.
        {
            Mob item = base.Items[index];

            int mostDamage = 0;
            Character owner = null;

            foreach (KeyValuePair<Character, uint> attacker in item.Attackers)
            {
                if (attacker.Key.Map == this.Map)
                {
                    if (attacker.Value > mostDamage)
                    {
                        owner = attacker.Key;
                    }

                    attacker.Key.Experience += (int)Math.Min(item.Experience, (attacker.Value * item.Experience) / item.MaxHealth) * WvsGame.ExperienceRate;
                }
            }

            item.Attackers.Clear();

            if (item.CanDrop)
            {
                List<Drop> drops = new List<Drop>();

                foreach (Loot loopLoot in item.Loots)
                {
                    if ((Application.Random.Next(1000000) / WvsGame.DropRate) <= loopLoot.Chance)
                    {
                        if (loopLoot.IsMeso)
                        {
                            drops.Add(new Meso((short)(Application.Random.Next(loopLoot.MinimumQuantity, loopLoot.MaximumQuantity) * WvsGame.MesoRate))
                            {
                                Dropper = item,
                                Owner = owner
                            });
                        }
                        else
                        {
                            drops.Add(new Item(loopLoot.MapleID, (short)Application.Random.Next(loopLoot.MinimumQuantity, loopLoot.MaximumQuantity))
                            {
                                Dropper = item,
                                Owner = owner
                            });
                        }
                    }
                }

                foreach (Drop loopDrop in drops)
                {
                    // TODO: Space out drops.

                    this.Map.Drops.Add(loopDrop);
                }
            }

            if (owner != null)
            {
                foreach (KeyValuePair<ushort, Dictionary<int, short>> loopStarted in owner.Quests.Started)
                {
                    if (loopStarted.Value.ContainsKey(item.MapleID))
                    {
                        if (loopStarted.Value[item.MapleID] < DataProvider.Quests[loopStarted.Key].PostRequiredKills[item.MapleID])
                        {
                            loopStarted.Value[item.MapleID]++;

                            using (var oPacket = new PacketWriter(ServerOperationCode.Message))
                            {
                                oPacket
                                    .WriteByte((byte)MessageType.QuestRecord)
                                    .WriteUShort(loopStarted.Key)
                                    .WriteByte(1);

                                string kills = string.Empty;

                                foreach (int kill in loopStarted.Value.Values)
                                {
                                    kills += kill.ToString().PadLeft(3, '0');
                                }

                                oPacket
                                    .WriteString(kills)
                                    .WriteInt()
                                    .WriteInt();

                                owner.Client.Send(oPacket);

                                if (owner.Quests.CanComplete(loopStarted.Key, true))
                                {
                                    owner.Quests.NotifyComplete(loopStarted.Key);
                                }
                            }
                        }
                    }
                }
            }

            if (DataProvider.IsInitialized)
            {
                item.Controller.ControlledMobs.Remove(item);
                this.Map.Broadcast(item.GetDestroyPacket());
            }

            base.RemoveItem(index);

            if (item.SpawnPoint != null)
            {
                Delay.Execute(() => item.SpawnPoint.Spawn(), 3 * 1000); // TODO: Actual respawn time.
            }

            foreach (int summonID in item.DeathSummons)
            {
                this.Map.Mobs.Add(new Mob(summonID)
                {
                    Position = item.Position // TODO: Set owner as well.
                });
            }
        }
    }
}