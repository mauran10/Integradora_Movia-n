using System.ComponentModel.DataAnnotations;

namespace Integradora_Movia.Models
{
    public class ConductorModel
    {
        public int IdConductor { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string CorreoElectronico { get; set; }
        public string NumeroLicencia { get; set; }
        public string Curp { get; set; }
        public string NumeroTelefono { get; set; }
        public string TipoLicencia { get; set; }

        // ⚠️ AHORA LO CORRECTO:
        public string Vencimiento { get; set; } // porque llega como string desde swagger

        public string Contrasena { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

}
