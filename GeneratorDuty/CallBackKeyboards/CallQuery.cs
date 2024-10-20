﻿using Telegram.Bot;
using Telegram.Bot.Types;

namespace GeneratorDuty.CallBackKeyboards;

public abstract class CallQuery
{
    public abstract string Name { get; set; }

    public abstract void Execute(ITelegramBotClient client, CallbackQuery callbackQuery);

    public bool Contains(CallbackQuery callbackQuery)
        => callbackQuery is { Data: not null } && callbackQuery.Data.Contains(Name);
    
    public string[]? TryGetArrayFromCallBack(CallbackQuery callbackQuery)
    {
        try
        {
            return callbackQuery.Data?.Split(' ').Skip(1).ToArray();
        }
        catch
        {
            return null;
        }
    }
}