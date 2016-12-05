/////////////////////////////////////////////////////////////////////////////
// SLABHIDDevice.cs
// SLABHIDDevice.dll imports and wrapper class Version 1.5
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
// Namespaces
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SLAB_HID_DEVICE
{
    /////////////////////////////////////////////////////////////////////////////
    // SLABHIDDevice.dll Imports
    /////////////////////////////////////////////////////////////////////////////

	public class HidXKUnity_DLL
    {
        #region SLABHIDDevice.dll Import Functions
		
		[DllImport("HidXKDevice")]
		public static extern void SetBikeTaBanInfo( uint bikeTaBanMaxNum, uint bikeTaBanMinNum );

		[DllImport("HidXKDevice")]
		public static extern float GetBikeRotationAZ( float mSteerTimeCur ); //maxSteerTime = 5.0f;

		[DllImport("HidXKDevice")]
		public static extern bool CheckBikeAngleLR( float eaZ );

		[DllImport("HidXKDevice")]
		public static extern float GetBikeWheelAngle( float vecDis );
		
		[DllImport("HidXKDevice")]
		public static extern int GetQiBuZuLi(float bikeSpeed);
		

		[DllImport("HidXKDevice")]
		public static extern float GetBikeSpeed(float magnitudeVel);

		[DllImport("HidXKDevice")]
		public static extern float GetBikePower(float currentEnginePower, float currentBrakeEnginePower, float currentBrakeWaterPower);

		[DllImport("HidXKDevice")]
		public static extern  float GetBikeThrottleForce(float power, float mass);

		[DllImport("HidXKDevice")]
		public static extern float GetTaBanCount(bool IsFanZhuangTaBan, uint bikeTaBanNum);

		[DllImport("HidXKDevice")]
		public static extern byte GetSendKey_A(byte[] buffer);

		[DllImport("HidXKDevice")]
		public static extern byte GetSendKey_B(byte[] buffer);

		[DllImport("HidXKDevice")]
		public static extern int CheckHidGetMsg(byte[] buffer);

        [DllImport("HidXKDevice")]
        public static extern uint HidXKUnity_GetNumHidDevices(ushort vid, ushort pid);

        [DllImport("HidXKDevice")]
        public static extern byte HidXKUnity_GetHidString(uint deviceIndex, ushort vid, ushort pid, byte hidStringType, StringBuilder deviceString, uint deviceStringLength);

        [DllImport("HidXKDevice")]
        public static extern byte HidXKUnity_GetHidIndexedString(uint deviceIndex, ushort vid, ushort pid, uint stringIndex, StringBuilder deviceString, uint deviceStringLength);

        [DllImport("HidXKDevice")]
        public static extern byte HidXKUnity_GetHidAttributes(uint deviceIndex, ushort vid, ushort pid, ref ushort deviceVid, ref ushort devicePid, ref ushort deviceReleaseNumber);

        [DllImport("HidXKDevice")]
        public static extern void HidXKUnity_GetHidGuid(ref Guid hidGuid);

        [DllImport("HidXKDevice")]
        public static extern byte HidXKUnity_GetHidLibraryVersion(ref byte major, ref byte minor, ref int release);

        [DllImport("HidXKDevice")]
        public static extern byte HidXKUnity_Open(ref IntPtr device, uint deviceIndex, ushort vid, ushort pid, uint numInputBuffers);

        [DllImport("HidXKDevice")]
        public static extern int HidXKUnity_IsOpened(IntPtr device);

        [DllImport("HidXKDevice")]
        public static extern uint HidXKUnity_GetHandle(IntPtr device);

        [DllImport("HidXKDevice")]
        public static extern byte HidXKUnity_GetString(IntPtr device, byte hidStringType, StringBuilder deviceString, uint deviceStringLength);

        [DllImport("HidXKDevice")]
        public static extern byte HidXKUnity_GetIndexedString(IntPtr device, uint stringIndex, StringBuilder deviceString, uint deviceStringLength);

        [DllImport("HidXKDevice")]
        public static extern byte HidXKUnity_GetAttributes(IntPtr device, ref ushort deviceVid, ref ushort devicePid, ref ushort deviceReleaseNumber);

        [DllImport("HidXKDevice")]
        public static extern byte HidXKUnity_SetFeatureReport_Control(IntPtr device, byte[] buffer, uint bufferSize);

        [DllImport("HidXKDevice")]
        public static extern byte HidXKUnity_GetFeatureReport_Control(IntPtr device, byte[] buffer, uint bufferSize);

        [DllImport("HidXKDevice")]
        public static extern byte HidXKUnity_SetOutputReport_Interrupt(IntPtr device, byte[] buffer, uint bufferSize);

        [DllImport("HidXKDevice")]
        public static extern byte HidXKUnity_GetInputReport_Interrupt(IntPtr device, byte[] buffer, uint bufferSize, uint numReports, ref uint bytesReturned);

        [DllImport("HidXKDevice")]
        public static extern byte HidXKUnity_SetOutputReport_Control(IntPtr device, byte[] buffer, uint bufferSize);

        [DllImport("HidXKDevice")]
        public static extern byte HidXKUnity_GetInputReport_Control(IntPtr device, byte[] buffer, uint bufferSize);

        [DllImport("HidXKDevice")]
        public static extern ushort HidXKUnity_GetInputReportBufferLength(IntPtr device);

        [DllImport("HidXKDevice")]
        public static extern ushort HidXKUnity_GetOutputReportBufferLength(IntPtr device);

        [DllImport("HidXKDevice")]
        public static extern ushort HidXKUnity_GetFeatureReportBufferLength(IntPtr device);

        [DllImport("HidXKDevice")]
        public static extern uint HidXKUnity_GetMaxReportRequest(IntPtr device);

        [DllImport("HidXKDevice")]
        public static extern int HidXKUnity_FlushBuffers(IntPtr device);

        [DllImport("HidXKDevice")]
        public static extern int HidXKUnity_CancelIo(IntPtr device);

        [DllImport("HidXKDevice")]
        public static extern void HidXKUnity_GetTimeouts(IntPtr device, ref uint getReportTimeout, ref uint setReportTimeout);

        [DllImport("HidXKDevice")]
        public static extern void HidXKUnity_SetTimeouts(IntPtr device, uint getReportTimeout, uint setReportTimeout);

        [DllImport("HidXKDevice")]
        public static extern byte HidXKUnity_Close(IntPtr device);

		/// <summary>
		/// 
		/// </summary>
		/// <returns>The device_ get number hid devices.</returns>
		/// <param name="vid">Vid.</param>
		/// <param name="pid">Pid.</param>
		[DllImport("SLABHIDDevice")]
		public static extern uint HidDevice_GetNumHidDevices(ushort vid, ushort pid);
		
		[DllImport("SLABHIDDevice")]
		public static extern byte HidDevice_GetHidString(uint deviceIndex, ushort vid, ushort pid, byte hidStringType, StringBuilder deviceString, uint deviceStringLength);
		
		[DllImport("SLABHIDDevice")]
		public static extern byte HidDevice_GetHidIndexedString(uint deviceIndex, ushort vid, ushort pid, uint stringIndex, StringBuilder deviceString, uint deviceStringLength);
		
		[DllImport("SLABHIDDevice")]
		public static extern byte HidDevice_GetHidAttributes(uint deviceIndex, ushort vid, ushort pid, ref ushort deviceVid, ref ushort devicePid, ref ushort deviceReleaseNumber);
		
		[DllImport("SLABHIDDevice")]
		public static extern void HidDevice_GetHidGuid(ref Guid hidGuid);
		
		[DllImport("SLABHIDDevice")]
		public static extern byte HidDevice_GetHidLibraryVersion(ref byte major, ref byte minor, ref int release);
		
		[DllImport("SLABHIDDevice")]
		public static extern byte HidDevice_Open(ref IntPtr device, uint deviceIndex, ushort vid, ushort pid, uint numInputBuffers);
		
		[DllImport("SLABHIDDevice")]
		public static extern int HidDevice_IsOpened(IntPtr device);
		
		[DllImport("SLABHIDDevice")]
		public static extern uint HidDevice_GetHandle(IntPtr device);
		
		[DllImport("SLABHIDDevice")]
		public static extern byte HidDevice_GetString(IntPtr device, byte hidStringType, StringBuilder deviceString, uint deviceStringLength);
		
		[DllImport("SLABHIDDevice")]
		public static extern byte HidDevice_GetIndexedString(IntPtr device, uint stringIndex, StringBuilder deviceString, uint deviceStringLength);
		
		[DllImport("SLABHIDDevice")]
		public static extern byte HidDevice_GetAttributes(IntPtr device, ref ushort deviceVid, ref ushort devicePid, ref ushort deviceReleaseNumber);
		
		[DllImport("SLABHIDDevice")]
		public static extern byte HidDevice_SetFeatureReport_Control(IntPtr device, byte[] buffer, uint bufferSize);
		
		[DllImport("SLABHIDDevice")]
		public static extern byte HidDevice_GetFeatureReport_Control(IntPtr device, byte[] buffer, uint bufferSize);
		
		[DllImport("SLABHIDDevice")]
		public static extern byte HidDevice_SetOutputReport_Interrupt(IntPtr device, byte[] buffer, uint bufferSize);
		
		[DllImport("SLABHIDDevice")]
		public static extern byte HidDevice_GetInputReport_Interrupt(IntPtr device, byte[] buffer, uint bufferSize, uint numReports, ref uint bytesReturned);
		
		[DllImport("SLABHIDDevice")]
		public static extern byte HidDevice_SetOutputReport_Control(IntPtr device, byte[] buffer, uint bufferSize);
		
		[DllImport("SLABHIDDevice")]
		public static extern byte HidDevice_GetInputReport_Control(IntPtr device, byte[] buffer, uint bufferSize);
		
		[DllImport("SLABHIDDevice")]
		public static extern ushort HidDevice_GetInputReportBufferLength(IntPtr device);
		
		[DllImport("SLABHIDDevice")]
		public static extern ushort HidDevice_GetOutputReportBufferLength(IntPtr device);
		
		[DllImport("SLABHIDDevice")]
		public static extern ushort HidDevice_GetFeatureReportBufferLength(IntPtr device);
		
		[DllImport("SLABHIDDevice")]
		public static extern uint HidDevice_GetMaxReportRequest(IntPtr device);
		
		[DllImport("SLABHIDDevice")]
		public static extern int HidDevice_FlushBuffers(IntPtr device);
		
		[DllImport("SLABHIDDevice")]
		public static extern int HidDevice_CancelIo(IntPtr device);
		
		[DllImport("SLABHIDDevice")]
		public static extern void HidDevice_GetTimeouts(IntPtr device, ref uint getReportTimeout, ref uint setReportTimeout);
		
		[DllImport("SLABHIDDevice")]
		public static extern void HidDevice_SetTimeouts(IntPtr device, uint getReportTimeout, uint setReportTimeout);
		
		[DllImport("SLABHIDDevice")]
		public static extern byte HidDevice_Close(IntPtr device);

        #endregion
    }

    /////////////////////////////////////////////////////////////////////////////
    // SLABHIDDevice.dll Wrapper Class
    /////////////////////////////////////////////////////////////////////////////

    public class Hid
    {
        // Protected members
        protected IntPtr m_hid;

        // Definitions
        #region SLABHIDDevice.dll Definitions

        // Return Codes
        public const byte HID_DEVICE_SUCCESS = 0x00;
        public const byte HID_DEVICE_NOT_FOUND = 0x01;
        public const byte HID_DEVICE_NOT_OPENED = 0x02;
        public const byte HID_DEVICE_ALREADY_OPENED = 0x03;
        public const byte HID_DEVICE_TRANSFER_TIMEOUT = 0x04;
        public const byte HID_DEVICE_TRANSFER_FAILED = 0x05;
        public const byte HID_DEVICE_CANNOT_GET_HID_INFO = 0x06;
        public const byte HID_DEVICE_HANDLE_ERROR = 0x07;
        public const byte HID_DEVICE_INVALID_BUFFER_SIZE = 0x08;
        public const byte HID_DEVICE_SYSTEM_CODE = 0x09;
        public const byte HID_DEVICE_UNSUPPORTED_FUNCTION = 0x0A;
        public const byte HID_DEVICE_UNKNOWN_ERROR = 0xFF;

        // Max number of USB Devices allowed
        public const byte MAX_USB_DEVICES = 64;

        // Max number of reports that can be requested at time
        public const uint MAX_REPORT_REQUEST_XP = 512;
        public const uint MAX_REPORT_REQUEST_2K = 200;
        public const uint DEFAULT_REPORT_INPUT_BUFFERS = 0;

        // String Types
        public const byte HID_VID_STRING = 0x01;
        public const byte HID_PID_STRING = 0x02;
        public const byte HID_PATH_STRING = 0x03;
        public const byte HID_SERIAL_STRING = 0x04;
        public const byte HID_MANUFACTURER_STRING = 0x05;
        public const byte HID_PRODUCT_STRING = 0x06;

        // String Lengths
        public const uint MAX_VID_LENGTH = 5;
        public const uint MAX_PID_LENGTH = 5;
        public const uint MAX_PATH_LENGTH = 260;
        public const uint MAX_SERIAL_STRING_LENGTH = 256;
        public const uint MAX_MANUFACTURER_STRING_LENGTH = 256;
        public const uint MAX_PRODUCT_STRING_LENGTH = 256;
        public const uint MAX_INDEXED_STRING_LENGTH = 256;
        public const uint MAX_STRING_LENGTH = 260;

        #endregion

        // Constructor/Destructor
        public Hid() { }
        ~Hid() { }

        // Public Methods
        public static uint GetNumHidDevices(ushort vid, ushort pid)
        {
            return HidXKUnity_DLL.HidXKUnity_GetNumHidDevices(vid, pid);
        }

        public static byte GetHidString(uint deviceIndex, ushort vid, ushort pid, byte hidStringType, StringBuilder deviceString, uint deviceStringLength)
        {
            return HidXKUnity_DLL.HidXKUnity_GetHidString(deviceIndex, vid, pid, hidStringType, deviceString, deviceStringLength);
        }

        public static byte GetHidIndexedString(uint deviceIndex, ushort vid, ushort pid, uint stringIndex, StringBuilder deviceString, uint deviceStringLength)
        {
            return HidXKUnity_DLL.HidXKUnity_GetHidIndexedString(deviceIndex, vid, pid, stringIndex, deviceString, deviceStringLength);
        }

        public static byte GetHidAttributes(uint deviceIndex, ushort vid, ushort pid, ref ushort deviceVid, ref ushort devicePid, ref ushort deviceReleaseNumber)
        {
            return HidXKUnity_DLL.HidXKUnity_GetHidAttributes(deviceIndex, vid, pid, ref deviceVid, ref devicePid, ref deviceReleaseNumber);
        }

        public static void GetHidGuid(ref Guid hidGuid)
        {
            HidXKUnity_DLL.HidXKUnity_GetHidGuid(ref hidGuid);
        }

        public static byte GetHidLibraryVersion(ref byte major, ref byte minor, ref int release)
        {
            return HidXKUnity_DLL.HidXKUnity_GetHidLibraryVersion(ref major, ref minor, ref release);
        }

        public static bool GetDeviceIndex(ushort vid, ushort pid, string serial, ref uint deviceIndex)
        {
            uint index = 0;
            bool found = false;

            // Iterate through each connected device and search for a device
            // with a serial string matching the user selected device in the
            // device list
            for (uint i = 0; i < Hid.GetNumHidDevices(vid, pid); i++)
            {
                StringBuilder str = new StringBuilder((int)Hid.MAX_SERIAL_STRING_LENGTH);

                if (Hid.GetHidString(i, vid, pid, Hid.HID_SERIAL_STRING, str, Hid.MAX_SERIAL_STRING_LENGTH) == Hid.HID_DEVICE_SUCCESS)
                {
                    // Device serial strings match
                    if (serial == str.ToString())
                    {
                        index = i;
                        found = true;
                        break;
                    }
                }
            }

            deviceIndex = index;

            return found;
        }

        // Connect to HID device with the VID/PID and serial string
        // and set timeouts
        public bool Connect(ushort vid, ushort pid, string serial, uint getReportTimeout, uint setReportTimeout)
        {
            bool connected = false;
            uint index = 0;
            bool found = false;

            // Find specified device
            found = GetDeviceIndex(vid, pid, serial, ref index);

            // Open device with matching serial string
            if (found)
            {
                if (Open(index, vid, pid, MAX_REPORT_REQUEST_XP) == Hid.HID_DEVICE_SUCCESS)
                {
                    // Set read/write timeouts
                    //
                    // Read timeouts are temporarily set to 0 ms
                    // in the read timer and then restored
                    SetTimeouts(getReportTimeout, setReportTimeout);

                    connected = true;
                }
            }

            return connected;
        }

        public byte Open(uint deviceIndex, ushort vid, ushort pid, uint numInputBuffers)
        {
            return HidXKUnity_DLL.HidXKUnity_Open(ref m_hid, deviceIndex, vid, pid, numInputBuffers);
        }

        public int IsOpened()
        {
            return HidXKUnity_DLL.HidXKUnity_IsOpened(m_hid);
        }

        public uint GetHandle()
        {
            return HidXKUnity_DLL.HidXKUnity_GetHandle(m_hid);
        }

        public byte GetString(byte hidStringType, StringBuilder deviceString, uint deviceStringLength)
        {
            return HidXKUnity_DLL.HidXKUnity_GetString(m_hid, hidStringType, deviceString, deviceStringLength);
        }

        public byte GetIndexedString(uint stringIndex, StringBuilder deviceString, uint deviceStringLength)
        {
            return HidXKUnity_DLL.HidXKUnity_GetIndexedString(m_hid, stringIndex, deviceString, deviceStringLength);
        }

        public byte GetAttributes(ref ushort deviceVid, ref ushort devicePid, ref ushort deviceReleaseNumber)
        {
            return HidXKUnity_DLL.HidXKUnity_GetAttributes(m_hid, ref deviceVid, ref devicePid, ref deviceReleaseNumber);
        }

        public byte SetFeatureReport_Control(byte[] buffer, uint bufferSize)
        {
            return HidXKUnity_DLL.HidXKUnity_SetFeatureReport_Control(m_hid, buffer, bufferSize);
        }

        public byte GetFeatureReport_Control(byte[] buffer, uint bufferSize)
        {
            return HidXKUnity_DLL.HidXKUnity_GetFeatureReport_Control(m_hid, buffer, bufferSize);
        }

        public byte SetOutputReport_Interrupt(byte[] buffer, uint bufferSize)
        {
            return HidXKUnity_DLL.HidXKUnity_SetOutputReport_Interrupt(m_hid, buffer, bufferSize);
        }

        public byte GetInputReport_Interrupt(byte[] buffer, uint bufferSize, uint numReports, ref uint bytesReturned)
        {
            return HidXKUnity_DLL.HidXKUnity_GetInputReport_Interrupt(m_hid, buffer, bufferSize, numReports, ref bytesReturned);
        }

        public byte SetOutputReport_Control(byte[] buffer, uint bufferSize)
        {
            return HidXKUnity_DLL.HidXKUnity_SetOutputReport_Control(m_hid, buffer, bufferSize);
        }

        public byte GetInputReport_Control(byte[] buffer, uint bufferSize)
        {
            return HidXKUnity_DLL.HidXKUnity_GetInputReport_Control(m_hid, buffer, bufferSize);
        }

        public ushort GetInputReportBufferLength()
        {
            return HidXKUnity_DLL.HidXKUnity_GetInputReportBufferLength(m_hid);
        }

        public ushort GetOutputReportBufferLength()
        {
            return HidXKUnity_DLL.HidXKUnity_GetOutputReportBufferLength(m_hid);
        }

        public ushort GetFeatureReportBufferLength()
        {
            return HidXKUnity_DLL.HidXKUnity_GetFeatureReportBufferLength(m_hid);
        }

        public uint GetMaxReportRequest()
        {
            return HidXKUnity_DLL.HidXKUnity_GetMaxReportRequest(m_hid);
        }

        public int FlushBuffers()
        {
            return HidXKUnity_DLL.HidXKUnity_FlushBuffers(m_hid);
        }

        public int CancelIo()
        {
            return HidXKUnity_DLL.HidXKUnity_CancelIo(m_hid);
        }

        public void GetTimeouts(ref uint getReportTimeout, ref uint setReportTimeout)
        {
            HidXKUnity_DLL.HidXKUnity_GetTimeouts(m_hid, ref getReportTimeout, ref setReportTimeout);
        }

        public void SetTimeouts(uint getReportTimeout, uint setReportTimeout)
        {
            HidXKUnity_DLL.HidXKUnity_SetTimeouts(m_hid, getReportTimeout, setReportTimeout);
        }

        public byte Close()
        {
            return HidXKUnity_DLL.HidXKUnity_Close(m_hid);
        }
    }
}
