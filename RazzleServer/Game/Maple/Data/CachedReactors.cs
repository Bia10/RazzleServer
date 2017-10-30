﻿using System.Collections.ObjectModel;
using RazzleServer.Common.Data;
using RazzleServer.Game.Maple.Life;

namespace RazzleServer.Game.Maple.Data
{
    public sealed class CachedReactors : KeyedCollection<int, Reactor>
    {
        public CachedReactors()
            : base()
        {
            using (Log.Load("Reactors"))
            {
                foreach (Datum datum in new Datums("reactor_data").Populate())
                {
                    this.Add(new Reactor(datum));
                }

                foreach (Datum datum in new Datums("reactor_events").Populate())
                {
                    this[(int)datum["reactorid"]].States[(sbyte)datum["state"]] = new ReactorState(datum);
                }
            }
        }

        protected override int GetKeyForItem(Reactor item)
        {
            return item.MapleID;
        }
    }
}