using AutoMapper;
using CH.Project.Demo;
using CH.Project.Dto;

namespace CH.Project
{
    public class ProjectApplicationAutoMapperProfile : Profile
    {
        public ProjectApplicationAutoMapperProfile()
        {
            /* You can configure your AutoMapper mapping configuration here.
             * Alternatively, you can split your mapping configurations
             * into multiple profile classes for a better organization. */

            CreateMap<OrderMaster, OrderMasterDto>();
            CreateMap<OrderDetail, OrderDetailDto>();
            
        }
    }
}
