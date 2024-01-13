using VisualHomeBackend.Routes.UserRoutes;

namespace VisualHomeBackend.Routes
{
    public static class RoutesMain
    {
        public static void Map(WebApplication app)
        {
            WebSocketRoute.Map(app);
            LoginRoute.Map(app);
            AuthRoute.Map(app);
            CreateUserRoute.Map(app);
            UpdateCurrentUserRoute.Map(app);
        }
    }
}
