namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides methods for registering behaviors.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the <typeparamref name="TBehavior"/> so it can be injected into a component, with the transient lifetime.
        /// </summary>
        /// <remarks>
        /// Since behaviors know about the component that uses them, a behavior's instance can only be used by one component.
        /// Behaviors should always have their lifetime set to transient.
        /// </remarks>
        /// <typeparam name="TBehavior">Type of the behavior to be registered.</typeparam>
        /// <param name="services">The service collection in which the  behavior will be registered.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        public static IServiceCollection AddBehavior<TBehavior>(this IServiceCollection services)
            where TBehavior : class
        {
            services.AddTransient<TBehavior>();

            return services;
        }
    }
}
