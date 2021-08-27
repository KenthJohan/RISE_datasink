using System;
using System.Runtime.InteropServices;

namespace Demo
{

	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public unsafe struct Msg_Float
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		[FieldOffset(0)] public fixed byte data[16];
		[FieldOffset(0)] public Int32 producer_id;
		[FieldOffset(4)] public Int64 time;
		[FieldOffset(12)] public float value;
		public unsafe static byte[] GetBytes(Msg_Float value)
		{
			byte[] arr = new byte[16];
			Marshal.Copy((IntPtr)value.data, arr, 0, 16);
			return arr;
		}
		public ArraySegment<byte> GetArraySegment()
		{
			return new ArraySegment<byte>(Msg_Float.GetBytes(this));
		}


	}

	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public unsafe struct Msg_Sub
	{
		[FieldOffset(0)] public fixed byte data[8];
		[FieldOffset(0)] public Int32 producer_id;
		[FieldOffset(4)] public Int32 mode;

		public unsafe static Msg_Sub FromBytes(byte[] data)
		{
			Msg_Sub msg = new Msg_Sub { };
			Marshal.Copy(data, 0, (IntPtr)msg.data, Marshal.SizeOf(typeof(Msg_Sub)));
			return msg;
		}

		public Msg_Sub FromArraySegment(ArraySegment<byte> data)
		{
			return FromBytes(data.Array);
		}

	}
}