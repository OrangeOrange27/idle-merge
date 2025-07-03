using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Gameplay.Scripts.UI
{
    public class PlayerLevelWidget : MonoBehaviour
    {
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private Slider _progressBar;

        public void UpdateLevel(int level)
        {
            _levelText.text = level.ToString();
        }

        public void UpdateXp(int currentXp, int totalXp)
        {
            _progressBar.value = currentXp;
            _progressBar.maxValue = totalXp;
        }
    }
}