﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using RazzleServer.Common.Constants;
using RazzleServer.Common.Util;
using RazzleServer.Game.Maple.Data.Cache;
using RazzleServer.Game.Maple.Data.References;

namespace RazzleServer.Game.Maple.Data.Loaders
{
    public sealed class SkillsLoader : ACachedDataLoader<CachedSkills>
    {
        public override string CacheName => "Skills";

        public override ILogger Log => LogManager.CreateLogger<SkillsLoader>();

        public override void LoadFromWz()
        {
            Log.LogInformation("Loading Skills");

            using (var file = GetWzFile("Skill.wz"))
            {
                file.ParseWzFile();

                foreach (var job in Enum.GetValues(typeof(Job)))
                {
                    var imgName = $"{((short)job).ToString().PadLeft(3, '0')}.img";
                    var img = file.WzDirectory.GetImageByName(imgName);

                    foreach (var skillImg in img["skill"].WzProperties)
                    {
                        if (!int.TryParse(skillImg.Name, out var id))
                        {
                            continue;
                        }

                        Data.Data[id] = new Dictionary<byte, SkillReference>();

                        foreach (var levelImg in skillImg["level"].WzProperties)
                        {
                            if (!byte.TryParse(levelImg.Name, out var level))
                            {
                                continue;
                            }

                            Data.Data[id][level] = new SkillReference(levelImg);
                        }
                    }
                }
            }
        }
    }
}
