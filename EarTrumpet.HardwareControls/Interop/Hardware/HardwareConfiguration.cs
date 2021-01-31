using EarTrumpet.HardwareControls.Interop.Deej;
using EarTrumpet.HardwareControls.Interop.MIDI;
using System.Xml.Serialization;

namespace EarTrumpet.HardwareControls.Interop.Hardware
{
    [XmlInclude(typeof(MidiConfiguration))]
    [XmlInclude(typeof(DeejConfiguration))]
    public abstract class HardwareConfiguration
    {
        public abstract override string ToString();
    }
}