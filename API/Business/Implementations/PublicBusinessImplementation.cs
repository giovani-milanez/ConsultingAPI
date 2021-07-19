using API.Data.Converter.Implementations;
using API.Data.VO;
using API.Data.VO.User;
using API.Exceptions;
using API.Hypermedia.Utils;
using Database.Extension;
using Database.Model;
using Database.Repository;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace API.Business.Implementations
{
    public class PublicBusinessImplementation : IPublicBusiness
    {
        private readonly IRepository<User> _userRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly ServiceConverter _converter;
        private readonly ConsultantConverter _consultantConverter;
        public PublicBusinessImplementation(IRepository<User> userRepository, IServiceRepository serviceRepository, FileConverter fileConverter)
        {
            _userRepository = userRepository;
            _serviceRepository = serviceRepository;
            _converter = new ServiceConverter(fileConverter);
            _consultantConverter = new ConsultantConverter(fileConverter);
        }

        public async Task<PagedSearchVO<ServiceVO>> FindWithPagedSearchAsync(string name, string sortDirection, int pageSize, int page, CancellationToken cancellationToken)
        {
            var result = await _serviceRepository
                .FindByServiceOrConsultantNameAsync(
                    name, 
                    sortDirection, 
                    pageSize, 
                    page, 
                    cancellationToken,
                    nameof(Service.Picture),
                    $"{nameof(Service.User)}.{nameof(User.ProfilePicture)}");

            return new PagedSearchVO<ServiceVO>
            {
                CurrentPage = result.CurrentPage,
                List = _converter.Parse(result.Items),
                PageSize = result.PageSize,
                TotalResults = result.TotalResults
            };
        }

        public async Task<ConsultantVO> GetConsultantByIdAsync(int id)
        {
            var user = await _userRepository.FindByIdAsync(id, false, 
                nameof(User.ProfilePicture),
                $"{nameof(User.Services)}.{nameof(Service.Picture)}");

            if (user == null || !user.IsConsultant())
                throw new NotFoundException($"Can't find consultant of id {id}");

            return _consultantConverter.Parse(user);
        }
    }
}
