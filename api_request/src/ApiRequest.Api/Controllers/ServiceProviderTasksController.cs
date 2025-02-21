using ApiRequest.Core.DTOs;
using ApiRequest.Core.Entities;
using ApiRequest.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiRequest.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceProviderTasksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceProviderTasksController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceProviderTaskDto>>> GetTasks()
        {
            // In a real application, get the serviceProviderId from the authenticated user's claims
            var serviceProviderId = 1; // Temporary for demo

            var assignments = await _unitOfWork.RequestAssignments
                .FindAsync(ra => ra.ServiceProviderId == serviceProviderId);

            var tasks = new List<ServiceProviderTaskDto>();

            foreach (var assignment in assignments)
            {
                var request = await _unitOfWork.Requests.GetByIdAsync(assignment.RequestId);
                var documents = await _unitOfWork.RequestDocuments
                    .FindAsync(d => d.RequestId == assignment.RequestId);

                tasks.Add(new ServiceProviderTaskDto
                {
                    AssignmentId = assignment.Id,
                    RequestNumber = request.RequestNumber,
                    ExecutionStatus = assignment.ExecutionStatus,
                    Documents = documents.Select(d => new DocumentDto
                    {
                        Id = d.Id,
                        DocumentName = d.DocumentName,
                        AccountType = d.AccountType,
                        UploadTime = d.UploadTime,
                        DocumentContent = d.DocumentContent
                    }).ToList()
                });
            }

            return Ok(tasks);
        }

        [HttpPut("{assignmentId}/status")]
        public async Task<ActionResult> UpdateTaskStatus(long assignmentId, UpdateTaskStatusDto updateDto)
        {
            var assignment = await _unitOfWork.RequestAssignments.GetByIdAsync(assignmentId);
            if (assignment == null)
                return NotFound("Assignment not found");

            // Update assignment status
            assignment.ExecutionStatus = updateDto.ExecutionStatus;
            _unitOfWork.RequestAssignments.Update(assignment);

            // Update request status
            var request = await _unitOfWork.Requests.GetByIdAsync(assignment.RequestId);
            request.RequestStatus = updateDto.ExecutionStatus;
            _unitOfWork.Requests.Update(request);

            // Add new documents if any
            if (updateDto.NewDocuments != null && updateDto.NewDocuments.Any())
            {
                foreach (var docDto in updateDto.NewDocuments)
                {
                    var document = new RequestDocument
                    {
                        DocumentName = docDto.DocumentName,
                        RequestId = assignment.RequestId,
                        AccountType = docDto.AccountType,
                        UploadTime = DateTime.UtcNow,
                        DocumentContent = docDto.DocumentContent
                    };
                    await _unitOfWork.RequestDocuments.AddAsync(document);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{assignmentId}/documents")]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> GetTaskDocuments(long assignmentId)
        {
            var assignment = await _unitOfWork.RequestAssignments.GetByIdAsync(assignmentId);
            if (assignment == null)
                return NotFound("Assignment not found");

            var documents = await _unitOfWork.RequestDocuments
                .FindAsync(d => d.RequestId == assignment.RequestId);

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
}
