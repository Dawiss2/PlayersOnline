using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using VampireCommandFramework;
using ProjectM.Network;
using Unity.Entities;
using Bloodstone.API;
using Unity.Collections;
using BepInEx.Configuration;

namespace PlayersOnline;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.VampireCommandFramework")]
[BepInDependency("gg.deca.Bloodstone")]
[Bloodstone.API.Reloadable]
public class Plugin : BasePlugin
{
    Harmony _harmony;
    private static EntityManager entityManager = VWorld.Server.EntityManager;
    public static ConfigEntry<int> PlayerSlots;

    public override void Load()
    {
        // Plugin startup logic
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded!");

        // Harmony patching
        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        _harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
        ConfigInit();

        // Register all commands in the assembly with VCF
        CommandRegistry.RegisterAll();
    }
    public void ConfigInit()
    {
        PlayerSlots = Config.Bind("PlayerSlots", "slots", 0);
    }

    public override bool Unload()
    {
        CommandRegistry.UnregisterAssembly();
        _harmony?.UnpatchSelf();
        return true;
    }

    [Command("online", description: "Check how many players are online", adminOnly: false)]
    public void OnlineCommand(ICommandContext ctx)
    {
        NativeArray<Entity> players = entityManager.CreateEntityQuery(ComponentType.ReadOnly<User>()).ToEntityArray(Allocator.Temp);
        ctx.Reply("There are " + players.Length.ToString() + "/" + PlayerSlots.Value + " players online!");
    }
}
