// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
//
// Author: Kenneth James Pouncey

using System;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Graphics
{
    public sealed partial class SamplerStateCollection
    {
        private int _d3dDirty;

        private void PlatformSetSamplerState(int index)
        {
            _d3dDirty |= 1 << index;
        }

        private void PlatformClear()
        {
            _d3dDirty = int.MaxValue;
        }

        private void PlatformDirty()
        {
            _d3dDirty = int.MaxValue;
        }

        internal void PlatformSetSamplers(GraphicsDevice device)
        {
            // Skip out if nothing has changed.
            if (_d3dDirty == 0)
                return;

            // NOTE: We make the assumption here that the caller has
            // locked the d3dContext for us to use.
            var pixelShaderStage = device._d3dContext.PixelShader;

            for (var i = 0; i < _samplers.Length; i++)
            {
                var mask = 1 << i;
                if ((_d3dDirty & mask) == 0)
                    continue;

                var sampler = _samplers[i];
                SharpDX.Direct3D11.SamplerState state = null;
                if (sampler != null)
                    state = sampler.GetState(device);

                pixelShaderStage.SetSampler(i, state);

                _d3dDirty &= ~mask;
                if (_d3dDirty == 0)
                    break;
            }

            _d3dDirty = 0;
        }
    }
}
