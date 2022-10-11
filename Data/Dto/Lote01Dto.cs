namespace msm_ws.Data.Dto
{
    public class Lote01Dto
    {
        public long? Id { get; set; }
        public long? TipoCliente { get; set; }
        public string CodProducto { get; set; }
        public string Lote { get; set; }
        public long? Cantidad { get; set; }
        public string FechaEntrada { get; set; }
        public string HoraEntrada { get; set; }
        public string FechaCad { get; set; }
        public string Ubicacion { get; set; }
        public bool? Selected { get; set; }
        public bool? Deleted { get; set; }
        public string Almacen { get; set; }
    }
}        