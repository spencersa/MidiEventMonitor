using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiEventMonitor
{
    public interface IMonitor
    {
        IEnumerable<string> GetConnectedDeviceInformation();
    }

    public class Monitor : IMonitor
    {
        int _numberOfDevices;
        List<MidiInCapabilities> _midiInCapabilities;
        public List<MidiIn> MidiIns { get; set; }


        public Monitor()
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
        }

        public IEnumerable<string> GetConnectedDeviceInformation()
        {
            return _midiInCapabilities.Select(x => $"ProductId: {x.ProductId} | ProductName:{x.ProductName} | Manufacturer:{x.Manufacturer}");
        }

        void MidiIn_ErrorReceived(object sender, MidiInMessageEventArgs e)
        {
            Logger(String.Format("Time {0} Message 0x{1:X8} Event {2}",
                e.Timestamp, e.RawMessage, e.MidiEvent));
        }

        void MidiIn_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            Logger(String.Format("Time {0} Message 0x{1:X8} Event {2}", e.Timestamp, e.RawMessage, e.MidiEvent));
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

        public void Logger(string lines)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("c:\\temp\\log.log", true);
            file.WriteLine(lines);

            file.Close();

        }
    }
}
