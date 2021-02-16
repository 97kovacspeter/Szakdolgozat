using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ArguSzem.WebAPI.Models;
using Microsoft.AspNet.Identity;

namespace ArguSzem.WebAPI.Controllers
{
    public class PortController : ApiController
    {
        private PortContext db = new PortContext();

        // GET: api/Port
        public int GetPortModels()
        {
            var userId = User.Identity.GetUserId();
            var port = db.PortModels.Where(l => l.UserId == userId).First().Port;
            if (8000 <= port && port <= 8100)
                return port;
            return -1;
        }

        // GET: api/Port/5
        [ResponseType(typeof(PortModel))]
        public IHttpActionResult GetPortModel(int id)
        {
            PortModel portModel = db.PortModels.Find(id);
            if (portModel == null)
            {
                return NotFound();
            }

            return Ok(portModel);
        }

        // PUT: api/Port/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPortModel(int id, PortModel portModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != portModel.Id)
            {
                return BadRequest();
            }

            db.Entry(portModel).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PortModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Port
        [ResponseType(typeof(int))]
        public IHttpActionResult PostPortModel()
        {
            var portModel = new PortModel();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IEnumerable<int> allPorts = Enumerable.Range(8000, 100).ToList();

            var takenPorts = db.PortModels.Where(l => allPorts.Contains(l.Port));

            IEnumerable<int> freePorts = allPorts.Except(takenPorts.Select(l => l.Port));

            portModel.Port = freePorts.First();

            portModel.UserId = User.Identity.GetUserId();

            db.PortModels.Add(portModel);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = portModel.Id }, portModel.Port);
        }

        // DELETE: api/Port/5
        [ResponseType(typeof(PortModel))]
        public IHttpActionResult DeletePortModel(int id)
        {
            PortModel portModel = db.PortModels.Find(id);
            if (portModel == null)
            {
                return NotFound();
            }

            db.PortModels.Remove(portModel);
            db.SaveChanges();

            return Ok(portModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PortModelExists(int id)
        {
            return db.PortModels.Count(e => e.Id == id) > 0;
        }
    }
}