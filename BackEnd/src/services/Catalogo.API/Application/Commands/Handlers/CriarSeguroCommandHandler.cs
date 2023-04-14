using AutoMapper;
using FluentValidation.Results;
using MediatR;
using Core.Enum;
using Core.Messages;
using Core.Messages.Integration;
using Core.Utils;
using Core.Validation;
using MessageBus;
using Catalogo.API.Application.Events;
using Catalogo.API.DTO;
using Catalogo.API.Models.Entities;
using Catalogo.API.Models.Repositories;
using Catalogo.API.Services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Catalogo.API.Application.Commands.Handlers
{
    public class CriarSeguroCommandHandler : CommandHandler, 
            IRequestHandler<CriarSeguroCommand, ValidationResult>
    {

        private readonly ICertificadoRepository _certificadoRepository;
        private readonly IMapper _mapper;
        private readonly IMessageBus _bus;





        public CriarSeguroCommandHandler(ICertificadoRepository certificadoRepository,
                                         IMapper mapper, IMessageBus bus)
        {
            _certificadoRepository = certificadoRepository;
            _mapper = mapper;
            _bus = bus;
        }


        public async Task<ValidationResult> Handle(CriarSeguroCommand seguroCommand, CancellationToken cancellationToken)
        {

            return seguroCommand.ValidationResult;


        }

    }
}
