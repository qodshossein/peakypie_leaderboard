using _Project.Scripts.Events.Leaderboard;
using _Template.Core.EventSystem;
using System;
using System.IO;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace _Project.Scripts.Leaderboard
{
    public class LeaderboardLoader
    {
        private NativeArray<LeaderboardEntry> _entries;
        private IEventService _eventService;

        public NativeArray<LeaderboardEntry> Entries => _entries;
        public bool IsLoaded => _entries.IsCreated;

        public LeaderboardLoader(IEventService eventService)
        {
            _eventService = eventService;
        }
        public async void LoadAsync(bool ascending)
        {
            var path = Path.Combine(Application.streamingAssetsPath, "leaderboard.csv");
            var startDataEvent = new StartReceivingLeaderboardDataEvent();
            _eventService.Publish(startDataEvent);

            byte[] bytes = await File.ReadAllBytesAsync(path);

            NativeArray<LeaderboardEntry> array = default;
            await Task.Run(() => array = Parse(bytes));
            bytes = null;

            var job = new SortJob { Data = array, Ascending = ascending };
            JobHandle handle = job.Schedule();
            while (!handle.IsCompleted)
                await Task.Yield();

            handle.Complete();

            if (_entries.IsCreated) _entries.Dispose();
            _entries = array;

            var loadDataEvent = new LeaderboardLoadAllDataEvent() { LeaderboardEntries = _entries };
            _eventService.Publish(loadDataEvent);
        }

        private static NativeArray<LeaderboardEntry> Parse(byte[] bytes)
        {
            int lineCount = 0;
            for (int i = 0; i < bytes.Length; i++)
                if (bytes[i] == (byte)'\n') lineCount++;

            var result = new NativeArray<LeaderboardEntry>(
                Math.Max(lineCount, 1), Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

            var span = new ReadOnlySpan<byte>(bytes);
            int row = 0;
            int start = 0;
            bool headerSkipped = false;

            for (int i = 0; i < span.Length; i++)
            {
                if (span[i] != (byte)'\n') continue;

                int end = i;
                if (end > start && span[end - 1] == (byte)'\r') end--;

                if (!headerSkipped)
                {
                    headerSkipped = true;
                }
                else if (end > start)
                {
                    result[row++] = ParseLine(span.Slice(start, end - start));
                }
                start = i + 1;
            }

            if (row == result.Length) return result;

            var trimmed = new NativeArray<LeaderboardEntry>(row, Allocator.Persistent);
            NativeArray<LeaderboardEntry>.Copy(result, trimmed, row);
            result.Dispose();
            return trimmed;
        }

        private static LeaderboardEntry ParseLine(ReadOnlySpan<byte> line)
        {
            int c1 = line.IndexOf((byte)',');
            int c2 = line.Slice(c1 + 1).IndexOf((byte)',') + c1 + 1;

            var idSpan = line.Slice(0, c1);
            var nameSpan = line.Slice(c1 + 1, c2 - c1 - 1);
            var scoreSpan = line.Slice(c2 + 1);

            var entry = new LeaderboardEntry
            {
                Id = (int)ParseLong(idSpan),
                Score = ParseLong(scoreSpan),
                Username = default
            };

            for (int i = 0; i < nameSpan.Length; i++)
                entry.Username.Append((char)nameSpan[i]);

            return entry;
        }

        private static long ParseLong(ReadOnlySpan<byte> span)
        {
            long result = 0;
            for (int i = 0; i < span.Length; i++)
                result = result * 10 + (span[i] - (byte)'0');
            return result;
        }

        [BurstCompile]
        private struct SortJob : IJob
        {
            public NativeArray<LeaderboardEntry> Data;
            public bool Ascending;

            public void Execute()
            {
                if (Ascending) Data.Sort();
                else Data.Sort(new DescendingComparer());
            }
        }

        private struct DescendingComparer : System.Collections.Generic.IComparer<LeaderboardEntry>
        {
            public int Compare(LeaderboardEntry a, LeaderboardEntry b) => b.Score.CompareTo(a.Score);
        }
    }
}
