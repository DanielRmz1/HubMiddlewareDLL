using HubMiddleware.Modelos.Shared;

namespace HubMiddleware.Modelos
{
    public class AndonEvent
    {
        public string Group { get; set; }

        public Andon Body { get; set; }
    }
}
