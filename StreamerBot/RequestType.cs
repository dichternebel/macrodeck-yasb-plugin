﻿namespace dichternebel.YaSB.StreamerBot
{
    public enum RequestType
    {
        Authenticate,
        Subscribe,
        UnSubscribe,
        GetEvents,
        GetActions,
        DoAction,
        GetBroadcaster,
        GetMonitoredYouTubeBroadcasts,
        GetCredits,
        TestCredits,
        ClearCredits,
        GetInfo,
        GetActiveViewers,
        ExecuteCodeTrigger,
        GetCodeTriggers,
        GetCommands,
        TwitchGetEmotes,
        YouTubeGetEmotes,
        GetGlobals,
        GetGlobal,
        TwitchGetUserGlobals,
        TwitchGetUserGlobal,
        YouTubeGetUserGlobals,
        YouTubeGetUserGlobal,
        TrovoGetUserGlobals,
        TrovoGetUserGlobal,
        SendMessage,
        GetUserPronouns
    }
}
