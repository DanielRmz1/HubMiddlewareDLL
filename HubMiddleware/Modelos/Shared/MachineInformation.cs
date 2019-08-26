
namespace HubMiddleware.Modelos.Shared
{
    public class MachineInformation
    {
        public string State { get; set; }

        public string TagSignal { get; set; }

        public int Order { get; set; }

        /// <summary>
        /// No está bien definido el objetivo de esta propiedad.
        /// </summary>
        public string Monitoreo { get; set; }

        public string WatcherPath { get; set; }
        
    }
}
