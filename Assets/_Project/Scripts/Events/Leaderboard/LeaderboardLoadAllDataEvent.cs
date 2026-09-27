using _Project.Scripts.Leaderboard;
using Unity.Collections;
using UnityEngine;

namespace _Project.Scripts.Events.Leaderboard
{
    public class LeaderboardLoadAllDataEvent
    {
        public NativeArray<LeaderboardEntry> LeaderboardEntries;
    }
}
