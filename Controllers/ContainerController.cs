using Microsoft.AspNetCore.Mvc;
using NHibernate;
using System.Collections.Generic;
using System.Linq;

namespace Waste.Controllers
{
    public class ContainerController : ControllerBase
    {
        private readonly ISession session;
        private ITransaction transaction;
        public ContainerController(ISession session)
        {
            this.session = session;
        }


        [HttpGet]
        public List<Container> Get()
        {
            var response = session.Query<Container>().ToList();
            return response;
        }

        [HttpGet("{id}")]
        public Container Get(long id)
        {
            var response = session.Query<Container>().Where(x => x.Id == id).FirstOrDefault();
            return response;
        }

        [HttpPost]
        public void Post([FromBody] Container request)
        {

            using (var transaction = session.BeginTransaction())
            {
                Container container = session.Query<Container>().Where(x => x.Id == request.Id).FirstOrDefault();

                container.ContainerName = request.ContainerName;
                container.Latitude = request.Latitude;
                container.Longitude = request.Longitude;


                session.Save(container);
                transaction.Commit();
            }

        }

        [HttpPut]
        public ActionResult<Container> Put([FromBody] Container request)
        {
            using (var transaction = session.BeginTransaction())
            {
                Container container = session.Query<Container>().Where(x => x.Id == request.Id).FirstOrDefault();

                container.ContainerName = request.ContainerName;
                container.Latitude = request.Latitude;
                container.Longitude = request.Longitude;

                session.Update(container);
                transaction.Commit();
                return Ok();
            }


        }

        [HttpDelete]

        public ActionResult Delete(long id)
        {
            var container = session.Query<Container>().FirstOrDefault(x => x.Id == id);

            if (container == null)
                return BadRequest();

            var containers = session.Query<Container>().Where(x => x.Id == id).ToList();

            try
            {
                session.BeginTransaction();
                for (int i = 0; i < containers.Count; i++)
                {
                    session.Delete(containers[i]);
                }
                session.Delete(container);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
            }
            finally
            {
                transaction.Dispose();
            }


            return Ok();
        }

    }
}
