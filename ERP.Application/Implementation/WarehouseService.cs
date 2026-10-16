using ERP.Application.Interfaces;
using ERP.Domain.DTO;
using ERP.Domain.DTO.PatternResult;
using ERP.Domain.Entities;
using ERP.Infraestructure.Interfaces;
using MapsterMapper;

namespace ERP.Application.Implementation;

public class WarehouseService : IWarehouseService
{
    public const int MaxPageSize = 50;

    private readonly IWarehouseRepository _repository;
    private readonly IMapper _mapper;

    public WarehouseService(IWarehouseRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<WarehouseDto>>> GetAllAsync()
    {
        var warehouses = await _repository.GetAllAsync();
        var data = _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);
        return Result<IEnumerable<WarehouseDto>>.Success(data);
    }

    public async Task<Result<IEnumerable<WarehouseDto>>> GetPaginationAsync(int pageNumber, int pageSize)
    {
        if (pageNumber < 1 || pageSize < 1 || pageSize > MaxPageSize)
            return Result<IEnumerable<WarehouseDto>>.Failure(400, "Bad Request",
                $"pageNumber must be 1 or more and pageSize between 1 and {MaxPageSize}!");

        var warehouses = await _repository.GetPaginationAsync(pageNumber, pageSize);
        var totalRecords = await _repository.CountAsync();
        var data = _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);

        return Result<IEnumerable<WarehouseDto>>.Paged(data, pageNumber, pageSize, totalRecords);
    }

    public async Task<Result<WarehouseDto>> GetByIdAsync(int id)
    {
        var warehouse = await _repository.GetByIdAsync(id);
        if (warehouse is null)
            return Result<WarehouseDto>.Failure(404, "Not Found", $"Warehouse {id} doesn't exist!");

        var data = _mapper.Map<WarehouseDto>(warehouse);
        return Result<WarehouseDto>.Success(data);
    }

    public async Task<Result<WarehouseDto>> AddAsync(WarehouseCreateDto warehouse)
    {
        var name = warehouse.WarehouseName!.Trim();
        if (await _repository.ExistsByNameAsync(name))
            return Result<WarehouseDto>.Failure(409, "Conflict", $"Ya existe una bodega con el nombre '{name}'.");

        var entity = _mapper.Map<Warehouse>(warehouse);
        entity.WarehouseName = name;
        await _repository.AddAsync(entity);

        var data = _mapper.Map<WarehouseDto>(entity);
        return Result<WarehouseDto>.Success(data, 201, "Created", "Warehouse saved!");
    }

    public async Task<Result<WarehouseDto>> UpdateAsync(int id, WarehouseUpdateDto warehouse)
    {
        if (!await _repository.ExistsAsync(id))
            return Result<WarehouseDto>.Failure(404, "Not Found", $"Warehouse {id} doesn't exist!");

        var name = warehouse.WarehouseName!.Trim();
        if (await _repository.ExistsByNameAsync(name, id))
            return Result<WarehouseDto>.Failure(409, "Conflict", $"Ya existe una bodega con el nombre '{name}'.");

        var entity = _mapper.Map<Warehouse>(warehouse);
        entity.IdWarehouse = id;
        entity.WarehouseName = name;
        await _repository.UpdateAsync(entity);

        var data = _mapper.Map<WarehouseDto>(entity);
        return Result<WarehouseDto>.Success(data, 200, "OK", "Warehouse updated!");
    }

    public async Task<Result<int>> DeleteAsync(int id)
    {
        if (!await _repository.ExistsAsync(id))
            return Result<int>.Failure(404, "Not Found", $"Warehouse {id} doesn't exist!");

        if (await _repository.HasProductsAsync(id))
            return Result<int>.Failure(409, "Conflict", $"Warehouse {id} has products and can't be deleted!");

        await _repository.DeleteAsync(id);
        return Result<int>.Success(id, 200, "OK", "Warehouse deleted!");
    }
}
