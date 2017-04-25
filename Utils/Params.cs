/**
 * This file is part of JLUtils library
 * (C) Jeremy Lam "JLChnToZ" 2016-2017.
 * Released under MIT License.
 */
using System;

namespace Utils {
    /// <summary>
    /// Alternative for <c>params</c> in method calls which intents
    /// to reuse created arrays up to 8 parameters.
    /// </summary>
    /// <typeparam name="T">Generic array type</typeparam>
    /// <example>
    /// // For example you have a method with this signature:
    /// public void Foo(params string[] bunchOfParameters);
    /// // You may call the method like this to prevent creating temporary arrays:
    /// Foo(Params&lt;string&gt;.Get("foo", "bar", "baz"));
    /// </example>
    /// <remarks>
    /// There has a limitation, it cannot be used inside a method which is same type and count of params
    /// in callstack as the child method call will overwrite the array content to contain the new passed
    /// parameters, in this case you have to clone it to a new array for temporary storage.
    /// Multithread is supported, parameters and array instances will not be shared between threads.
    /// </remarks>
    public static class Params<T> {
        #region Private
        [ThreadStatic] private static T[][] arrays = new T[9][];

        private static int GetNextPowerOf2(int x) {
            x--;
            x |= (x >> 1);
            x |= (x >> 2);
            x |= (x >> 4);
            x |= (x >> 8);
            x |= (x >> 16);
            return x + 1;
        }

        private static T[] GetArray(int length) {
            if(length < 0)
                throw new ArgumentOutOfRangeException("length");
            if(arrays.Length < length)
                Array.Resize(ref arrays, GetNextPowerOf2(length));
            if(arrays[length] == null)
                arrays[length] = new T[length];
            return arrays[length];
        }
        #endregion
        #region Public methods
        /// <summary>
        /// Get a 0 parameter array.
        /// </summary>
        /// <returns>An empty array</returns>
        public static T[] Get() {
            return GetArray(0);
        }

        /// <summary>
        /// Get a 1 parameter array filled with provided data.
        /// </summary>
        /// <returns>An array filled with parameter</returns>
        public static T[] Get(T param1) {
            T[] param = GetArray(1);
            param[0] = param1;
            return param;
        }

        /// <summary>
        /// Get a 2 parameters array filled with provided data.
        /// </summary>
        /// <returns>An array filled with parameters</returns>
        public static T[] Get(T param1, T param2) {
            T[] param = GetArray(2);
            param[0] = param1;
            param[1] = param2;
            return param;
        }

        /// <summary>
        /// Get a 3 parameters array filled with provided data.
        /// </summary>
        /// <returns>An array filled with parameters</returns>
        public static T[] Get(T param1, T param2, T param3) {
            T[] param = GetArray(3);
            param[0] = param1;
            param[1] = param2;
            param[2] = param3;
            return param;
        }

        /// <summary>
        /// Get a 4 parameters array filled with provided data.
        /// </summary>
        /// <returns>An array filled with parameters</returns>
        public static T[] Get(T param1, T param2, T param3, T param4) {
            T[] param = GetArray(4);
            param[0] = param1;
            param[1] = param2;
            param[2] = param3;
            param[3] = param4;
            return param;
        }

        /// <summary>
        /// Get a 5 parameters array filled with provided data.
        /// </summary>
        /// <returns>An array filled with parameters</returns>
        public static T[] Get(T param1, T param2, T param3, T param4, T param5) {
            T[] param = GetArray(5);
            param[0] = param1;
            param[1] = param2;
            param[2] = param3;
            param[3] = param4;
            param[4] = param5;
            return param;
        }

        /// <summary>
        /// Get a 6 parameters array filled with provided data.
        /// </summary>
        /// <returns>An array filled with parameters</returns>
        public static T[] Get(T param1, T param2, T param3, T param4, T param5, T param6) {
            T[] param = GetArray(6);
            param[0] = param1;
            param[1] = param2;
            param[2] = param3;
            param[3] = param4;
            param[4] = param5;
            param[5] = param6;
            return param;
        }

        /// <summary>
        /// Get a 7 parameters array filled with provided data.
        /// </summary>
        /// <returns>An array filled with parameters</returns>
        public static T[] Get(T param1, T param2, T param3, T param4, T param5, T param6, T param7) {
            T[] param = GetArray(7);
            param[0] = param1;
            param[1] = param2;
            param[2] = param3;
            param[3] = param4;
            param[4] = param5;
            param[5] = param6;
            param[6] = param7;
            return param;
        }

        /// <summary>
        /// Get a 8 parameters array filled with provided data.
        /// </summary>
        /// <returns>An array filled with parameters</returns>
        public static T[] Get(T param1, T param2, T param3, T param4, T param5, T param6, T param7, T param8) {
            T[] param = GetArray(8);
            param[0] = param1;
            param[1] = param2;
            param[2] = param3;
            param[3] = param4;
            param[4] = param5;
            param[5] = param6;
            param[6] = param7;
            param[7] = param8;
            return param;
        }

        /// <summary>
        /// Get a variable-length array filled with data provided.
        /// </summary>
        /// <returns>An array filled with parameters, which you passed in or auto constructed</returns>
        /// <remarks>
        /// This is just a fallback, there is no optimization within this overload.
        /// </remarks>
        public static T[] Get(params T[] paramN) {
            return paramN ?? GetArray(0);
        }
        #endregion
    }
}
