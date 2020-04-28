using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Methods.Core.DTOs;
using Methods.Core.IKafka;
using Methods.Core.Messages.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Methods.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Homecontroller : ControllerBase
    {
        private readonly IKafkaProducer _kafkaProducer;
        private readonly IConfiguration Configuration;
        private readonly string METHODS_TOPIC;

        public Homecontroller(IKafkaProducer kafkaProducer, IConfiguration configuration)
        {
            _kafkaProducer = kafkaProducer;
            Configuration = configuration;
            METHODS_TOPIC = Configuration["MethodsTopic"];
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Up and running..." + METHODS_TOPIC);
        }

        [HttpPost]
        public IActionResult CreateMethod([FromBody] MethodDTO methodDTO)
        {
            long loggedInUserId = GetLoggedInUserIdMockUp();

            if (loggedInUserId == -1)
                return Unauthorized();

            CreateMethod createMethod =
                new CreateMethod(methodDTO.Creator, methodDTO.Name, methodDTO.ApplicationRate, loggedInUserId);

            _kafkaProducer.Produce(createMethod, METHODS_TOPIC);

            return Ok("Currently processing your request...");
        }

        [Route("CreateMethods")]
        [HttpPost]
        public IActionResult CreateMethods([FromBody] List<MethodDTO> methodsDTO)
        {
            long loggedInUserId = GetLoggedInUserIdMockUp();

            if (loggedInUserId == -1)
                return Unauthorized();

            List<CreateMethod> createMethodsList =
                methodsDTO.Select(m => new CreateMethod(m.Creator, m.Name, m.ApplicationRate, loggedInUserId)).ToList();

            CreateMethods createMethods =
                new CreateMethods(createMethodsList, loggedInUserId);

            _kafkaProducer.Produce(createMethods, METHODS_TOPIC);

            return Ok("Currently processing your request...");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMethod(long id)
        {
            long loggedInUserId = GetLoggedInUserIdMockUp();

            if (loggedInUserId == -1)
                return Unauthorized();

            DeleteMethod deleteMethod = new DeleteMethod(id, loggedInUserId);

            _kafkaProducer.Produce(deleteMethod, METHODS_TOPIC);

            return Ok("Currently processing your request...");
        }

        [Route("DeleteMethods")]
        [HttpPost]
        public IActionResult DeleteMethods([FromBody] List<long> ids)
        {
            long loggedInUserId = GetLoggedInUserIdMockUp();

            if (loggedInUserId == -1)
                return Unauthorized();

            Console.Write(ids);

            DeleteMethods deleteMethods = new DeleteMethods(ids, loggedInUserId);

            _kafkaProducer.Produce(deleteMethods, METHODS_TOPIC);

            return Ok("Currently processing your request...");
        }

        [HttpPut("{id}")]
        public IActionResult UpdateMethod(long id, [FromBody]MethodDTO methodDTO)
        {
            long loggedInUserId = GetLoggedInUserIdMockUp();

            if (loggedInUserId == -1)
                return Unauthorized();

            UpdateMethod updateMethod =
                new UpdateMethod(id, methodDTO.Creator, methodDTO.Name, methodDTO.ApplicationRate, loggedInUserId);

            _kafkaProducer.Produce(updateMethod, METHODS_TOPIC);

            return Ok("Currently processing your request...");
        }

        private long GetLoggedInUserIdMockUp()
        {
            var authorizationHeader = Request.Headers[HeaderNames.Authorization].ToString();
            if (authorizationHeader == "")
                return -1;

            string jwtInput = authorizationHeader.Split(' ')[1];

            var jwtHandler = new JwtSecurityTokenHandler();

            if (!jwtHandler.CanReadToken(jwtInput)) throw new Exception("The token doesn't seem to be in a proper JWT format.");

            var token = jwtHandler.ReadJwtToken(jwtInput);

            var jwtPayload = JsonConvert.SerializeObject(token.Claims.Select(c => new { c.Type, c.Value }));

            JArray rss = JArray.Parse(jwtPayload);
            var firstChild = rss.First;
            var lastChild = firstChild.Last;
            var idString = lastChild.Last.ToString();

            long.TryParse(idString, out long id);

            return id;
        }

    }
}

/*
 * Flow
 * Api Controller: Creates the command and calls kafka producer produce method
 * Kafka Producer: Produce(Message)
 * Kafka Consumer: Catch Message and Pass to corresponding Handler
 * MethodCommandHandler: Handle(message) => calls repo
 * MethodReposistory: Alters Db and returns Result
 * MethodCommandHandler: Produce(Event) of either Success Or Failure
 */