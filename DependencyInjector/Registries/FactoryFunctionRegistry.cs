namespace CC.DependencyInjector.Registries
{
    /// <summary>
    /// Represents a factory function registration object
    /// </summary>
    internal class FactoryFunctionRegistry<T> : IDependencyRegistration
    {
        internal bool Singleton { get; private set; }
        Container.RegistrationScope IDependencyRegistration.RegistrationScope => Singleton ? Container.RegistrationScope.SINGLETON : Container.RegistrationScope.TRANSIENT;

        Func<T> _factory;
        object _cachedObject;

        // Initialise
        public FactoryFunctionRegistry(Func<T> createFunction, bool singleton)
        {
            _factory = createFunction;
            Singleton = singleton;

            // Create immediately if the dependency is a singleton
            if (Singleton)
                _cachedObject = _factory();
        }

        // Return the cached object if this is a singleton, otherwise return a new object
        public object? GetObject() => Singleton ? _cachedObject : _factory();

        object IDependencyRegistration.GetObject()
        {
            throw new NotImplementedException();
        }
    }
}
