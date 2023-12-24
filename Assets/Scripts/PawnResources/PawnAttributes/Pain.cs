using System;
using System.Runtime.InteropServices;
using CareBoo.Serially;

namespace TagFighter.Resources
{
    [Guid("30A0DDC8-3FCF-496A-B578-81DBF333C7EC")]
    [Serializable]
    [ProvideSourceInfo]
    public sealed class Pain : Resource<PainUnit> { }
}
