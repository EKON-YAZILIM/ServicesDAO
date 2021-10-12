using AutoMapper;
using PagedList.Core;
using System.Collections.Generic;

namespace DAO_DbService.Mapping
{
    public static class AutoMapperBase
    {
        public static readonly IMapper _mapper;
        static AutoMapperBase()
        {
            var config = new MapperConfiguration(c => c.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
        }

        public static IPagedList<TDestination> ToMappedPagedList<TSource, TDestination>(this IPagedList<TSource> list)
        {
            IEnumerable<TDestination> sourceList = _mapper.Map<IEnumerable<TSource>, IEnumerable<TDestination>>(list);
            IPagedList<TDestination> pagedResult = new StaticPagedList<TDestination>(sourceList, list.GetMetaData());
            return pagedResult;
        }
    }
}
