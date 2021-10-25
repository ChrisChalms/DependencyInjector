using System.Reflection;

namespace CC.DependencyInjector.Registries
{
    /// <summary>
    /// Represents an implementation registration object
    /// </summary>
    internal class ImplentationRegistry : IDependencyRegistration
    {
        public bool Singleton { get; private set; }
        Container.RegistrationScope IDependencyRegistration.RegistrationScope => Singleton ? Container.RegistrationScope.SINGLETON : Container.RegistrationScope.TRANSIENT;


        ConstructorInfo _constructorInfo;
        object _cachedObject;

        // Initialise
        public ImplentationRegistry(Type type, bool singleton)
        {
            // Get constructor
            var constructors = type.GetConstructors();

            if (constructors.Length == 0)
                constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

            var constructor = constructors.First();

            if (constructor.GetParameters().Length > 0)
                throw new Exception("Parameters are not supported");

            // TODO: Support parameters

            _constructorInfo = constructor;

            // Create immediately if the dependency is a singleton
            Singleton = singleton;
            if (Singleton)
                _cachedObject = _constructorInfo.Invoke(null);
        }

        // Return the cached object if this is a singleton, otherwise return a new object
        public object GetObject() => Singleton ? _cachedObject : _constructorInfo.Invoke(null);
    }
}
