namespace msm_ws.Data.Dto
{
    public class Lote03Dto
    {
        public int? Id { get; set; }
        public int? TipoCliente { get; set; }
        public int? CodCliente { get; set; }
        public string CodProducto { get; set; }
        public string Lote { get; set; }
        public int? Cantidad { get; set; }
        public string? FechaEntrega { get; set; }
        public string? MedioEntrega { get; set; }
        public int? IdUsuario { get; set; }
        public bool? Deleted { get; set; }
    }
}        