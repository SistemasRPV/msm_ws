
 namespace msm_ws.Data.Dto
{
    public class FotoDto
    {
        
        public int Id_Foto { get; set; }
        public int Id_User { get; set; }
        public string Nombre { get; set; }
        public string Categoria_Foto { get; set; }
        public string Detalle_Foto { get; set; }
        public string Foto64 { get; set; }
        public string Fecha_Realizacion { get; set; }
        public int Cod_Cliente { get; set; }
        public string Cadena { get; set; }
        public string Direccion { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string ubicacion { get; set; }
        public int IdVisita { get; set; }
        public bool Deleted { get; set; }
        public string IdRelOrdenMat { get; set; }
        
    }
}
