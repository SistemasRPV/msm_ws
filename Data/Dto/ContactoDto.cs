﻿
 namespace msm_ws.Data.Dto
{
    public class ContactoDto
    {
        public int IdContacto { get; set; }
        public int IdVisita { get; set; }
        public string Responsable { get; set; }
        public string Cargo { get; set; }
        public string Observaciones { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public bool deleted { get; set; }
    }
}
