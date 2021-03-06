﻿using System;
using System.Runtime.InteropServices;

namespace Cubic.Core.Runtime
{
  public class AutoPinner : IDisposable
  {
    private GCHandle _pinnedArray;
    public AutoPinner( object obj )
    {
      _pinnedArray = GCHandle.Alloc( obj , GCHandleType.Pinned );
    }
    public static implicit operator IntPtr( AutoPinner ap )
    {
      return ap._pinnedArray.AddrOfPinnedObject();
    }
    public void Dispose()
    {
      _pinnedArray.Free();
    }
  }
}