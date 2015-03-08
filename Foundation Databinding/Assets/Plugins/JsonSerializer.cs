using System;

namespace FullSerializer
{
    /// <summary>
    /// Static instance for easy serializing
    /// </summary>
    public static class JsonSerializer
    {
        /// <summary>
        /// Static Serializer Instance
        /// </summary>
        public static readonly fsSerializer Internal = new fsSerializer();

        /// <summary>
        /// Serializes the object into json
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string Serialize(object value, Type type)
        {
            // serialize the data
            fsData data;
            Internal.TrySerialize(type, value, out data).AssertSuccessWithoutWarnings();

            // emit the data via JSON
            return fsJsonPrinter.CompressedJson(data);
        }

        /// <summary>
        /// Serializes the object into json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="prettyJson"></param>
        /// <returns></returns>
        public static string Serialize<T>(T value, bool prettyJson = false)
        {
            fsData data;
            fsResult r = Internal.TrySerialize<T>(value, out data);

            if (r.Failed)
                throw r.AsException;

            if (!prettyJson)
                return fsJsonPrinter.CompressedJson(data);

            return fsJsonPrinter.PrettyJson(data);
        }

        /// <summary>
        /// Deserializes the Json into an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            fsData data;
            fsResult fsFailure1 = fsJsonParser.Parse(json, out data);
            if (fsFailure1.Failed)
                throw fsFailure1.AsException;

            T instance = default(T);

            fsResult fsFailure2 = Internal.TryDeserialize<T>(data, ref instance);

            if (fsFailure2.Failed)
                throw fsFailure2.AsException;

            return instance;
        }

        /// <summary>
        /// Deserializes the Json into an object
        /// </summary>
        /// <param name="json"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Deserialize(string json, Type type)
        {
            // step 1: parse the JSON data
            fsData data = fsJsonParser.Parse(json);

            // step 2: deserialize the data
            object deserialized = null;
            Internal.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();

            return deserialized;
        }
    }
}