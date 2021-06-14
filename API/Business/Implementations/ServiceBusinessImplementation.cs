using API.Data;
using API.Data.Converter.Implementations;
using API.Data.VO;
using API.Exceptions;
using API.Hypermedia.Utils;
using Database.Extension;
using Database.Model;
using Database.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Business.Implementations
{
    public class ServiceBusinessImplementation : IServiceBusiness
    {
        private readonly User _requester;
        private readonly IServiceRepository _repository;
        private readonly IStepBusiness _step;
        private readonly ServiceConverter _converter;

        public ServiceBusinessImplementation(User requester, IServiceRepository repository, IStepBusiness step)
        {
            _requester = requester;
            _repository = repository;
            _step = step;
            _converter = new ServiceConverter();
        }

        public async Task<ServiceVO> CreateAsync(ServiceCreateVO vo)
        {
            if (!_requester.IsConsultantOrAdmin())
            {
                throw new UnauthorizedException("User must be consultant to create a service");
            }

            if (vo.Steps.Count == 0)
            {
                throw new FieldValidationException("No steps given", new FieldError(nameof(vo.Steps), "empty steps"));
            }
            FieldError fieldError = null;
            foreach (var item in vo.Steps)
            {
                try
                {
                    _ = await _step.FindByIdAsync(item.StepId);
                }
                catch(NotFoundException)
                {
                    if (fieldError == null)
                    {
                        fieldError = new FieldError(nameof(vo.Steps), $"Step id {item.StepId} not found");
                    }
                    else
                    {
                        fieldError.AddError($"Step id {item.StepId} not found");
                    }
                }
                catch(UnauthorizedException)
                {
                    if (fieldError == null)
                    {
                        fieldError = new FieldError(nameof(vo.Steps), $"User not allowed to view step id {item.StepId}");
                    }
                    else
                    {
                        fieldError.AddError($"User not allowed to view step id {item.StepId}");
                    }
                }
            }

            if (fieldError != null)
            {
                throw new FieldValidationException("Invalid request", fieldError);
            }

            var entity = _converter.Parse(vo);
            entity.UserId = _requester.Id;
            entity = await _repository.CreateAsync(entity);
            return await this.FindByIdAsync(entity.Id);
        }

        public async Task<ServiceVO> UpdateAsync(ServiceEditVO vo)
        {
            // TODO: BLOCK IF THERE IS ALREADY AN APPOINTMENT FOR THIS SERVICE
            if (!_requester.IsConsultantOrAdmin())
            {
                throw new UnauthorizedException("User must be consultant to edit a service");
            }

            var found = await _repository.FindByIdAsync(vo.Id);

            if (found == null)
            {
                throw new NotFoundException($"Can't find service of id {vo.Id}");
            }

            if (!_requester.IsAdmin() && found.UserId != _requester.Id)
            {
                throw new UnauthorizedException("User is not allowed to edit this service");
            }

            if (vo.Steps.Count == 0)
            {
                throw new FieldValidationException("No steps given", new FieldError(nameof(vo.Steps), "empty steps"));
            }
            FieldError fieldError = null;
            foreach (var item in vo.Steps)
            {
                try
                {
                    _ = await _step.FindByIdAsync(item.StepId);
                }
                catch (NotFoundException)
                {
                    if (fieldError == null)
                    {
                        fieldError = new FieldError(nameof(vo.Steps), $"Step id {item.StepId} not found");
                    }
                    else
                    {
                        fieldError.AddError($"Step id {item.StepId} not found");
                    }
                }
                catch (UnauthorizedException)
                {
                    if (fieldError == null)
                    {
                        fieldError = new FieldError(nameof(vo.Steps), $"User not allowed to view step id {item.StepId}");
                    }
                    else
                    {
                        fieldError.AddError($"User not allowed to view step id {item.StepId}");
                    }
                }
            }

            if (fieldError != null)
            {
                throw new FieldValidationException("Invalid request", fieldError);
            }

            var entity = _converter.Parse(vo);
            entity = await _repository.UpdateManuallyAsync(entity);
            return _converter.Parse(entity);
        }

        public async Task<ServiceVO> FindByIdAsync(long id)
        {
            if (!_requester.IsConsultantOrAdmin())
            {
                throw new UnauthorizedException("User must be consultant to view a service");
            }

            var entity = await _repository.FindByIdAsync(id,
                    nameof(Service.ServicesSteps),
                    $"{nameof(Service.ServicesSteps)}.{nameof(ServicesStep.Step)}",
                    nameof(Service.User)
                );
            
            if (entity == null)
            {
                throw new NotFoundException($"Service id {id} not found");
            }

            if (!_requester.IsAdmin() && entity.UserId != _requester.Id)
            {
                throw new UnauthorizedException("User is not allowed to view this service");
            }

            return _converter.Parse(entity);
        }

        public async Task<List<ServiceVO>> FindAllAsync()
        {
            return _converter.Parse(
                await _repository.FindAllAsync(_requester,
                    nameof(Service.ServicesSteps),
                    $"{nameof(Service.ServicesSteps)}.{nameof(ServicesStep.Step)}",
                    nameof(Service.User)
                )
            );
        }

        public async Task<PagedSearchVO<ServiceVO>> FindWithPagedSearchAsync(string title, string sortDirection, int pageSize, int page)
        {
            var result = await _repository.FindWithPagedSearchAsync(title, _requester, sortDirection, pageSize, page);
            return new PagedSearchVO<ServiceVO>
            {
                CurrentPage = result.CurrentPage,
                List = _converter.Parse(result.List),
                PageSize = result.PageSize,
                SortDirections = result.SortDirections,
                TotalResults = result.TotalResults
            };
        }

        public async Task DeleteAsync(long id)
        {
            // TODO: BLOCK IF THERE IS ALREADY AN APPOINTMENT FOR THIS SERVICE
            if (!_requester.IsConsultantOrAdmin())
            {
                throw new UnauthorizedException("User must be consultant to delete a service");
            }
            var entity = await _repository.FindByIdAsync(id);

            if (entity == null)
            {
                throw new NotFoundException($"Can't find service of id {id}");
            }

            if (!_requester.IsAdmin() && entity.UserId != _requester.Id)
            {
                throw new UnauthorizedException("User is not allowed to delete this service");
            }

            await _repository.DeleteAsync(id);
        }
    }
}
