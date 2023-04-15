using AutoMapper;
using Core.DTOs;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CargoProject.Controllers
{
    
    public class CarrierController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly ICarrierService _service;

        public CarrierController(IMapper mapper, ICarrierService carrierService)
        {

            _mapper = mapper;
            _service = carrierService;
        }
        //Kargo firmaları listelenir
        [HttpGet]
        public async Task<IActionResult> All()
        {
            var carriers = await _service.GetAllAsync();
            var carriersDtos = _mapper.Map<List<CarrierDto>>(carriers.ToList());
            //return CreateActionResult(CustomResponseDto<List<CarrierDto>>.Success(200, carriersDtos));
            return CreateActionResult(CustomResponseDto<List<CarrierDto>>.Success(200, carriersDtos, "Tüm kargo firmaları listelendi."));

        }
        // id bilgisine göre kargo firması getirme
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var carrier = await _service.GetByIdAsync(id);
            var carriersDto = _mapper.Map<CarrierDto>(carrier);
            return CreateActionResult(CustomResponseDto<CarrierDto>.Success(200, carriersDto, $"{id} ID'li kargo firması getirildi."));

        }

        // kargo firması ekleme
        [HttpPost]
        public async Task<IActionResult> Save(CarrierDto carrierDto)
        {
            var carrier = await _service.AddAsync(_mapper.Map<Carrier>(carrierDto));
            var carriersDto = _mapper.Map<CarrierDto>(carrier);
            return CreateActionResult(CustomResponseDto<CarrierDto>.Success(201, carriersDto, "Yeni kargo firması eklendi."));

        }

        //Güncelleme işlemi
        [HttpPut]
        public async Task<IActionResult> Update(CarrierDto carrierDto)
        {
            await _service.UpdateAsync(_mapper.Map<Carrier>(carrierDto));
            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204, null, "KArgo firması bilgileri güncellendi."));

        }

        //Silme işlemi
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var carrier = await _service.GetByIdAsync(id);
            await _service.RemoveAsync(carrier);

            //return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204, null, "Kargo Firması başarılı bir şekilde silindi"));
            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204, null, $"{id} ID'li kargo firması silindi."));

        }

    }
}
