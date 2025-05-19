using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuProyecto.Models;

namespace TAIDE.BACKEND.Models
{
    public class ReporteMedico
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("Titulo")]
        public string Titulo { get; set; }

        [Required]
        [Column("Contenido")]
        public string Contenido { get; set; }

        public DateTime Fecha { get; set; }

        [ForeignKey("PM")]
        public int PM_ID { get; set; }

        [ForeignKey("Paciente")]
        public int PacienteID { get; set; }

        public PM PM { get; set; }
        public Paciente Paciente { get; set; }
    }
}
