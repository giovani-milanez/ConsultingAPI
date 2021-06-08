using API.Data.VO;
using API.Hypermedia.Constants;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Hypermedia.Enricher
{
    public class StepEnricher : ContentResponseEnricher<StepVO>
    {        
        protected override async Task EnrichModel(StepVO content, IUrlHelper urlHelper)
        {
            var path = "api/v1.0/step";
            string link = await GetLink(content.Id, urlHelper, path);

            content.Links.Add(new HyperMediaLink()
            {
                Action = HttpActionVerb.GET,
                Href = link,
                Rel = RelationType.self,
                Type = ResponseTypeFormat.DefaultGet
            });
            content.Links.Add(new HyperMediaLink()
            {
                Action = HttpActionVerb.POST,
                Href = link,
                Rel = RelationType.self,
                Type = ResponseTypeFormat.DefaultPost
            });
            content.Links.Add(new HyperMediaLink()
            {
                Action = HttpActionVerb.PUT,
                Href = link,
                Rel = RelationType.self,
                Type = ResponseTypeFormat.DefaultPut
            });
            //content.Links.Add(new HyperMediaLink()
            //{
            //    Action = HttpActionVerb.PATCH,
            //    Href = link,
            //    Rel = RelationType.self,
            //    Type = ResponseTypeFormat.DefaultPatch
            //});
            content.Links.Add(new HyperMediaLink()
            {
                Action = HttpActionVerb.DELETE,
                Href = link,
                Rel = RelationType.self,
                Type = "int"
            });

        }
    }
}
