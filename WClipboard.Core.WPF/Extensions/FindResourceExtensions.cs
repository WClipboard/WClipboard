using System.Windows;

#nullable disable

namespace WClipboard.Core.WPF.Extensions
{
    public static class FindResourceExtensions
    {
        public static T FindResource<T>(this Application app, object resourceKey)
        {
            return (T)app.FindResource(resourceKey);
        }

        public static T TryFindResource<T>(this Application app, object resourceKey)
        {
            return app.TryFindResource(resourceKey) is T r ? r : default;
        }

        public static bool TryFindResource<T>(this Application app, object resourceKey, out T resource)
        {
            var resourceO = app.TryFindResource(resourceKey);
            if(resourceO is T resourceT)
            {
                resource = resourceT;
                return true;
            } 
            else
            {
                resource = default;
                return false;
            }
        }

        public static T FindResource<T>(this FrameworkElement fe, object resourceKey)
        {
            return (T)fe.FindResource(resourceKey);
        }

        public static T TryFindResource<T>(this FrameworkElement fe, object resourceKey)
        {
            return fe.TryFindResource(resourceKey) is T r ? r : default;
        }

        public static bool TryFindResource<T>(this FrameworkElement fe, object resourceKey, out T resource)
        {
            var resourceO = fe.TryFindResource(resourceKey);
            if (resourceO is T resourceT)
            {
                resource = resourceT;
                return true;
            }
            else
            {
                resource = default;
                return false;
            }
        }
    }
}
