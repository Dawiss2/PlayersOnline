using VampireCommandFramework;
using ProjectM.Network;
using Unity.Entities;
using Bloodstone.API;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
using Il2CppSystem;


namespace PlayersOnline.Command;
public static class Command
{
    public static Dictionary<string, DateTime> CooldownList = new Dictionary<string, DateTime>();

    [Command("online", description: "Check how many players are online", adminOnly: false)]
    public static void OnlineCommand(ICommandContext ctx)
    {
        if (CooldownList.ContainsKey(ctx.Name))
        {
            CooldownList.TryGetValue(ctx.Name, out DateTime cooldownTime);
            if (cooldownTime > DateTime.Now)
            {
                var seconds = (cooldownTime - DateTime.Now).TotalSeconds;
                seconds = Mathf.FloorToInt((float)seconds);
                ctx.Reply("Please wait " + seconds + " seconds to use this command again.");
            }
            else if (cooldownTime <= DateTime.Now)
            {
                CooldownList.Remove(ctx.Name);
                SendPlayersCount(ctx);

                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.AddSeconds(PlayersOnline.Plugin.Cooldown.Value);
                CooldownList.Add(ctx.Name, dateTime);
            }
        }
        else
        {
            DateTime dateTime = DateTime.Now;
            dateTime = dateTime.AddSeconds(PlayersOnline.Plugin.Cooldown.Value);
            CooldownList.Add(ctx.Name, dateTime);
            SendPlayersCount(ctx);
        }
    }
    public static void SendPlayersCount(ICommandContext ctx)
    {
        NativeArray<Entity> players = VWorld.Server.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<User>()).ToEntityArray(Allocator.Temp);
        ctx.Reply("There are " + players.Length.ToString() + "/" + PlayersOnline.Plugin.PlayerSlots.Value + " players online!");
    }
}
