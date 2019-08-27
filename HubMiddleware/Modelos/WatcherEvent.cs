using HubMiddleware.Modelos.Shared;

namespace HubMiddleware.Modelos
{
    public class WatcherEvent
    {
        public string Group { get; set; }

        public Production Production { get; set; }

        public MachineInformation MachineInformation { get; set; }
    }
}
