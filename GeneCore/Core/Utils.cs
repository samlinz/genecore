using System;
using Newtonsoft.Json;

namespace GeneCore.Core {
    /// <summary>
    /// Various cross-cutting pure utility functions used by the library. 
    /// </summary>
    public static class Utils {
        
        /// <summary>
        /// Perform deep copy of an object using serialization.
        /// </summary>
        /// <param name="obj">Object to be clones.</param>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <remarks>This is slow! Don't perform in a loop or often.</remarks>
        /// <returns>A clone of the passed in object with no identical references.</returns>
        public static T DeepCopy<T>(T obj) {
            String serialized = JsonConvert.SerializeObject(obj);
            T copy = JsonConvert.DeserializeObject<T>(serialized);
            
            return copy;
        }
    }
}