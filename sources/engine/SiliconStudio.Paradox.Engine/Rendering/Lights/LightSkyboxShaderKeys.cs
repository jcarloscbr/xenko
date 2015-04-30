﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using SiliconStudio.Paradox.Shaders;

namespace SiliconStudio.Paradox.Rendering.Lights
{
    public partial class LightSkyboxShaderKeys
    {
        public static readonly ParameterKey<ShaderSource> LightDiffuseColor = ParameterKeys.New<ShaderSource>();
        public static readonly ParameterKey<ShaderSource> LightSpecularColor = ParameterKeys.New<ShaderSource>();
    }
}