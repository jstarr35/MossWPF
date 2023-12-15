using MossWPF.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MossWPF.Domain.Services
{
    public interface ISubmissionFileService : IDataService<SubmissionFile>
    {
        Task<SubmissionFile> GetSubmissionFileByPath(string filePath);
    }
}
