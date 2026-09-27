using _Project.Scripts.Events.Leaderboard;
using _Template.Core.EventSystem;
using System.Collections;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Leaderboard
{
    public class LeaderboardSearchController : MonoBehaviour
    {
        [Inject(Id = "SearchID")] private TMP_InputField _searchInput;
        private IEventService _eventService;
        private LeaderboardScrollView _scrollView;

        [SerializeField] private float debounceSeconds = 0.15f;

        private NativeArray<LeaderboardEntry> _entries;
        private int _requestToken;
        private Coroutine _debounceRoutine;

        [Inject]
        private void Construct(LeaderboardScrollView scrollView, IEventService eventService)
        {
            _eventService = eventService;
            _scrollView = scrollView;
            _searchInput.onValueChanged.AddListener(OnSearchChanged);
            _eventService.Subscribe<LeaderboardLoadAllDataEvent>(entries => _entries = entries.LeaderboardEntries);
        }

        private void OnSearchChanged(string _)
        {
            if (_debounceRoutine != null) StopCoroutine(_debounceRoutine);
            _debounceRoutine = StartCoroutine(DebounceThenFilter());
        }

        private IEnumerator DebounceThenFilter()
        {
            yield return new WaitForSecondsRealtime(debounceSeconds);
            ApplyFilter(_searchInput.text);
        }

        private void ApplyFilter(string query)
        {
            int token = ++_requestToken; // invalidates any still-running older filter job

            if (!_entries.IsCreated) return;

            if (string.IsNullOrEmpty(query))
            {
                _scrollView.ClearFilter(); // show everything, exactly like before search existed
                return;
            }

            StartCoroutine(RunFilterJob(query, token));
        }

        private IEnumerator RunFilterJob(string query, int token)
        {
            var queryFixed = new FixedString64Bytes();
            // Silently cap at the buffer size - fine for a search box.
            for (int i = 0; i < query.Length && i < 61; i++)
                queryFixed.Append(query[i]);

            var indices = new NativeList<int>(Allocator.Persistent);
            var job = new SearchFilterJob { Entries = _entries, Query = queryFixed };
            JobHandle handle = job.ScheduleAppend(indices, _entries.Length, 256);

            while (!handle.IsCompleted)
                yield return null; // never blocks the frame

            handle.Complete();

            if (token != _requestToken)
            {
                // A newer search started while this one was still running - discard it.
                indices.Dispose();
                yield break;
            }

            _scrollView.SetFilteredIndices(indices.AsArray()); // copies what it needs synchronously
            indices.Dispose();
        }

        [BurstCompile]
        private struct SearchFilterJob : IJobFilter
        {
            [ReadOnly] public NativeArray<LeaderboardEntry> Entries;
            public FixedString64Bytes Query;

            public bool Execute(int index) => Contains(Entries[index].Username, Query);

            // Plain ASCII, case-insensitive substring search - no managed
            // string calls, so this stays fully Burst-compatible.
            private static bool Contains(in FixedString64Bytes haystack, in FixedString64Bytes needle)
            {
                int hLen = haystack.Length, nLen = needle.Length;
                if (nLen == 0) return true;
                if (nLen > hLen) return false;

                for (int start = 0; start <= hLen - nLen; start++)
                {
                    bool match = true;
                    for (int i = 0; i < nLen; i++)
                    {
                        if (ToLower(haystack[start + i]) != ToLower(needle[i])) { match = false; break; }
                    }
                    if (match) return true;
                }
                return false;
            }

            private static byte ToLower(byte c) => (c >= (byte)'A' && c <= (byte)'Z') ? (byte)(c + 32) : c;
        }
    }
}
