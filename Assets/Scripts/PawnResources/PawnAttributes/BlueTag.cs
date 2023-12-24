using System;
using System.Runtime.InteropServices;
using CareBoo.Serially;

namespace TagFighter.Resources
{
    [Guid("AECC1739-A0DF-4013-B430-22D2EB13F0FF")]
    [Serializable]
    [ProvideSourceInfo]
    public sealed class BlueTag : Resource<BlueTagUnit> { }
}
