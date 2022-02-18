namespace EmpleadosMVC.Migrations
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<EmpleadosMVC.Models.EmpleadoDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(EmpleadosMVC.Models.EmpleadoDBContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            context.Empleados.AddOrUpdate(i => i.Nombre, new Empleado
            {
                Nombre = "Angel Arias",
                Antiguedad = 15,
                Edad = 38, 
                Categoria = "A"
            },

            new Empleado
            {
                Nombre = "Patricia Durango",
                Antiguedad = 15,
                Edad = 36,
                Categoria = "A"
            },
            new Empleado
            {
                Nombre = "Raul Martínez",
                Antiguedad = 4,
                Edad = 25,
                Categoria = "B"
            },

            new Empleado
            {
                Nombre = "Carolina Pérez",
                Antiguedad = 2,
                Edad = 35,
                Categoria = "C"
            },

            new Empleado
            {
                Nombre = "Jacobo Ferreira",
                Antiguedad = 5,
                Edad = 28,
                Categoria = "C"
            },

            new Empleado
            {
                Nombre = "Francisco Arenas",
                Antiguedad = 3,
                Edad = 33,
                Categoria = "C"
            }
            );

        }
    }
}
