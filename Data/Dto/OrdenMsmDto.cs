namespace msm_ws.Data.Dto
{    
    public class OrdenMsmDto
    {
        public string usuario                     { get; set; }
        public string prioridad                   { get; set; }
        public string centros                     { get; set; }
        public string instrucciones               { get; set; }
        public string fecha                       { get; set; }
        public string materiales                  { get; set; }
    }

    // public class OrdenCentroDto
    // {
    //     string centro     { get; set; }
    // }
    //
    // public class OrdenMaterialesDto
    // {
    //     string material     { get; set; }
    //     int cantidad        { get; set; }
    // }
}
