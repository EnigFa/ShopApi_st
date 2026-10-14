using MediatR;
using Shop.Application.DTOs.ProductDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Queries.Product;

public record GetProductByIdQuery(int id) : IRequest<ProductReadDTO?>;