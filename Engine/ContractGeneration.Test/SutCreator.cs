using System;

namespace ContractGeneration.Test
{
    public static class SutCreator
    {
        public static Contract Generate(params Type[] types) { return Contract.Generate(types); }
    }
}