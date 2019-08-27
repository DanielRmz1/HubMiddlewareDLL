using HubMiddleware.Modelos;
using HubMiddleware.Modelos.Shared;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubMiddleware.RealTime
{
    public class Broadcaster : INotifyPropertyChanged
    {
        #region events

        /// <summary>
        /// Ignorar esta propiedad, la interfaz INotifyPropertyChanged solicita declarar un evento con este nombre
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangedEventHandler TagChanged;

        /// <summary>
        /// Evento que es disparado cuando cambia el estado de la conexión.
        /// </summary>
        public event PropertyChangedEventHandler LostConnection;

        /// <summary>
        /// Evento que notifica el cambio de las alarmas del Sampras.
        /// </summary>
        public event PropertyChangedEventHandler AlarmNotification;

        public event PropertyChangedEventHandler WatcherNotification;

        public event PropertyChangedEventHandler AndonNotification;

        public event PropertyChangedEventHandler SuscriptionAlertNotification;

        #endregion

        #region LocalProperties

        private HubConnection hubConnection;

        /// <summary>
        /// Contiene el estado actual de la conexión al servicio.
        /// </summary>
        private bool _isConnected = false;
        public bool IsConnected { get { return _isConnected; } set { _isConnected = value; OnLostConnection("IsConnected"); } }

        /// <summary>
        /// Almacena los mensajes de error provocados por la clase
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Almacena la información recibida por las notificaciones de SignalR
        /// </summary>
        private Tag _tag;
        public Tag Tag { get { return _tag; } set { _tag = value; OnTagChanged("Tag"); } }

        private Alarm _alarm;
        public Alarm Alarm { get { return _alarm; } set { _alarm = value; OnAlarmChanged("Alarm"); } }

        private WatcherEvent _watcherEvent;
        public WatcherEvent WatcherEvent { get { return _watcherEvent; } set { _watcherEvent = value; OnWatcherChanged("WatcherEvent"); } }

        private AndonEvent _andonEvent;
        public AndonEvent AndonEvent { get { return _andonEvent; } set { _andonEvent = value; OnAndonChanged("AndonEvent"); } }

        private string _sucriptionId;
        public string SuscriptionId { get { return _sucriptionId; } set { _sucriptionId = value; OnSuscriptionAlert("SuscriptionId"); } }

        private string Url { get; set; }
        
        /// <summary>
        /// Almacena la respuesta obtenida del servidor a traves del método Test()
        /// </summary>
        public bool ServerResponded { get; set; }

        protected void OnPropertyChanged(string data)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(data));
            }
        }

        protected void OnTagChanged(string data)
        {
            PropertyChangedEventHandler handler = TagChanged;
            if(handler != null)
            {
                handler(this, new PropertyChangedEventArgs(data));
            }
        }

        protected void OnLostConnection(string isConnected)
        {
            PropertyChangedEventHandler handler = LostConnection;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(isConnected));
            }
        }

        protected void OnAlarmChanged(string alarm)
        {
            PropertyChangedEventHandler handler = AlarmNotification;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(alarm));
            }
        }

        protected void OnWatcherChanged(string watcher)
        {
            PropertyChangedEventHandler handler = WatcherNotification;
            if(handler != null)
            {
                handler(this, new PropertyChangedEventArgs(watcher));
            }
        }

        protected void OnAndonChanged(string andon)
        {
            PropertyChangedEventHandler handler = AndonNotification;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(andon));
            }
        }

        protected void OnSuscriptionAlert(string suscriptionId)
        {
            PropertyChangedEventHandler handler = SuscriptionAlertNotification;
            if(handler != null)
            {
                handler(this, new PropertyChangedEventArgs(suscriptionId));
            }
        }

        #endregion

        #region constructor

        /// <summary>
        /// Establece los parámetros necesarios para la conexión al servicio
        /// y crea los eventos de comunicación con SignalR
        /// </summary>
        /// <param name="url">Ruta del servidor donde se encuentra instalado el servicio Signal</param>
        public Broadcaster(string url)
        {
            this.Url = url;
            hubConnection = new HubConnectionBuilder()
                .WithUrl(Url)
                .Build();

            hubConnection.On<string, WatcherEvent>("watcherEvent", (group, watcher) =>
            {
                watcher.Group = group;
                this.WatcherEvent = watcher;
            });

            hubConnection.On<string, AndonEvent>("andonEvent", (group, andon) =>
            {
                andon.Group = group;
                this.AndonEvent = andon;
            });

            hubConnection.On<object, string>("tagEvent", (data, tag) =>
            {
                this.Tag = new Tag()
                {
                    Body = data,
                    Name = tag
                };
            });

            hubConnection.On<bool>("handshake", (response) =>
            {
                this.ServerResponded = response;
            });

            hubConnection.On<string, string, DateTime>("alarmEvent", (type, description, start_date) => {
                this.Alarm = new Alarm()
                {
                    Description = description,
                    Type = type,
                    StartDate = start_date
                };
            });

            hubConnection.On<string>("alertSuscription", (id) => {
                this.SuscriptionId = id;
            });

            hubConnection.Closed += async (error) => {
                this.IsConnected = false;
                this.Error = error.ToString();
            };
        }

        #endregion

        /// <summary>
        /// Establece la conexión con el servicio de SignalR
        /// </summary>
        public async Task StartConnection()
        {
            try
            {
                await hubConnection.StartAsync();

                IsConnected = true;
            }
            catch (Exception e)
            {
                this.Error = e.ToString();
                throw;
            }

        }

        /// <summary>
        /// Establece el estado de escucha suscribiendo al cliente para recibir notificaciones 
        /// en base aun determinado nombre TAG 
        /// </summary>
        /// <param name="tagName">Variable tipo string que indica el nombre del "Grupo" al cual se suscribe el usuario</param>
        public async Task Suscribe(string groupName)
        {
            await hubConnection.InvokeAsync("Suscribe", groupName);
        }

        /// <summary>
        /// Cierra el estado de escucha del cliente, se tiene que pasar el mismo groupName que cuando se suscribio
        /// </summary>
        /// <param name="tagName">Variable tipo string que indica el nombre del "Grupo" al cual se desuscribe el usuario</param>
        public async Task Unsuscribe(string groupName)
        {
            await hubConnection.InvokeAsync("Unsuscribe", groupName);
        }

        #region broadcasters

        #region tags

        /// <summary>
        /// Trasmite un mensaje de tipo string a todos los clientes suscritos al nombre del tag indicado
        /// </summary>
        /// <param name="tagName">Nombre del tag al cual se quiere hacer la trasmisión.</param>
        /// <param name="message">Información que se desea enviar</param>
        public async Task BroadcastTag(string tagName, string message)
        {
            await hubConnection.InvokeAsync("BroadcastTag", tagName, message);
        }

        /// <summary>
        /// Trasmite un mensaje de tipo boolean a todos los clientes suscritos al nombre del tag indicado
        /// </summary>
        /// <param name="tagName">Nombre del tag al cual se quiere hacer la trasmisión.</param>
        /// <param name="message">Información que se desea enviar</param>
        public async Task BroadcastTag(string tagName, bool message)
        {
            await hubConnection.InvokeAsync("BroadcastTag", tagName, message);
        }

        /// <summary>
        /// Trasmite un mensaje de tipo entero a todos los clientes suscritos al nombre del tag indicado
        /// </summary>
        /// <param name="tagName">Nombre del tag al cual se quiere hacer la trasmisión.</param>
        /// <param name="data">Información que se desea enviar</param>
        public async Task BroadcastTag(string tagName, int data)
        {
            await hubConnection.InvokeAsync("BroadcastTag", tagName, data);
        }

        /// <summary>
        /// Trasmite un mensaje de tipo double a todos los clientes suscritos al nombre del tag indicado
        /// </summary>
        /// <param name="tagName">Nombre del tag al cual se quiere hacer la trasmisión.</param>
        /// <param name="data">Información que se desea enviar</param>
        public async Task BroadcastTag(string tagName, double data)
        {
            await hubConnection.InvokeAsync("BroadcastTag", tagName, data);
        }

        #endregion

        /// <summary>
        /// Notifica de una alarma a todos los clientes suscritos al nombre de grupo.
        /// </summary>
        /// <param name="groupName">Nombre del grupo</param>
        /// <param name="type">Tipo de alarma (Paro, LLamada, Paro Programado, Panico, etc)</param>
        /// <param name="description">Descripción de la alarma (Paro por mantenimiento, Comida 1er Turno, etc)</param>
        public async Task BroadcastAlarm(string groupName, string type, string description, DateTime start_date)
        {
            await hubConnection.InvokeAsync("BroadcastAlarm", groupName, type, description, start_date);
        }

        /// <summary>
        /// Trasmite la información del watcher a todos los clientes suscritos a un nombre de grupo
        /// </summary>
        /// <param name="groupName">Nombre del grupo</param>
        /// <param name="watcher">Objeto que contiene la informacion del watcher</param>
        /// <returns></returns>
        public async Task BroadcastWatcher(string groupName, WatcherEvent watcher)
        {
            await hubConnection.InvokeAsync("WatcherEvent", groupName, watcher);
        }

        /// <summary>
        /// Trasmite la información del Andon a todos los clientes suscritos a un nombre de grupo
        /// </summary>
        /// <param name="groupName">Nombre del grupo</param>
        /// <param name="andon">Objeto que contiene la informacion del andon</param>
        /// <returns></returns>
        public async Task BroadcastAndon(string groupName, AndonEvent andon)
        {
            await hubConnection.InvokeAsync("AndonEvent", groupName, andon);
        }

        #endregion

        /// <summary>
        /// Manda una señal al servicio SignalR, para despues esperar una respuesta tipo booleana 
        /// que se guarda en la propiedad ServerResponded
        /// </summary>
        /// <returns></returns>
        public async Task Test()
        {
            this.ServerResponded = false;
            await hubConnection.InvokeAsync("Handshake");
        }
    }
}
