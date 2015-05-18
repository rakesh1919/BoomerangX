using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomerangX.SyncEngine.AzureML
{
    /// <summary>
    /// represent the build status
    /// </summary>
    public enum ModelBuildStatus
    {
        Create,
        Queued,
        Building,
        Success,
        Error,
        Cancelling,
        Cancelled
    }
}
