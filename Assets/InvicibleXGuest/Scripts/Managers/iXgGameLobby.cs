using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InvicibleXGuest.Scripts.Managers
{
    public class iXgGameLobby : MonoBehaviour
    {
        private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
        private float heartBeatTimer;

        private Lobby joinedLobby;
        private float listLobbiesTimer;
        public static iXgGameLobby Instance { get; private set; }

        public void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeUnityAuthentication();
        }

        private void Update()
        {
            HandleHearBeat();
            HandlePeriodicListLobbies();
        }

        public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;

        public event EventHandler OnCreateLobbyStarted;
        public event EventHandler OnCreateLobbyFailed;
        public event EventHandler OnJoinStarted;
        public event EventHandler OnQuickJoinFailed;
        public event EventHandler OnJoinFailed;

        private async void InitializeUnityAuthentication()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                var initializationOptions = new InitializationOptions();
                //initializationOptions.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());

                await UnityServices.InitializeAsync(initializationOptions);

                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }

        private void HandleHearBeat()
        {
            if (IsLobbyHost())
            {
                heartBeatTimer -= Time.deltaTime;
                if (heartBeatTimer <= 0)
                {
                    var heartBeatTimerMax = 15f;
                    heartBeatTimer = heartBeatTimerMax;
                    LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
                }
            }
        }

        private bool IsLobbyHost()
        {
            return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
        }

        public async void CreateLobby(string lobbyName, bool isPrivate)
        {
            OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
            try
            {
                var options = new CreateLobbyOptions();
                options.IsPrivate = isPrivate;
                joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, iXgGameMultiplayer.MaxPlayers,
                    options);

                var regions = await RelayService.Instance.ListRegionsAsync();
                var region = regions[0].Id;
                var hostAllocation =
                    await RelayService.Instance.CreateAllocationAsync(iXgGameMultiplayer.MaxPlayers, region);

                var relayJoinCode = await GetRelayJoinCode(hostAllocation);
                var connectionType = "udp";
                var relayServerData = hostAllocation.ToRelayServerData(connectionType);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                    }
                });


                iXgGameMultiplayer.Instance.StartHost();
                Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
            }
            catch (LobbyServiceException e)
            {
                OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
                Debug.Log(e);
            }
        }


        public async void QuickJoin()
        {
            OnJoinStarted?.Invoke(this, EventArgs.Empty);

            try
            {
                // Join a lobby
                joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
                Debug.Log($"Quick joined lobby: {joinedLobby.Name}");

                // Retrieve Relay join code from lobby data
                if (!joinedLobby.Data.TryGetValue(KEY_RELAY_JOIN_CODE, out var relayJoinCodeObject))
                {
                    Debug.LogError("Relay join code not found in lobby data.");
                    OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
                    return;
                }

                var relayJoinCode = relayJoinCodeObject.Value;
                Debug.Log($"Relay join code retrieved: {relayJoinCode}");

                // Join Relay allocation using the join code
                var joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
                Debug.Log("Successfully joined Relay allocation.");

                // Set Relay server data
                var connectionType = "udp"; // Set connection type (UDP in this case)
                var relayServerData = joinAllocation.ToRelayServerData(connectionType);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                Debug.Log("Relay server data set successfully.");

                // Start the client
                iXgGameMultiplayer.Instance.StartClient();
                Debug.Log("Client started successfully.");
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Failed to join lobby: {e.Message}");
                OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
            }
            catch (RelayServiceException e)
            {
                Debug.LogError($"Failed to join Relay: {e.Message}");
                OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
            }
        }


        public async void JoinWithCode(string lobbyCode)
        {
            OnJoinStarted?.Invoke(this, EventArgs.Empty);
            try
            {
                // Join the lobby using the provided code
                joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
                Debug.Log($"Successfully joined lobby: {joinedLobby.Name}");

                // Check if the lobby has the Relay join code in its data
                if (!joinedLobby.Data.TryGetValue(KEY_RELAY_JOIN_CODE, out var relayJoinCodeObject))
                {
                    Debug.LogError("Relay join code not found in lobby data.");
                    OnJoinFailed?.Invoke(this, EventArgs.Empty);
                    return;
                }

                var relayJoinCode = relayJoinCodeObject.Value;
                Debug.Log($"Retrieved Relay join code: {relayJoinCode}");

                // Join the Relay allocation using the join code
                var joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
                Debug.Log("Successfully joined Relay allocation.");

                // Set Relay server data for the UnityTransport
                var connectionType = "udp"; // Use UDP connection type
                var relayServerData = joinAllocation.ToRelayServerData(connectionType);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                Debug.Log("Relay server data set successfully.");

                // Start the client
                iXgGameMultiplayer.Instance.StartClient();
                Debug.Log("Client started successfully.");
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"LobbyServiceException: {e.Message}");
                OnJoinFailed?.Invoke(this, EventArgs.Empty);
            }
            catch (RelayServiceException e)
            {
                Debug.LogError($"RelayServiceException: {e.Message}");
                OnJoinFailed?.Invoke(this, EventArgs.Empty);
            }
        }

        public async void JoinWithId(string lobbyId)
        {
            OnJoinStarted?.Invoke(this, EventArgs.Empty);
            try
            {
                // Join the lobby using the provided lobby ID
                joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
                Debug.Log($"Successfully joined lobby: {joinedLobby.Name}");

                // Check if the lobby contains the Relay join code in its data
                if (!joinedLobby.Data.TryGetValue(KEY_RELAY_JOIN_CODE, out var relayJoinCodeObject))
                {
                    Debug.LogError("Relay join code not found in lobby data.");
                    OnJoinFailed?.Invoke(this, EventArgs.Empty);
                    return;
                }

                var relayJoinCode = relayJoinCodeObject.Value;
                Debug.Log($"Retrieved Relay join code: {relayJoinCode}");

                // Join the Relay allocation using the join code
                var joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
                Debug.Log("Successfully joined Relay allocation.");

                // Set Relay server data for the UnityTransport
                var connectionType = "udp"; // Use UDP connection type
                var relayServerData = joinAllocation.ToRelayServerData(connectionType);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                Debug.Log("Relay server data set successfully.");

                // Start the client
                iXgGameMultiplayer.Instance.StartClient();
                Debug.Log("Client started successfully.");
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"LobbyServiceException: {e.Message}");
                OnJoinFailed?.Invoke(this, EventArgs.Empty);
            }
            catch (RelayServiceException e)
            {
                Debug.LogError($"RelayServiceException: {e.Message}");
                OnJoinFailed?.Invoke(this, EventArgs.Empty);
            }
        }


        public async void LeaveLobby()
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void KickPlayerFromLobby(string playerId)
        {
            try
            {
                if (IsLobbyHost())
                    await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public Lobby GetLobby()
        {
            return joinedLobby;
        }

        public async void DeleteLobby()
        {
            try
            {
                if (joinedLobby == null) return;

                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        private async Task<string> GetRelayJoinCode(Allocation allocation)
        {
            try
            {
                var relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                return relayJoinCode;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
                return default;
            }
        }

        private void HandlePeriodicListLobbies()
        {
            if (joinedLobby == null &&
                UnityServices.State == ServicesInitializationState.Initialized &&
                AuthenticationService.Instance.IsSignedIn &&
                SceneManager.GetActiveScene().name == Loader.Scene.LobbyScene.ToString())
            {
                listLobbiesTimer -= Time.deltaTime;
                if (listLobbiesTimer <= 0f)
                {
                    var listLobbiesTimerMax = 3f;
                    listLobbiesTimer = listLobbiesTimerMax;
                    ListLobbies();
                }
            }
        }

        private async void ListLobbies()
        {
            try
            {
                var options = new QueryLobbiesOptions();
                options.Count = 25;

                // Filter for open lobbies only
                options.Filters = new List<QueryFilter>
                {
                    new(
                        QueryFilter.FieldOptions.AvailableSlots,
                        op: QueryFilter.OpOptions.GT,
                        value: "0")
                };
                var lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);

                OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs
                {
                    lobbyList = lobbies.Results
                });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public class OnLobbyListChangedEventArgs : EventArgs
        {
            public List<Lobby> lobbyList;
        }
    }
}