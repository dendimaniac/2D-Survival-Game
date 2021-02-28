using System.Collections;
using Data;
using Signals;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup loseMenuCanvas;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI losingReasonText;
        [SerializeField] private float canvasFadeSpeed;

        private Button[] _buttonArray;
        private SignalBus _signalBus;
        private Score _score;

        [Inject]
        private void Construct(SignalBus signalBus, Score score)
        {
            _signalBus = signalBus;
            _score = score;
        }

        private void Awake()
        {
            _buttonArray = loseMenuCanvas.GetComponentsInChildren<Button>();
            
            _signalBus.Subscribe<GameLostSignal>(LoseMenuHandler);
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.R)) return;

            Restart();
        }

        private void LoseMenuHandler()
        {
            StartCoroutine(FadeLoseMenu());
        }

        private IEnumerator FadeLoseMenu()
        {
            highScoreText.text = _score.LoadHighScore().ToString();
            while (loseMenuCanvas.alpha < 1)
            {
                loseMenuCanvas.alpha += Time.unscaledDeltaTime * canvasFadeSpeed;
                yield return null;
            }

            foreach (var button in _buttonArray) button.interactable = true;
        }

        public void SetLosingReasonText(string reason)
        {
            losingReasonText.text = reason;
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void MainMenu()
        {
            SceneManager.LoadScene("Main Menu");
        }

        public void StartGame()
        {
            SceneManager.LoadScene("Level 01");
        }

        private void OnDestroy()
        {
            _signalBus?.Unsubscribe<GameLostSignal>(LoseMenuHandler);
        }
    }
}