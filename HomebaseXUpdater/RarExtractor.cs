using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;
using SharpCompress.Archives;

namespace HomebaseXUpdater
{
    internal class RarExtractor
    {
        public static void ExtractRARFile(string filePath, string destinationDirectory)
        {
            using (var archive = RarArchive.Open(filePath))
            {
                foreach (var entry in archive.Entries)
                {
                    if (!entry.IsDirectory)
                    {
                        entry.WriteToDirectory(destinationDirectory, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });
                    }
                }
            }
        }
    }
}
