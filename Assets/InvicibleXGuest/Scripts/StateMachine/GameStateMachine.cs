using System;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace InvicibleXGuest.Scripts.StateMachine
{
    public class GameStateMachine : MonoBehaviour
    {
        public enum GameCurrentState
        {
            NotStarted,
            WaitingToStart,
            Started,
            Paused,
            Finished,
        }
        
        public GameCurrentState currentState;

        private void Start()
        {
            TransitionToState(GameCurrentState.NotStarted);
        }

        private void Update()
        {
            HandleState();
        }

        private void HandleState()
        {
            switch (currentState)   
            {
                case GameCurrentState.NotStarted:
                    break;
                case GameCurrentState.WaitingToStart:
                    break;
                case GameCurrentState.Started:
                    break;
                case GameCurrentState.Paused:
                    break;
                case GameCurrentState.Finished:
                    break;
            }
        }

        private void EnterState(GameCurrentState state)
        {
            switch (state)
            {
                case GameCurrentState.NotStarted:
                    Debug.Log("Game is entered NotStarted state.");
                    break;
                case GameCurrentState.WaitingToStart:
                    Debug.Log("Game is entered WaitingToStart state.");
                    break;
                case GameCurrentState.Started:
                    Debug.Log("Game is entered Started state.");
                    break;
                case GameCurrentState.Paused:
                    Debug.Log("Game is entered Paused state.");
                    break;
                case GameCurrentState.Finished:
                    Debug.Log("Game is entered Finished state.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void ExitState(GameCurrentState state)
        {
            switch (state)
            {
                case GameCurrentState.NotStarted:
                    Debug.Log("Game is exited NotStarted state.");
                    break;
                case GameCurrentState.WaitingToStart:
                    Debug.Log("Game is exited WaitingToStart state.");
                    break;
                case GameCurrentState.Started:
                    Debug.Log("Game is exited Started state.");
                    break;
                case GameCurrentState.Paused:
                    Debug.Log("Game is exited Paused state.");
                    break;
                case GameCurrentState.Finished:
                    Debug.Log("Game is exited Finished state.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void TransitionToState(GameCurrentState newState)
        {
            Debug.Log("Transitioning to Game State" + newState);
            ExitState(currentState);
            currentState = newState;
            EnterState(newState);
        }
    }

    
}