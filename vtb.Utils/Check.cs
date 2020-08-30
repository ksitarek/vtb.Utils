using System;
using System.Collections.Generic;

namespace vtb.Utils
{
    public static class Check
    {
        public static void NotEmpty(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(fieldName);
        }

        public static void NotEmpty(string[] values, string fieldName)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentException(fieldName);
        }

        public static void NotNull(object value, string fieldName)
        {
            if (value == null)
                throw new ArgumentNullException(fieldName);
        }

        public static void NotDefault(DateTime value, string fieldName)
        {
            if (value == default)
                throw new ArgumentException(fieldName);
        }

        public static void GuidNotEmpty(Guid? value, string fieldName)
        {
            if (value == null)
                throw new ArgumentNullException(fieldName);

            if (value == Guid.Empty)
                throw new ArgumentException(fieldName);
        }

        public static void EntityFound<T>(Guid id, T entity)
        {
            GuidNotEmpty(id, nameof(id));

            if (entity == null)
                throw new KeyNotFoundException($"Entity {typeof(T)}#{id} was not found.");
        }

        public static void GreaterThan(int val, int reference, string fieldName)
        {
            if (val <= reference)
                throw new ArgumentException(fieldName);
        }
    }
}