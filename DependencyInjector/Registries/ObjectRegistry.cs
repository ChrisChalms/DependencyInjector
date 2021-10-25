namespace CC.DependencyInjector.Registries
{
    /// <summary>
    /// Represents an object registration. No transient option here, object already exists, then is registered
    /// </summary>
    internal class ObjectRegistry : IDependencyRegistration
    {
        Container.RegistrationScope IDependencyRegistration.RegistrationScope => Container.RegistrationScope.SINGLETON;

        object _object;

        // Initialise
        public ObjectRegistry(object obj) => _object = obj;

        // Return the cached object
        public object GetObject() => _object;
    }
}
