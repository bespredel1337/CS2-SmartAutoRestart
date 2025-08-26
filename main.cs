using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace SmartAutoRestart;

public class SmartAutoRestart : BasePlugin
{
    public override string ModuleName => "SmartAutoRestart";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "besprede1";
    private const int RestartUptime = 84600; // 24h in seconds
    private Dictionary<int, CCSPlayerController> playerData = new Dictionary<int, CCSPlayerController>();

    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventPlayerConnect>(OnPlayerConnect);
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        
        if (hotReload)
        {
            InitializePlayerData();
        }
    }

    private void InitializePlayerData()
    {
        playerData.Clear();
        foreach (var player in Utilities.GetPlayers())
        {
            if (player != null && player.IsValid && !player.IsBot)
            {
                playerData[player.Slot] = player;
            }
        }
    }

    private HookResult OnPlayerConnect(EventPlayerConnect @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (player != null && player.IsValid && !player.IsBot)
        {
            playerData[player.Slot] = player;
        }
        return HookResult.Continue;
    }

    private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (player != null)
        {
            playerData.Remove(player.Slot);
        }
        
        int PlayersCount = playerData.Count;
        var UptimeSeconds = Server.EngineTime;

        if (PlayersCount == 0 && UptimeSeconds > RestartUptime)
        {
            Console.WriteLine("restart");
            Server.ExecuteCommand("quit");
        }

        return HookResult.Continue;
    }
}