using Evenex.Domain.Entities;

namespace Evenex.Domain.Repositories;


public interface ISectionRepository
{
    Task<Section?> GetSectionAsync (int id);

    Task AddAsync (Section section);
}