﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Phil Garcia (tgiphil) <phil@thinkedge.com>
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Mosa.Kernel
{
	public class KernelMemory
	{
		// Never called - Necessary for reference
		static public uint AllocateMemory(uint size)
		{
			return 0;
		}

	}
}
