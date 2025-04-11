using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class RoundTimer : MonoBehaviour
    {
        public TextMeshProUGUI timerText;
        private Image _timeBar;
        private const float RoundDuration = 90f;
        private float _timeLeft;


        private void Start()
        {
            _timeLeft = RoundDuration;
            _timeBar = GetComponent<Image>();
        }

        private void Update()
        {
            if (_timeLeft > 0)
            {
                _timeLeft -= Time.deltaTime;
                UpdateTimerUI();
                _timeBar.fillAmount = _timeLeft / RoundDuration;
            }
            else
            {
                _timeLeft = 0f;
                EndRound();
            }
            
        }

        private void UpdateTimerUI()
        {
            var minutes = Mathf.FloorToInt(_timeLeft / 60f);
            var seconds = Mathf.FloorToInt(_timeLeft % 60f);
            var timer = $"{minutes:0}:{seconds:00}";
            timerText.text = timer == "-01:-01" 
                ? "00:00" 
                : timer;
        }

        private void EndRound()
        {
            // Здесь можно вызвать событие окончания игры
            Debug.Log("Время вышло! Конец раунда.");
            SceneManager.LoadScene("MainMenu");
        }
    }           
}