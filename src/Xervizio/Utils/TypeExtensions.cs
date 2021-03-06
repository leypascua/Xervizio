﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Xervizio {

    internal class TypeNotFound : Type {

        public TypeNotFound(string typeName) {
            typeName.ShouldNotBeEmpty("typeName");
            RequestedTypeName = typeName;
        }

        public string RequestedTypeName { get; private set; }

        #region useless overrides

        public override System.Reflection.Assembly Assembly {
            get { throw new NotImplementedException(); }
        }

        public override string AssemblyQualifiedName {
            get { throw new NotImplementedException(); }
        }

        public override Type BaseType {
            get { throw new NotImplementedException(); }
        }

        public override string FullName {
            get { throw new NotImplementedException(); }
        }

        public override Guid GUID {
            get { throw new NotImplementedException(); }
        }

        protected override System.Reflection.TypeAttributes GetAttributeFlagsImpl() {
            throw new NotImplementedException();
        }

        protected override System.Reflection.ConstructorInfo GetConstructorImpl(System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Reflection.CallingConventions callConvention, Type[] types, System.Reflection.ParameterModifier[] modifiers) {
            throw new NotImplementedException();
        }

        public override System.Reflection.ConstructorInfo[] GetConstructors(System.Reflection.BindingFlags bindingAttr) {
            throw new NotImplementedException();
        }

        public override Type GetElementType() {
            throw new NotImplementedException();
        }

        public override System.Reflection.EventInfo GetEvent(string name, System.Reflection.BindingFlags bindingAttr) {
            throw new NotImplementedException();
        }

        public override System.Reflection.EventInfo[] GetEvents(System.Reflection.BindingFlags bindingAttr) {
            throw new NotImplementedException();
        }

        public override System.Reflection.FieldInfo GetField(string name, System.Reflection.BindingFlags bindingAttr) {
            throw new NotImplementedException();
        }

        public override System.Reflection.FieldInfo[] GetFields(System.Reflection.BindingFlags bindingAttr) {
            throw new NotImplementedException();
        }

        public override Type GetInterface(string name, bool ignoreCase) {
            throw new NotImplementedException();
        }

        public override Type[] GetInterfaces() {
            throw new NotImplementedException();
        }

        public override System.Reflection.MemberInfo[] GetMembers(System.Reflection.BindingFlags bindingAttr) {
            throw new NotImplementedException();
        }

        protected override System.Reflection.MethodInfo GetMethodImpl(string name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Reflection.CallingConventions callConvention, Type[] types, System.Reflection.ParameterModifier[] modifiers) {
            throw new NotImplementedException();
        }

        public override System.Reflection.MethodInfo[] GetMethods(System.Reflection.BindingFlags bindingAttr) {
            throw new NotImplementedException();
        }

        public override Type GetNestedType(string name, System.Reflection.BindingFlags bindingAttr) {
            throw new NotImplementedException();
        }

        public override Type[] GetNestedTypes(System.Reflection.BindingFlags bindingAttr) {
            throw new NotImplementedException();
        }

        public override System.Reflection.PropertyInfo[] GetProperties(System.Reflection.BindingFlags bindingAttr) {
            throw new NotImplementedException();
        }

        protected override System.Reflection.PropertyInfo GetPropertyImpl(string name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, Type returnType, Type[] types, System.Reflection.ParameterModifier[] modifiers) {
            throw new NotImplementedException();
        }

        protected override bool HasElementTypeImpl() {
            throw new NotImplementedException();
        }

        public override object InvokeMember(string name, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, object target, object[] args, System.Reflection.ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, string[] namedParameters) {
            throw new NotImplementedException();
        }

        protected override bool IsArrayImpl() {
            throw new NotImplementedException();
        }

        protected override bool IsByRefImpl() {
            throw new NotImplementedException();
        }

        protected override bool IsCOMObjectImpl() {
            throw new NotImplementedException();
        }

        protected override bool IsPointerImpl() {
            throw new NotImplementedException();
        }

        protected override bool IsPrimitiveImpl() {
            throw new NotImplementedException();
        }

        public override System.Reflection.Module Module {
            get { throw new NotImplementedException(); }
        }

        public override string Namespace {
            get { throw new NotImplementedException(); }
        }

        public override Type UnderlyingSystemType {
            get { throw new NotImplementedException(); }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(bool inherit) {
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit) {
            throw new NotImplementedException();
        }

        public override string Name {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }

#if !DEBUG
        [DebuggerStepThrough]
#endif
    public static class TypeExtensions {


        public static T CreateInstance<T>(this string typeName, params object[] constructorArgs) where T : class {
            var type = typeName.ToType();
            return type.CreateInstance<T>(constructorArgs);
        }

        public static Type ToType(this string typeName) {
            typeName.ShouldNotBeEmpty("typeName");
            var result = Type.GetType(typeName, false, true);
            return result ?? new TypeNotFound(typeName);
        }

        public static string ToQualifiedAssemblyName(this Type type) {
            string typeName = type.FullName;
            string assemblyName = type.Assembly.GetName().Name;

            return "{0}, {1}".WithTokens(typeName, assemblyName);
        }

        public static T CreateInstance<T>(this Type type, params object[] constructorArgs) where T : class {
            type.ShouldNotBeNull("type");

            var typeNotFound = type as TypeNotFound;
            var requestedType = typeNotFound == null ? "unknown" : typeNotFound.RequestedTypeName;

            Protect.AgainstInvalidOperation(typeNotFound != null
                , "The type {0} cannot be found or is not loaded."
                .WithTokens(requestedType));

            return InstanceHelper.CreateObjectInstance(type, constructorArgs) as T;
        }

        public static int ToInt(this Guid value) {

            //extract an integer from the beginning of the Guid
            byte[] _bytes = value.ToByteArray();
            int i = ((int)_bytes[0]) | ((int)_bytes[1] << 8) | ((int)_bytes[2] << 16) | ((int)_bytes[3] << 24);

            return i;
        }
    }
}
