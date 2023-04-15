using AutoMapper;
using Core.DTOs;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace CargoProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IOrderService _service;
        private readonly AppDBContext _context;

        public OrderController(IMapper mapper, IOrderService orderService, AppDBContext context)
        {
            _mapper = mapper;
            _service = orderService;
            _context = context;
        }

        //Siparişler listelenir
        [HttpGet]
        public async Task<IActionResult> All()
        {
            var orders = await _service.GetAllAsync();
            var ordersDtos = _mapper.Map<List<OrderDto>>(orders.ToList());
            return CreateActionResult(CustomResponseDto<List<OrderDto>>.Success(200, ordersDtos, "siparişler başarılı bir şekilde listelendi"));
        }

        // id bilgisine göre sipariş listelenir
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {


            var order = await _service.GetByIdAsync(id);
            var ordersDto = _mapper.Map<OrderDto>(order);
            return CreateActionResult(CustomResponseDto<OrderDto>.Success(200, ordersDto, $"{id} ID'li sipariş listelendi"));
        }


            //Sipariş eklenir
            [HttpPost]
            [Route("add")]
            public async Task<ActionResult> Save(OrderDto orderDto)
            {
                var carrier = await _context.Carriers.Include(c => c.CarrierConfigurations).FirstOrDefaultAsync(c => c.CarrierIsActive);

                if (carrier == null)
                    return BadRequest("Aktif kargo firması bulunamadı.");

                decimal carrierCost = 0;

                // Siparişin kargo firmasına ait Min-Max aralığında olup olmadığını kontrol et
                var carrierConfig = carrier.CarrierConfigurations
                    .FirstOrDefault(c => c.CarrierMinDesi <= orderDto.OrderDesi && c.CarrierMaxDesi >= orderDto.OrderDesi);

                if (carrierConfig != null)
                {
                    // Kargo firmasına ait standart fiyat hesaplanır
                    carrierCost = carrierConfig.CarrierCost;
                }
                else
                {
                    // Kargo firmasına ait standart fiyat hesaplanır
                    var nearestConfig = carrier.CarrierConfigurations
                        .OrderBy(c => Math.Abs(c.CarrierMinDesi - orderDto.OrderDesi))
                        .FirstOrDefault();

                    if (nearestConfig == null)
                        return BadRequest("Kargo firmasına ait konfigürasyon bulunamadı.");

                    // Kargo firmasına ait standart fiyat hesaplanır
                    decimal standardCost = nearestConfig.CarrierCost;

                    // Fark fiyatı hesaplanır
                    int difference = orderDto.OrderDesi - nearestConfig.CarrierMinDesi;

                    // Fark fiyatı ile +1 desi fiyatı çarpılır ve standart fiyata eklenir
                    carrierCost = standardCost + (difference * carrier.CarrierPlusDesiCost);
                }

                // Yeni sipariş oluşturulur
                var newOrder = new Order
                {
                    CarrierId = carrier.CarrierId,
                    OrderDesi = orderDto.OrderDesi,
                    OrderDate = DateTime.Now,
                    OrderCarrierCost = carrierCost
                };

                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync();
                string response = $"Sipariş başarıyla eklendi. Sipariş numarası: {newOrder.OrderId}";
                return Ok(response);
            }
        

        //Sipariş silinir
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var order = await _service.GetByIdAsync(id);

            await _service.RemoveAsync(order);

            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204, null, "sipariş başarılı bir şekilde silindi"));
        }
    }
}
