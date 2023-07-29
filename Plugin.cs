using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using VampireCommandFramework;
using BepInEx.Configuration;


namespace PlayersOnline;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.VampireCommandFramework")]
[BepInDependency("gg.deca.Bloodstone")]
[Bloodstone.API.Reloadable]
public class Plugin : BasePlugin
{
    Harmony _harmony;
    public static ConfigEntry<int> PlayerSlots;
    public static ConfigEntry<int> Cooldown;

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
        Cooldown = Config.Bind("Cooldown", "Seconds", 60);
    }

    public override bool Unload()
    {
        CommandRegistry.UnregisterAssembly();
        _harmony?.UnpatchSelf();
        return true;
    }
}
