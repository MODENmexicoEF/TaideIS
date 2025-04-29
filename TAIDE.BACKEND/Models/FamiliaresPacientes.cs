using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TuProyecto.Models
{
    [Table("FamiliaresPacientes")]
    public class PacientesFamiliares
    {
        [Key]
        [Column(Order = 0)]
        public int FamiliarID { get; set; }

        [Key]
        [Column(Order = 1)]
        public int PacienteID { get; set; }

        [ForeignKey("FamiliarID")]
        public Familiar Familiar { get; set; }

        [ForeignKey("PacienteID")]
        public Paciente Paciente { get; set; }
    }
}
