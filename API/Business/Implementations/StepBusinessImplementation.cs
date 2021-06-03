using API.Data.Converter.Implementations;
using API.Data.VO;
using API.Hypermedia.Utils;
using Database.Model;
using Database.Repository;
using System;
using System.Collections.Generic;

namespace API.Business.Implementations
{
    public class StepBusinessImplementation : IStepBusiness
    {
        private readonly IRepository<Step> _repository;
        private readonly StepConverter _converter;

        public StepBusinessImplementation(IRepository<Step> repository)
        {
            _repository = repository;
            _converter = new StepConverter();
        }

        public StepVO Create(StepVO vo)
        {
            var entity = _converter.Parse(vo);
            entity = _repository.Create(entity);
            return _converter.Parse(entity);
        }

        public StepVO Update(StepVO vo)
        {
            var entity = _converter.Parse(vo);
            entity = _repository.Update(entity);
            return _converter.Parse(entity);
        }

        public void Delete(long id)
        {
            _repository.Delete(id);
        }

        public List<StepVO> FindAll()
        {
            return _converter.Parse(_repository.FindAll());
        }

        public StepVO FindById(long id)
        {
            return _converter.Parse(_repository.FindById(id));
        }

        public List<StepVO> FindByType(string type)
        {
            throw new NotImplementedException();
        }

        public PagedSearchVO<StepVO> FindWithPagedSearch(string type, string sortDirection, int pageSize, int page)
        {
            var sort = (!string.IsNullOrWhiteSpace(sortDirection) && !sortDirection.Equals("desc")) ? "asc" : "desc";
            var size = (pageSize < 1) ? 10 : pageSize;
            var offset = page > 0 ? (page - 1) * size : 0;

            string query = @"select * from steps p where 1 = 1 ";
            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query + $" and p.type like '%{type}%' ";
            }
            query = query + $" order by p.type {sort} limit {size} offset {offset}";

            string countQuery = "select count(*) from steps p where 1 = 1 ";
            if (!string.IsNullOrWhiteSpace(type))
            {
                countQuery = countQuery + $" and p.type like '%{type}%' ";
            }
            var items = _repository.FindWithPagedSearch(query);
            int totalResults = _repository.GetCount(countQuery);
            return new PagedSearchVO<StepVO>
            {
                CurrentPage = page,
                List = _converter.Parse(items),
                PageSize = size,
                SortDirections = sort,
                TotalResults = totalResults
            };
        }
    }
}
