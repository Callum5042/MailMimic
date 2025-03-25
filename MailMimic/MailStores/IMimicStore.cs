using MailMimic.Models;

namespace MailMimic.MailStores;

public interface IMimicStore
{
    Task AddAsync(MimicMessageEntity message);
    Task<MimicMessageEntity?> FindAsync(Guid id);
    Task<ICollection<MimicMessageEntity>> GetAllAsync();
    Task RemoveAsync(MimicMessageEntity message);
}
