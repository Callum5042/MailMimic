using MailMimic.ExchangeServer.Models;

namespace MailMimic.ExchangeServer.MailStores;

public interface IMimicStore
{
    Task AddAsync(MimicMessage message);
    Task<MimicMessage?> FindAsync(Guid id);
    Task<ICollection<MimicMessage>> GetAllAsync();
    Task RemoveAsync(MimicMessage message);
}
