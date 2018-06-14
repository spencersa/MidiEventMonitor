using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace MidiEventMonitor
{
    public interface IMonitor
    {
        IEnumerable<string> GetConnectedDeviceInformation();
    }

    public class Monitor : IMonitor
    {

        private int _numberOfDevices;
        private readonly Process[] _processes;
        private List<MidiInCapabilities> _midiInCapabilities;
        public List<MidiIn> MidiIns { get; set; }


        public Monitor(string processToSendKeyPressesTo)
        {
            _numberOfDevices = MidiIn.NumberOfDevices;
            _midiInCapabilities = GetMidiInCapabilities();

            MidiIns = new List<MidiIn>();
            for (int i = 0; i < _numberOfDevices; i++)
            {
                var midiIn = new MidiIn(i);
                midiIn.MessageReceived += MidiIn_MessageReceived;
                midiIn.ErrorReceived += MidiIn_ErrorReceived;
                midiIn.Start();
                MidiIns.Add(midiIn);
            }

            _processes = Process.GetProcessesByName(processToSendKeyPressesTo);
        }

        public IEnumerable<string> GetConnectedDeviceInformation()
        {
            return _midiInCapabilities.Select(x => $"ProductId: {x.ProductId} | ProductName:{x.ProductName} | Manufacturer:{x.Manufacturer}");
        }

        private List<MidiInCapabilities> GetMidiInCapabilities()
        {
            var devices = new List<MidiInCapabilities>();
            for (int i = 0; i < _numberOfDevices; i++)
            {
                devices.Add(MidiIn.DeviceInfo(i));
            }
            return devices;
        }

        private void MidiIn_ErrorReceived(object sender, MidiInMessageEventArgs e)
        {
            Logger(String.Format("Time {0} Message 0x{1:X8} Event {2}",
                e.Timestamp, e.RawMessage, e.MidiEvent));
        }

        private void MidiIn_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            Logger(String.Format("Time {0} Message 0x{1:X8} Event {2}", e.Timestamp, e.RawMessage, e.MidiEvent));
        }

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);
        public void SendKey(string key)
        {
            IntPtr child = FindWindowEx(_processes[0].MainWindowHandle, new IntPtr(0), "Edit", null);
            SendMessage(child, 0x000C, 0, key);
        }

        private void Logger(string lines)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("c:\\temp\\log.log", true);
            file.WriteLine(lines);

            file.Close();
        }
    }
}
