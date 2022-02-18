﻿using BGTG.Entities.POS;
using BGTG.Entities.POS.DurationByTCPToolEntities;
using BGTG.Web.Infrastructure.Mappers.Base;
using BGTG.Web.ViewModels.POS;

namespace BGTG.Web.Infrastructure.Mappers.POS
{
    public class POSMapperConfiguration : MapperConfigurationBase
    {
        public POSMapperConfiguration()
        {
            CreateMap<POSEntity, POSViewModel>()
                .ForMember(x => x.CalendarPlanViewModel, o => o.MapFrom(x => x.CalendarPlan))
                .ForMember(x => x.DurationByLCViewModel, o => o.MapFrom(x => x.DurationByLC))
                .ForMember(x => x.DurationByTCPViewModel, o => o.MapFrom((s, d) =>
                {
                    if (s.InterpolationDurationByTCP != null)
                    {
                        return (DurationByTCPEntity)s.InterpolationDurationByTCP;
                    }

                    if (s.ExtrapolationDurationByTCP != null)
                    {
                        return (DurationByTCPEntity)s.ExtrapolationDurationByTCP;
                    }

                    if (s.StepwiseExtrapolationDurationByTCP != null)
                    {
                        return (DurationByTCPEntity)s.StepwiseExtrapolationDurationByTCP;
                    }

                    return null;
                }))
                .ForMember(x => x.EnergyAndWaterViewModel, o => o.MapFrom(x => x.EnergyAndWater));
        }
    }
}