using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Newtonsoft.Json.Serialization;

namespace vtb.Utils.Extensions
{
    internal sealed class PatchableObjectAdapterFactory : AdapterFactory
    {
        public override IAdapter Create(object target, IContractResolver contractResolver)
        {
            var adapter = base.Create(target, contractResolver);
            if (adapter is PocoAdapter)
                adapter = new PatchablePocoAdapter();

            return adapter;
        }
    }
}