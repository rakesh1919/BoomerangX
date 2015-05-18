﻿namespace BoomerangX.SyncEngine.AzureML
{
    /// <summary>
    /// Utility class holding the result of import operation
    /// </summary>
    public class ImportReport
    {
        public string Info { get; set; }
        public int ErrorCount { get; set; }
        public int LineCount { get; set; }

        public override string ToString()
        {
            return string.Format("successfully imported {0}/{1} lines for {2}", LineCount - ErrorCount, LineCount,
                Info);
        }
    }
}
