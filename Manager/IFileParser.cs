using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public interface IFileParser
    {
        Task ParseFileAsync(IProgress<int> progress, CancellationToken cancellationToken);
    }
}
