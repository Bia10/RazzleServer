using Microsoft.Extensions.Logging;
using RazzleServer.Common.Util;
using RazzleServer.Game.Maple.Data.Cache;
using RazzleServer.Game.Maple.Data.References;

namespace RazzleServer.Game.Maple.Data.Loaders
{
    public sealed class QuestsLoader : ACachedDataLoader<CachedQuests>
    {
        public override string CacheName => "Quests";

        private readonly ILogger _log = LogManager.Log;

        public override void LoadFromWz()
        {
            _log.LogInformation("Loading Quests");

            using (var file = GetWzFile("Quest.wz"))
            {
                file.ParseWzFile();
                var img = file.WzDirectory.GetImageByName("QuestInfo.img");

                foreach (var item in img.WzProperties)
                {
                    if(ushort.TryParse(item.Name, out var itemId))
                    {
                        Data.Data.Add(itemId, new QuestReference
                        {
                            MapleId = itemId,
                            Name = item["name"].GetString()
                        });
                    }
                }
            }

            //    foreach (Datum datum in new Datums("quest_data").Populate())
            //    {
            //        this.Add(new Quest(datum));
            //    }

            //    foreach (Datum datum in new Datums("quest_requests").Populate())
            //    {
            //        if (!this.Contains((ushort)(int)datum["questid"]))
            //        {
            //            continue;
            //        }

            //        string state = (string)datum["quest_state"];

            //        switch ((string)datum["request_type"])
            //        {
            //            case "mob":
            //                this[(ushort)(int)datum["questid"]].PostRequiredKills.Add((int)datum["objectid"], (short)datum["quantity"]);
            //                break;

            //            case "item":
            //                {
            //                    switch (state)
            //                    {
            //                        case "start":
            //                            this[(ushort)(int)datum["questid"]].PreRequiredItems.Add((int)datum["objectid"], (short)datum["quantity"]);
            //                            break;

            //                        case "end":
            //                            this[(ushort)(int)datum["questid"]].PostRequiredItems.Add((int)datum["objectid"], (short)datum["quantity"]);
            //                            break;
            //                    }
            //                }
            //                break;

            //            case "quest":
            //                {
            //                    switch (state)
            //                    {
            //                        case "start":
            //                            this[(ushort)(int)datum["questid"]].PreRequiredQuests.Add((ushort)(int)datum["objectid"]);
            //                            break;

            //                        case "end":
            //                            this[(ushort)(int)datum["questid"]].PostRequiredQuests.Add((ushort)(int)datum["objectid"]);
            //                            break;
            //                    }
            //                }
            //                break;
            //        }
            //    }


            //    foreach (Datum datum in new Datums("quest_rewards").Populate())
            //    {
            //        int state = ((string)datum["quest_state"]) == "start" ? 0 : 1;

            //        switch ((string)datum["reward_type"])
            //        {
            //            case "exp":
            //                this[(ushort)(int)datum["questid"]].ExperienceReward[state] = (int)datum["rewardid"];
            //                break;

            //            case "mesos":
            //                this[(ushort)(int)datum["questid"]].MesoReward[state] = (int)datum["rewardid"];
            //                break;

            //            case "fame":
            //                this[(ushort)(int)datum["questid"]].FameReward[state] = (int)datum["rewardid"];
            //                break;

            //            case "item":
            //                {
            //                    if (state == 0)
            //                    {
            //                        if (this[(ushort)(int)datum["questid"]].PreItemRewards.ContainsKey((int)datum["rewardid"]))
            //                        {
            //                            // NOTE: Neckson'd. Quest 8801 has rewards of the same Id.

            //                            continue;
            //                        }

            //                        this[(ushort)(int)datum["questid"]].PreItemRewards.Add((int)datum["rewardid"], (short)datum["quantity"]);
            //                    }
            //                    else
            //                    {
            //                        if (this[(ushort)(int)datum["questid"]].PostItemRewards.ContainsKey((int)datum["rewardid"]))
            //                        {
            //                            // NOTE: Neckson'd. Quest 8801 has rewards of the same Id.

            //                            continue;
            //                        }

            //                        this[(ushort)(int)datum["questid"]].PostItemRewards.Add((int)datum["rewardid"], (short)datum["quantity"]);
            //                    }
            //                }
            //                break;

            //            case "skill":
            //                {
            //                    if (state == 0)
            //                    {
            //                        this[(ushort)(int)datum["questid"]].PreSkillRewards.Add(new Skill((int)datum["rewardid"], (byte)(short)datum["quantity"], (byte)(short)datum["master_level"]), (Job)datum["job"]);
            //                    }
            //                    else
            //                    {
            //                        this[(ushort)(int)datum["questid"]].PostSkillRewards.Add(new Skill((int)datum["rewardid"], (byte)(short)datum["quantity"], (byte)(short)datum["master_level"]), (Job)datum["job"]);
            //                    }
            //                }
            //                break;

            //            case "pet_speed":
            //                this[(ushort)(int)datum["questid"]].PetSpeedReward[state] = true;
            //                break;

            //            case "pet_closeness":
            //                this[(ushort)(int)datum["questid"]].PetClosenessReward[state] = (int)datum["rewardid"];
            //                break;

            //            case "pet_skill":
            //                this[(ushort)(int)datum["questid"]].PetSkillReward[state] = (int)datum["rewardid"];
            //                break;
            //        }
            //    }

            //    foreach (Datum datum in new Datums("quest_required_jobs").Populate())
            //    {
            //        this[(ushort)datum["questid"]].ValidJobs.Add((Job)datum["valid_jobid"]);
            //    }
            //}
        }
    }
}