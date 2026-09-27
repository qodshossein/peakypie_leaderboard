using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace _Template.Core.InputSystem
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField]
        private PlayerInputManager _playerInputManager;

        private IInputService _inputService;

        [Inject]
        public void Constructor(IInputService inputService)
        {
            _inputService = inputService;
        }

        private void Start()
        {
            _inputService.SetPlayerInputManager(
                _playerInputManager);

            //_playerInputManager.onPlayerJoined += OnPlayerJoined;
            //_playerInputManager.onPlayerLeft += OnPlayerLeft;
        }

        private void OnDestroy()
        {
            if (_playerInputManager == null)
                return;

            //_playerInputManager.onPlayerJoined -= OnPlayerJoined;
            //_playerInputManager.onPlayerLeft -= OnPlayerLeft;
        }

        private void OnPlayerJoined(PlayerInput playerInput)
        {
            var player = _inputService.RegisterPlayer(
                playerInput);

            Debug.Log(
                $"Player Joined: {player.Id}");
        }

        private void OnPlayerLeft(PlayerInput playerInput)
        {
            _inputService.UnregisterPlayer(
                playerInput);

            Debug.Log(
                $"Player Left: {playerInput.playerIndex}");
        }
    }
}