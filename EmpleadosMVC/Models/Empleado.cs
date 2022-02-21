using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace EmpleadosMVC.Models
{
    public class Empleado
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string Nombre { get; set; }

        [Required]
        [Range(1, 40)]
        public int Antiguedad { get; set; }

        [Required]
        [Range(18, 65)]
        public int Edad
        {
            get; set;
            
            /*get
            {
                return this.Edad;
            }
            set
            {
                if (logictest(this.Edad, Antiguedad)) { this.Edad = value; }
            }
            */
        }
        [Required]
        [StringLength(5, MinimumLength = 0)]
        public string Categoria { get; set; }

        /*[Compare("", ErrorMessage = "La Edad no es coherente con la antiguedad.")]
        private bool logictest(int Edad, int Antiguedad)
        {
            return Edad - 18 <= Antiguedad;
        }*/
    }

    public class EmpleadoDBContext : DbContext
    {
        public DbSet<Empleado> Empleados { get; set; }
    }
}