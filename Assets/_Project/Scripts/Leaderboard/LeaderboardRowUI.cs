using TMPro;
using UnityEngine;

namespace _Project.Scripts.Leaderboard
{
    public class LeaderboardRowUI : MonoBehaviour
    {
        public TMP_Text rankText;
        public TMP_Text nameText;
        public TMP_Text scoreText;

        RectTransform _rect;
        public RectTransform rectTransform => _rect != null ? _rect : (_rect = (RectTransform)transform);

        public void Bind(int rank, LeaderboardEntry entry)
        {
            rankText.text = FormatCompactRank(rank + 1);
            nameText.text = entry.Username.ToString();
            scoreText.text = entry.Score.ToString("N0");
        }

        static string FormatCompactRank(int value)
        {
            if (value < 1000) return value.ToString();

            if (value < 1_000_000)
                return FormatUnit(value, 1000.0, "K");

            return FormatUnit(value, 1_000_000.0, "M");
        }

        static string FormatUnit(int value, double unit, string suffix)
        {
            double scaled = value / unit;
            scaled = System.Math.Floor(scaled);
            string text = (scaled == System.Math.Floor(scaled))
                ? ((long)scaled).ToString()
                : scaled.ToString("0.#");
            return text + suffix;
        }
    }
}