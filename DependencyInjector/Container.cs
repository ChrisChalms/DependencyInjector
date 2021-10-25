using CC.DependencyInjector.Registries;

namespace CC.DependencyInjector
{
    /// <summary>
    /// Dependency container. Handles injection for reistered types
    /// </summary>
    public class Container
    {
        public enum RegistrationScope
        {
            SINGLETON,
            TRANSIENT
        }

        Dictionary<Type, IDependencyRegistration> _dependencies = new Dictionary<Type, IDependencyRegistration>();

        #region Registration

        /// <summary>
        /// Register an factory function to be called on resolution
        /// <code>Container.Register&lt;interface&gt;(() => new implementation())</code>
        /// </summary>
        /// <typeparam name="T">Interface to register</typeparam>
        /// <param name="factory">Function to call on resolution</param>
        /// <param name="singleton">Is the class a singleton? Default false</param>
        public void Register<T>(Func<T> factory, bool singleton = false)
        {
            if (factory == null)
                throw new Exception("No factory function passed");

            var typeOf = typeOf<T>();
            if (IsRegistered(typeOf))
                throw new Exception("Dependency already registered");

            _dependencies[typeOf] = new FactoryFunctionRegistry<T>(factory, singleton);
        }

        /// <summary>
        /// Register an implementation type for the supplied interface
        /// <code>Container.Register&lt;interface&gt;(implementation)</code>
        /// </summary>
        /// <typeparam name="T">Interface to register</typeparam>
        /// <param name="implementation">Implementation type</param>
        /// <param name="singleton">Is the class a singleton? Default false</param>
        /// <remarks>Uses reflection</remarks>
        public void Register<T>(Type implementation, bool singleton = false)
        {
            if (implementation == null)
                throw new Exception("Passed implementation cannot be null");
            var typeOf = typeOf<T>();
            if (IsRegistered(typeOf))
                throw new Exception("Dependency already registered");

            _dependencies[typeOf] = new ImplentationRegistry(implementation, singleton);
        }

        /// <summary>
        /// Register an existing implementation of an interface
        /// <code>Container.Register&lt;interface&gt;(new implementation())</code>
        /// </summary>
        /// <typeparam name="T">Interface to register</typeparam>
        /// <param name="implementationObject">Implementation object</param>
        /// <remarks>Singleton only, does not support transient scope</remarks>
        public void Register<T>(object implementationObject)
        {
            if (implementationObject == null)
                throw new Exception("Passed implementation object cannot be null");
            var typeOf = typeOf<T>();
            if (IsRegistered(typeOf))
                throw new Exception("Dependency already registered");

            _dependencies[typeOf] = new ObjectRegistry(implementationObject);
        }

        /// <summary>
        /// Unregister a dependency
        /// <code>Container.Unregister&lt;interface&gt;()</code>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Unregister<T>()
        {
            var typeOf = typeOf<T>();
            if (!IsRegistered(typeOf))
                throw new Exception($"No {typeOf} dependency registered");

            _dependencies.Remove(typeOf);
        }

        /// <summary>
        /// Unregister a dependency
        /// <code>Container.Unregister(typeof(interface))</code>
        /// </summary>
        /// <param name="key">Interface type</param>
        public void Unregister(Type key)
        {
            if (!IsRegistered(key))
                throw new Exception($"No {key} dependency registered");

            _dependencies.Remove(key);
        }

        #endregion

        #region Resolution

        /// <summary>
        /// Returns the object registered to a given type if registered
        /// <code>Container.Resolve&lt;interface&gt;()</code>
        /// </summary>
        /// <typeparam name="T">Interface type</typeparam>
        /// <returns>Implementation of interface</returns>
        public T Resolve<T>()
        {
            var typeOf = typeOf<T>();
            if (!IsRegistered(typeOf))
                throw new Exception($"No dependency of type {typeOf} registered");

            return (T)_dependencies[typeOf].GetObject();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Return whether an implementation of the passed interface has been registered
        /// <code>Container.IsRegistered(typeOf(interface))</code>
        /// </summary>
        /// <param name="key">Interface to check</param>
        /// <returns>bool</returns>
        public bool IsRegistered(Type key) => _dependencies.ContainsKey(key);

        /// <summary>
        /// Return whether an implementation of the passed interface has been registered
        /// <code>Container.IsRegistered&lt;interface&gt;()</code>
        /// </summary>
        /// <typeparam name="T">Interface to check</typeparam>
        /// <returns>bool</returns>
        public bool IsRegistered<T>() => _dependencies.ContainsKey(typeOf<T>());

        /// <summary>
        /// Returns the scope of the dependency registration. Currently SINGLETON or TRANSIENT
        /// <code>Container.IsRegistered(typeOf(interface))</code>
        /// </summary>
        /// <param name="key">Interface type</param>
        /// <returns>Container.RegistrationScope</returns>
        public RegistrationScope GetRegistrationScope(Type key)
        {
            if (key == null)
                throw new Exception("Passed key cannot be null");
            if (!IsRegistered(key))
                throw new Exception("Dependency not registered");

            return _dependencies[key].RegistrationScope;
        }

        /// <summary>
        /// Returns the scope of the dependency registration. Currently SINGLETON or TRANSIENT
        /// <code>Container.GetRegistrationScope&lt;interface&gt;()</code>
        /// </summary>
        /// <typeparam name="T">Interface type</typeparam>
        /// <returns>Container.RegistrationScope</returns>
        public RegistrationScope GetRegistrationScope<T>()
        {
            var typeOf = typeOf<T>();
            if (!IsRegistered(typeOf))
                throw new Exception("Dependency not registered");

            return _dependencies[typeOf].RegistrationScope;
        }

        // Return the typeOf of the passed generic
        private Type typeOf<T>() => typeof(T);

        #endregion
    }
}