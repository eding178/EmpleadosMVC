﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmpleadosMVC.Models;
using EmpleadosMVC.Enums;
using Microsoft.AspNetCore.Cors;

namespace EmpleadosMVC.Controllers
{   


    public class EmpleadosController : Controller
    {
        private EmpleadoDBContext db = new EmpleadoDBContext();

        // GET: Empleados
        public ActionResult Index(
            string EmpleadoCategoria, 
            string BuscarNombre,
            string BuscarAntiguedad, 
            string BuscarEdad,
            ESize MenorMayor = 0,
            ECol OrderBy =0)
        {
            var antiguedad=0;
            try 
            {
                antiguedad = Convert.ToInt32(BuscarAntiguedad);
            }
            catch { }

            var ListaCategoria = new List<string>();
            var ConsultaCategoria = from e in db.Empleados orderby e.Categoria select e.Categoria;
            ListaCategoria.AddRange(ConsultaCategoria.Distinct());

            ViewBag.EmpleadoCategoria = new SelectList(ListaCategoria);
            ViewBag.MenorMayor = new SelectList(new List<string> { ESize.Menor.ToString(), ESize.Mayor.ToString() });
            ViewBag.OrderBy = new SelectList(new List<string> 
            { 
                ECol.Nombre.ToString(),
                ECol.Antiguedad.ToString(), 
                ECol.Edad.ToString(),
                ECol.Categoria.ToString()
            });
           
            var Empleados = from e in db.Empleados select e;

            //Edad filter
            if (!string.IsNullOrEmpty(BuscarEdad))
            {
                Empleados = Empleados.Where(e => e.Edad.Equals(BuscarEdad));
            }

            //Nombre filter
            if (!string.IsNullOrEmpty(BuscarNombre))
            {
                Empleados = Empleados.Where(e => e.Nombre.Contains(BuscarNombre));
            }

            //Categoria filter
            if (!string.IsNullOrEmpty(EmpleadoCategoria))
            {
                Empleados = Empleados.Where(e => e.Categoria == EmpleadoCategoria);
            }

            //Antiguedad filter
            switch (MenorMayor)
            {
                case ESize.Default:
                    break;
                case ESize.Menor:
                        Empleados = Empleados.Where(e => e.Antiguedad < antiguedad);
                    break;
                case ESize.Mayor:
                        Empleados = Empleados.Where(e => e.Antiguedad > antiguedad);
                    break;
            }

            //OrderBy
            switch (OrderBy)
            {
                case ECol.Default:
                    break;
                case ECol.Nombre:
                    Empleados = Empleados.OrderBy(e => e.Nombre);
                    break;
                case ECol.Antiguedad:
                    Empleados = Empleados.OrderBy(e => e.Antiguedad);
                    break;
                case ECol.Edad:
                    Empleados = Empleados.OrderBy(e => e.Edad);
                    break;
                case ECol.Categoria:
                    Empleados = Empleados.OrderBy(e => e.Categoria);
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

        //API
        // GET:ENDPOINT Empleados/allAPI
        [EnableCors("origins: *, headers: *, methods: *")]
        public string allAPI()
        {
            IList<Empleado> empleados = db.Empleados.ToList();
            return "Method(" + empleadoToJson(empleados) + ")";
        }

        //API
        // GET:ENDPOINT Empleados/DetailsAPI/5
        [EnableCors("origins: *, headers: *, methods: *")]
        public string DetailsAPI(int? id)
        {
            if (id != null)
            {
                Empleado empleado = db.Empleados.Find(id);
                if(empleado != null)
                    return "Method(" + empleadoToJson(empleado) + ")";
            }
            return "Method({ })";
        }
        private Empleado llenarCamposNull(Empleado empleado) 
        {
            var empleadoResult = db.Empleados.Where(e => e.ID.Equals(empleado.ID));

            if (empleado.Nombre == null)            
                empleado.Nombre = empleadoResult.First().Nombre;

            if (empleado.Categoria == null)
                empleado.Categoria = empleadoResult.First().Categoria;

            if (empleado.Edad == 0)
                empleado.Edad = empleadoResult.First().Edad;

            if (empleado.Antiguedad == 0)
                empleado.Antiguedad = empleadoResult.First().Antiguedad;

            return empleado;

        }
        //API
        // GET:ENDPOINT Empleados/EditAPI/5
        //[ValidateAntiForgeryToken]
        [HttpGet]//post o put me dan error de CORS
        [EnableCors("origins: *, headers: *, methods: *")]
        public void EditAPI( Empleado empleado)
        {
            empleado = llenarCamposNull(empleado);

            if (validacionLogicaEdadAntiguedad(empleado.Edad, empleado.Antiguedad))
            {
                var query = (from a in db.Empleados
                             where a.ID == empleado.ID
                             select a).FirstOrDefault();

                query.Nombre = empleado.Nombre;
                query.Edad = empleado.Edad;
                query.Antiguedad = empleado.Antiguedad;
                query.Categoria = empleado.Categoria;

                db.SaveChanges();
                
            } 
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

        //
        //JSON CONVERSORS
        //
        private string empleadoToJson(IList<Empleado> elist)
        {
            int totalEmpleados = db.Empleados.Count();

            string empleadoListJSON = "{\"empleado\": [";

            foreach (Empleado e in elist)
            {
                totalEmpleados -= 1;
                empleadoListJSON += "{\"Id\": \"" + e.ID + "\",\"Nombre\": \"" + e.Nombre + "\",\"Edad\": \"" + e.Edad + "\",\"Antiguedad\": \"" + e.Antiguedad + "\",\"Categoria\": \"" + e.Categoria + "\"}";
                if (totalEmpleados > 0)
                    empleadoListJSON += ",";
            }

            empleadoListJSON += "]}";

            return empleadoListJSON;
        }
        private string empleadoToJson(Empleado e)
        {
            return "{\"empleado\": {\"Id\": \"" + e.ID + "\",\"Nombre\": \"" + e.Nombre + "\",\"Edad\": \"" + e.Edad + "\",\"Antiguedad\": \"" + e.Antiguedad + "\",\"Categoria\": \"" + e.Categoria + "\"}}";
        }
    }
}
