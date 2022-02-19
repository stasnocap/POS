﻿using BGTG.Entities.POSEntities.DurationByLCToolEntities;
using BGTG.POS.DurationTools.DurationByLCTool;
using BGTG.Web.Infrastructure.Mappers.Base;
using BGTG.Web.ViewModels.POSViewModels.DurationByLCViewModels;
using Calabonga.UnitOfWork;

namespace BGTG.Web.Infrastructure.Mappers.POSMapperConfigurations
{
    public class DurationByLCMapperConfiguration : MapperConfigurationBase
    {
        public DurationByLCMapperConfiguration()
        {
            CreateMap<DurationByLCEntity, DurationByLCViewModel>()
                .ForMember(x => x.CreatedAt, o => o.MapFrom(x => x.CreatedAt.ToLocalTime()));

            CreateMap<DurationByLCEntity, DurationByLC>();

            CreateMap<IPagedList<DurationByLCEntity>, IPagedList<DurationByLCViewModel>>()
                .ConvertUsing<PagedListConverter<DurationByLCEntity, DurationByLCViewModel>>();

            CreateMap<DurationByLC, DurationByLCEntity>()
                .ForMember(x => x.POSId, o => o.Ignore())
                .ForMember(x => x.POS, o => o.Ignore())
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.CreatedAt, o => o.Ignore())
                .ForMember(x => x.CreatedBy, o => o.Ignore())
                .ForMember(x => x.UpdatedAt, o => o.Ignore())
                .ForMember(x => x.UpdatedBy, o => o.Ignore());
        }
    }
}