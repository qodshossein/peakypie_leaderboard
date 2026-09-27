using _Template.Core.InputSystem;
using UnityEngine;
using Zenject;

public class Test : MonoBehaviour
{
    [Inject] private IInputService _inputService;

    private InputPlayer _inputPlayer;
    private bool _hasPlayer;

    private void Update()
    {
        // اگر Player نداریم، یکی پیدا کن
        if (!_hasPlayer)
        {
            if (!_inputService.TryGetUnpairedPlayer(out _inputPlayer))
                return;

            _hasPlayer = true;

            Debug.Log($"Input Player Assigned: {_inputPlayer.Id}");
        }

        // بررسی کن Player هنوز وجود دارد
        if (!_inputService.TryGetPlayer(_inputPlayer.Id, out _))
        {
            _hasPlayer = false;
            _inputPlayer = default;

            return;
        }

        // دریافت Input
        if (_inputService.WasPressed(_inputPlayer, "Attack"))
        {
            Debug.Log($"Player {_inputPlayer.Id} Attack");
        }
    }
}