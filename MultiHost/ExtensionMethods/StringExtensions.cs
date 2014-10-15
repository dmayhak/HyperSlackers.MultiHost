// Copyright (C) 2014 Del Mayhak
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HyperSlackers.MultiHost.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns true if the string is either null, or empty.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        [Pure]
        public static bool IsNullOrEmpty(this String value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Returns true if the string is null, empty, or contains only whitespace.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        [Pure]
        public static bool IsNullOrWhiteSpace(this String value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
    }
}
