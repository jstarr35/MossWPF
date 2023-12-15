using Microsoft.EntityFrameworkCore;
using MossWPF.Data.Services.Common;
using MossWPF.Domain.Models;
using MossWPF.Domain.Services;

namespace MossWPF.Data.Services
{
    public class FilePairDataService : IFilePairService
    {
        private readonly MossDbContextFactory _contextFactory;
        private readonly NonQueryDataService<FilePair> _nonQueryDataService;

        public FilePairDataService(MossDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
            _nonQueryDataService = new NonQueryDataService<FilePair>(contextFactory);
        }

        public async Task<FilePair> Create(FilePair entity)
        {
            return await _nonQueryDataService.Create(entity);
        }

        public async Task<bool> Delete(int id)
        {
            return await _nonQueryDataService.Delete(id);
        }

        public async Task<FilePair> Get(int id)
        {
            using MossDbContext context = _contextFactory.CreateDbContext();
            FilePair entity = await context.FilesPairs.FirstOrDefaultAsync();
            return entity;
        }

        public async Task<IEnumerable<FilePair>> GetAll()
        {
            using MossDbContext context = _contextFactory.CreateDbContext();
            IEnumerable<FilePair> entities = await context.FilesPairs.ToListAsync();
            return entities;
        }

        public async Task<FilePair> Update(int id, FilePair entity)
        {
            return await _nonQueryDataService.Update(id, entity);
        }
    }
}
