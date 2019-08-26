using HubMiddleware.Modelos.Shared;

namespace HubMiddleware.Modelos
{
    public class WatcherEvent
    {
        public string Group { get; set; }

        public Watcher Body { get; set; }
    }
}
