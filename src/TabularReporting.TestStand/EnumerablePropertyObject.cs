using NationalInstruments.TestStand.Interop.API;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TabularReporting.TestStand
{
    /// <summary>
    /// Decorator which extends PropertyObject by IEnumerable&lt;PropertyObject&gt;.
    /// </summary>
    public class EnumerablePropertyObject : PropertyObject, IEnumerable<EnumerablePropertyObject>
    {
        readonly PropertyObject _propObj;

        public EnumerablePropertyObject(PropertyObject propObj)
        {
            _propObj = propObj ?? throw new ArgumentNullException(nameof(propObj));
        }

        #region Auto-implemented PropertyObject

        public double GetValNumber(string lookupString, int options)
        {
            return _propObj.GetValNumber(lookupString, options);
        }

        public void SetValNumber(string lookupString, int options, double newValue)
        {
            _propObj.SetValNumber(lookupString, options, newValue);
        }

        public bool GetValBoolean(string lookupString, int options)
        {
            return _propObj.GetValBoolean(lookupString, options);
        }

        public void SetValBoolean(string lookupString, int options, bool newValue)
        {
            _propObj.SetValBoolean(lookupString, options, newValue);
        }

        public string GetValString(string lookupString, int options)
        {
            return _propObj.GetValString(lookupString, options);
        }

        public void SetValString(string lookupString, int options, string newValue)
        {
            _propObj.SetValString(lookupString, options, newValue);
        }

        public PropertyObject GetPropertyObject(string lookupString, int options)
        {
            return _propObj.GetPropertyObject(lookupString, options);
        }

        public void SetPropertyObject(string lookupString, int options, PropertyObject newValue)
        {
            _propObj.SetPropertyObject(lookupString, options, newValue);
        }

        public object GetValVariant(string lookupString, int options)
        {
            return _propObj.GetValVariant(lookupString, options);
        }

        public void SetValVariant(string lookupString, int options, object newValue)
        {
            _propObj.SetValVariant(lookupString, options, newValue);
        }

        public object GetValIDispatch(string lookupString, int options)
        {
            return _propObj.GetValIDispatch(lookupString, options);
        }

        public void SetValIDispatch(string lookupString, int options, object newValue)
        {
            _propObj.SetValIDispatch(lookupString, options, newValue);
        }

        public object GetValInterface(string lookupString, int options)
        {
            return _propObj.GetValInterface(lookupString, options);
        }

        public void SetValInterface(string lookupString, int options, object newValue)
        {
            _propObj.SetValInterface(lookupString, options, newValue);
        }

        public int GetFlags(string lookupString, int options)
        {
            return _propObj.GetFlags(lookupString, options);
        }

        public void SetFlags(string lookupString, int options, int Flags)
        {
            _propObj.SetFlags(lookupString, options, Flags);
        }

        public PropertyValueTypes GetType(string lookupString, int options, out bool IsObject, out bool IsArray, out string typeNameParam)
        {
            return _propObj.GetType(lookupString, options, out IsObject, out IsArray, out typeNameParam);
        }

        public PropertyObject GetTypeDefinition(string lookupString, int options)
        {
            return _propObj.GetTypeDefinition(lookupString, options);
        }

        public void GetDimensions(string lookupString, int options, out string lowerBounds, out string upperBounds, out int numElements, out PropertyValueTypes ElementType)
        {
            _propObj.GetDimensions(lookupString, options, out lowerBounds, out upperBounds, out numElements, out ElementType);
        }

        public void SetDimensions(string lookupString, int options, string lowerBounds, string upperBounds)
        {
            _propObj.SetDimensions(lookupString, options, lowerBounds, upperBounds);
        }

        public string GetArrayIndex(string lookupString, int options, int arrayOffset)
        {
            return _propObj.GetArrayIndex(lookupString, options, arrayOffset);
        }

        public int GetArrayOffset(string lookupString, int options, string arrayIndex)
        {
            return _propObj.GetArrayOffset(lookupString, options, arrayIndex);
        }

        public void NewSubProperty(string lookupString, PropertyValueTypes ValueType, bool asArray, string typeNameParam, int options)
        {
            _propObj.NewSubProperty(lookupString, ValueType, asArray, typeNameParam, options);
        }

        public void DeleteSubProperty(string lookupString, int options)
        {
            _propObj.DeleteSubProperty(lookupString, options);
        }

        public int GetNumSubProperties(string lookupString)
        {
            return _propObj.GetNumSubProperties(lookupString);
        }

        public string GetNthSubPropertyName(string lookupString, int index, int options)
        {
            return _propObj.GetNthSubPropertyName(lookupString, index, options);
        }

        public void SetNthSubPropertyName(string lookupString, int index, int options, string newValue)
        {
            _propObj.SetNthSubPropertyName(lookupString, index, options, newValue);
        }

        public bool Exists(string lookupString, int options)
        {
            return _propObj.Exists(lookupString, options);
        }

        public PropertyObject Clone(string lookupString, int options)
        {
            return _propObj.Clone(lookupString, options);
        }

        public PropertyObject Evaluate(string exprString)
        {
            return _propObj.Evaluate(exprString);
        }

        public void Write(string pathString, string objectName, int RWoptions)
        {
            _propObj.Write(pathString, objectName, RWoptions);
        }

        public void Read(string pathString, string objectName, int RWoptions)
        {
            _propObj.Read(pathString, objectName, RWoptions);
        }

        public void Serialize(ref string stream, string objectName, int RWoptions)
        {
            _propObj.Serialize(ref stream, objectName, RWoptions);
        }

        public void Unserialize(string stream, string objectName, int RWoptions)
        {
            _propObj.Unserialize(stream, objectName, RWoptions);
        }

        public PropertyObject EvaluateEx(string exprString, int EvaluationOptions)
        {
            return _propObj.EvaluateEx(exprString, EvaluationOptions);
        }

        public string ValidateNewName(string newName, bool allowDuplicates, out bool isValid)
        {
            return _propObj.ValidateNewName(newName, allowDuplicates, out isValid);
        }

        public int DisplayPropertiesDialog(string dlgTitle = "", PropertyObjectFile file = null, int dlgOptions = 0)
        {
            return _propObj.DisplayPropertiesDialog(dlgTitle, file, dlgOptions);
        }

        public int GetSubPropertyIndex(string lookupString, int options, string propName)
        {
            return _propObj.GetSubPropertyIndex(lookupString, options, propName);
        }

        public void SetSubPropertyIndex(string lookupString, int options, string propName, int index)
        {
            _propObj.SetSubPropertyIndex(lookupString, options, propName, index);
        }

        public void InsertSubProperty(string lookupString, int options, int index, PropertyObject subProperty)
        {
            _propObj.InsertSubProperty(lookupString, options, index, subProperty);
        }

        public string GetTypeDisplayString(string lookupString, int options)
        {
            return _propObj.GetTypeDisplayString(lookupString, options);
        }

        public string GetFormattedValue(string lookupString, int options, string formatString = "", bool useValueFormatIfDefined = false, string separator = "")
        {
            return _propObj.GetFormattedValue(lookupString, options, formatString, useValueFormatIfDefined, separator);
        }

        public int GetNumElements()
        {
            return _propObj.GetNumElements();
        }

        public void SetNumElements(int numElements, int options = 0)
        {
            _propObj.SetNumElements(numElements, options);
        }

        public void DeleteElements(int arrayOffset, int numElements, int options = 0)
        {
            _propObj.DeleteElements(arrayOffset, numElements, options);
        }

        public double GetValNumberByOffset(int arrayOffset, int options)
        {
            return _propObj.GetValNumberByOffset(arrayOffset, options);
        }

        public void SetValNumberByOffset(int arrayOffset, int options, double newValue)
        {
            _propObj.SetValNumberByOffset(arrayOffset, options, newValue);
        }

        public bool GetValBooleanByOffset(int arrayOffset, int options)
        {
            return _propObj.GetValBooleanByOffset(arrayOffset, options);
        }

        public void SetValBooleanByOffset(int arrayOffset, int options, bool newValue)
        {
            _propObj.SetValBooleanByOffset(arrayOffset, options, newValue);
        }

        public string GetValStringByOffset(int arrayOffset, int options)
        {
            return _propObj.GetValStringByOffset(arrayOffset, options);
        }

        public void SetValStringByOffset(int arrayOffset, int options, string newValue)
        {
            _propObj.SetValStringByOffset(arrayOffset, options, newValue);
        }

        public PropertyObject GetPropertyObjectByOffset(int arrayOffset, int options)
        {
            return _propObj.GetPropertyObjectByOffset(arrayOffset, options);
        }

        public void SetPropertyObjectByOffset(int arrayOffset, int options, PropertyObject newValue)
        {
            _propObj.SetPropertyObjectByOffset(arrayOffset, options, newValue);
        }

        public object GetValVariantByOffset(int arrayOffset, int options)
        {
            return _propObj.GetValVariantByOffset(arrayOffset, options);
        }

        public void SetValVariantByOffset(int arrayOffset, int options, object newValue)
        {
            _propObj.SetValVariantByOffset(arrayOffset, options, newValue);
        }

        public object GetValIDispatchByOffset(int arrayOffset, int options)
        {
            return _propObj.GetValIDispatchByOffset(arrayOffset, options);
        }

        public void SetValIDispatchByOffset(int arrayOffset, int options, object newValue)
        {
            _propObj.SetValIDispatchByOffset(arrayOffset, options, newValue);
        }

        public object GetValInterfaceByOffset(int arrayOffset, int options)
        {
            return _propObj.GetValInterfaceByOffset(arrayOffset, options);
        }

        public void SetValInterfaceByOffset(int arrayOffset, int options, object newValue)
        {
            _propObj.SetValInterfaceByOffset(arrayOffset, options, newValue);
        }

        public int GetStructureChangeCount(string lookupString, int options)
        {
            return _propObj.GetStructureChangeCount(lookupString, options);
        }

        public int GetInstanceDefaultFlags(string lookupString, int options)
        {
            return _propObj.GetInstanceDefaultFlags(lookupString, options);
        }

        public void SetInstanceDefaultFlags(string lookupString, int options, int Flags)
        {
            _propObj.SetInstanceDefaultFlags(lookupString, options, Flags);
        }

        public bool ContainsTypeInstance(string lookupString, int options, string typeNameParam)
        {
            return _propObj.ContainsTypeInstance(lookupString, options, typeNameParam);
        }

        public bool IsEqualTo(PropertyObject objectToCompare, int options)
        {
            return _propObj.IsEqualTo(objectToCompare, options);
        }

        public bool IsAliasObject(string lookupString, int options)
        {
            return _propObj.IsAliasObject(lookupString, options);
        }

        public string GetLocation(PropertyObject topObject)
        {
            return _propObj.GetLocation(topObject);
        }

        public int DisplayFlagsDialog(string dlgTitle = "", int dlgOptions = 0)
        {
            return _propObj.DisplayFlagsDialog(dlgTitle, dlgOptions);
        }

        public bool DisplayArrayBoundsDialog(string dlgTitle = "", int dlgOptions = 0)
        {
            return _propObj.DisplayArrayBoundsDialog(dlgTitle, dlgOptions);
        }

        public void ReadEx(string pathString, string objectName, int RWoptions, TypeConflictHandlerTypes handlerType = TypeConflictHandlerTypes.ConflictHandler_Error)
        {
            _propObj.ReadEx(pathString, objectName, RWoptions, handlerType);
        }

        public void UnserializeEx(string stream, string objectName, int RWoptions, TypeConflictHandlerTypes handlerType = TypeConflictHandlerTypes.ConflictHandler_Error)
        {
            _propObj.UnserializeEx(stream, objectName, RWoptions, handlerType);
        }

        public int GetTypeFlags(string lookupString, int options)
        {
            return _propObj.GetTypeFlags(lookupString, options);
        }

        public void SetTypeFlags(string lookupString, int options, int Flags)
        {
            _propObj.SetTypeFlags(lookupString, options, Flags);
        }

        public int GetInstanceOverrideFlags(string lookupString, int options)
        {
            return _propObj.GetInstanceOverrideFlags(lookupString, options);
        }

        public void SetInstanceOverrideFlags(string lookupString, int options, int Flags)
        {
            _propObj.SetInstanceOverrideFlags(lookupString, options, Flags);
        }

        public void let_ArrayElementPrototype(PropertyObject val)
        {
            _propObj.let_ArrayElementPrototype(val);
        }

        public string ValidateNewSubPropertyName(string newName, bool allowDuplicates, out bool isValid)
        {
            return _propObj.ValidateNewSubPropertyName(newName, allowDuplicates, out isValid);
        }

        public string ValidateNewElementName(string newName, bool allowDuplicates, out bool isValid)
        {
            return _propObj.ValidateNewElementName(newName, allowDuplicates, out isValid);
        }

        public int GetInternalPtr(string engineId)
        {
            return _propObj.GetInternalPtr(engineId);
        }

        public string GetXML(int GenerationOptions, int InitialIndentation, string DefaultName = "", string formatString = "")
        {
            return _propObj.GetXML(GenerationOptions, InitialIndentation, DefaultName, formatString);
        }

        public SearchResults Search(string lookupString, string searchString, int SearchOptions, int filterOptions, int elementsToSearch, string[] limitToAdapters, string[] limitToNamedProps, string[] limitToPropsOfNamedTypes, string[] subpropLookupStringsToExclude)
        {
            return _propObj.Search(lookupString, searchString, SearchOptions, filterOptions, elementsToSearch, limitToAdapters, limitToNamedProps, limitToPropsOfNamedTypes, subpropLookupStringsToExclude);
        }

        public void GetDisplayNames(string lookupString, int options, out string propDisplayName, out string valueDisplayName)
        {
            _propObj.GetDisplayNames(lookupString, options, out propDisplayName, out valueDisplayName);
        }

        public void SetXML(string xmlStream, int reserved1, TypeConflictHandlerTypes reserved2 = TypeConflictHandlerTypes.ConflictHandler_Error)
        {
            _propObj.SetXML(xmlStream, reserved1, reserved2);
        }

        public byte[] GetValBinary(string lookupString, int options)
        {
            return _propObj.GetValBinary(lookupString, options);
        }

        public void SetValBinary(string lookupString, int options, byte[] newValue)
        {
            _propObj.SetValBinary(lookupString, options, newValue);
        }

        public PropertyObject GetNthSubProperty(string lookupString, int index, int options)
        {
            return _propObj.GetNthSubProperty(lookupString, index, options);
        }

        public void DeleteNthSubProperty(string lookupString, int index, int options)
        {
            _propObj.DeleteNthSubProperty(lookupString, index, options);
        }

        public bool DisplayAttributesDialog(string dlgTitle = "", PropertyObjectFile file = null, int dlgOptions = 0)
        {
            return _propObj.DisplayAttributesDialog(dlgTitle, file, dlgOptions);
        }

        public ReportSection CreateReportSection(int GenerationOptions, int InitialIndentation, string DefaultName = "", string formatString = "", string Format = "")
        {
            return _propObj.CreateReportSection(GenerationOptions, InitialIndentation, DefaultName, formatString, Format);
        }

        public long GetValInteger64(string lookupString, int options)
        {
            return _propObj.GetValInteger64(lookupString, options);
        }

        public void SetValInteger64(string lookupString, int options, long newValue)
        {
            _propObj.SetValInteger64(lookupString, options, newValue);
        }

        public long GetValInteger64ByOffset(int arrayOffset, int options)
        {
            return _propObj.GetValInteger64ByOffset(arrayOffset, options);
        }

        public void SetValInteger64ByOffset(int arrayOffset, int options, long newValue)
        {
            _propObj.SetValInteger64ByOffset(arrayOffset, options, newValue);
        }

        public ulong GetValUnsignedInteger64(string lookupString, int options)
        {
            return _propObj.GetValUnsignedInteger64(lookupString, options);
        }

        public void SetValUnsignedInteger64(string lookupString, int options, ulong newValue)
        {
            _propObj.SetValUnsignedInteger64(lookupString, options, newValue);
        }

        public ulong GetValUnsignedInteger64ByOffset(int arrayOffset, int options)
        {
            return _propObj.GetValUnsignedInteger64ByOffset(arrayOffset, options);
        }

        public void SetValUnsignedInteger64ByOffset(int arrayOffset, int options, ulong newValue)
        {
            _propObj.SetValUnsignedInteger64ByOffset(arrayOffset, options, newValue);
        }

        public bool DisplayEditNumericFormatDialog(string dlgTitle, out bool validFormat, int dlgOptions = 0)
        {
            return _propObj.DisplayEditNumericFormatDialog(dlgTitle, out validFormat, dlgOptions);
        }

        public void InsertElements(int arrayOffset, int numElements, int options = 0)
        {
            _propObj.InsertElements(arrayOffset, numElements, options);
        }

        public int GetTypeDefinitionProtection()
        {
            return _propObj.GetTypeDefinitionProtection();
        }

        public void SetTypeDefinitionProtection(int newValue, object passwordString)
        {
            _propObj.SetTypeDefinitionProtection(newValue, passwordString);
        }

        public void LockTypeDefinition()
        {
            _propObj.LockTypeDefinition();
        }

        public void UnlockTypeDefinition(string passwordString)
        {
            _propObj.UnlockTypeDefinition(passwordString);
        }

        public void ClearTypeDefinitionPasswordHistory()
        {
            _propObj.ClearTypeDefinitionPasswordHistory();
        }

        public PropertyObject[] GetSubProperties(string lookupString, int options)
        {
            return _propObj.GetSubProperties(lookupString, options);
        }

        public PropertyObject[] GetPropertyObjectElements(string lookupString, int options)
        {
            return _propObj.GetPropertyObjectElements(lookupString, options);
        }

        public object GetInternalPtrEx(string engineId)
        {
            return _propObj.GetInternalPtrEx(engineId);
        }

        public bool UpdateEnumerators(PropertyObject newValues)
        {
            return _propObj.UpdateEnumerators(newValues);
        }

        public bool EnumIsValid()
        {
            return _propObj.EnumIsValid();
        }

        public string GetValueDisplayName(string lookupString, int options)
        {
            return _propObj.GetValueDisplayName(lookupString, options);
        }

        public string Name { get => _propObj.Name; set => _propObj.Name = value; }
        public string Comment { get => _propObj.Comment; set => _propObj.Comment = value; }

        public PropertyObject Parent => _propObj.Parent;

        public bool IsRootTypeDefinition => _propObj.IsRootTypeDefinition;

        public bool CanAddSubProperty => _propObj.CanAddSubProperty;

        public bool IsTypeDefinition => _propObj.IsTypeDefinition;

        public TypeCategories TypeCategory => _propObj.TypeCategory;

        public string TypeVersion { get => _propObj.TypeVersion; set => _propObj.TypeVersion = value; }
        public string NumericFormat { get => _propObj.NumericFormat; set => _propObj.NumericFormat = value; }
        public PropertyObject ArrayElementPrototype { get => _propObj.ArrayElementPrototype; set => _propObj.ArrayElementPrototype = value; }

        public string TypeLastModified => _propObj.TypeLastModified;

        public string TypeMinimumTestStandVersion { get => _propObj.TypeMinimumTestStandVersion; set => _propObj.TypeMinimumTestStandVersion = value; }

        public PropertyObjectType Type => _propObj.Type;

        public bool IsModifiedType { get => _propObj.IsModifiedType; set => _propObj.IsModifiedType = value; }

        public PropertyObject Attributes => _propObj.Attributes;

        public bool HasAttributes => _propObj.HasAttributes;

        public PropertyObject TypeAttributes => _propObj.TypeAttributes;

        public bool HasTypeAttributes => _propObj.HasTypeAttributes;

        public bool TypeDefinitionLocked => _propObj.TypeDefinitionLocked;

        public PropertyObject Enumerators => _propObj.Enumerators;

        #endregion

        #region Iterator

        public IEnumerator<EnumerablePropertyObject> GetEnumerator()
        {
            if (!_propObj.IsArray()) // If PropertyObject is not an array do not iterate.
                yield break;

            int numElements = _propObj.GetNumElements();
            for (int i = 0; i < numElements; i++)
            {
                yield return new EnumerablePropertyObject(_propObj.GetPropertyObjectByOffset(i, PropertyOptions.PropOption_NoOptions));
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}