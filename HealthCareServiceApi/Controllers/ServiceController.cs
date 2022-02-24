using HealthCareServiceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using ModelsRepository;
using ModelsRepository.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace HealthCareServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : CustomControllerBase
    {
        private readonly IConfiguration _config;
        public ServiceController(IConfiguration config, IServiceUnit serviceunit) : base(serviceunit)
        {
            _config = config;
        }

        [Route("SaveServiceType")]
        [HttpPost]
        public IActionResult SaveServiceType(ServiceType _serviceType)
        {
            try
            {
                ServiceType ServiceType = ServiceUnit.ServiceType.Add(_serviceType);
                return Ok(ServiceType);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("SaveService")]
        [HttpPost]
        [Authorize]
        public IActionResult SaveService([FromForm] string ServiceType, List<IFormFile> battlePlans)
        {
            try
            {
                Service _service = JsonSerializer.Deserialize<Service>(ServiceType);
                _service.UserId = CurrentUser.Id;
                Service Service = ServiceUnit.Service.Add(_service);

                if (battlePlans != null && battlePlans.Count > 0)
                {
                    var path = String.Concat(_config["Directories:ServiceAttachment"], Service.Id, "/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    foreach (IFormFile file in battlePlans)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            //var myUniqueFileName = Convert.ToString(Guid.NewGuid());
                            //var fileExtension = Path.GetExtension(fileName);
                            //var newFileName = String.Concat(myUniqueFileName, fileExtension);

                            using (FileStream fs = System.IO.File.Create(String.Concat(path, fileName)))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }

                            ServiceUnit.ServiceAttachment.Add(new ServiceAttachment()
                            { Attachment = fileName.ToString(), ServiceId = Service.Id });
                        }
                    }
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("SaveRequest")]
        [HttpPost]
        public IActionResult SaveRequest(Request _request)
        {
            try
            {
                Request Request = ServiceUnit.Request.Add(_request);
                return Ok(Request);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("SaveDeliveredRequest")]
        [HttpPost]
        public IActionResult SaveDeliveredRequest(DeliveredRequest _deliveredRequest)
        {
            try
            {
                DeliveredRequest DeliveredRequest = ServiceUnit.DeliveredRequest.Add(_deliveredRequest);
                return Ok(DeliveredRequest);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("SaveAcceptedRequest")]
        [HttpPost]
        public IActionResult SaveAcceptedRequest(AcceptedRequest _acceptedRequest)
        {
            try
            {
                AcceptedRequest AcceptedRequest = ServiceUnit.AcceptedRequest.Add(_acceptedRequest);
                return Ok(AcceptedRequest);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("SaveFailedRequest")]
        [HttpPost]
        public IActionResult SaveFailedRequest(FailedRequest _failedRequest)
        {
            try
            {
                FailedRequest FailedRequest = ServiceUnit.FailedRequest.Add(_failedRequest);
                return Ok(FailedRequest);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("SaveUserRating")]
        [HttpPost]
        public IActionResult SaveUserRating(UserRating _userRating)
        {
            try
            {
                UserRating UserRating = ServiceUnit.UserRating.Add(_userRating);
                return Ok(UserRating);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("SaveReport")]
        [HttpPost]
        public IActionResult SaveReport(Report _report)
        {
            try
            {
                Report Report = ServiceUnit.Report.Add(_report);
                return Ok(Report);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("GetVolunteerRequest")]
        [HttpPost]
        public IActionResult GetVolunteerRequest()
        {
            try
            {
                User user = CurrentUser;
                IEnumerable<Request> RequestsInScope =
                    ServiceUnit.Request.GetAll(x => (1000 >= CalculateDistance(x.Lattiud, x.Longtiud, user.Lattiud, user.Longtiud)));

                IEnumerable<Request> RequestsAroundScope =
                  ServiceUnit.Request.GetAll(x => (3000 >= CalculateDistance(x.Lattiud, x.Longtiud, user.Lattiud, user.Longtiud)
                  && 1000 < CalculateDistance(x.Lattiud, x.Longtiud, user.Lattiud, user.Longtiud)));

                foreach (Request request in RequestsInScope)
                {
                    int count = ServiceUnit.RequestReceivers.Count(x => x.RequestId == request.Id && x.UserId == user.Id);
                    if (count == 0)
                    {
                        ServiceUnit.RequestReceivers.Add(new RequestReceivers()
                        {
                            RequestId = request.Id,
                            UserId = user.Id,
                            distance = CalculateDistance(request.Lattiud, request.Longtiud, user.Lattiud, user.Longtiud)
                        });
                    }
                }

                foreach (Request request in RequestsAroundScope)
                {
                    int count = ServiceUnit.RequestReceivers.Count(x => x.RequestId == request.Id && x.UserId == user.Id);
                    if (count == 0)
                    {
                        ServiceUnit.RequestReceivers.Add(new RequestReceivers()
                        {
                            RequestId = request.Id,
                            UserId = user.Id,
                            distance = CalculateDistance(request.Lattiud, request.Longtiud, user.Lattiud, user.Longtiud)
                        });
                    }
                }

                return new JsonResult(RequestsInScope, RequestsAroundScope);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }
        [Route("GetServicesType")]
        [HttpGet]
        [Authorize]
        public IActionResult GetServicesType()
        {
            try
            {
                User u = CurrentUser;
                IEnumerable<ServiceType> Services = ServiceUnit.ServiceType.GetAll(x => x.Id != -1);
                return new JsonResult(new { Services });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }


        private double CalculateDistance(double lat1, double long1, double lat2, double long2)
        {
            var d1 = lat1 * (Math.PI / 180.0);
            var num1 = long1 * (Math.PI / 180.0);
            var d2 = lat2 * (Math.PI / 180.0);
            var num2 = long2 * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                     Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }
    }
}

