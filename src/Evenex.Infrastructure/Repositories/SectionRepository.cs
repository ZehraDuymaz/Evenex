using Evenex.Domain.Entities;
using Evenex.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Evenex.Infrastructure.Repositories;

public class SectionRepository : ISectionRepository
{
    private readonly AppDbContext _db;

    public SectionRepository (AppDbContext db) => _db = db;

    public async Task<Section?> GetSectionAsync (int id)
    {
        return await _db.Sections.FirstOrDefaultAsync(e => e.Id == id);
    }

    // list of sections 
    public async Task<IEnumerable<Section>> GetSectionByVenueIdAsync (int venueId)
    {
        return await _db.Sections.Where(s => s.VenueId == venueId && !s.IsDeleted).ToListAsync();
    }

    public async Task AddAsync (Section section)
    {
        await _db.Sections.AddAsync(section);
    }
}