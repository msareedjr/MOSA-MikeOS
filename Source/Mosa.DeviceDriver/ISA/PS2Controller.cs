using Mosa.DeviceSystem;

namespace Mosa.DeviceDriver.ISA
{
	public class PS2Controller : BaseDeviceDriver
	{
		protected BaseIOPortReadWrite dataPort;

		protected BaseIOPortRead statusPort;

		protected BaseIOPortWrite commandPort;

		private enum Command : byte
		{
			ReadByte = 0x20,
			WriteByte = 0x60,
			DisablePort1 = 0xAD,
			DisablePort2 = 0xA7,
			EnablePort1 = 0xAE,
			EnablePort2 = 0xA8,
			SelfTest = 0xAA,
			TestPort1 = 0xAB,
			TestPort2 = 0xA9,
			SendToPort2 = 0xD4,
			DeviceReset = 0xFF,
			DisableScanning = 0xF5,
			EnableScanning = 0xF4,
			Identify = 0xF2
		}

		public enum DeviceType
		{
			UNKNOWN = 0,
			KEYBOARD,
			MOUSE_STANDARD,
			MOUSE_WITH_SCROLL,
			MOUSE_FIVE_BUTTON,
			
		}

		public const bool PORT1 = true;
		public const bool PORT2 = false;
		public bool Port1Connected { get; set; }
		public DeviceType Port1Device { get; set; }

		public bool Port2Connected { get; set; }

		public DeviceType Port2Device { get; set; }

		public override void Initialize()
		{
			Device.Name = "PS/2Controller";

			dataPort = Device.Resources.GetIOPortReadWrite(0, 0);       // 0x60
			statusPort = Device.Resources.GetIOPortRead(1, 0);          // 0x64
			commandPort = Device.Resources.GetIOPortWrite(1, 0);        // 0x64

		}
		public override void Start()
		{
			bool port1TestPassed = false;
			bool port2TestPassed = false;
			// disable port 1
			SendCommand(Command.DisablePort1);
			// disable port 2
			SendCommand(Command.DisablePort2);
			var data = ReadData();

			SendCommand(Command.ReadByte);
			var currentConfiguration = ReadData();
			currentConfiguration &= 0xBC;
			// see if port 2 exists
			bool hasSecondPort = (currentConfiguration & 0x20) == 0x20;

			SendCommand(Command.WriteByte, currentConfiguration);

			SendCommand(0xAA);
			if (ReadData() != 0x55)
			{
				Device.Status = DeviceStatus.Error;
				return;
			}
			SendCommand(Command.WriteByte, currentConfiguration);
			SendCommand(Command.TestPort1);

			if (ReadData() == 0)
				port1TestPassed = true;

			if (hasSecondPort)
			{
				SendCommand(Command.TestPort2);
				if (ReadData() == 0)
					port2TestPassed = true;
			}
			if (port1TestPassed)
			{
				SendCommand(Command.EnablePort1);
			}
			if (port2TestPassed)
			{
				SendCommand(Command.EnablePort2);
			}
			currentConfiguration |= 0x03;
			SendCommand(Command.WriteByte, currentConfiguration);
			
			if (port1TestPassed)
			{
				SendToDevice(PORT1, Command.DeviceReset);
				byte nextByte;
				do
				{
					nextByte = ReadData();
					if (nextByte == 0xFA)
						Port1Connected = true;
				}
				while (nextByte != 0xFF);
				SendToDevice(PORT1, Command.DisableScanning);
				
				nextByte = ReadData();
				SendToDevice(PORT1, Command.Identify);
				byte[] identBuffer = new byte[8];
				for (var i = 0; i< 8 && nextByte != 0xFF; i++)
				{
					nextByte = ReadData();
					identBuffer[i] = nextByte;
					
				}
				if (identBuffer[0] == 0xFA)
				{
					switch(identBuffer[1])
					{
						case 0xAB:
							Port1Device = DeviceType.KEYBOARD;
							break;
						case 0x00:
							Port1Device = DeviceType.MOUSE_STANDARD;
							break;
						case 0x03:
							Port1Device = DeviceType.MOUSE_WITH_SCROLL;
							break;
						case 0x04:
							Port1Device = DeviceType.MOUSE_FIVE_BUTTON;
							break;
					}
					SendToDevice(PORT1, Command.EnableScanning);
					
				}
				SendToDevice(PORT2, Command.Identify);
				identBuffer = new byte[8];
				for (var i = 0; i < 8 && nextByte != 0xFF; i++)
				{
					nextByte = ReadData();
					identBuffer[i] = nextByte;

				}
				if (identBuffer[0] == 0xFA)
				{
					switch (identBuffer[1])
					{
						case 0xAB:
							Port2Device = DeviceType.KEYBOARD;
							break;
						case 0x00:
							Port2Device = DeviceType.MOUSE_STANDARD;
							break;
						case 0x03:
							Port2Device = DeviceType.MOUSE_WITH_SCROLL;
							break;
						case 0x04:
							Port2Device = DeviceType.MOUSE_FIVE_BUTTON;
							break;
					}
					SendToDevice(PORT2, Command.EnableScanning);

				}
			}


			Device.Status = DeviceStatus.Online;
		}

		private void SendToDevice(bool port1, Command command)
		{
			if (port1)
			{
				dataPort.Write8((byte)command);
			}
			else
			{
				SendCommand(Command.SendToPort2);
				dataPort.Write8((byte)command);

			}
		}

		private void SendCommand(byte command)
		{
			byte status = 0;
			do
			{
				HAL.Sleep(5);
				status = statusPort.Read8();
			} while ((status & 0x02) != 0);
			commandPort.Write8(command);
		}
		private void SendCommand(byte command, byte data)
		{
			byte status = 0;
			SendCommand(command);
			do
			{
				HAL.Sleep(5);
				status = statusPort.Read8();
			} while ((status & 0x02) != 0);
			dataPort.Write8(data);

		}

		private void SendCommand(Command command)
		{
			SendCommand((byte)command);
		}
		private void SendCommand(Command command, byte data)
		{
			SendCommand((byte)command, data);
		}
		private byte ReadData()
		{
			byte status = statusPort.Read8();
			int retries = 0;
			while ((status & 0x01) == 0 || retries > 3)
			{
				HAL.Sleep(5);
				status = statusPort.Read8();
				retries++;
			}
			if ((status & 0x01) == 0x01)
			{
				byte data = dataPort.Read8();

				return data;
			}
			return 0xFF;
		}
	}
}
