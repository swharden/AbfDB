using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfWatcher.Database
{
    internal struct AbfRecord
    {
        public string Folder;
        public string Filename;
        public string Guid;
        public string Comments;
        public string Created;
        public string Protocol;
        public double LengthSec;
    }
}
