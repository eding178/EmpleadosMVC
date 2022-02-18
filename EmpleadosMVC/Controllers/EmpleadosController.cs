using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmpleadosMVC.Models;

namespace EmpleadosMVC.Controllers
{
    public class EmpleadosController : Controller
    {
        private EmpleadoDBContext db = new EmpleadoDBContext();

        // GET: Empleados
        public ActionResult Index(string EmpleadoCategoria, string BuscarNombre, string MenorMayor, string BuscarAntiguedad, string BuscarEdad)
        {
           
            var ListaCategoria = new List<string>();
            var ConsultaCategoria = from gq in db.Empleados orderby gq.Categoria select gq.Categoria; ListaCategoria.AddRange(ConsultaCategoria.Distinct());
            ViewBag.EmpleadoCategoria = new SelectList(ListaCategoria);
            ViewBag.MenorMayor = new SelectList(new List<string> { "Menor que", "Mayor que" });
            var Empleados = from cr in db.Empleados select cr;

            var antiguedad = 0;
            try
            {
                antiguedad = Convert.ToInt32(BuscarAntiguedad);
            }
            catch
            {
                return View(Empleados);
            }

            //Name filter
            if (!String.IsNullOrEmpty(BuscarNombre))
            {
                Empleados = Empleados.Where(c => c.Nombre.Contains(BuscarNombre));
            }
            //Category filter
            if (!string.IsNullOrEmpty(EmpleadoCategoria))
            {
                Empleados = Empleados.Where(g => g.Categoria == EmpleadoCategoria);
            }
            //Antiguedad filter

            if (MenorMayor != "")
            switch (MenorMayor)
            {
                case "Menor que":
                        Empleados = Empleados.Where(e => e.Antiguedad < antiguedad);
                    break;
                case "Mayor que":
                        Empleados = Empleados.Where(e => e.Antiguedad > antiguedad);
                    break;
            }

            return View(Empleados);
        }



        // GET: Empleados/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empleado empleado = db.Empleados.Find(id);
            if (empleado == null)
            {
                return HttpNotFound();
            }
            return View(empleado);
        }

        // GET: Empleados/Create

        
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult CreateError()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateError([Bind(Include = "ID,Nombre,Antiguedad,Edad,Categoria")] Empleado empleado)
        {
            if (!ModelState.IsValid)
            {
                return View(empleado);
            }

            if (!validacionLogicaEdadAntiguedad(empleado.Edad, empleado.Antiguedad))
            {

                return RedirectToAction("CreateError");
            }

            db.Empleados.Add(empleado);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private bool validacionLogicaEdadAntiguedad(int edad, int antiguedad) 
        {
            return edad - 18 >= antiguedad;
        }

        // POST: Empleados/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nombre,Antiguedad,Edad,Categoria")] Empleado empleado)
        {
            if (!ModelState.IsValid)
            {
                return View(empleado);
            }
            
            if (!validacionLogicaEdadAntiguedad(empleado.Edad, empleado.Antiguedad))
            {
                return RedirectToAction("CreateError");
            }

            db.Empleados.Add(empleado);
            db.SaveChanges();
            return RedirectToAction("Index");


        }

        // GET: Empleados/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empleado empleado = db.Empleados.Find(id);
            if (empleado == null)
            {
                return HttpNotFound();
            }
            return View(empleado);
        }

        // POST: Empleados/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nombre,Antiguedad,Edad,Categoria")] Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                if(validacionLogicaEdadAntiguedad(empleado.Edad, empleado.Antiguedad)) 
                { 
                db.Entry(empleado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
                }
            }
            return View(empleado);
        }

        // GET: Empleados/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empleado empleado = db.Empleados.Find(id);
            if (empleado == null)
            {
                return HttpNotFound();
            }
            return View(empleado);
        }

        // POST: Empleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Empleado empleado = db.Empleados.Find(id);
            db.Empleados.Remove(empleado);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
