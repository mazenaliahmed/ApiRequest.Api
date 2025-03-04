using ApiRequest.Core.DTOs;
using ApiRequest.Core.Entities;
using ApiRequest.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiRequest.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediatorController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public MediatorController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("requests")]
        public async Task<ActionResult<IEnumerable<MediatorRequestDto>>> GetAllRequests()
        {
            var requests = await _unitOfWork.Requests.GetAllAsync();
            var assignments = await _unitOfWork.RequestAssignments.GetAllAsync();

            var requestDtos = requests.Select(r => new MediatorRequestDto
            {
                RequestId = r.Id,
                RequestNumber = r.RequestNumber,
                Amount = r.Amount,
                RequestDate = r.RequestDate,
                Notes = r.Notes,
                RequestStatus = r.RequestStatus,
                UserId = r.UserId,
                ServiceProviderId = assignments.FirstOrDefault(a => a.RequestId == r.Id)?.ServiceProviderId
            });

            return Ok(requestDtos);
        }

        [HttpGet("service-providers")]
        public async Task<ActionResult<IEnumerable<ServiceProviderListItemDto>>> GetServiceProviders()
        {
            var serviceProviders = await _unitOfWork.Users.FindAsync(u => u.AccountType == "مقدم خدمة");

            var serviceProviderDtos = serviceProviders.Select(sp => new ServiceProviderListItemDto
            {
                Id = sp.Id,
                FullName = sp.FullName,
                PhoneNumber = sp.PhoneNumber,
                Address = sp.Address
            });

            return Ok(serviceProviderDtos);
        }

        [HttpPost("assign")]
        public async Task<ActionResult> AssignServiceProvider(ServiceProviderAssignmentDto assignmentDto)
        {
            var request = await _unitOfWork.Requests.GetByIdAsync(assignmentDto.RequestId);
            if (request == null)
                return NotFound("Request not found");

            var serviceProvider = await _unitOfWork.Users.GetByIdAsync(assignmentDto.ServiceProviderId);
            if (serviceProvider == null || serviceProvider.AccountType != "مقدم خدمة")
                return BadRequest("Invalid service provider");

            // Check if request is already assigned
            var existingAssignment = (await _unitOfWork.RequestAssignments
                .FindAsync(ra => ra.RequestId == assignmentDto.RequestId))
                .FirstOrDefault();

            if (existingAssignment != null)
            {
                existingAssignment.ServiceProviderId = assignmentDto.ServiceProviderId;
                existingAssignment.ExecutionStatus = assignmentDto.ExecutionStatus;
                _unitOfWork.RequestAssignments.Update(existingAssignment);
            }
            else
            {
                var assignment = new Core.Entities.RequestAssignment
                {
                    RequestId = assignmentDto.RequestId,
                    ServiceProviderId = assignmentDto.ServiceProviderId,
                    ExecutionStatus = assignmentDto.ExecutionStatus
                };
                await _unitOfWork.RequestAssignments.AddAsync(assignment);
            }

            request.RequestStatus = "Assigned";
            _unitOfWork.Requests.Update(request);

            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("requests/{requestId}/documents")]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> GetRequestDocuments(long requestId)
        {
            var documents = await _unitOfWork.RequestDocuments.FindAsync(d => d.RequestId == requestId);

            var documentDtos = documents.Select(d => new DocumentDto
            {
                Id = d.Id,
                DocumentName = d.DocumentName,
                AccountType = d.AccountType,
                UploadTime = d.UploadTime,
                DocumentContent = d.DocumentContent
            });

            return Ok(documentDtos);
        }
    }



    [Route("api/[controller]")]
    [ApiController]
    public class RequestAssignmentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RequestAssignmentsController> _logger;

        public RequestAssignmentsController(IUnitOfWork unitOfWork, ILogger<RequestAssignmentsController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost("create-assignment")]
        public async Task<IActionResult> CreateAssignment([FromBody] CreateRequestAssignmentDto dto)
        {
            _logger.LogInformation("Starting CreateAssignment for RequestID: {RequestID} and ServiceProviderID: {ServiceProviderID}",
                dto.RequestID, dto.ServiceProviderID);

            try
            {
                // Optionally, validate that the referenced Request exists:
                // var request = await _unitOfWork.Requests.GetByIdAsync(dto.RequestID);
                // if (request == null) return BadRequest("Invalid RequestID.");

                var assignment = new RequestAssignment
                {
                    RequestId = dto.RequestID,
                    ServiceProviderId = dto.ServiceProviderID,
                    // If no status is provided, use the default value
                    //ExecutionStatus = string.IsNullOrEmpty(dto.ExecutionStatus) ? "PaymentPending" : dto.ExecutionStatus,
                    ExecutionStatus = "PaymentPending",
                    Note = dto.Note,
                    Amount = dto.Amount,
                    // EntryTime will use either the default value from the database or this value.

                };

                await _unitOfWork.RequestAssignments.AddAsync(assignment);
                await _unitOfWork.SaveChangesAsync();

                // Return a standard response object (adjust StandardApiResponse to your implementation)
                var response = new StandardApiResponse<object>
                {
                    Success = true,
                    Message = "Request assignment created successfully.",
                    Data = assignment, // You can shape the returned data as needed.
                    Errors = null
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating request assignment");
                var errorResponse = new StandardApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while processing your request.",
                    Data = null,
                    Errors = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }

}
