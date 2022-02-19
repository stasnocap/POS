﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BGTG.Entities.Core;
using Calabonga.EntityFrameworkCore.Entities.Base;

namespace BGTG.Web.ViewModels.POSViewModels.DurationByTCPViewModels
{
    public class DurationByTCPCreateViewModel : IViewModel, IValidatableObject
    {
        public string ObjectCipher { get; set; } = null!;
        public char AppendixKey { get; set; }
        public string PipelineCategoryName { get; set; } = null!;
        public string PipelineMaterial { get; set; } = null!;
        public int PipelineDiameter { get; set; }
        public decimal PipelineLength { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(ObjectCipher) || !(AppData.ObjectCipherExpression1.IsMatch(ObjectCipher) || AppData.ObjectCipherExpression2.IsMatch(ObjectCipher)))
            {
                yield return new ValidationResult(AppData.ObjectCipherValidationMessage);
            }

            if (PipelineDiameter <= 0)
            {
                yield return new ValidationResult(AppData.PipelineDiameterValidationMessage);

            }

            if (PipelineLength <= 0)
            {
                yield return new ValidationResult(AppData.PipelineLengthValidationMessage);
            }
        }
    }
}