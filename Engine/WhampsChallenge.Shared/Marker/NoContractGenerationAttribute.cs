using System;

namespace WhampsChallenge.Shared.Marker
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Enum)]
    public class NoContractGenerationAttribute : Attribute
    {
    }
}