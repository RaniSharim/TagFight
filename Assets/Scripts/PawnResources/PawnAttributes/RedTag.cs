using System;
using System.Runtime.InteropServices;
using CareBoo.Serially;

namespace TagFighter.Resources
{
    [Guid("7F412C19-18FE-476F-92E4-469FFAD0C5D4")]
    [Serializable]
    [ProvideSourceInfo]
    public sealed class RedTag : Resource<RedTagUnit> { }
}
