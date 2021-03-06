﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using SiliconStudio.Core;

namespace SiliconStudio.Xenko.Updater
{
    /// <summary>
    /// Defines how to set and get values from a field for the <see cref="UpdateEngine"/>.
    /// </summary>
    public abstract class UpdatableField : UpdatableMember
    {
        /// <summary>
        /// Offset of this field in its containing object.
        /// </summary>
        public int Offset;

        /// <summary>
        /// Size of this field.
        /// </summary>
        public int Size;

        /// <summary>
        /// Sets a non-blittable struct field (given in boxed form).
        /// </summary>
        /// <param name="obj">The container object.</param>
        /// <param name="data">The new value to unbox and set</param>
        public abstract void SetStruct(IntPtr obj, object data);

        /// <summary>
        /// Gets a reference object from a field.
        /// </summary>
        /// <param name="obj">The object encoded as a native pointer (<see cref="UpdateEngine"/> will make sure it is pinned).</param>
        /// <returns>The object value from the field.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetObject(IntPtr obj)
        {
#if IL
            // Note: IL is injected by UpdateEngineProcessor
            ldarg obj
            ldind.ref
            ret
#endif
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets a reference object from a field.
        /// </summary>
        /// <param name="obj">The object encoded as a native pointer (<see cref="UpdateEngine"/> will make sure it is pinned).</param>
        /// <param name="data">The object value to set.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetObject(IntPtr obj, object data)
        {
#if IL
            // Note: IL is injected by UpdateEngineProcessor
            ldarg obj
            ldarg data
            stind.ref
            ret
#endif
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets a blittable field (from its pointer).
        /// </summary>
        /// <param name="obj">The container object.</param>
        /// <param name="data">The struct data.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SetBlittable(IntPtr obj, IntPtr data)
        {
            Interop.memcpy((void*)obj, (void*)data, Size);
        }

        /// <summary>
        /// Internally used to know type of set operation to use.
        /// </summary>
        public UpdateOperationType GetSetOperationType()
        {
            if (MemberType.GetTypeInfo().IsValueType)
            {
                if (BlittableHelper.IsBlittable(MemberType))
                {
                    if (Size == 4)
                        return UpdateOperationType.ConditionalSetBlittableField4;
                    if (Size == 8)
                        return UpdateOperationType.ConditionalSetBlittableField8;
                    if (Size == 12)
                        return UpdateOperationType.ConditionalSetBlittableField12;
                    if (Size == 16)
                        return UpdateOperationType.ConditionalSetBlittableField16;

                    return UpdateOperationType.ConditionalSetBlittableField;
                }

                return UpdateOperationType.ConditionalSetStructField;
            }
            else
            {
                return UpdateOperationType.ConditionalSetObjectField;
            }
        }
    }
}