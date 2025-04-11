using TMPro;
using UnityEngine;

namespace Game
{
    public class ScoreManager : MonoBehaviour
    {
        public TextMeshProUGUI scoreText;

        private int score = 0;

        private void OnEnable()
        {
            GameEvents.onPointsEarned += AddScore;
        }

        private void OnDisable()
        {
            GameEvents.onPointsEarned -= AddScore;
        }

        private void AddScore(int points)
        {
            score += points;
            UpdateScoreUI();
        }

        private void UpdateScoreUI()
        {
            if (scoreText != null)
            {
                scoreText.text = $"{score}";
            }
        }
    }
}