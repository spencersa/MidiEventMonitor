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
        IEnumerable<string> GetConnectedDeviceInformation()
    }

    public class Monitor : IMonitor
    {
        int _numberOfDevices;
        List<MidiInCapabilities> _midiInCapabilities;

        public Monitor()
        {
            _numberOfDevices = MidiIn.NumberOfDevices;
            _midiInCapabilities = GetMidiInCapabilities();
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
    }
}
