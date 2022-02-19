﻿using System;
using System.Threading;
using System.Threading.Tasks;
using BGTG.Entities;
using BGTG.Entities.POSEntities.DurationByTCPToolEntities;
using Calabonga.AspNetCore.Controllers;
using Calabonga.AspNetCore.Controllers.Records;
using Calabonga.Microservices.Core.Exceptions;
using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace BGTG.Web.Controllers.POSControllers.DurationByTCPs.Commands;

public record ExtrapolationDurationByTCPDeleteRequest(Guid Id) : OperationResultRequestBase<Guid>;

public class ExtrapolationDurationByTCPDeleteRequestHandler : OperationResultRequestHandlerBase<ExtrapolationDurationByTCPDeleteRequest, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public ExtrapolationDurationByTCPDeleteRequestHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override async Task<OperationResult<Guid>> Handle(ExtrapolationDurationByTCPDeleteRequest request, CancellationToken cancellationToken)
    {
        var operation = OperationResult.CreateResult<Guid>();
        operation.Result = request.Id;

        var repository = _unitOfWork.GetRepository<ExtrapolationDurationByTCPEntity>();
        var extrapolationDurationByTCPEntity = await repository.GetFirstOrDefaultAsync(
            predicate: x => x.Id == request.Id,
            include: x => x
                .Include(x => x.POS).ThenInclude(x => x.ConstructionObject)
                .Include(x => x.POS).ThenInclude(x => x.CalendarPlan)
                .Include(x => x.POS).ThenInclude(x => x.DurationByLC)
                .Include(x => x.POS).ThenInclude(x => x.InterpolationDurationByTCP)
                .Include(x => x.POS).ThenInclude(x => x.StepwiseExtrapolationDurationByTCP)
                .Include(x => x.POS).ThenInclude(x => x.EnergyAndWater)
        );

        if (extrapolationDurationByTCPEntity == null)
        {
            operation.AddError(new MicroserviceNotFoundException());
            return operation;
        }

        if (extrapolationDurationByTCPEntity.POS.CalendarPlan is null
            && extrapolationDurationByTCPEntity.POS.DurationByLC is null
            && extrapolationDurationByTCPEntity.POS.InterpolationDurationByTCP is null
            && extrapolationDurationByTCPEntity.POS.StepwiseExtrapolationDurationByTCP is null
            && extrapolationDurationByTCPEntity.POS.EnergyAndWater is null)
        {
            _unitOfWork.GetRepository<ConstructionObjectEntity>().Delete(extrapolationDurationByTCPEntity.POS.ConstructionObject);
        }
        else
        {
            repository.Delete(extrapolationDurationByTCPEntity);
        }

        await _unitOfWork.SaveChangesAsync();

        if (!_unitOfWork.LastSaveChangesResult.IsOk)
        {
            operation.AddError(new MicroserviceSaveChangesException());
            return operation;
        }

        return operation;
    }
}