using MailMimic.MailStores;
using MailMimic.Models;
using MailMimic.Storage.Redis.Models;
using Redis.OM;
using Redis.OM.Contracts;
using Redis.OM.Searching;

namespace MailMimic.Storage.Redis;

public class RedisMimicStore : IMimicStore
{
    private readonly IRedisConnectionProvider _provider;
    private readonly IRedisCollection<RedisMimicMessage> _messages;

    public RedisMimicStore(IRedisConnectionProvider redisConnectionProvider)
    {
        _provider = redisConnectionProvider;
        _messages = _provider.RedisCollection<RedisMimicMessage>();

        // Ensure the index exists
        var existingIndexes = _provider.Connection.Execute("FT._LIST")
                                .ToArray()
                                .Select(x => x.ToString())
                                .ToList(); 

        if (!existingIndexes.Contains("mimic-messages"))
        {
            _provider.Connection.CreateIndex(typeof(RedisMimicMessage));
        }
    }

    public async Task AddAsync(MimicMessageEntity message)
    {
        var redisMessage = RedisMimicMessage.FromDomain(message);
        await _messages.InsertAsync(redisMessage);
    }

    public async Task<MimicMessageEntity?> FindAsync(Guid id)
    {
        var redisMessage = await _messages.FindByIdAsync(id.ToString());
        return redisMessage?.ToDomain();
    }

    public async Task<ICollection<MimicMessageEntity>> GetAllAsync()
    {
        var redisMessages = await _messages.ToListAsync();
        return redisMessages.Select(msg => msg.ToDomain()).ToList();
    }

    public async Task RemoveAsync(MimicMessageEntity message)
    {
        var redisMessage = await _messages.FindByIdAsync(message.Id.ToString());
        if (redisMessage is null)
        {
            return;
        }

        await _messages.DeleteAsync(redisMessage);
    }
}