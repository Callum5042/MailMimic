using MailMimic.ExchangeServer.Models;
using System.Collections.Concurrent;

namespace MailMimic.ExchangeServer.MailStores;

public class InMemoryMimicStore : IMimicStore
{
    private readonly ConcurrentDictionary<Guid, MimicMessage> _messages = new();

    public Task AddAsync(MimicMessage message)
    {
        _ = _messages.TryAdd(message.Id, message);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(MimicMessage message)
    {
        _messages.TryRemove(message.Id, out _);
        return Task.CompletedTask;
    }

    public Task<ICollection<MimicMessage>> GetAllAsync()
    {
        ICollection<MimicMessage> messages = _messages.Select(x => x.Value).ToList();
        return Task.FromResult(messages);
    }

    public Task<MimicMessage?> FindAsync(Guid id)
    {
        _messages.TryGetValue(id, out var message);
        return Task.FromResult(message);
    }
}