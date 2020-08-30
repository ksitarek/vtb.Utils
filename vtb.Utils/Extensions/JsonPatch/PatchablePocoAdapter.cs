using System;
using System.Collections;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Newtonsoft.Json.Serialization;
using vtb.Utils.Attributes;

namespace vtb.Utils.Extensions
{
    internal sealed class PatchablePocoAdapter : PocoAdapter
    {
        public override bool TryTraverse(object target, string segment, IContractResolver contractResolver,
            out object value, out string errorMessage)
        {
            if (!base.TryTraverse(target, segment, contractResolver, out value, out errorMessage))
                return false;

            if (value is IList)
            {
                TryGetJsonProperty(target, contractResolver, segment, out var jsonProperty);
                if (!jsonProperty.Writable)
                    return TryAdd(target, segment, contractResolver, value, out errorMessage);
            }

            return true;
        }

        protected override bool TryGetJsonProperty(object target, IContractResolver contractResolver, string segment,
            out JsonProperty jsonProperty)
        {
            if (contractResolver.ResolveContract(target.GetType()) is JsonObjectContract jsonObjectContract)
            {
                var pocoProperty = jsonObjectContract
                    .Properties
                    .FirstOrDefault(p => string.Equals(p.PropertyName, segment, StringComparison.OrdinalIgnoreCase));

                if (pocoProperty != null)
                {
                    jsonProperty = pocoProperty;
                    if (jsonProperty.Writable && pocoProperty.AttributeProvider.GetAttributes(false)
                        .Any(a => a is ForbidPatchAttribute))
                        jsonProperty.Writable = false;

                    return true;
                }
            }

            jsonProperty = null;
            return false;
        }
    }
}