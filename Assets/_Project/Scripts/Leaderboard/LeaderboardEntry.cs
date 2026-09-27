using System;
using Unity.Collections;

namespace _Project.Scripts.Leaderboard
{
    /// <summary>
    /// Blittable, zero-GC leaderboard record.
    /// Because it only contains unmanaged fields (int, long, FixedString64Bytes),
    /// it can live inside a NativeArray/NativeList and be sorted by a
    /// Burst-compiled job with zero managed heap allocations per element.
    /// </summary>
    public struct LeaderboardEntry : IComparable<LeaderboardEntry>
    {
        public int Id;
        public FixedString64Bytes Username; // ~64 bytes fixed buffer, no heap alloc
        public long Score;

        // Ascending order (low score -> high score), as requested.
        public int CompareTo(LeaderboardEntry other) => Score.CompareTo(other.Score);
    }
}
