using System;
using System.Runtime.InteropServices;
using CareBoo.Serially;

namespace TagFighter.Resources
{
    [Guid("DA13F1C1-4359-4EB9-B969-7DB50AFE6423")]
    [Serializable]
    [ProvideSourceInfo]
    public sealed class GreenTag : Resource<GreenTagUnit> { }
}
