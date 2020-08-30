using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace vtb.Utils.Extensions
{
    [Serializable]
    public class UnprocessablePatchException : Exception
    {
        public UnprocessablePatchException(Dictionary<string, List<string>> errorsList)
            : base("Model state errors prevented this request execution.")
        {
            ErrorsList = errorsList;
        }

        protected UnprocessablePatchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ErrorsList =
                (Dictionary<string, List<string>>) info.GetValue(nameof(ErrorsList),
                    typeof(Dictionary<string, List<string>>));
        }

        public Dictionary<string, List<string>> ErrorsList { get; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ErrorsList", ErrorsList, typeof(Dictionary<string, List<string>>));
        }
    }
}