﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Collections.Generic;

namespace Mosa.DeviceSystem
{
	/// <summary>
	/// Device
	/// </summary>
	public class Device
	{
		public string Name { get; set; }
		public BaseDeviceDriver DeviceDriver { get; set; }
		public DeviceStatus Status { get; set; }

		public Device Parent { get; set; }

		//public IService Service { get; set; }

		public List<Device> Children { get; } = new List<Device>();
		public HardwareResources Resources { get; set; }
		public BaseDeviceConfiguration Configuration { get; set; }

		public ulong SubComponentID { get; set; }

		public DeviceDriverRegistryEntry DeviceDriverRegistryEntry { get; set; }
		public DeviceManager DeviceManager { get; internal set; }
	}
}
