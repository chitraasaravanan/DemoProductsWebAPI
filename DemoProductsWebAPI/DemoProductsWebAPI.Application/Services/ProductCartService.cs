using AutoMapper;
using DemoProductsWebAPI.Common.DTOs;
using DemoProductsWebAPI.Common.Interfaces;
using DemoProductsWebAPI.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace DemoProductsWebAPI.Application.Services
{
    public class ProductCartService(IUnitOfWork uow, IMapper mapper, ILogger<ProductCartService> logger) : IProductCartService
    {
        private readonly IUnitOfWork _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly ILogger<ProductCartService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<ProductCartDto> CreateAsync(ProductCartDto dto, CancellationToken cancellationToken = default)
        {
            var item = _mapper.Map<ProductCart>(dto);
            await _uow.ProductCarts.AddAsync(item, cancellationToken).ConfigureAwait(false);
            await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return _mapper.Map<ProductCartDto>(item);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var item = await _uow.ProductCarts.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (item == null) return false;
            _uow.ProductCarts.Remove(item);
            await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return true;
        }

        public async Task<IEnumerable<ProductCartDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var list = await _uow.ProductCarts.GetAllAsync(cancellationToken).ConfigureAwait(false);
            return list.Select(i => _mapper.Map<ProductCartDto>(i));
        }

        public async Task<ProductCartDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var item = await _uow.ProductCarts.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
            return item == null ? null : _mapper.Map<ProductCartDto>(item);
        }

        public async Task<bool> UpdateAsync(ProductCartDto dto, CancellationToken cancellationToken = default)
        {
            var existing = await _uow.ProductCarts.GetByIdAsync(dto.Id, cancellationToken).ConfigureAwait(false);
            if (existing == null) return false;
            _mapper.Map(dto, existing);
            _uow.ProductCarts.Update(existing);
            await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return true;
        }
    }
}
