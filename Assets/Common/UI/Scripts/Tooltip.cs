using System;
using System.Threading;
using Common.Utils;
using Common.Utils.Extensions;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Common.UI
{
    public class Tooltip : MonoBehaviour
    {
        [Header("Parameters")] 
        [SerializeField] private float _hideDelay = 2f;
        [SerializeField] private bool _closeOnTouch;
        [Space] 
        [Header("References")] 
        [SerializeField] private TMP_Text _tooltipText;
        [SerializeField] private Animator _animator;

        private bool _isShowing;
        private bool _isClosing;
        private CancellationTokenSource _cts;

        public void Show()
            => Show(string.Empty);

        public async UniTask ShowAndHideAsync()
            => await ShowAndHideAsync(string.Empty);

        public void Show(string text)
        {
            ShowAndHideAsync(text).Forget();
        }

        public async UniTask ShowAndHideAsync(string text)
        {
            if (_isShowing)
            {
                return;
            }

            if (!text.IsNullOrEmpty())
            {
                _tooltipText.text = text;
            }

            gameObject.SetActive(true);
            _isShowing = true;

            if (_animator != null)
            {
                await _animator.TriggerAsync("show", cancellationToken: destroyCancellationToken);
            }

            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            
            if(_hideDelay>0) 
                await ScheduleHide();
        }

        public void ForceHide()
        {
            if (_closeOnTouch && _isShowing && !_isClosing)
            {
                _cts?.Cancel();
            }
        }

        private async UniTask ScheduleHide()
        {
            _cts.Token.ThrowIfCancellationRequested();
            try
            {
                if (_closeOnTouch)
                {
                    await UniTask.WhenAny(UniTask.WaitForSeconds(_hideDelay, cancellationToken: _cts.Token),
                        Helpers.WaitForPlayerInput(_cts.Token));
                }
                else
                {
                    await UniTask.WaitForSeconds(_hideDelay, cancellationToken: _cts.Token);
                }
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                await HideAsync();
            }
        }

        public async UniTask HideAsync()
        {
            _isClosing = true;

            if (_animator != null)
            {
                await _animator.TriggerAsync("hide", cancellationToken: destroyCancellationToken);
            }

            _isClosing = false;
            _isShowing = false;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}