using API.Data.Converter.Implementations;
using API.Data.VO;
using API.Exceptions;
using API.Hypermedia.Utils;
using Database.Enum;
using Database.Extension;
using Database.Model;
using Database.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Business.Implementations
{
    public class StepBusinessImplementation :  IStepBusiness
    {
        private readonly User _requester;
        private readonly IStepRepository _repository;
        private readonly StepConverter _converter;

        public StepBusinessImplementation(User requester, IStepRepository repository)
        {
            _requester = requester;
            _repository = repository;
            _converter = new StepConverter();
        }

        public async Task<StepVO> CreateAsync(StepCreateVO vo)
        {
            if (!_requester.IsConsultantOrAdmin())
            {
                throw new UnauthorizedException("Only consultants can create steps");
            }

            if (vo.TargetUser != AccountType.CLIENT && vo.TargetUser != AccountType.CONSULTANT)
            {
                throw new FieldValidationException("invalid target user", new Data.FieldError(nameof(vo.TargetUser), $"must be equal '{AccountType.CLIENT}' or '{AccountType.CONSULTANT}'"));
            }

            var entity = _converter.Parse(vo);
            if (_requester.IsConsultant())
                entity.UserId = _requester.Id;

            bool exists = await _repository.ExistsAsync(entity.Type);
            if (exists)
            {
                throw new APIException("Type already exists");
            }
            entity = await _repository.CreateAsync(entity);
            return _converter.Parse(entity);
        }

        public async Task<StepVO> UpdateAsync(StepEditVO vo)
        {
            // TODO: BLOCK IF THERE IS ALREADY A SERVICE USING THIS STEP
            if (!_requester.IsConsultantOrAdmin())
            {
                throw new UnauthorizedException("Only consultants can edit steps");
            }
            if (vo.TargetUser != AccountType.CLIENT && vo.TargetUser != AccountType.CONSULTANT)
            {
                throw new FieldValidationException("invalid target user", new Data.FieldError(nameof(vo.TargetUser), $"must be equal '{AccountType.CLIENT}' or '{AccountType.CONSULTANT}'"));
            }
            var step = await _repository.FindByIdAsync(vo.Id);
            if (step == null)
            {
                throw new NotFoundException($"Can't find step of id {vo.Id}");
            }
            if (step.UserId == null && !_requester.IsAdmin())
            {
                throw new UnauthorizedException("Only admins can edit global steps");
            }
            if (step.UserId != null && step.UserId.Value != _requester.Id)
            {
                throw new UnauthorizedException("User is not allowed to edit this step");
            }

            var entity = _converter.Parse(vo);
            entity = await _repository.UpdateAsync(entity);
            return _converter.Parse(entity);
        }

        public async Task DeleteAsync(long id)
        {
            // TODO: BLOCK IF THERE IS ALREADY A SERVICE USING THIS STEP
            var step = await _repository.FindByIdAsync(id);
            if (step == null)
            {
                throw new NotFoundException($"Can't find step of id {id}");
            }
            if (step.UserId == null && !_requester.IsAdmin())
            {
                throw new UnauthorizedException("Only admins can delete global steps");
            }
            if (step.UserId.Value != _requester.Id)
            {
                throw new UnauthorizedException("User is not allowed to delete this step");
            }

            await _repository.DeleteAsync(id);
        }

        public async Task<List<StepVO>> FindAllAsync()
        {
            return _converter.Parse(await _repository.FindAllAsync(_requester));
        }

        public async Task<StepVO> FindByIdAsync(long id)
        {
            var step = await _repository.FindByIdAsync(id);
            if (step == null)
            {
                throw new NotFoundException($"Step id {id} not found");
            }

            if (_requester.IsAdmin())
                return _converter.Parse(step);

            if (step.UserId != null && step.UserId.Value != _requester.Id)
                throw new UnauthorizedException("User is not allowed to view this step");

            return _converter.Parse(step);
        }

        public Task<List<StepVO>> FindByTypeAsync(string type)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedSearchVO<StepVO>> FindWithPagedSearchAsync(string type, string sortDirection, int pageSize, int page)
        {
            var result = await _repository.FindWithPagedSearchAsync(type,  _requester, sortDirection, pageSize, page);
            return new PagedSearchVO<StepVO>
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
