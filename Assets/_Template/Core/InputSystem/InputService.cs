using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Template.Core.InputSystem
{
    public class InputService : IInputService
    {
        private readonly Dictionary<int, PlayerInput> _playerInputs = new();
        private readonly Dictionary<int, InputPlayer> _players = new();

        private readonly HashSet<int> _pairedPlayers = new();

        private readonly Dictionary<
            int,
            InputActionRebindingExtensions.RebindingOperation
        > _rebindOperations = new();

        private PlayerInputManager _playerInputManager;

        private int _nextPlayerId = 1;

        #region Properties

        public IReadOnlyCollection<InputPlayer> Players =>
            _players.Values;

        public int PlayerCount =>
            _players.Count;

        public int PairedPlayerCount =>
            _pairedPlayers.Count;

        #endregion

        #region Player Management

        public InputPlayer RegisterPlayer(
            PlayerInput playerInput)
        {
            if (playerInput == null)
                throw new ArgumentNullException(
                    nameof(playerInput));

            // اگر قبلاً Register شده باشد،
            // Player موجود را برمی‌گردانیم.
            int existingId =
                FindPlayerId(playerInput);

            if (existingId != -1)
                return _players[existingId];

            int id = _nextPlayerId++;

            var player = new InputPlayer(
                id,
                false);

            _players.Add(
                id,
                player);

            _playerInputs.Add(
                id,
                playerInput);

            return player;
        }

        public void UnregisterPlayer(
            PlayerInput playerInput)
        {
            if (playerInput == null)
                return;

            int id = FindPlayerId(
                playerInput);

            if (id == -1)
                return;

            UnregisterPlayer(id);
        }

        public void UnregisterPlayer(
            InputPlayer player)
        {
            UnregisterPlayer(player.Id);
        }

        private void UnregisterPlayer(
            int playerId)
        {
            if (!_players.TryGetValue(
                    playerId,
                    out var player))
            {
                return;
            }

            CancelRebind(player);

            _pairedPlayers.Remove(
                playerId);

            _playerInputs.Remove(
                playerId);

            _players.Remove(
                playerId);
        }

        public bool TryGetPlayer(
            int id,
            out InputPlayer player)
        {
            return _players.TryGetValue(
                id,
                out player);
        }

        public bool TryGetPlayerInput(
            InputPlayer player,
            out PlayerInput playerInput)
        {
            return _playerInputs.TryGetValue(
                player.Id,
                out playerInput);
        }

        public bool TryGetPlayer(
            PlayerInput playerInput,
            out InputPlayer player)
        {
            int id = FindPlayerId(
                playerInput);

            if (id == -1)
            {
                player = default;
                return false;
            }

            return _players.TryGetValue(
                id,
                out player);
        }

        private int FindPlayerId(
            PlayerInput playerInput)
        {
            foreach (var pair in _playerInputs)
            {
                if (pair.Value == playerInput)
                    return pair.Key;
            }

            return -1;
        }

        #endregion

        #region Player Lookup

        public bool TryGetFirstPlayer(
            out InputPlayer player)
        {
            foreach (var pair in _players)
            {
                player = pair.Value;
                return true;
            }

            player = default;
            return false;
        }

        public bool TryGetUnpairedPlayer(
            out InputPlayer player)
        {
            foreach (var pair in _players)
            {
                if (_pairedPlayers.Contains(
                        pair.Key))
                {
                    continue;
                }

                player = pair.Value;
                return true;
            }

            player = default;
            return false;
        }

        #endregion

        #region Player Pairing

        public bool PairPlayer(
            InputPlayer player)
        {
            if (!_players.ContainsKey(
                    player.Id))
            {
                return false;
            }

            return _pairedPlayers.Add(
                player.Id);
        }

        public bool UnpairPlayer(
            InputPlayer player)
        {
            return _pairedPlayers.Remove(
                player.Id);
        }

        public bool IsPlayerPaired(
            InputPlayer player)
        {
            return _pairedPlayers.Contains(
                player.Id);
        }

        #endregion

        #region Joining

        public void SetPlayerInputManager(
            PlayerInputManager playerInputManager)
        {
            _playerInputManager =
                playerInputManager;
        }

        public void EnableJoining()
        {
            if (_playerInputManager == null)
                return;

            _playerInputManager.EnableJoining();
        }

        public void DisableJoining()
        {
            if (_playerInputManager == null)
                return;

            _playerInputManager.DisableJoining();
        }

        #endregion

        #region Player Leave

        public void LeavePlayer(
            InputPlayer player)
        {
            if (!_playerInputs.TryGetValue(
                    player.Id,
                    out var playerInput))
            {
                return;
            }

            CancelRebind(player);

            playerInput.DeactivateInput();

            UnityEngine.Object.Destroy(
                playerInput.gameObject);
        }

        #endregion

        #region Input

        public bool IsPressed(
            InputPlayer player,
            string action)
        {
            InputAction input =
                GetAction(
                    player,
                    action);

            return input != null &&
                   input.IsPressed();
        }

        public bool WasPressed(
            InputPlayer player,
            string action)
        {
            InputAction input =
                GetAction(
                    player,
                    action);

            return input != null &&
                   input.WasPressedThisFrame();
        }

        public bool WasReleased(
            InputPlayer player,
            string action)
        {
            InputAction input =
                GetAction(
                    player,
                    action);

            return input != null &&
                   input.WasReleasedThisFrame();
        }

        public T ReadValue<T>(
            InputPlayer player,
            string action)
            where T : struct
        {
            InputAction input =
                GetAction(
                    player,
                    action);

            if (input == null)
                return default;

            return input.ReadValue<T>();
        }

        private InputAction GetAction(
            InputPlayer player,
            string action)
        {
            if (string.IsNullOrWhiteSpace(action))
                return null;

            if (!_playerInputs.TryGetValue(
                    player.Id,
                    out var playerInput))
            {
                return null;
            }

            return playerInput.actions.FindAction(
                action,
                throwIfNotFound: false);
        }

        #endregion

        #region Action Maps

        public void EnableActionMap(
            InputPlayer player,
            string map)
        {
            InputActionMap actionMap =
                GetActionMap(
                    player,
                    map);

            if (actionMap == null)
                return;

            actionMap.Enable();
        }

        public void DisableActionMap(
            InputPlayer player,
            string map)
        {
            InputActionMap actionMap =
                GetActionMap(
                    player,
                    map);

            if (actionMap == null)
                return;

            actionMap.Disable();
        }

        public void SwitchActionMap(
            InputPlayer player,
            string map)
        {
            if (!_playerInputs.TryGetValue(
                    player.Id,
                    out var playerInput))
            {
                throw new InvalidOperationException(
                    $"Player {player.Id} does not exist.");
            }

            if (string.IsNullOrWhiteSpace(map))
            {
                throw new ArgumentException(
                    "Action Map name cannot be empty.",
                    nameof(map));
            }

            playerInput.SwitchCurrentActionMap(
                map);
        }

        private InputActionMap GetActionMap(
            InputPlayer player,
            string map)
        {
            if (string.IsNullOrWhiteSpace(map))
                return null;

            if (!_playerInputs.TryGetValue(
                    player.Id,
                    out var playerInput))
            {
                return null;
            }

            return playerInput.actions.FindActionMap(
                map,
                throwIfNotFound: false);
        }

        #endregion

        #region Rebinding

        public void StartRebind(
            InputPlayer player,
            string action,
            int bindingIndex,
            Action<string> onComplete)
        {
            InputAction inputAction =
                GetAction(
                    player,
                    action);

            if (inputAction == null)
            {
                throw new InvalidOperationException(
                    $"Input Action '{action}' was not found.");
            }

            if (bindingIndex < 0 ||
                bindingIndex >= inputAction.bindings.Count)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(bindingIndex));
            }

            CancelRebind(player);

            var operation =
                inputAction
                    .PerformInteractiveRebinding(
                        bindingIndex)
                    .OnComplete(op =>
                    {
                        string binding =
                            inputAction
                                .GetBindingDisplayString(
                                    bindingIndex);

                        _rebindOperations.Remove(
                            player.Id);

                        op.Dispose();

                        onComplete?.Invoke(
                            binding);
                    })
                    .OnCancel(op =>
                    {
                        _rebindOperations.Remove(
                            player.Id);

                        op.Dispose();
                    });

            _rebindOperations[player.Id] =
                operation;

            operation.Start();
        }

        public void CancelRebind(
            InputPlayer player)
        {
            if (!_rebindOperations.TryGetValue(
                    player.Id,
                    out var operation))
            {
                return;
            }

            _rebindOperations.Remove(
                player.Id);

            operation.Cancel();
            operation.Dispose();
        }

        public void ResetBindings(
            InputPlayer player)
        {
            if (!_playerInputs.TryGetValue(
                    player.Id,
                    out var playerInput))
            {
                return;
            }

            playerInput.actions
                .RemoveAllBindingOverrides();
        }

        #endregion
    }
}