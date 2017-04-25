/**
 * This file is part of JLUtils library
 * (C) Jeremy Lam "JLChnToZ" 2016-2017.
 * Released under MIT License.
 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace Utils {
    /// <summary>
    /// Helpers and extensions for comparisions.
    /// </summary>
    public static class Compare {
        #region Comparison to comparer
        /// <summary>
        /// Wraps a <see cref="Comparison{T}"/> into a <see cref="IComparer{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comparison">The comparison delegate to be wrapped.</param>
        /// <returns>A new comparer object witch wrapped the comparison delegate in it.</returns>
        public static Comparer<T> AsComparer<T>(this Comparison<T> comparison) {
            if(comparison == null)
                throw new ArgumentNullException("comparison");
            return new ComparisonComparer<T>(comparison);
        }

        private class ComparisonComparer<T>: Comparer<T> {
            private readonly Comparison<T> comparison;

            public ComparisonComparer(Comparison<T> comparison) {
                this.comparison = comparison;
            }

            public override int Compare(T x, T y) {
                return comparison.Invoke(x, y);
            }
        }

        #endregion

        #region In
        /// <summary>
        /// Check if a object is in the set of values provided.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">The object have to checked.</param>
        /// <param name="check">The set of values to check if the target is in it.</param>
        /// <returns><c>true</c> if the object is in the set of values, otherwise, <c>false</c>.</returns>
        public static bool In<T>(this T target, params T[] check) {
            return In(target, null, check as IEnumerable<T>);
        }

        public static bool In<T>(this T target, T check1, T check2) {
            var comparer = EqualityComparer<T>.Default;
            return comparer.Equals(target, check1) ||
                comparer.Equals(target, check2);
        }

        public static bool In<T>(this T target, T check1, T check2, T check3) {
            var comparer = EqualityComparer<T>.Default;
            return comparer.Equals(target, check1) ||
                comparer.Equals(target, check2) ||
                comparer.Equals(target, check3);
        }

        public static bool In<T>(this T target, T check1, T check2, T check3, T check4) {
            var comparer = EqualityComparer<T>.Default;
            return comparer.Equals(target, check1) ||
                comparer.Equals(target, check2) ||
                comparer.Equals(target, check3) ||
                comparer.Equals(target, check4);
        }

        /// <summary>
        /// Check if a object is in the set of values provided.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">The object have to checked.</param>
        /// <param name="comparer">The comparer to be used to check the equality,
        /// if <c>null</c> is passed, default comparer will be used.</param>
        /// <param name="check">The set of values to check if the target is in it.</param>
        /// <returns><c>true</c> if the object is in the set of values, otherwise, <c>false</c>.</returns>
        public static bool In<T>(this T target, IEqualityComparer<T> comparer, params T[] check) {
            return In(target, comparer, check as IEnumerable<T>);
        }

        /// <summary>
        /// Check if a object is in the set of values provided.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">The object have to checked.</param>
        /// <param name="comparer">The comparer to be used to check the equality,
        /// if <c>null</c> is passed, default comparer will be used.</param>
        /// <param name="check">The set of values to check if the target is in it.</param>
        /// <returns><c>true</c> if the object is in the set of values, otherwise, <c>false</c>.</returns>
        public static bool In<T>(this T target, IEqualityComparer<T> comparer, IEnumerable<T> check) {
            if(check == null) return false;
            comparer = comparer ?? EqualityComparer<T>.Default;
            foreach(T item in check)
                if(comparer.Equals(item, target))
                    return true;
            return false;
        }

        #endregion

        #region Between
        /// <summary>
        /// Check a value if it is between min (exclusive) and max (exclusive).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The smaller one to compare.</param>
        /// <param name="max">The bigger one to compare.</param>
        /// <returns>
        /// <c>true</c> if the value is between min and max, otherwise, <c>false</c>.
        /// </returns>
        public static bool Between<T>(this T value, T min, T max) where T : IComparable<T> {
            return Between(null, value, min, max);
        }

        /// <summary>
        /// Check a value if it is between min (exclusive) and max (exclusive).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comparer">The comparer will be used to compare the values, if <c>null</c> is setted, default comparer will be used.</param>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The smaller one to compare.</param>
        /// <param name="max">The bigger one to compare.</param>
        /// <returns>
        /// <c>true</c> if the value is between min and max, otherwise, <c>false</c>.
        /// </returns>
        public static bool Between<T>(this IComparer<T> comparer, T value, T min, T max) {
            comparer = comparer ?? Comparer<T>.Default;
            return comparer.Compare(value, min) > 0 && comparer.Compare(value, max) < 0;
        }

        /// <summary>
        /// Check a value if it is between min (exclusive) and max (exclusive).
        /// </summary>
        /// <param name="comparer">The comparer will be used to compare the values, if <c>null</c> is setted, default comparer will be used.</param>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The smaller one to compare.</param>
        /// <param name="max">The bigger one to compare.</param>
        /// <returns>
        /// <c>true</c> if the value is between min and max, otherwise, <c>false</c>.
        /// </returns>
        public static bool Between(this IComparer comparer, object value, object min, object max) {
            comparer = comparer ?? Comparer.Default;
            return comparer.Compare(value, min) > 0 && comparer.Compare(value, max) < 0;
        }

        /// <summary>
        /// Check a value if it is between min (inclusive) and max (inclusive).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The smaller one to compare.</param>
        /// <param name="max">The bigger one to compare.</param>
        /// <returns>
        /// <c>true</c> if the value is between min and max, otherwise, <c>false</c>.
        /// </returns>
        public static bool InBetween<T>(this T value, T min, T max) where T : IComparable<T> {
            return InBetween(null, value, min, max);
        }

        /// <summary>
        /// Check a value if it is between min (inclusive) and max (inclusive).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comparer">The comparer will be used to compare the values,
        /// if <c>null</c> is passed, default comparer will be used.</param>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The smaller one to compare.</param>
        /// <param name="max">The bigger one to compare.</param>
        /// <returns>
        /// <c>true</c> if the value is between min and max, otherwise, <c>false</c>.
        /// </returns>
        public static bool InBetween<T>(this IComparer<T> comparer, T value, T min, T max) {
            comparer = comparer ?? Comparer<T>.Default;
            return comparer.Compare(value, min) >= 0 && comparer.Compare(value, max) <= 0;
        }

        /// <summary>
        /// Check a value if it is between min (inclusive) and max (inclusive).
        /// </summary>
        /// <param name="comparer">The comparer will be used to compare the values,
        /// if <c>null</c> is passed, default comparer will be used.</param>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The smaller one to compare.</param>
        /// <param name="max">The bigger one to compare.</param>
        /// <returns>
        /// <c>true</c> if the value is between min and max, otherwise, <c>false</c>.
        /// </returns>
        public static bool InBetween(this IComparer comparer, object value, object min, object max) {
            comparer = comparer ?? Comparer.Default;
            return comparer.Compare(value, min) >= 0 && comparer.Compare(value, max) <= 0;
        }

        /// <summary>
        /// Check a value if it is between min (exclusive) and max (inclusive).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The smaller one to compare.</param>
        /// <param name="max">The bigger one to compare.</param>
        /// <returns>
        /// <c>true</c> if the value is between min and max, otherwise, <c>false</c>.
        /// </returns>
        public static bool InBetweenMax<T>(this T value, T min, T max) where T : IComparable<T> {
            return InBetweenMax(null, value, min, max);
        }

        /// <summary>
        /// Check a value if it is between min (exclusive) and max (inclusive).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comparer">The comparer will be used to compare the values,
        /// if <c>null</c> is passed, default comparer will be used.</param>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The smaller one to compare.</param>
        /// <param name="max">The bigger one to compare.</param>
        /// <returns>
        /// <c>true</c> if the value is between min and max, otherwise, <c>false</c>.
        /// </returns>
        public static bool InBetweenMax<T>(this IComparer<T> comparer, T value, T min, T max) {
            comparer = comparer ?? Comparer<T>.Default;
            return comparer.Compare(value, min) > 0 && comparer.Compare(value, max) <= 0;
        }

        /// <summary>
        /// Check a value if it is between min (exclusive) and max (inclusive).
        /// </summary>
        /// <param name="comparer">The comparer will be used to compare the values,
        /// if <c>null</c> is passed, default comparer will be used.</param>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The smaller one to compare.</param>
        /// <param name="max">The bigger one to compare.</param>
        /// <returns>
        /// <c>true</c> if the value is between min and max, otherwise, <c>false</c>.
        /// </returns>
        public static bool InBetweenMax(this IComparer comparer, object value, object min, object max) {
            comparer = comparer ?? Comparer.Default;
            return comparer.Compare(value, min) > 0 && comparer.Compare(value, max) <= 0;
        }

        /// <summary>
        /// Check a value if it is between min (inclusive) and max (exclusive).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The smaller one to compare.</param>
        /// <param name="max">The bigger one to compare.</param>
        /// <returns>
        /// <c>true</c> if the value is between min and max, otherwise, <c>false</c>.
        /// </returns>
        public static bool InBetweenMin<T>(this T value, T min, T max) where T : IComparable<T> {
            return InBetweenMin(null, value, min, max);
        }

        /// <summary>
        /// Check a value if it is between min (inclusive) and max (exclusive).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comparer">The comparer will be used to compare the values,
        /// if <c>null</c> is passed, default comparer will be used.</param>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The smaller one to compare.</param>
        /// <param name="max">The bigger one to compare.</param>
        /// <returns>
        /// <c>true</c> if the value is between min and max, otherwise, <c>false</c>.
        /// </returns>
        public static bool InBetweenMin<T>(this IComparer<T> comparer, T value, T min, T max) {
            comparer = comparer ?? Comparer<T>.Default;
            return comparer.Compare(value, min) >= 0 && comparer.Compare(value, max) < 0;
        }

        /// <summary>
        /// Check a value if it is between min (inclusive) and max (exclusive).
        /// </summary>
        /// <param name="comparer">The comparer will be used to compare the values,
        /// if <c>null</c> is passed, default comparer will be used.</param>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The smaller one to compare.</param>
        /// <param name="max">The bigger one to compare.</param>
        /// <returns>
        /// <c>true</c> if the value is between min and max, otherwise, <c>false</c>.
        /// </returns>
        public static bool InBetweenMin(this IComparer comparer, object value, object min, object max) {
            comparer = comparer ?? Comparer.Default;
            return comparer.Compare(value, min) >= 0 && comparer.Compare(value, max) < 0;
        }

        #endregion

        #region Clamp
        /// <summary>
        /// Clamps the value between min and max.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The smaller one.</param>
        /// <param name="max">The bigger one.</param>
        /// <returns>The value which does not bigger than the max and
        /// not smaller than the min.</returns>
        public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T> {
            return Clamp(null, value, min, max);
        }

        /// <summary>
        /// Clamps the value between min and max.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comparer">The comparer to be used to check the equality,
        /// if <c>null</c> is passed, default comparer will be used.</param>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The smaller one.</param>
        /// <param name="max">The bigger one.</param>
        /// <returns>The value which does not bigger than the max and
        /// not smaller than the min.</returns>
        public static T Clamp<T>(this IComparer<T> comparer, T value, T min, T max) {
            comparer = comparer ?? Comparer<T>.Default;
            if(comparer.Compare(value, min) < 0) return min;
            if(comparer.Compare(value, max) > 0) return max;
            return value;
        }

        /// <summary>
        /// Clamps the value between min and max.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The smaller one.</param>
        /// <param name="max">The bigger one.</param>
        /// <returns>The value which does not bigger than the max and
        /// not smaller than the min.</returns>
        public static object Clamp(object value, object min, object max) {
            return Clamp(null as IComparer, value, min, max);
        }

        /// <summary>
        /// Clamps the value between min and max.
        /// </summary>
        /// <param name="comparer">The comparer to be used to check the equality,
        /// if <c>null</c> is passed, default comparer will be used.</param>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The smaller one.</param>
        /// <param name="max">The bigger one.</param>
        /// <returns>The value which does not bigger than the max and
        /// not smaller than the min.</returns>
        public static object Clamp(this IComparer comparer, object value, object min, object max) {
            comparer = comparer ?? Comparer.Default;
            if(comparer.Compare(value, min) < 0) return min;
            if(comparer.Compare(value, max) > 0) return max;
            return value;
        }

        /// <summary>
        /// Gets the bigger value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>The bigger one.</returns>
        public static T Max<T>(T x, T y) where T : IComparable<T> {
            return Comparer<T>.Default.Compare(x, y) > 0 ? x : y;
        }

        /// <summary>
        /// Gets the bigger value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comparer">The comparer to be used to check the equality,
        /// if <c>null</c> is passed, default comparer will be used.</param>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>The bigger one.</returns>
        public static T Max<T>(this IComparer<T> comparer, T x, T y) {
            return (comparer ?? Comparer<T>.Default).Compare(x, y) > 0 ? x : y;
        }

        /// <summary>
        /// Gets the biggest value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">The values to compare.</param>
        /// <returns>The biggest one.</returns>
        public static T Max<T>(params T[] values) where T : IComparable<T> {
            return Max<T>(null, values as IEnumerable<T>);
        }

        /// <summary>
        /// Gets the biggest value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comparer">The comparer to be used to check the equality,
        /// if <c>null</c> is passed, default comparer will be used.</param>
        /// <param name="values">The the values to compare.</param>
        /// <returns>The biggest one.</returns>
        public static T Max<T>(this IComparer<T> comparer, params T[] values) {
            return Max(comparer, values as IEnumerable<T>);
        }

        /// <summary>
        /// Gets the biggest value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comparer">The comparer to be used to check the equality,
        /// if <c>null</c> is passed, default comparer will be used.</param>
        /// <param name="values">The the values to compare.</param>
        /// <returns>The biggest one.</returns>
        public static T Max<T>(this IComparer<T> comparer, IEnumerable<T> values) {
            if(values == null) throw new ArgumentNullException("values");
            comparer = comparer ?? Comparer<T>.Default;
            T result = default(T);
            bool hasResult = false;
            foreach(T current in values) {
                if(hasResult) {
                    if(comparer.Compare(current, result) > 0)
                        result = current;
                } else {
                    result = current;
                    hasResult = true;
                }
            }
            if(!hasResult)
                throw new InvalidOperationException("Maximum value not found");
            return result;
        }

        /// <summary>
        /// Gets the bigger value.
        /// </summary>
        /// <param name="comparer">The comparer to be used to check the equality,
        /// if <c>null</c> is passed, default comparer will be used.</param>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>The bigger one.</returns>
        public static object Max(this IComparer comparer, object x, object y) {
            return (comparer ?? Comparer.Default).Compare(x, y) > 0 ? x : y;
        }

        /// <summary>
        /// Gets the biggest value.
        /// </summary>
        /// <param name="comparer">The comparer to be used to check the equality,
        /// if <c>null</c> is passed, default comparer will be used.</param>
        /// <param name="values">The the values to compare.</param>
        /// <returns>The biggest one.</returns>
        public static object Max(this IComparer comparer, params object[] values) {
            return Max(comparer, values as IEnumerable);
        }

        /// <summary>
        /// Gets the biggest value.
        /// </summary>
        /// <param name="comparer">The comparer to be used to check the equality,
        /// if <c>null</c> is passed, default comparer will be used.</param>
        /// <param name="values">The the values to compare.</param>
        /// <returns>The biggest one.</returns>
        public static object Max(this IComparer comparer, IEnumerable values) {
            if(values == null) throw new ArgumentNullException("values");
            comparer = comparer ?? Comparer.Default;
            object result = null;
            foreach(object current in values)
                if(current != null && (result == null || comparer.Compare(current, result) > 0))
                    result = current;
            return result;
        }

        /// <summary>
        /// Gets the smaller value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>The smaller one.</returns>
        public static T Min<T>(T x, T y) where T : IComparable<T> {
            return Comparer<T>.Default.Compare(x, y) < 0 ? x : y;
        }

        /// <summary>
        /// Gets the smaller value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comparer">The comparer to be used to check the equality,
        /// if <c>null</c> is passed, default comparer will be used.</param>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>The smaller one.</returns>
        public static T Min<T>(this IComparer<T> comparer, T x, T y) {
            return (comparer ?? Comparer<T>.Default).Compare(x, y) < 0 ? x : y;
        }

        /// <summary>
        /// Gets the smallest value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">The the values to compare.</param>
        /// <returns>The smallest one.</returns>
        public static T Min<T>(params T[] values) where T : IComparable<T> {
            return Min<T>(null, values as IEnumerable<T>);
        }

        /// <summary>
        /// Gets the smallest value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comparer">The comparer to be used to check the equality,
        /// if <c>null</c> is passed, default comparer will be used.</param>
        /// <param name="values">The values.</param>
        /// <returns>The smallest one.</returns>
        public static T Min<T>(this IComparer<T> comparer, params T[] values) {
            return Min(comparer, values as IEnumerable<T>);
        }

        /// <summary>
        /// Gets the smallest value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comparer">The comparer to be used to check the equality,
        /// if <c>null</c> is passed, default comparer will be used.</param>
        /// <param name="values">The values.</param>
        /// <returns>The smallest one.</returns>
        public static T Min<T>(this IComparer<T> comparer, IEnumerable<T> values) {
            if(values == null) throw new ArgumentNullException("values");
            comparer = comparer ?? Comparer<T>.Default;
            T result = default(T);
            bool hasResult = false;
            foreach(T current in values) {
                if(hasResult) {
                    if(comparer.Compare(current, result) < 0)
                        result = current;
                } else {
                    result = current;
                    hasResult = true;
                }
            }
            if(!hasResult)
                throw new InvalidOperationException("Minimum value not found");
            return result;
        }

        /// <summary>
        /// Gets the smaller value.
        /// </summary>
        /// <param name="comparer">The comparer to be used to check the equality,
        /// if <c>null</c> is passed, default comparer will be used.</param>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>The smaller one.</returns>
        public static object Min(this IComparer comparer, object x, object y) {
            return (comparer ?? Comparer.Default).Compare(x, y) < 0 ? x : y;
        }

        /// <summary>
        /// Gets the smallest value.
        /// </summary>
        /// <param name="comparer">The comparer to be used to check the equality,
        /// if <c>null</c> is passed, default comparer will be used.</param>
        /// <param name="values">The values.</param>
        /// <returns>The smallest one.</returns>
        public static object Min(this IComparer comparer, params object[] values) {
            return Min(comparer, values as IEnumerable);
        }

        /// <summary>
        /// Gets the smallest value.
        /// </summary>
        /// <param name="comparer">The comparer to be used to check the equality,
        /// if <c>null</c> is passed, default comparer will be used.</param>
        /// <param name="values">The values.</param>
        /// <returns>The smallest one.</returns>
        public static object Min(this IComparer comparer, IEnumerable values) {
            if(values == null) throw new ArgumentNullException("values");
            comparer = comparer ?? Comparer.Default;
            object result = null;
            foreach(object current in values)
                if(current != null && (result == null || comparer.Compare(current, result) < 0))
                    result = current;
            return result;
        }

        #endregion
    }
}
