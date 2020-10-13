using JetBrains.Annotations;

namespace GZipTest
{
    [PublicAPI]
    public enum VeeamErrorLevel : byte
    {
        None = 0,
        Warning = 1,
        Error = 2
    }
}
