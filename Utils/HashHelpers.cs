﻿/**
 * This file is part of JLUtils library
 * (C) Jeremy Lam "JLChnToZ" 2016-2017.
 * Released under MIT License.
 * 
 * This file contains modified source code from reference source
 * https://github.com/Microsoft/referencesource/blob/master/mscorlib/system/collections/hashtable.cs
 * Copyright (c) Microsoft Corporation
 */
using System;
using System.Collections.Generic;

namespace Utils {
    internal static class HashHelpers {
        public const int MaxPrimeArrayLength = 0x7FEFFFFD;
        public static readonly int[] primes = { 3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89,
            107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919, 1103, 1327, 1597,
            1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631,
            130363, 156437, 187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827,
            807403, 968897, 1162687, 1395263, 1674319, 2009191, 2411033, 2893249, 3471899,
            4166287, 4999559, 5999471, 7199369, 8639249, 10367101, 12440537, 14928671,
            17914409, 21497293, 25796759, 30956117, 37147349, 44576837, 53492207, 64190669,
            77028803, 92434613, 110921543, 133105859, 159727031, 191672443, 230006941,
            276008387, 331210079, 397452101, 476942527, 572331049, 686797261, 824156741,
            988988137, 1186785773, 1424142949, 1708971541, 2050765853, MaxPrimeArrayLength };
        private static readonly Comparer<int> intComparer = Comparer<int>.Default;

        public static int GetPrime(int min) {
            if(min < 0)
                throw new ArgumentException("Minimum cannot be negative.");
            return primes.FindClosestValue(min, true, 0, -1, intComparer, MaxPrimeArrayLength);
        }

        public static int ExpandPrime(int oldSize) {
            int newSize = 2 * oldSize;
            if((uint)newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize)
                return MaxPrimeArrayLength;
            return GetPrime(newSize);
        }
    }
}
