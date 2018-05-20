﻿using System;
using System.Collections.Generic;

namespace RazzleServer.Common.Util
{
    public static class ListExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            var r = new Random();
            var len = list.Count;
            for (var i = len - 1; i >= 1; --i)
            {
                var j = r.Next(i);
                var tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }
        }
    }
}