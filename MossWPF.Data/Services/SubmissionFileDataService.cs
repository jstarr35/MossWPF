using Microsoft.EntityFrameworkCore;
using MossWPF.Data.Services.Common;
using MossWPF.Domain.Models;
using MossWPF.Domain.Services;

namespace MossWPF.Data.Services
{
    public class SubmissionFileDataService : ISubmissionFileService
    {
        private readonly MossDbContextFactory _contextFactory;
        private readonly NonQueryDataService<SubmissionFile> _nonQueryDataService;

        public SubmissionFileDataService(MossDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
            _nonQueryDataService = new NonQueryDataService<SubmissionFile>(contextFactory);
        }

        public async Task<SubmissionFile> Create(SubmissionFile entity)
        {
            return await _nonQueryDataService.Create(entity);
        }

        public async Task<bool> Delete(int id)
        {
            return await _nonQueryDataService.Delete(id);
        }

        public async Task<SubmissionFile> Get(int id)
        {
            using MossDbContext context = _contextFactory.CreateDbContext();
            SubmissionFile entity = await context.Files.FirstOrDefaultAsync();
            return entity;
        }

        public async Task<IEnumerable<SubmissionFile>> GetAll()
        {
            using MossDbContext context = _contextFactory.CreateDbContext();
            IEnumerable<SubmissionFile> entities = await context.Files.ToListAsync();
            return entities;
        }

        public async Task<SubmissionFile> GetSubmissionFileByPath(string filePath)
        {
            using MossDbContext context = _contextFactory.CreateDbContext();
            return await context.Files.FirstOrDefaultAsync(f => f.FilePath == filePath);
        }

        public async Task<SubmissionFile> Update(int id, SubmissionFile entity)
        {
            return await _nonQueryDataService.Update(id, entity);
        }
    }
}
