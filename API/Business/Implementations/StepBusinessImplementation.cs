using API.Data.Converter.Implementations;
using API.Data.VO;
using API.Hypermedia.Utils;
using Database.Model;
using Database.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Business.Implementations
{
    public class StepBusinessImplementation : IStepBusiness
    {
        private readonly IStepRepository _repository;
        private readonly StepConverter _converter;

        public StepBusinessImplementation(IStepRepository repository)
        {
            _repository = repository;
            _converter = new StepConverter();
        }

        public async Task<StepVO> CreateAsync(StepVO vo)
        {
            var entity = _converter.Parse(vo);
            entity = await _repository.CreateAsync(entity);
            return _converter.Parse(entity);
        }

        public async Task<StepVO> UpdateAsync(StepVO vo)
        {
            var entity = _converter.Parse(vo);
            entity = await _repository.UpdateAsync(entity);
            return _converter.Parse(entity);
        }

        public Task DeleteAsync(long id)
        {
            return _repository.DeleteAsync(id);
        }

        public async Task<List<StepVO>> FindAllAsync()
        {
            return _converter.Parse(await _repository.FindAllAsync());
        }

        public async Task<StepVO> FindByIdAsync(long id)
        {
            return _converter.Parse(await _repository.FindByIdAsync(id));
        }

        public Task<List<StepVO>> FindByTypeAsync(string type)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedSearchVO<StepVO>> FindWithPagedSearchAsync(string type, string sortDirection, int pageSize, int page)
        {
            var result = await _repository.FindWithPagedSearchAsync(type, sortDirection, pageSize, page);
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
