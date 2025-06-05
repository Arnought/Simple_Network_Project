using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityUtils;

public class SessionManager : Singleton<SessionManager>
{
    ISession activeSession;
    ISession ActiveSession
    {
        get => activeSession;
        set
        {
            activeSession = value;
            Debug.Log($"Active Session: {activeSession}");
        }
    }

    const string playerNamePropertyKey = "playerName";

    async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Sign in anonymously successful! PlayerID: {AuthenticationService.Instance.PlayerId}");
            StartSessionHost();

        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    async UniTask<Dictionary<string, PlayerProperty>> GetPlayerProperties()
    {
        var playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
        var playerNameProperty = new PlayerProperty(playerName, VisibilityPropertyOptions.Member);
        return new Dictionary<string, PlayerProperty> { { playerNamePropertyKey, playerNameProperty } };
    }

    async void StartSessionHost()
    {
        var playerProperties = await GetPlayerProperties();
        var option = new SessionOptions
        {
            MaxPlayers = 4,
            IsLocked = false,
            IsPrivate = false
        }.WithRelayNetwork();

        ActiveSession = await MultiplayerService.Instance.CreateSessionAsync(option);
        Debug.Log($"Session {ActiveSession.Id} Created! Join Code:{ActiveSession.Code}");
    }

    async UniTask JoinSessionId(string session)
    {
        ActiveSession = await MultiplayerService.Instance.JoinSessionByIdAsync(session);
        Debug.Log($"Session {ActiveSession.Id} Joined!");
    }

    async UniTask JoinSessionByCode(string sessionCode)
    {
        ActiveSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(sessionCode);
        Debug.Log($"Session {ActiveSession.Id} Joined!");
    }

    async UniTask KickPlayer(string playerId)
    {
        if (ActiveSession.IsHost) return;
        await ActiveSession.AsHost().RemovePlayerAsync(playerId);
    }

    async UniTask<IList<ISessionInfo>> QuerySession()
    {
        var sessionQueryOptions = new QuerySessionsOptions();
        var result = await MultiplayerService.Instance.QuerySessionsAsync(sessionQueryOptions);
        return result.Sessions;
    }

    async UniTask LeaveSession()
    {
        if (ActiveSession != null)
        {
            try
            {
                await ActiveSession.LeaveAsync();
            }
            catch
            {

            }
            finally
            {
                ActiveSession = null;
            }
        }
    }
}
