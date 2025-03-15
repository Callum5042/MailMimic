using MailMimic.Models;

namespace MailMimic.MailStores;

public interface IMimicStore
{
    Task AddAsync(MimicMessage message);
    Task<MimicMessage?> FindAsync(Guid id);
    Task<ICollection<MimicMessage>> GetAllAsync();
    Task RemoveAsync(MimicMessage message);
}
