﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System;
using System.Collections.Generic;

using SiliconStudio.Paradox.Graphics;

namespace SiliconStudio.Paradox.Effects
{
    /// <summary>
    /// Used to detect parameters change for dynamic effect.
    /// </summary>
    internal class EffectParameterUpdaterDefinition : ParameterUpdaterDefinition
    {
        internal object[] SortedCompilationValues;

        internal int[] SortedCounters;

        internal int[] SortedLevels;

        internal ParameterCollection Parameters;

        public EffectParameterUpdaterDefinition(Effect effect, ParameterCollection usedParameters)
        {
            Initialize(effect, usedParameters);
        }

        public void Initialize(Effect effect, ParameterCollection usedParameters)
        {
            if (effect == null) throw new ArgumentNullException("effect");

            // TODO: Should we ignore various compiler keys such as CompilerParameters.GraphicsPlatformKey, CompilerParameters.GraphicsProfileKey and CompilerParameters.DebugKey?
            //       That was done previously in Effect.CompilerParameters
            // TODO: Should we clone usedParameters? Or somehow make sure it is immutable? (for now it uses the one straight from EffectCompiler, which might not be a good idea...)
            Parameters = usedParameters;
            var parameters = usedParameters;

            var internalValues = parameters.InternalValues;
            SortedKeys = new ParameterKey[internalValues.Count];
            SortedKeyHashes = new ulong[internalValues.Count];
            SortedCompilationValues = new object[internalValues.Count];
            SortedCounters = new int[internalValues.Count];

            for (int i = 0; i < internalValues.Count; ++i)
            {
                var internalValue = internalValues[i];

                SortedKeys[i] = internalValue.Key;
                SortedKeyHashes[i] = internalValue.Key.HashCode;
                SortedCompilationValues[i] = internalValue.Value.Object;
                SortedCounters[i] = internalValue.Value.Counter;
            }

            var keyMapping = new Dictionary<ParameterKey, int>();
            for (int i = 0; i < SortedKeys.Length; i++)
                keyMapping.Add(SortedKeys[i], i);
            Parameters.SetKeyMapping(keyMapping);
        }

        public void UpdateCounter(ParameterCollection parameters)
        {
            var internalValues = parameters.InternalValues;
            for (int i = 0; i < internalValues.Count; ++i)
            {
                SortedCounters[i] = internalValues[i].Value.Counter;
            }
        }
    }

    internal class EffectParameterUpdater : ParameterUpdater
    {
        public BoundInternalValue GetAtIndex(int index)
        {
            return InternalValues[index];
        }

        public bool HasChanged(EffectParameterUpdaterDefinition definition)
        {
            for (var i = 0; i < definition.SortedLevels.Length; ++i)
            {
                var kvp = GetAtIndex(i);

                // We can skip keys defined by the first level (which is the effect default parameters + key mapping)
                // TODO: Make sure this is a valid assumption in all cases
                if (kvp.DirtyCount == 0)
                    continue;

                if (definition.SortedLevels[i] == kvp.DirtyCount)
                {
                    if (definition.SortedCounters[i] != kvp.Value.Counter && !kvp.Value.ValueEquals(definition.SortedCompilationValues[i]))
                        return true;
                }
                else
                {
                    if (!kvp.Value.ValueEquals(definition.SortedCompilationValues[i]))
                        return true;
                }
            }
            return false;          
        }

        public void ComputeLevels(EffectParameterUpdaterDefinition definition)
        {
            var levels = definition.SortedLevels;
            if (levels == null || levels.Length != definition.SortedKeyHashes.Length)
            {
                levels = new int[definition.SortedKeyHashes.Length];
            }

            for (var i = 0; i < levels.Length; ++i)
            {
                levels[i] = GetAtIndex(i).DirtyCount;
            }

            definition.SortedLevels = levels;            
        }

        public void UpdateCounters(EffectParameterUpdaterDefinition definition)
        {
            for (var i = 0; i < definition.SortedLevels.Length; ++i)
            {
                var kvp = GetAtIndex(i);
                definition.SortedCounters[i] = kvp.Value.Counter;
            }
        }



    }
}
