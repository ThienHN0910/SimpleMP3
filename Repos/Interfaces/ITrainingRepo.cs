using Models;
using SimpleMP3.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos.Interfaces
{
    public interface ITrainingRepo
    {
        public List<PlayHistoryEntry> LoadTrainingDataFromEf();
    }
}
