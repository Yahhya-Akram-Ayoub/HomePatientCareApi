﻿using HealthCareServiceApi.Services;
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
        public IActionResult SaveService([FromForm] string service, List<IFormFile> battlePlans, [FromForm] string Id)
        {
            try
            {
                User user = ServiceUnit.Users.GetUserBy(x => x.Id == CurrentUser.Id);
                Service _service = JsonSerializer.Deserialize<Service>(service);
                Service Service;
                if (string.IsNullOrEmpty(Id) || Id == "null")
                {
                    _service.UserId = CurrentUser.Id;
                    Service = ServiceUnit.Service.Add(_service);
                }
                else
                {
                    Service = ServiceUnit.Service.GetById(Convert.ToInt32(Id));
                    // Service.Lat = _service.Lat;
                    // Service.Lat = _service.Lng;
                    Service.AgeFrom = _service.AgeFrom;
                    Service.AgeTo = _service.AgeTo;
                    Service.TypeId = _service.TypeId;
                    Service.Attachments = _service.Attachments;
                    ServiceUnit.Service.SaveChanges();
                }

                user.Role = "Volunteer";
                ServiceUnit.Users.SaveChanges();

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

        [Route("RemoveService")]
        [HttpPost]
        [Authorize]
        public IActionResult RemoveService([FromForm] int Id)
        {
            try
            {
                ServiceUnit.Service.RemoveObj(ServiceUnit.Service.GetById(Id));
                User user = ServiceUnit.Users.GetUserBy(x => x.Id == CurrentUser.Id);
                List<Service> services = ServiceUnit.Service.GetAll(x => x.UserId == user.Id).ToList();

                if (services.Count == 0)
                {
                    user.Role = "User";
                    ServiceUnit.Users.SaveChanges();
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        public double GetUserRate(Guid id)
        {
            List<UserRating> rates = ServiceUnit.UserRating.GetAll(x => x.request.SenderId == id /*&& !x.IsVolunteer*/).ToList();
            if (rates.Count > 5)
            {
                return rates.Sum(x => x.Value) / rates.Count;
            }
            else
            {
                return 2.0;
            }
        }

        [Route("SaveRequest")]
        [HttpPost]
        [Authorize]
        public IActionResult SaveRequest([FromForm] string request, [FromForm] bool CurrentLocation, [FromForm] bool CurrentInfo)
        {
            try
            {
                Request _r = JsonSerializer.Deserialize<Request>(request);
                Request _request = new Request()
                {
                    Date = new DateTime(),
                    SenderId = CurrentUser.Id,
                    Description = _r.Description,
                    Lattiud = !CurrentLocation ? _r.Lattiud : CurrentUser.Lat,
                    Longtiud = !CurrentLocation ? _r.Longtiud : CurrentUser.Lng,
                    ExpireTime = _r.ExpireTime,
                    SeviceTypeId = _r.SeviceTypeId,
                    PGender = !CurrentInfo ? _r.PGender : CurrentUser.Gender,
                    PDescription = _r.PDescription,
                    PAge = !CurrentInfo ? _r.PAge : (DateTime.Today.Year - CurrentUser.BirthDate.Year),
                    PName = !CurrentInfo ? _r.PName : CurrentUser.Name,
                    VGender = _r.VGender,
                };
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
        [Authorize]
        [HttpGet]
        public IActionResult GetVolunteerRequest()
        {
            try
            {
                User user = CurrentUser;
                List<Request> RequestsInScope = ServiceUnit.Request.GetAll(x => x.status == 0).ToList();
                List<Service> UserServices = ServiceUnit.Service.GetAll(x => x.UserId == user.Id).ToList();
                if (UserServices.Count == 0)
                {
                    return BadRequest(new JsonResult(new { UserServices, RequestsInScope }));
                }
                List<Request> InScopeRequests = RequestsInScope.FindAll(x =>
                UserServices.FirstOrDefault(e => e.TypeId == x.SeviceTypeId && x.PAge <= e.AgeTo && x.PAge >= e.AgeFrom) != null &&
                (1000 >= CalculateDistance(x.Lattiud, x.Longtiud, user.Lat, user.Lng)));

                List<Request> AroundScopeRequests = RequestsInScope.FindAll(x =>
                         UserServices.FirstOrDefault(e => e.TypeId == x.SeviceTypeId && x.PAge <= e.AgeTo && x.PAge >= e.AgeFrom) != null &&
                        (1000 < CalculateDistance(x.Lattiud, x.Longtiud, user.Lat, user.Lng)) &&
                        (3000 >= CalculateDistance(x.Lattiud, x.Longtiud, user.Lat, user.Lng)));

                foreach (Request request in InScopeRequests)
                {
                    request.seviceType = ServiceUnit.ServiceType.GetById(request.SeviceTypeId);
                    int count = ServiceUnit.RequestReceivers.Count(x => x.RequestId == request.Id && x.UserId == user.Id);
                    if (count == 0)
                    {
                        ServiceUnit.RequestReceivers.Add(new RequestReceivers()
                        {
                            RequestId = request.Id,
                            UserId = user.Id,
                            distance = CalculateDistance(request.Lattiud, request.Longtiud, user.Lat, user.Lng)
                        });
                    }
                }

                foreach (Request request in AroundScopeRequests)
                {
                    request.seviceType = ServiceUnit.ServiceType.GetById(request.SeviceTypeId);
                    int count = ServiceUnit.RequestReceivers.Count(x => x.RequestId == request.Id && x.UserId == user.Id);
                    if (count == 0)
                    {
                        ServiceUnit.RequestReceivers.Add(new RequestReceivers()
                        {
                            RequestId = request.Id,
                            UserId = user.Id,
                            distance = CalculateDistance(request.Lattiud, request.Longtiud, user.Lat, user.Lng)
                        });
                    }
                }

                return Ok(new JsonResult(new { InScopeRequests, AroundScopeRequests }));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }


        [Route("GetProvidedList")]
        [Authorize]
        [HttpGet]
        public IActionResult GetProvidedList()
        {
            try
            {
                User user = CurrentUser;
                List<Service> Services = ServiceUnit.Service.GetAll(x => x.UserId == user.Id).ToList();
                List<ServiceType> ServiceTypes = ServiceUnit.ServiceType.GetAll(x => x.Id != -1).ToList();

                return Ok(new JsonResult(new { Services, ServiceTypes }));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("GetVolunteerRequests")]
        [Authorize]
        [HttpGet]
        public IActionResult GetVolunteerRequests()
        {
            try
            {
                User user = CurrentUser;
                List<AcceptedRequest> AcceptedRequests =
                    ServiceUnit.AcceptedRequest.GetAll(x => x.VolunteerId == CurrentUser.Id).ToList();
                List<Request> requests = new List<Request>();
                foreach (AcceptedRequest acr in AcceptedRequests)
                {
                    Request request = ServiceUnit.Request.GetById(acr.RequestId);
                    request.seviceType = ServiceUnit.ServiceType.GetById(request.SeviceTypeId);
                    requests.Add(request);
                }

                return Ok(new JsonResult(new { Success = true, AcceptedRequests, requests }));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("GetUserRequests")]
        [Authorize]
        [HttpGet]
        public IActionResult GetUserRequests()
        {
            try
            {
                User user = CurrentUser;
                List<Request> requests = ServiceUnit.Request.GetAll(x => x.SenderId == CurrentUser.Id).ToList(); ;

                List<AcceptedRequest> AcceptedRequests = new List<AcceptedRequest>();

                foreach (Request acr in requests)
                {
                    AcceptedRequest acceptedRequest = ServiceUnit.AcceptedRequest.GetUserBy(x => x.RequestId == acr.Id);
                    acr.seviceType = ServiceUnit.ServiceType.GetById(acr.SeviceTypeId);
                    AcceptedRequests.Add(acceptedRequest);
                }

                return Ok(new JsonResult(new { Success = true, AcceptedRequests, requests }));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("UserReportVolunteer")]
        [Authorize]
        [HttpPost]
        public IActionResult UserReportVolunteer([FromForm] int reqId, [FromForm] string desc)
        {
            try
            {
                User user = CurrentUser;
                AcceptedRequest request = ServiceUnit.AcceptedRequest.GetUserBy(x => x.RequestId == reqId);

                Report _report = new Report()
                {
                    RequestId = request.RequestId,
                    UserId = user.Id,
                    UserReportedId = request.VolunteerId,
                    Description = desc,
                    Date = new DateTime(),
                    Type = 0
                };

                Report report = ServiceUnit.Report.Add(_report);

                return Ok(new JsonResult(new { Success = true, report }));
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message.ToString());
            }
        }

        [Route("DeliveredRequest")]
        [Authorize]
        [HttpPost]
        public IActionResult DeliveredRequest([FromForm] int RequestId, [FromForm] string Evaluation, [FromForm] double Rate)
        {
            try
            {
                User user = CurrentUser;

                Request req = ServiceUnit.Request.GetById(RequestId);
                req.status = 3;
                ServiceUnit.Request.SaveChanges();

                DeliveredRequest delReqs = new DeliveredRequest()
                {
                    RequestId = RequestId,
                    Date = new DateTime(),
                    Evaluation = Evaluation
                };
                ServiceUnit.DeliveredRequest.Add(delReqs);

                UserRating userRate = new UserRating()
                {
                    Date = new DateTime(),
                    IsVolunteer = req.SenderId == user.Id,
                    RequestId = req.Id,
                    UserId = user.Id,
                    Description = Evaluation,
                    Value = Rate
                };
                ServiceUnit.UserRating.Add(userRate);

                return Ok(new JsonResult(new { Success = true }));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }
        [Route("FailedRequest")]
        [Authorize]
        [HttpPost]
        public IActionResult FailedRequest([FromForm] int RequestId, [FromForm] string Reason)
        {
            try
            {
                User user = CurrentUser;

                Request req = ServiceUnit.Request.GetById(RequestId);
                req.status = 2;
                ServiceUnit.Request.SaveChanges();

                FailedRequest failReqs = new FailedRequest()
                {
                    RequestId = RequestId,
                    Date = new DateTime(),
                    Reason = Reason
                };
                ServiceUnit.FailedRequest.Add(failReqs);

                return Ok(new JsonResult(new { Success = true }));
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

        [Route("AcceptRequest")]
        [HttpPost]
        [Authorize]
        public IActionResult AcceptRequest([FromForm] int Id)
        {
            try
            {

                Request req = ServiceUnit.Request.GetById(Id);
                req.status = 1;
                ServiceUnit.Request.SaveChanges();

                AcceptedRequest acr = new AcceptedRequest()
                {
                    RequestId = Id,
                    Date = new DateTime(),
                    VolunteerId = CurrentUser.Id
                };
                ServiceUnit.AcceptedRequest.Add(acr);

                return Ok(new JsonResult(new { Success = true }));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }


        [Route("GetRequest")]
        [HttpGet]
        [Authorize]
        public IActionResult GetRequest(int id)
        {
            try
            {
                Request _request = ServiceUnit.Request.GetById(id);
                _request.seviceType = ServiceUnit.ServiceType.GetById(_request.SeviceTypeId);
                double rate = GetUserRate(_request.SenderId);
                return Ok(new JsonResult(new { rate, request = _request }));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }
        [Route("GetService")]
        [HttpGet]
        [Authorize]
        public IActionResult GetService(int id)
        {
            try
            {
                Service Service = ServiceUnit.Service.GetById(id);
                return Ok(new JsonResult(new { Service }));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        //private double CalculateDistance(double lat1, double long1, double lat2, double long2)
        //{
        //    var d1 = lat1 * (Math.PI / 180.0);
        //    var num1 = long1 * (Math.PI / 180.0);
        //    var d2 = lat2 * (Math.PI / 180.0);
        //    var num2 = long2 * (Math.PI / 180.0) - num1;
        //    var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
        //             Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
        //    double result = 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        //    return result;
        //}
        public  double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;
            dist = dist * 1.609344;
            return dist;
        }
    }
}

