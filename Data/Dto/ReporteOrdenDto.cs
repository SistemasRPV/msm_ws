﻿
 namespace msm_ws.Data.Dto
{
    public class ReporteOrdenDto
    {
        public int IdOrden { get; set; }
        public string Centro { get; set; }
        public string Rotulo { get; set; }
        public int IdUsuario { get; set; }
        public string FechaOrden { get; set; }
        public string Semana { get; set; }
        public string Instrucciones { get; set; }
        public int IdrelOrdenMat { get; set; }
        public int CodMaterial { get; set; }
        public string DescMaterial { get; set; }
        public int Cantidad { get; set; }
        public string TipoMat { get; set; }
        public int IdVisita { get; set; }
        public string FechaVisita { get; set; }
        public int IdROrden { get; set; }
        public int Estado { get; set; }
        public string EstadoTexto { get; set; }
        public string Observaciones { get; set; }
        public int IdrOrdenMat { get; set; }
        public int CantidadR { get; set; }
        public int Motivo { get; set; }
        public string MotivoTexto { get; set; }
    }
}
