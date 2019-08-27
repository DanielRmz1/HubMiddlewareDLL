using System;

namespace HubMiddleware.Modelos.Shared
{
    public class Production
    {
        public double OEE { get; set; }

        public double Performance { get; set; }

        public double Avaibility { get; set; }

        public double Quality { get; set; }

        public int FPY { get; set; }

        public int PPM { get; set; }

        public int LineDownTime { get; set; }

        public int IdMotivo { get; set; }

        public double Produccion { get; set; }

        public double PlannedPieces { get; set; }

        public double Pieces { get; set; }

        public int Scrap { get; set; }

        public int Rework { get; set; }

        public double Rate { get; set; }

        public double ShiftGoal { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime ShiftStart { get; set; }

        public DateTime ShiftEnd { get; set; }

        public int PartId { get; set; }

        /// <summary>
        /// Esta propiedad se repite con la propiedad de la clase MachineInformation
        /// </summary>
        public string ShiftIdOrder { get; set; }

        public int IdTurno { get; set; }

        public string Turno { get; set; }

        public DateTime DownTimeDate { get; set; }

        public string OEEOrders { get; set; }

        public double Tolerance { get; set; }

        public int PieceTime { get; set; }

        public int PieceOk { get; set; }

        public int PieceBad { get; set; }
    }
}
