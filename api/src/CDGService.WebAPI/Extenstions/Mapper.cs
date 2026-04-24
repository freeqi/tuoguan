using AutoMapper;

namespace CDGService.WebAPI.Extenstions
{
    public static class AutoMapperHelper
    {
        private static IMapper _mapper;

        public static void Initialize(IMapper mapper)
        {
            _mapper = mapper;
        }

        public static TDestination Map<TDestination>(object source)
        {
            return _mapper.Map<TDestination>(source);
        }

        public static TDestination Map<TSource, TDestination>(TSource source)
        {
            return _mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return _mapper.Map(source, destination);
        }
    }
}