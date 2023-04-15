using AutoMapper;
using Core.DTOs;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CargoProject.Controllers
{
 
    public class CarrierConfigurationController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly ICarrierConfigurationService _service;

        public CarrierConfigurationController(IMapper mapper, ICarrierConfigurationService carrierConfigurationService)
        {
            _mapper = mapper;
            _service = carrierConfigurationService;
        }

        //Kargo firma konfigürasyonu listeleme
        [HttpGet]
        public async Task<IActionResult> All()
        {
            var carrierConfigurations = await _service.GetAllAsync();
            var carrierConfigurationsDtos = _mapper.Map<List<CarrierConfigurationDto>>(carrierConfigurations.ToList());
            return CreateActionResult(CustomResponseDto<List<CarrierConfigurationDto>>.Success(200, carrierConfigurationsDtos, "Tüm Kargo Firma Konfigürasyonları Listelendi"));
        }

        //id bilgisine göre Kargo firma konfigürasyonu listeleme
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {


            var carrierConfiguration = await _service.GetByIdAsync(id);
            var carrierConfigurationsDto = _mapper.Map<CarrierConfigurationDto>(carrierConfiguration);
            return CreateActionResult(CustomResponseDto<CarrierConfigurationDto>.Success(200, carrierConfigurationsDto, $"{id} ID'li kargo firma konfigürasyon bilgileri getirildi."));
        }

        //Kargo firma konfigürasyonu ekleme
        [HttpPost]
        public async Task<IActionResult> Save(CarrierConfigurationDto carrierConfigurationDto)
        {
            var carrierConfiguration = await _service.AddAsync(_mapper.Map<CarrierConfiguration>(carrierConfigurationDto));
            var carrierConfigurationsDto = _mapper.Map<CarrierConfigurationDto>(carrierConfiguration);
            return CreateActionResult(CustomResponseDto<CarrierConfigurationDto>.Success(201, carrierConfigurationsDto, "Yeni Kargo Firma Konfigürasyonu eklendi"));
        }

        //Kargo firma konfigürasyonu güncelleme
        [HttpPut]
        public async Task<IActionResult> Update(CarrierConfigurationDto carrierConfigurationDto)
        {
            await _service.UpdateAsync(_mapper.Map<CarrierConfiguration>(carrierConfigurationDto));

            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204, null, "Kargo Firma Konfigürasyonu Güncellendi"));
        }

        //Kargo firma konfigürasyonu silme
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var carrierConfiguration = await _service.GetByIdAsync(id);
            await _service.RemoveAsync(carrierConfiguration);

            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204, null, $"{id} ID'li kargo firma konfigürasyonu silindi."));
        }
    }
}
