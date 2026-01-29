using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace CoreORM
{
    public class RazorViewToStringRenderer
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public RazorViewToStringRenderer(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> RenderViewToStringAsync<TModel>(string viewPath, TModel model)
        {
            var actionContext = GetActionContext();
            var view = FindView(actionContext, viewPath);

            await using var output = new StringWriter();

            var viewData = new ViewDataDictionary<TModel>(
                new EmptyModelMetadataProvider(),
                new ModelStateDictionary())
            {
                Model = model
            };

            var tempData = new TempDataDictionary(actionContext.HttpContext, _tempDataProvider);

            var viewContext = new ViewContext(
                actionContext,
                view,
                viewData,
                tempData,
                output,
                new HtmlHelperOptions()
            );

            await view.RenderAsync(viewContext);

            return output.ToString();
        }

        private IView FindView(ActionContext actionContext, string viewPath)
        {
            CoreUtils.ConsoleLogger.Info($"Finding view at path: {viewPath}");
            
            var getViewResult = _viewEngine.GetView(null, viewPath, true);
            if (getViewResult.Success)
                return getViewResult.View;

            var findViewResult = _viewEngine.FindView(actionContext, viewPath, true);
            if (findViewResult.Success)
                return findViewResult.View;

            var searched = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
            var msg = $"Unable to find view '{viewPath}'. Searched:{Environment.NewLine}" +
                      string.Join(Environment.NewLine, searched);

            CoreUtils.ConsoleLogger.Error(msg);
            throw new InvalidOperationException(msg);
        }

        private ActionContext GetActionContext()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = _serviceProvider
            };

            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }
    }
}
