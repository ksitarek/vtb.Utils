using Microsoft.AspNetCore.Builder;

namespace vtb.Utils.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder applicationBuilder, string apiName,
            bool enableUi)
        {
            applicationBuilder.UseSwagger();
            if (enableUi)
                applicationBuilder.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", apiName); });

            return applicationBuilder;
        }
    }
}