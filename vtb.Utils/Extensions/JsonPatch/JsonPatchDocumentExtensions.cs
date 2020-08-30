using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json.Serialization;

namespace vtb.Utils.Extensions
{
    public static class JsonPatchDocumentExtensions
    {
        public static void ApplyToIfPatchable<T>(this JsonPatchDocument<T> document, T @object)
            where T : class
        {
            var errorsList = new Dictionary<string, List<string>>();
            var logErrorAction = LogErrorAction(errorsList);

            var adapter = new PatchableObjectAdapter(new DefaultContractResolver(), logErrorAction);
            document.ApplyTo(@object, adapter, logErrorAction);

            if (errorsList.Any()) throw new UnprocessablePatchException(errorsList);
        }

        public static void ApplyToIfPatchable(this JsonPatchDocument document, object @object)
        {
            var errorsList = new Dictionary<string, List<string>>();
            var logErrorAction = LogErrorAction(errorsList);

            var adapter = new PatchableObjectAdapter(new DefaultContractResolver(), logErrorAction);
            document.ApplyTo(@object, adapter, logErrorAction);

            if (errorsList.Any()) throw new UnprocessablePatchException(errorsList);
        }

        private static Action<JsonPatchError> LogErrorAction(Dictionary<string, List<string>> errorsList)
        {
            return e =>
            {
                if (!errorsList.ContainsKey(e.Operation.path)) errorsList[e.Operation.path] = new List<string>();

                errorsList[e.Operation.path].Add(e.ErrorMessage);
            };
        }
    }
}