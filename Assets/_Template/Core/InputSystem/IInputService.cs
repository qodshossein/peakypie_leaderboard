using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace _Template.Core.InputSystem
{
    public interface IInputService
    {
        #region Properties 
        IReadOnlyCollection<InputPlayer> Players { get; } 
        int PlayerCount { get; } 
        int PairedPlayerCount { get; }
        #endregion 
        #region Player Management 
        InputPlayer RegisterPlayer( PlayerInput playerInput);
        void UnregisterPlayer( PlayerInput playerInput); 
        void UnregisterPlayer( InputPlayer player); 
        bool TryGetPlayer( int id, out InputPlayer player); 
        bool TryGetPlayer( PlayerInput playerInput, out InputPlayer player);
        bool TryGetPlayerInput( InputPlayer player, out PlayerInput playerInput); 
        #endregion 
        #region Player Lookup 
        bool TryGetFirstPlayer( out InputPlayer player); 
        bool TryGetUnpairedPlayer( out InputPlayer player);
        #endregion 
        #region Player Pairing 
        bool PairPlayer( InputPlayer player);
        bool UnpairPlayer( InputPlayer player);
        bool IsPlayerPaired( InputPlayer player); 
        #endregion
        #region Joining 
        void SetPlayerInputManager( PlayerInputManager playerInputManager); 
        void EnableJoining(); 
        void DisableJoining();
        #endregion
        #region Player Leave 
        void LeavePlayer( InputPlayer player);
        #endregion 
        #region Input 
        bool IsPressed( InputPlayer player, string action);
        bool WasPressed( InputPlayer player, string action); 
        bool WasReleased( InputPlayer player, string action); 
        T ReadValue<T>( InputPlayer player, string action) where T : struct;
        #endregion
        #region Action Maps 
        void EnableActionMap( InputPlayer player, string map);
        void DisableActionMap( InputPlayer player, string map);
        void SwitchActionMap( InputPlayer player, string map);
        #endregion
        #region Rebinding 
        void StartRebind( InputPlayer player, string action, int bindingIndex, Action<string> onComplete);
        void CancelRebind( InputPlayer player);
        void ResetBindings( InputPlayer player);
        #endregion
    }
}