using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.Leaderboard
{
    public class LeaderboardScrollView : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
    {
        [SerializeField] private RectTransform viewport;
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private RectTransform content;

        [SerializeField] private float rowHeight = 60f;
        [SerializeField] private float spacing = 4f;
        [SerializeField] private int bufferRows = 4;

        [Header("Feel")]
        [SerializeField] private float wheelPixelsPerNotch = 120f;
        [SerializeField] private float inertiaDecay = 4f;


        private LeaderboardRowUI _rowPrefab;
        [Inject(Id = "LeaderboardCanvas")] private Canvas _canvas;

        private float Step => rowHeight + spacing;

        private NativeArray<LeaderboardEntry> _data;
        private bool _filterActive; // explicit flag - do NOT infer this from _indices.IsCreated
        private NativeArray<int> _indices; // meaningful only while _filterActive is true (can legitimately be empty)
        private readonly List<LeaderboardRowUI> _pool = new List<LeaderboardRowUI>();
        private int[] _boundIndex;

        private double _scrollY;
        private double _maxScrollY;
        private float _velocity;
        private bool _dragging;
        private bool _scrollbarDrivingUs;

        // Can legitimately be zero (e.g. a search that matches nobody) - do
        // not confuse "filter active, zero matches" with "no filter".
        private int Count => _filterActive ? _indices.Length : (_data.IsCreated ? _data.Length : 0);

        int EntryIndexFor(int shownIndex) => _filterActive ? _indices[shownIndex] : shownIndex;

        [Inject]
        private void Construct(LeaderboardRowUI rowPrefab)
        {
            _rowPrefab = rowPrefab;

            EnforceContentSetup();
            if (scrollbar != null) scrollbar.onValueChanged.AddListener(OnScrollbarChanged);
        }

        void EnforceContentSetup()
        {
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(content.pivot.x, 1f);
            content.anchoredPosition = Vector2.zero;
        }

        public void SetData(NativeArray<LeaderboardEntry> data)
        {
            _data = data;

            if (_indices.IsCreated) { _indices.Dispose(); _indices = default; }
            _filterActive = false;

            int poolSize = Mathf.CeilToInt(viewport.rect.height / Step) + bufferRows + 1;
            while (_pool.Count < poolSize)
            {
                var row = Instantiate(_rowPrefab, content);
                var rt = row.rectTransform;
                rt.anchorMin = new Vector2(0f, 1f);
                rt.anchorMax = new Vector2(1f, 1f);
                rt.pivot = new Vector2(0.5f, 1f);
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, rowHeight);
                rt.localScale = Vector3.one;
                _pool.Add(row);
            }

            if (_boundIndex == null || _boundIndex.Length != _pool.Count)
                _boundIndex = new int[_pool.Count];

            ResetViewForNewList();
        }

        /// <summary>
        /// A zero-length array is valid and correctly shows an EMPTY list
        /// ("no user matches this search") - it never falls back to showing
        /// everything. Call ClearFilter() to go back to the full,
        /// unfiltered leaderboard.
        /// </summary>
        public void SetFilteredIndices(NativeArray<int> matchingIndices)
        {
            if (_indices.IsCreated) _indices.Dispose();

            // Always allocate our OWN array, even for zero matches. Relying
            // on matchingIndices.IsCreated here was the bug: a NativeList
            // that never had anything appended can report IsCreated=false
            // for its AsArray() view even though "zero matches" is a real,
            // meaningful result - not the same thing as "no filter applied".
            int length = matchingIndices.IsCreated ? matchingIndices.Length : 0;
            _indices = new NativeArray<int>(length, Allocator.Persistent);
            if (length > 0)
                NativeArray<int>.Copy(matchingIndices, _indices, length);

            _filterActive = true;
            ResetViewForNewList();
        }

        public void ClearFilter()
        {
            if (_indices.IsCreated) { _indices.Dispose(); _indices = default; }
            _filterActive = false;
            ResetViewForNewList();
        }

        void ResetViewForNewList()
        {
            int count = Count;
            _maxScrollY = System.Math.Max(0.0, count * (double)Step - viewport.rect.height);
            _scrollY = 0;
            _velocity = 0f;

            for (int i = 0; i < _boundIndex.Length; i++) _boundIndex[i] = -1;

            if (scrollbar != null)
                scrollbar.size = count > 0 ? Mathf.Clamp01((float)_pool.Count / count) : 1f;

            RefreshVisibleRows();
            UpdateScrollbarFromScroll();
        }

        void Update()
        {
            if (_dragging || _velocity == 0f) return;

            ScrollBy(_velocity * Time.unscaledDeltaTime);

            float decel = Mathf.Abs(_velocity) * inertiaDecay * Time.unscaledDeltaTime + 1f;
            _velocity = Mathf.MoveTowards(_velocity, 0f, decel);
            if (Mathf.Abs(_velocity) < 1f) _velocity = 0f;
        }

        public void OnBeginDrag(PointerEventData e) { _dragging = true; _velocity = 0f; }

        public void OnDrag(PointerEventData e)
        {
            float scale = _canvas != null ? _canvas.scaleFactor : 1f;
            float delta = e.delta.y / Mathf.Max(0.0001f, scale);
            ScrollBy(delta);
            _velocity = delta / Mathf.Max(0.0001f, Time.unscaledDeltaTime);
        }

        public void OnEndDrag(PointerEventData e) { _dragging = false; }

        public void OnScroll(PointerEventData e)
        {
            ScrollBy(-e.scrollDelta.y * wheelPixelsPerNotch / 3f);
            _velocity = 0f;
        }

        void ScrollBy(float deltaPixels)
        {
            double next = _scrollY + deltaPixels;
            _scrollY = next < 0.0 ? 0.0 : (next > _maxScrollY ? _maxScrollY : next);
            RefreshVisibleRows();
            UpdateScrollbarFromScroll();
        }

        void OnScrollbarChanged(float value)
        {
            if (_scrollbarDrivingUs) return;
            _scrollY = (1.0 - value) * _maxScrollY;
            _velocity = 0f;
            RefreshVisibleRows();
        }

        void UpdateScrollbarFromScroll()
        {
            if (scrollbar == null) return;
            _scrollbarDrivingUs = true;
            scrollbar.value = _maxScrollY > 0.0 ? (float)(1.0 - _scrollY / _maxScrollY) : 1f;
            _scrollbarDrivingUs = false;
        }

        void RefreshVisibleRows()
        {
            int count = Count;
            if (count == 0 || _pool.Count == 0)
            {
                // Empty result (e.g. a search that matches nobody) must
                // actively hide whatever was showing before, not just skip
                // the update and leave stale rows on screen.
                for (int i = 0; i < _pool.Count; i++)
                {
                    _pool[i].gameObject.SetActive(false);
                    _boundIndex[i] = -1;
                }
                return;
            }

            double continuousIndex = _scrollY / Step;

            int firstIndex = Mathf.Max(0, (int)System.Math.Floor(continuousIndex) - bufferRows / 2);
            if (firstIndex + _pool.Count > count)
                firstIndex = Mathf.Max(0, count - _pool.Count);

            for (int i = 0; i < _pool.Count; i++)
            {
                int shownIndex = firstIndex + i;
                var row = _pool[i];

                if (shownIndex >= count)
                {
                    row.gameObject.SetActive(false);
                    _boundIndex[i] = -1;
                    continue;
                }

                row.gameObject.SetActive(true);

                double offsetFromViewportTop = shownIndex - continuousIndex;
                row.rectTransform.anchoredPosition = new Vector2(0, (float)(-offsetFromViewportTop * Step));

                int entryIndex = EntryIndexFor(shownIndex);

                if (_boundIndex[i] != entryIndex)
                {
                    row.Bind(entryIndex, _data[entryIndex]);
                    _boundIndex[i] = entryIndex;
                }
            }
        }

        void OnDestroy()
        {
            if (_indices.IsCreated) _indices.Dispose();
        }
    }
}