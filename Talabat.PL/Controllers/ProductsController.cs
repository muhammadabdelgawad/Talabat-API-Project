using System;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repository;
using Talabat.Core.Specifications;
using Talabat.PL.DTOs;
using Talabat.PL.Errors;
using Talabat.PL.Helper;

namespace Talabat.PL.Controllers
{
	public class ProductsController : APIBaseController
	{

		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;

		public ProductsController(  IMapper mapper,
									IUnitOfWork UnitOfWork)
        {
			_mapper = mapper;
			_unitOfWork = UnitOfWork;
		}

		//Get All Products
		[CachedAttribute(300)]
		[HttpGet]
		public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetAllProducts([FromQuery] ProductSpecParams Params)
		{
			var Spec = new ProductWithBrandAndTypeSpec(Params);
			var Products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(Spec);
			var MappedProduct = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(Products);
			var Count = Spec.Count;
			return Ok(new Pagination<ProductToReturnDto>(Params.PageSize,Params.index,MappedProduct, Count));
		}

		// Get Product Using Id
		[HttpGet("{Id}")]
		[ProducesResponseType(typeof(ProductToReturnDto),200)]
		[ProducesResponseType(typeof(ApiResponse),404)]
		public async Task<ActionResult<Product>> GetProductById(int Id)
		{
			var Spec = new ProductWithBrandAndTypeSpec(Id);
			var product = await _unitOfWork.Repository<Product>().GetByIdWithSpecAsync(Spec);
			if (product is null)
				return NotFound(new ApiResponse(404));
			var MappedProduct = _mapper.Map<Product, ProductToReturnDto>(product);
			return Ok(MappedProduct);
		}


		// Get All Brands
		[HttpGet("Brands")]
		public async Task<ActionResult<IEnumerable<ProductBrand>>> GetAllBrands()
		{
			var brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();
			return Ok(brands);
		}


		// Get All Types
		[HttpGet("Types")]
		public async Task<ActionResult<IEnumerable<ProductType>>> GetAllTypes()
		{
			var type = await _unitOfWork.Repository<ProductType>().GetAllAsync();
			return Ok(type);
		}
	}
} 
