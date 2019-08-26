using System;
using System.Collections.Generic;

namespace HubMiddleware.Modelos
{
    public class Email
    {
        public string Header { get; set; }
        public StopBody Stop { get; set; }
        public ProductionBody Production { get; set; }
        public List<string> Destinations { get; set; }
    }

    public class StopBody
    {
        public string Machine { get; set; }
        public string Producto { get; set; }
        public string Message { get; set; }
        public string ActualDownTime { get; set; }
        public string StopType { get; set; }
        public string Detail { get; set; }
    }

    public class ProductionBody
    {
        public string ProductionTime { get; set; }
        public int CurrentProduction { get; set; }
        public int RejectedItems { get; set; }
        public string Oee { get; set; }
        public string Avaibility { get; set; }
        public string Performance { get; set; }
        public string Quality { get; set; }
    }

    public class SMSModel
    {
        public string Message { get; set; }
        public List<Phone> Phones { get; set; }
        public string Company { get; set; }
        public string Expiration { get; set; }
    }

    public class Phone
    {
        public string Number { get; set; }
    }
}
