using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.SplashScreen
{
    public class SplashSceneView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Slider m_ProgressBar;
        [SerializeField] private TMP_Text m_ProgressText;

        public void ShowLoadingCompleted()
        {
            m_ProgressBar.value = 1f;
            m_ProgressText.text = "LOADING COMPLETED";
        }
    
        public void SetProgress(float progress)
        {
            m_ProgressBar.value = progress;
            m_ProgressText.text = (int)(progress * 100) + "%";
        }

        public async UniTask HideView()
        {
            await _canvasGroup.DOFade(0f, 0.3f).ToUniTask();
            gameObject.SetActive(false);
        }
    
    }
}