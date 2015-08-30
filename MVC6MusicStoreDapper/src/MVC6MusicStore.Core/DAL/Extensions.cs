using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MVC6MusicStore.Core.DAL
{
    public static class Extensions
    {
        public static TValue GetValue<TValue>(this IDataRecord input, string column)
        {
            var result = default(TValue);
            int ordinal;
            try
            {
                ordinal = input.GetOrdinal(column);
            }
            catch (Exception inner)
            {
                throw new DataException(string.Format("The column '{0}' was not found in the record", column), inner);
            }

            if (!input.IsDBNull(ordinal))
            {
                var value = input.GetValue(ordinal);
                var typeofTValue = typeof(TValue);
                if (typeofTValue.IsEnum)
                {
                    return (TValue)Enum.Parse(typeofTValue, value.ToString());
                }

                if (typeofTValue.IsGenericType && typeofTValue.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var nullableConverter = new NullableConverter(typeofTValue);
                    return (TValue)nullableConverter.ConvertFrom(value);
                }

                var valueAsConvertible = value as IConvertible;
                if (valueAsConvertible == null)
                {
                    if (value is Guid && typeof(TValue) == typeof(string))
                    {
                        return (TValue)(object)value.ToString();
                    }

                    result = (TValue)value;
                }
                else
                {
                    result = (TValue)valueAsConvertible.ToType(typeof(TValue), null);
                }
            }

            return result;
        }
        
        public static void DoForCurrentResultSetAndMoveNext(this IDataReader reader, Action<IDataReader> action)
        {
            while (reader.Read())
            {
                action(reader);
            }

            reader.NextResult();
        }
        
        public static bool ShouldRetry(this Task task)
        {
            if (task.IsFaulted && task.Exception != null)
            {
                return
                    task.Exception.InnerExceptions
                        .Select(innerException => innerException as SqlException)
                        .Any(sqlException => sqlException != null && sqlException.Class >= 17 && sqlException.Class < 20);
            }

            return false;
        }
    }
}