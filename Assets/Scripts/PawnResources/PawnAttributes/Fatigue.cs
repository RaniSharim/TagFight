using System;
using System.Runtime.InteropServices;
using CareBoo.Serially;

namespace TagFighter.Resources
{
    [Guid("59A26139-CA50-4E6F-8AC0-27DFB70CCB8B")]
    [Serializable]
    [ProvideSourceInfo]
    public sealed class Fatigue : Resource<FatigueUnit> { }
}
