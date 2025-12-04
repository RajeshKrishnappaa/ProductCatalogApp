using AutoMapper;
using ProdApp.Configuration;

namespace ProdApp.Tests
{
    public static class MapperHelper
    {
        public static IMapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });

            return config.CreateMapper();
        }
    }
}
