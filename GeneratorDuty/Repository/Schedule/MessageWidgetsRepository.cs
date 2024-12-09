using GeneratorDuty.Models.Schedule;
using GeneratorDuty.Repository.Database;
using Microsoft.EntityFrameworkCore;

namespace GeneratorDuty.Repository.Schedule;

public class MessageWidgetsRepository(DutyContext ef)
{
    public async Task<IList<MessageWidget>> GetMessageWidgetsAsync(long chatId)
    {
        return await ef.MessageWidgets.Where(w => w.ChatId == chatId).ToListAsync();
    }

    public async Task<MessageWidget?> GetWidgetByChatIdAsync(long chatId)
    {
        return await ef.MessageWidgets.FirstOrDefaultAsync(w => w.ChatId == chatId);
    }
    
    public async Task<MessageWidget> CreateMessageWidgetAsync(MessageWidget messageWidget)
    {
        await ef.AddAsync(messageWidget);
        await ef.SaveChangesAsync();
        return messageWidget;
    }

    public async Task DeleteMessageWidgetAsync(MessageWidget messageWidget)
    {
        ef.Remove(messageWidget);
        await ef.SaveChangesAsync();
    }

    public async Task UpdateMessageWidgetAsync(MessageWidget messageWidget)
    {
        ef.Update(messageWidget);
        await ef.SaveChangesAsync();
    }

    public async Task RemoveAllWidgetInChat(long chatId)
    {
        foreach (var widget in (await GetMessageWidgetsAsync(chatId)).ToList())
        {
            ef.Remove(widget);
        }
        await ef.SaveChangesAsync();
    }
}