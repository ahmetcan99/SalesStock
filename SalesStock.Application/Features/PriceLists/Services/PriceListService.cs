using AutoMapper;
using AutoMapper.QueryableExtensions;
using SalesStock.Application.Features.PriceLists.DTOs;
using SalesStock.Application.Interfaces;
using SalesStock.Domain.Entities;

namespace SalesStock.Application.Features.PriceLists.Services
{
    public class PriceListService : IPriceListService
    {
        private readonly IPriceListRepository _priceListRepository;
        private readonly IMapper _mapper;
        public PriceListService(IPriceListRepository priceListRepository, IMapper mapper)
        {
            _priceListRepository = priceListRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PriceListDTO>> GetAllPriceListsAsync()
        {
            var priceLists = await _priceListRepository.GetAllSortedByIdAsync();
            return _mapper.Map<IEnumerable<PriceListDTO>>(priceLists);
        }
    }
}
