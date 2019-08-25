using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabTest.Repository.Registration;
using LabTest.Repository.Task;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LabTest.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IRegistrationRepository _registrationRepository;
        public TaskController(ITaskRepository taskRepository,IRegistrationRepository registrationRepository)
        {
            _taskRepository = taskRepository;
            _registrationRepository = registrationRepository;
        }

        // GET: api/Task
        [HttpGet]
        public async Task<IActionResult> Get(int? assainedId)
        {
            var list = await _taskRepository.Get(assainedId);
            foreach(var task in list)
            {
                task.Users = await _registrationRepository.GetByIdAysnc(task.AsaainTo);
            }
            if (list.Any())
                return Ok(list);
            else
                return NotFound();
        }

        //GET: api/Task/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var task = await _taskRepository.GetByIdAysnc(id);
                if (task == null)
                {
                    return NotFound();
                }

                return Ok(task);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        // POST: api/Registration
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Models.Task task)
        {

            try
            {
                if (task == null)
                    return BadRequest();
                if (task.TaskId == 0)
                {
                    var result = await _taskRepository.Insert(task);
                    if (result != null)
                        return Ok(result);
                    else
                        return BadRequest();
                }
                else
                {
                    var result = await _taskRepository.Update(task);
                    if (result != null)
                        return Ok(result);
                    else
                        return BadRequest();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        //GET: api/Task/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var task = await _taskRepository.Delete(id);              
                return Ok(task);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    
}
}