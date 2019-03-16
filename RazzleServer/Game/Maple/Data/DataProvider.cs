﻿using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RazzleServer.Common.Util;
using RazzleServer.Game.Maple.Data.Cache;
using RazzleServer.Game.Maple.Data.Loaders;

namespace RazzleServer.Game.Maple.Data
{
    public class DataProvider
    {
        public static CachedAvailableStyles Styles { get; private set; } = new CachedAvailableStyles();
        public static CachedItems Items { get; private set; } = new CachedItems();
        public static CachedSkills Skills { get; private set; } = new CachedSkills();
        public static CachedMobs Mobs { get; private set; } = new CachedMobs();
        public static CachedMobSkills MobSkills { get; private set; } = new CachedMobSkills();
        public static CachedReactors Reactors { get; private set; } = new CachedReactors();
        public static CachedQuests Quests { get; private set; } = new CachedQuests();
        public static CachedNpcs Npcs { get; private set; } = new CachedNpcs();
        public static CachedCreationData CreationData { get; private set; } = new CachedCreationData();
        public static CachedMaps Maps { get; private set; } = new CachedMaps();
        public static CachedStrings Strings { get; private set; } = new CachedStrings();
        public static CachedShops Shops { get; private set; } = new CachedShops();
        public static CachedRechargeTiers RechargeTiers { get; private set; } = new CachedRechargeTiers();
        public static CachedQuizzes Quizzes { get; private set; } = new CachedQuizzes();

        private static readonly ILogger Log = LogManager.CreateLogger<DataProvider>();

        public static async Task Initialize()
        {
            var sw = Stopwatch.StartNew();

            await Task.WhenAll(
                Task.Run(async () => Styles = await new AvailableStylesDataLoader().Load()),
                Task.Run(async () => Items = await new ItemsLoader().Load()),
                Task.Run(async () => CreationData = await new CreationDataLoader().Load()),
                Task.Run(async () => Skills = await new SkillsLoader().Load()),
                Task.Run(async () => Mobs = await new MobsLoader().Load()),
                Task.Run(async () => Npcs = await new NpcsLoader().Load()),
                Task.Run(async () => MobSkills = await new MobSkillsLoader().Load()),
                Task.Run(async () => Reactors = await new ReactorsLoader().Load()),
                Task.Run(async () => Quests = await new QuestsLoader().Load()),
                Task.Run(async () => Maps = await new MapsLoader().Load()),
                Task.Run(async () => Strings = await new StringLoader().Load()),
                Task.Run(async () => Quizzes = await new QuizzesLoader().Load())
            );

            sw.Stop();

            Log.LogInformation("Data loaded in {0}ms.", sw.ElapsedMilliseconds);
        }
    }
}
