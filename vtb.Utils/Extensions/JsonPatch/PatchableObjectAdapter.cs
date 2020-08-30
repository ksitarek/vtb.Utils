using System;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Newtonsoft.Json.Serialization;

namespace vtb.Utils.Extensions
{
    internal sealed class PatchableObjectAdapter : ObjectAdapter
    {
        public PatchableObjectAdapter(IContractResolver contractResolver, Action<JsonPatchError> logErrorAction)
            : base(contractResolver, logErrorAction, new PatchableObjectAdapterFactory())
        {
        }
    }
}