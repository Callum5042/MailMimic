using MailMimic.Models;
using System.Collections.Concurrent;

namespace MailMimic.MailStores;

public class InMemoryMimicStore : IMimicStore
{
    private readonly ConcurrentDictionary<Guid, MimicMessageEntity> _messages = new();

    public Task AddAsync(MimicMessageEntity message)
    {
        _ = _messages.TryAdd(message.Id, message);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(MimicMessageEntity message)
    {
        _messages.TryRemove(message.Id, out _);
        return Task.CompletedTask;
    }

    public Task<ICollection<MimicMessageEntity>> GetAllAsync()
    {
        ICollection<MimicMessageEntity> messages = _messages.Select(x => x.Value).ToList();
        return Task.FromResult(messages);
    }

    public Task<MimicMessageEntity?> FindAsync(Guid id)
    {
        _messages.TryGetValue(id, out var message);
        return Task.FromResult(message);
    }
}