﻿using POS.DomainModels.DurationByTCPDomainModels;

namespace POS.Infrastructure.Creators.Base;

public interface IDurationByTCPCreator
{
    DurationByTCP? Create(string pipelineMaterial, int pipelineDiameter, decimal pipelineLength, char appendixKey, string pipelineCategoryName);
}