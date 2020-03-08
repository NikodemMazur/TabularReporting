using System.Runtime.InteropServices;
using System;
using NationalInstruments.TestStand.Interop.API;

namespace TabularReporting.TestStand
{
    internal static class PropertyObjectExtensions
    {
        /// <summary>
        /// Checks if <paramref name="source"/> contains Property Object at <paramref name="lookupString"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="lookupString"></param>
        /// <returns></returns>
        public static bool Has(this PropertyObject source, string lookupString)
        {
            try
            {
                source.GetPropertyObject(lookupString, PropertyOptions.PropOption_NoOptions); // test if PropObj pointed by LookupString exists
                return true;
            }
            catch (COMException ex)
            {
                if (ex.ErrorCode != -17306) // "-17306; Unknown variable or property name." - Property Object not found
                    throw; // rethrow the exception if it's different
                return false;
            }
        }

        /// <summary>
        /// Checks if location of <paramref name="source"/> matches <paramref name="lookupString"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="lookupString"></param>
        /// <returns></returns>
        public static bool MatchesLocation(this PropertyObject source, string lookupString)
        {
            if (source.Name == string.Empty)
                return false;
            return GetLocationUntilNullOrEmptyString(source) == lookupString;

            string GetLocationUntilNullOrEmptyString(PropertyObject propObj)
            {
                if (propObj == null)
                    throw new ArgumentNullException(nameof(propObj));
                PropertyObject parent = propObj.Parent;
                string parentName = parent?.Name ?? string.Empty;
                if (parent == null || parentName == string.Empty)
                    return propObj.Name;
                return GetLocationUntilNullOrEmptyString(propObj.Parent) + $".{propObj.Name}";
            }
        }

        public static bool IsArray(this PropertyObject source)
        {
            try // test if PropertyObject is an array
            {
                source.GetNumElements();
            }
            catch (COMException ex)
            {
                if (ex.ErrorCode != -17308) // "-17308; Specified value does not have the expected type." - so PropertyObject is not an array
                    throw; // rethrow the exception if it's different than -17308
                return false;
            }
            return true;
        }
    }
}