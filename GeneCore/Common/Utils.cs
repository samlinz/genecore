using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace GeneCore.Common
{
    /// <summary>
    ///     Various cross-cutting pure utility functions used by the library.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        ///     Perform deep copy of an object using serialization.
        /// </summary>
        /// <param name="obj">Object to be clones.</param>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <remarks>This is slow! Don't perform in a loop or often.</remarks>
        /// <returns>A clone of the passed in object with no identical references.</returns>
        [NotNull]
        public static T DeepCopy<T>(T obj) where T : class
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            String serialized = JsonConvert.SerializeObject(obj);
            T copy = JsonConvert.DeserializeObject<T>(serialized);

            return copy ?? throw new InvalidOperationException("Serialize-deserialize resulted in null");
        }

        public static Double Mean([NotNull] this IList<Int32> elements)
        {
            if (elements == null) throw new ArgumentNullException(nameof(elements));
            return (Double) elements.Sum() / elements.Count();
        }

        public static Double Mean([NotNull] this IList<Double> elements)
        {
            if (elements == null) throw new ArgumentNullException(nameof(elements));
            return elements.Sum() / elements.Count();
        }

        public static T ChooseRandom<T>([NotNull] this IList<T> list, [NotNull] Random rng)
        {
            #region Validation

            if (list == null) throw new ArgumentNullException(nameof(list));
            if (rng == null) throw new ArgumentNullException(nameof(rng));
            if (!list.Any()) throw new ArgumentException("Empty collection");

            #endregion

            return list[rng.Next(list.Count)];
        }
    }
}