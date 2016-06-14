using System;
using System.Collections.Generic;
using System.Linq;

namespace OverwatchSynergy.Api
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach (var item in sequence)
            {
                action(item);
            }
        }

        public static IEnumerable<Weight> SuppressDuplicates(this IEnumerable<Weight> weights, IEnumerable<Hero> currentTeam)
        {
            //currentTeam.ForEach(h => weights.Single(w => w.Hero.Id == h.Id).Value = 0);

            return weights.Select(w =>
                           {
                               if (currentTeam.Select(h => h.Id).Contains(w.Hero.Id))
                               {
                                   return new Weight()
                                          {
                                              Value = 0,
                                              Hero = w.Hero
                                          };
                               }

                               return w;
                           });
        }
    }
}