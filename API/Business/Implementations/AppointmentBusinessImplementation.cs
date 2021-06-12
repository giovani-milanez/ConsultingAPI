﻿using API.Data.Converter.Implementations;
using API.Data.VO;
using API.Exceptions;
using API.Hypermedia.Utils;
using Database.Extension;
using Database.Model;
using Database.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Business.Implementations
{
    public class AppointmentBusinessImplementation : IAppointmentBusiness
    {
        private readonly User _requester;
        private readonly IAppointmentRepository _repository;
        private readonly IRepository<User> _userRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly AppointmentConverter _converter;
        private readonly AppointmentStepConverter _stepConverter;

        public AppointmentBusinessImplementation(User requester, IAppointmentRepository repository, IRepository<User> userRepository, IServiceRepository serviceRepository)
        {
            _requester = requester;
            _repository = repository;
            _userRepository = userRepository;
            _serviceRepository = serviceRepository;
            _converter = new AppointmentConverter();
            _stepConverter = new AppointmentStepConverter();
        }

        public async Task<AppointmentVO> CreateAsync(AppointmentCreateVO vo)
        {
            if (!_requester.IsConsultantOrAdmin())
            {
                throw new UnauthorizedException("User must be consultant to create a service");
            }

            var service = await _serviceRepository.FindByIdAsync(vo.ServiceId, nameof(Service.ServicesSteps));

            if (service == null)
            {
                throw new NotFoundException($"Service id {vo.ServiceId} not found");
            }

            if (!_requester.IsAdmin && service.UserId != _requester.Id)
            {
                throw new UnauthorizedException("User is not allowed to create an appointment for this service");
            }

            var client = await _userRepository.FindByIdAsync(vo.ClientId);

            if (client == null)
            {
                throw new NotFoundException($"Client id {vo.ClientId} not found");
            }

            if (client.Id == _requester.Id)
            {
                throw new APIException("Cannot create appointment to itself");
            }

            var entity = _converter.Parse(vo);
            entity.AppointmentSteps = service.ServicesSteps.Select(x => new AppointmentStep
            {
                StepId = x.StepId,
                IsCompleted = false,
                SubmitData = "{}"
            }).ToList();

            entity = await _repository.CreateAsync(entity);
            return await this.FindByIdAsync(entity.Id);
        }

        public async Task<AppointmentStepVO> SubmitStep(AppointmentStepSubmitVO submit)
        {
            var entity = await _repository.FindByIdAsync(submit.AppointmentId, nameof(Appointment.AppointmentSteps));

            if (entity == null)
            {
                throw new NotFoundException($"Appointment id {submit.AppointmentId} not found");
            }

            if (!_requester.IsAdmin && entity.ClientId != _requester.Id)
            {
                throw new UnauthorizedException("User is not allowed to submit this step on this appointment");
            }

            var step = entity.AppointmentSteps.FirstOrDefault(x => x.StepId == submit.StepId);
            if (step == null)
            {
                throw new NotFoundException($"Step id {submit.StepId} of appointment id {submit.AppointmentId} not found");
            }

            if (step.IsCompleted)
            {
                throw new APIException("Step already completed");
            }
            
            // TODO: Check against json-schema

            step.SubmitData = JsonSerializer.Serialize(submit.SubmitData);
            step.IsCompleted = true;
            step.DateCompleted = DateTime.UtcNow;

            if (entity.AppointmentSteps.All(x => x.IsCompleted))
            {
                entity.IsCompleted = true;
                entity.EndDate = DateTime.UtcNow;
            }

            await _repository.UpdateAsync(entity);
            return _stepConverter.Parse(step);
        }

        public async Task<AppointmentStepVO> EditStep(AppointmentStepSubmitVO submit)
        {
            var entity = await _repository.FindByIdAsync(submit.AppointmentId, nameof(Appointment.AppointmentSteps));

            if (entity == null)
            {
                throw new NotFoundException($"Appointment id {submit.AppointmentId} not found");
            }

            if (!_requester.IsAdmin && entity.ClientId != _requester.Id)
            {
                throw new UnauthorizedException("User is not allowed to submit this step on this appointment");
            }

            var step = entity.AppointmentSteps.FirstOrDefault(x => x.StepId == submit.StepId);
            if (step == null)
            {
                throw new NotFoundException($"Step id {submit.StepId} of appointment id {submit.AppointmentId} not found");
            }

            if (!step.IsCompleted)
            {
                throw new APIException("Step not completed yet");
            }

            // TODO: Check against json-schema

            step.SubmitData = JsonSerializer.Serialize(submit.SubmitData);
            step.DateCompleted = DateTime.UtcNow;

            await _repository.UpdateAsync(entity);
            return _stepConverter.Parse(step);
        }

        public async Task DeleteAsync(long id)
        {
            if (!_requester.IsAdmin)
            {
                throw new UnauthorizedException("User must be admin to delete an appointment");
            }
            var entity = await _repository.FindByIdAsync(id);

            if (entity == null)
            {
                throw new NotFoundException($"Can't find appointment of id {id}");
            }

            await _repository.DeleteAsync(id);
        }

        public async Task<List<AppointmentVO>> FindAllAsync()
        {
            return _converter.Parse(
                await _repository.FindAllAsync(_requester,
                    $"{nameof(Appointment.Service)}.{nameof(Service.User)}",
                    $"{nameof(Appointment.Service)}.{nameof(Service.ServicesSteps)}.{nameof(ServicesStep.Step)}",
                    nameof(Appointment.AppointmentSteps),
                    nameof(Appointment.Client)
                )
            );
        }

        public async Task<AppointmentVO> FindByIdAsync(long id)
        {
            if (!_requester.IsConsultantOrAdmin())
            {
                throw new UnauthorizedException("User must be consultant to view a service");
            }

            var entity = await _repository.FindByIdAsync(id,
                    $"{nameof(Appointment.Service)}.{nameof(Service.User)}",
                    $"{nameof(Appointment.Service)}.{nameof(Service.ServicesSteps)}.{nameof(ServicesStep.Step)}",
                    nameof(Appointment.AppointmentSteps),
                    nameof(Appointment.Client)
                );

            if (entity == null)
            {
                throw new NotFoundException($"Appointment id {id} not found");
            }

            if (!_requester.IsAdmin && (entity.ClientId != _requester.Id && entity.Service.UserId != _requester.Id))
            {
                throw new UnauthorizedException("User is not allowed to view this appointment");
            }

            return _converter.Parse(entity);
        }

        public async Task<PagedSearchVO<AppointmentVO>> FindWithPagedSearchAsync(string clientName, string sortDirection, int pageSize, int page)
        {
            var result = await _repository.FindWithPagedSearchAsync(clientName, _requester, sortDirection, pageSize, page);
            return new PagedSearchVO<AppointmentVO>
            {
                CurrentPage = result.CurrentPage,
                List = _converter.Parse(result.List),
                PageSize = result.PageSize,
                SortDirections = result.SortDirections,
                TotalResults = result.TotalResults
            };
        }
    }
}